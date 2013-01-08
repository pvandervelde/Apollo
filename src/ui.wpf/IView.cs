//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Apollo.UI.Wpf
{
    /// <summary>
    /// Implemented by views.
    /// </summary>
    /// <typeparam name="TModel">The type of the model.</typeparam>
    public interface IView<TModel>
    {
        /// <summary>
        /// Gets or sets the model.
        /// </summary>
        TModel Model 
        { 
            get; 
            set; 
        }
    }
}
