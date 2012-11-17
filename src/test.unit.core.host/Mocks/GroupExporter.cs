//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Apollo.Core.Extensions.Plugins;
using Apollo.Core.Extensions.Scheduling;

namespace Apollo.Core.Host.Mocks
{
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
        Justification = "Unit tests do not need documentation.")]
    public sealed class GroupExporter : IExportGroupDefinitions
    {
        public const string GroupExportName = "groupExport";
        public const string GroupImportName = "groupImport";
        public const string GroupName = "MyGroup";

        private static void RegisterExportingGroup(IRegisterGroupDefinitions builder)
        {
            var importOnProperty = builder.RegisterObject(typeof(ImportOnProperty));
            var exportOnProperty = builder.RegisterObject(typeof(ExportOnProperty));
            var actionOnMethod = builder.RegisterObject(typeof(ActionOnMethod));
            var conditionOnProperty = builder.RegisterObject(typeof(ConditionOnProperty));
            builder.Connect(importOnProperty.RegisteredImports.First(), exportOnProperty.RegisteredExports.First());

            EditableInsertVertex insertPoint;
            var registrator = builder.ScheduleRegistrator();
            {
                var actionVertex = registrator.AddExecutingAction(actionOnMethod.RegisteredActions.First());
                insertPoint = registrator.AddInsertPoint();

                registrator.LinkFromStart(actionVertex, conditionOnProperty.RegisteredConditions.First());
                registrator.LinkTo(actionVertex, insertPoint);
                registrator.LinkToEnd(insertPoint);
                registrator.Register();
            }

            builder.DefineExport(GroupExportName);
            builder.DefineImport(GroupImportName, insertPoint);

            builder.Register(GroupName);
        }

        public void RegisterGroups(IRegisterGroupDefinitions builder)
        {
            RegisterExportingGroup(builder);
        }
    }
}
