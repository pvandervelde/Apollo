//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;
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
        public const string GroupExportName = "myExport";
        public const string GroupImportName = "myImport";
        public const string GroupName1 = "MyFirstGroup";
        public const string GroupName2 = "MySecondGroup";
        public const string GroupName3 = "MyThirdGroup";

        private static void RegisterFirstGroup(IRegisterGroupDefinitions builder)
        {
            builder.Clear();

            var importOnProperty = builder.RegisterObject(typeof(ImportOnProperty));
            var exportOnProperty = builder.RegisterObject(typeof(ExportOnProperty));
            var freeImportOnProperty = builder.RegisterObject(typeof(ImportOnProperty));

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
            builder.DefineImport(
                GroupImportName, 
                insertPoint,
                new List<ImportRegistrationId> { freeImportOnProperty.RegisteredImports.First() });

            builder.Register(GroupName1);
        }

        private static void RegisterSecondGroup(IRegisterGroupDefinitions builder)
        {
            builder.Clear();
            
            var importOnProperty = builder.RegisterObject(typeof(ImportOnProperty));
            
            var actionOnMethod = builder.RegisterObject(typeof(ActionOnMethod));
            var conditionOnProperty = builder.RegisterObject(typeof(ConditionOnProperty));

            var registrator = builder.ScheduleRegistrator();
            {
                var actionVertex = registrator.AddExecutingAction(actionOnMethod.RegisteredActions.First());

                registrator.LinkFromStart(actionVertex, conditionOnProperty.RegisteredConditions.First());
                registrator.LinkToEnd(actionVertex);
                registrator.Register();
            }

            builder.DefineImport(GroupExportName, new List<ImportRegistrationId> { importOnProperty.RegisteredImports.First() });
            builder.DefineExport(GroupImportName);
            builder.Register(GroupName2);
        }

        private static void RegisterThirdGroup(IRegisterGroupDefinitions builder)
        {
            builder.Clear();

            var exportOnProperty = builder.RegisterObject(typeof(ExportOnProperty));
            var actionOnMethod = builder.RegisterObject(typeof(ActionOnMethod));
            var conditionOnProperty = builder.RegisterObject(typeof(ConditionOnProperty));

            var registrator = builder.ScheduleRegistrator();
            {
                var actionVertex = registrator.AddExecutingAction(actionOnMethod.RegisteredActions.First());

                registrator.LinkFromStart(actionVertex, conditionOnProperty.RegisteredConditions.First());
                registrator.LinkToEnd(actionVertex);
                registrator.Register();
            }

            builder.DefineExport(GroupImportName);
            builder.Register(GroupName3);
        }

        public void RegisterGroups(IRegisterGroupDefinitions builder)
        {
            RegisterFirstGroup(builder);
            RegisterSecondGroup(builder);
            RegisterThirdGroup(builder);
        }
    }
}
