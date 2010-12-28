//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using Autofac;

namespace Apollo.UI.Common.Views.Datasets
{
    /// <summary>
    /// The presenter for the <see cref="DatasetModel"/>.
    /// </summary>
    public sealed class DatasetPresenter : Presenter<IDatasetView, DatasetModel, DatasetParameter>
    {
    }
}
