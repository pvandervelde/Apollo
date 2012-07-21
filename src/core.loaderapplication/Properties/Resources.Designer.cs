﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.17626
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Apollo.Core.LoaderApplication.Properties {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Apollo.Core.LoaderApplication.Properties.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The specified argument was not inside the valid range..
        /// </summary>
        internal static string Exceptions_Messages_ArgumentOutOfRange {
            get {
                return ResourceManager.GetString("Exceptions_Messages_ArgumentOutOfRange", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The specified argument was not inside the valid range. The argument value was: {0}..
        /// </summary>
        internal static string Exceptions_Messages_ArgumentOutOfRange_WithArgument {
            get {
                return ResourceManager.GetString("Exceptions_Messages_ArgumentOutOfRange_WithArgument", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to It is not possible to log a message if the LogLevel is set to None..
        /// </summary>
        internal static string Exceptions_Messages_CannotLogMessageWithLogLevelSetToNone {
            get {
                return ResourceManager.GetString("Exceptions_Messages_CannotLogMessageWithLogLevelSetToNone", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to An internal error occurred. The error code is: {0}. Please contact your vendor..
        /// </summary>
        internal static string Exceptions_Messages_InternalError_WithCode {
            get {
                return ResourceManager.GetString("Exceptions_Messages_InternalError_WithCode", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The license verification has failed. Please contact your vendor..
        /// </summary>
        internal static string Exceptions_Messages_VerificationFailure {
            get {
                return ResourceManager.GetString("Exceptions_Messages_VerificationFailure", resourceCulture);
            }
        }
    }
}
