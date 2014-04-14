//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.ComponentModel.Composition;
using System.Diagnostics.CodeAnalysis;

namespace Test.Mocks
{
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
        Justification = "Unit tests do not need documentation.")]
    [SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass",
        Justification = "These classes are only here for testing purposes so there's little point in having them in a separate file each.")]
    public interface IExportingInterface
    {
    }

    [Export(typeof(IExportingInterface))]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
        Justification = "Unit tests do not need documentation.")]
    [SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass",
        Justification = "These classes are only here for testing purposes so there's little point in having them in a separate file each.")]
    public class MockExportingInterfaceImplementation : IExportingInterface
    { 
    }

    [Export(typeof(IExportingInterface))]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
        Justification = "Unit tests do not need documentation.")]
    [SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass",
        Justification = "These classes are only here for testing purposes so there's little point in having them in a separate file each.")]
    public sealed class MockChildExportingInterfaceImplementation : MockExportingInterfaceImplementation
    {
    }

    [Export("OnTypeWithName")]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
        Justification = "Unit tests do not need documentation.")]
    [SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass",
        Justification = "These classes are only here for testing purposes so there's little point in having them in a separate file each.")]
    public sealed class ExportOnTypeWithName : IExportingInterface
    {
    }

    [Export(typeof(IExportingInterface))]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
        Justification = "Unit tests do not need documentation.")]
    [SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass",
        Justification = "These classes are only here for testing purposes so there's little point in having them in a separate file each.")]
    public sealed class ExportOnTypeWithType : IExportingInterface
    {
    }

    [Export]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
        Justification = "Unit tests do not need documentation.")]
    [SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass",
        Justification = "These classes are only here for testing purposes so there's little point in having them in a separate file each.")]
    public sealed class ExportOnType : IExportingInterface
    {
    }

    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
        Justification = "Unit tests do not need documentation.")]
    [SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass",
        Justification = "These classes are only here for testing purposes so there's little point in having them in a separate file each.")]
    public sealed class ExportOnPropertyWithName
    {
        private IExportingInterface m_Value = new MockExportingInterfaceImplementation();

        [Export("OnPropertyWithName")]
        public IExportingInterface ExportingProperty
        {
            get
            {
                return m_Value;
            }

            set
            {
                m_Value = value;
            }
        }
    }

    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
        Justification = "Unit tests do not need documentation.")]
    [SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass",
        Justification = "These classes are only here for testing purposes so there's little point in having them in a separate file each.")]
    public sealed class ExportOnPropertyWithType
    {
        private IExportingInterface m_Value = new MockExportingInterfaceImplementation();

        [Export(typeof(IExportingInterface))]
        public IExportingInterface ExportingProperty
        {
            get
            {
                return m_Value;
            }

            set
            {
                m_Value = value;
            }
        }
    }

    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
        Justification = "Unit tests do not need documentation.")]
    [SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass",
        Justification = "These classes are only here for testing purposes so there's little point in having them in a separate file each.")]
    public sealed class ExportOnProperty
    {
        private IExportingInterface m_Value = new MockExportingInterfaceImplementation();

        [Export]
        public IExportingInterface ExportingProperty
        {
            get
            {
                return m_Value;
            }

            set
            {
                m_Value = value;
            }
        }
    }

    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
        Justification = "Unit tests do not need documentation.")]
    [SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass",
        Justification = "These classes are only here for testing purposes so there's little point in having them in a separate file each.")]
    public sealed class ExportOnMethodWithName
    {
        private IExportingInterface m_Value = new MockExportingInterfaceImplementation();

        [Export("OnMethodWithName")]
        public IExportingInterface ExportingMethod()
        {
            return m_Value;
        }
    }

    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
        Justification = "Unit tests do not need documentation.")]
    [SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass",
        Justification = "These classes are only here for testing purposes so there's little point in having them in a separate file each.")]
    public sealed class ExportOnMethodWithType
    {
        private IExportingInterface m_Value = new MockExportingInterfaceImplementation();

        [Export(typeof(IExportingInterface))]
        public IExportingInterface ExportingMethod()
        {
            return m_Value;
        }
    }

    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
        Justification = "Unit tests do not need documentation.")]
    [SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass",
        Justification = "These classes are only here for testing purposes so there's little point in having them in a separate file each.")]
    public sealed class ExportOnMethod
    {
        private IExportingInterface m_Value = new MockExportingInterfaceImplementation();

        [Export]
        public IExportingInterface ExportingMethod()
        {
            return m_Value;
        }
    }

    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
        Justification = "Unit tests do not need documentation.")]
    [SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass",
        Justification = "These classes are only here for testing purposes so there's little point in having them in a separate file each.")]
    public sealed class ExportOnMultiParameterMethod
    {
        private IExportingInterface m_Value = new MockExportingInterfaceImplementation();

        [Export]
        public IExportingInterface ExportingMethod(bool input1, bool input2)
        {
            return m_Value;
        }
    }
}
