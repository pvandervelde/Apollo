//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;

namespace Apollo.UI.Console.Nuclei.ExceptionHandling
{
    /// <summary>
    /// This class provides some utilities for working with exceptions and exception filters.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Code inside of exception filters runs before the stack has been logically unwound, and so the throw point 
    /// is still visible in tools like debuggers, and back out code from finally blocks has not yet been run.
    /// See http://blogs.msdn.com/rmbyers/archive/2008/12/22/getting-good-dumps-when-an-exception-is-thrown.aspx.
    /// Filters can also be used to provide more fine-grained control over which exceptions are caught.  
    /// </para>
    /// <para>
    /// Be aware, however, that filters run at a surprising time - after an exception has occurred but before
    /// any finally clause has been run to restore broken invariants for things lexically in scope.  This can lead to 
    /// confusion if you access or manipulate program state from your filter.  See this blog entry for details
    /// and more specific guidance: http://blogs.msdn.com/clrteam/archive/2009/08/25/the-good-and-the-bad-of-exception-filters.aspx.
    /// </para>
    /// <para>
    /// This class relies on Reflection.Emit to generate code which can use filters.  If you are willing to add some
    /// complexity to your build process, a static technique (like writing in VB and use ILMerge, or rewriting with CCI)
    /// may be a better choice (e.g. more performant and easier to specialize).  Again see
    /// http://blogs.msdn.com/rmbyers/archive/2008/12/22/getting-good-dumps-when-an-exception-is-thrown.aspx for details.
    /// </para>
    /// </remarks>
    /// <source>
    /// http://blogs.msdn.com/b/rmbyers/archive/2010/01/30/sample-reflection-emit-code-for-using-exception-filters-from-c.aspx
    /// </source>
    internal static class ExceptionFilter
    {
        private static Action<Action, Func<Exception, bool>, Action<Exception>> s_Filter = GenerateFilter();

        /// <summary>
        /// Execute the body with the specified filter.
        /// </summary>
        /// <param name="body">The code to run inside the "try" block.</param>
        /// <param name="filter">
        /// Called whenever an exception escapes body, passing the exception object.  
        /// For exceptions that aren't derived from System.Exception, they'll show up as an instance of 
        /// RuntimeWrappedException.
        /// </param>
        /// <param name="handler">
        /// Invoked (with the exception) whenever the filter returns true, causes the exception to be swallowed.
        /// </param>
        public static void ExecuteWithFilter(Action body, Func<Exception, bool> filter, Action<Exception> handler)
        {
            s_Filter(body, filter, handler);
        }

        /// <summary>
        /// Execute the body with the specified filter with no handler ever being invoked.
        /// </summary>
        /// <param name="body">The code to run inside the "try" block.</param>
        /// <param name="filter">
        /// Called whenever an exception escapes body, passing the exception object.  
        /// For exceptions that aren't derived from System.Exception, they'll show up as 
        /// an instance of RuntimeWrappedException.
        /// </param>
        /// <remarks>
        /// Note that this allocates a delegate and closure class, a small amount of overhead but something that may not be appropriate
        /// for inside of a tight inner loop.  If you want to call this on a very hot path, you may want to consider a direct call
        /// rather than using an anonymous method.
        /// </remarks>
        public static void ExecuteWithFilter(Action body, Action<Exception> filter)
        {
            ExecuteWithFilter(body, (e) => { filter(e); return false; }, null);
        }

        /// <summary>
        /// Generate a function which has an EH filter.
        /// </summary>
        /// <returns>The delegate that holds the EH filter.</returns>
        [SuppressMessage("Microsoft.Performance", "CA1804:RemoveUnusedLocals", MessageId = "exLoc",
            Justification = "If we remove this statement then we're generating illegal IL and the application crashes on startup.")]
        private static Action<Action, Func<Exception, bool>, Action<Exception>> GenerateFilter()
        {
            // Create a dynamic assembly with reflection emit
            var name = new AssemblyName("DynamicFilter");
#if (DEBUG)
            AssemblyBuilder assembly = AppDomain.CurrentDomain.DefineDynamicAssembly(name, AssemblyBuilderAccess.RunAndSave);
            var module = assembly.DefineDynamicModule("DynamicFilter", "DynamicFilter.dll");
#else
            AssemblyBuilder assembly = AppDomain.CurrentDomain.DefineDynamicAssembly(name, AssemblyBuilderAccess.Run);
            var module = assembly.DefineDynamicModule("DynamicFilter");
#endif

            // Add an assembly attribute that tells the CLR to wrap non-CLS-compliant exceptions in a RuntimeWrappedException object
            // (so the cast to Exception in the code will always succeed).  C# and VB always do this, C++/CLI doesn't.
            assembly.SetCustomAttribute(
                new CustomAttributeBuilder(
                    typeof(RuntimeCompatibilityAttribute).GetConstructor(new Type[] { }),
                    new object[] { },
                    new PropertyInfo[] { typeof(RuntimeCompatibilityAttribute).GetProperty("WrapNonExceptionThrows") },
                    new object[] { true }));

            // Add an assembly attribute that tells the CLR not to attempt to load PDBs when compiling this assembly 
            assembly.SetCustomAttribute(
                new CustomAttributeBuilder(
                    typeof(DebuggableAttribute).GetConstructor(new Type[] { typeof(DebuggableAttribute.DebuggingModes) }),
                    new object[] { DebuggableAttribute.DebuggingModes.IgnoreSymbolStoreSequencePoints }));

            // Create the type and method which will contain the filter
            TypeBuilder type = module.DefineType("Filter", TypeAttributes.Class | TypeAttributes.Public);
            var argTypes = new Type[] { typeof(Action), typeof(Func<Exception, bool>), typeof(Action<Exception>) };
            MethodBuilder meth = type.DefineMethod("InvokeWithFilter", MethodAttributes.Public | MethodAttributes.Static, typeof(void), argTypes);

            var il = meth.GetILGenerator();

            // This variable shouldn't be necessary (it's never used)
            // but for some reason the compiler generates illegal IL if this 
            // variable isn't there ...
            var exLoc = il.DeclareLocal(typeof(Exception));

            // Invoke the body delegate inside the try
            il.BeginExceptionBlock();
            il.Emit(OpCodes.Ldarg_0);
            il.EmitCall(OpCodes.Callvirt, typeof(Action).GetMethod("Invoke"), null);

            // Invoke the filter delegate inside the filter block
            il.BeginExceptFilterBlock();
            il.Emit(OpCodes.Castclass, typeof(Exception));
            il.Emit(OpCodes.Stloc_0);
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Ldloc_0);
            il.EmitCall(OpCodes.Callvirt, typeof(Func<Exception, bool>).GetMethod("Invoke"), null);

            // Invoke the handler delegate inside the catch block
            il.BeginCatchBlock(null);
            il.Emit(OpCodes.Castclass, typeof(Exception));
            il.Emit(OpCodes.Stloc_0);
            il.Emit(OpCodes.Ldarg_2);
            il.Emit(OpCodes.Ldloc_0);
            il.EmitCall(OpCodes.Callvirt, typeof(Action<Exception>).GetMethod("Invoke"), null);

            il.EndExceptionBlock();
            il.Emit(OpCodes.Ret);

            var bakedType = type.CreateType();
#if (DEBUG)
            assembly.Save("DynamicFilter.dll");
#endif

            // Construct a delegate to the filter function and return it
            var bakedMeth = bakedType.GetMethod("InvokeWithFilter");
            var del = Delegate.CreateDelegate(typeof(Action<Action, Func<Exception, bool>, Action<Exception>>), bakedMeth);
            return (Action<Action, Func<Exception, bool>, Action<Exception>>)del;
        }
    }
}

