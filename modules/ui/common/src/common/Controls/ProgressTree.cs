using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hardcodet.Wpf.GenericTreeView;
using Apollo.Ui.Common.ViewModels;

namespace Apollo.Ui.Common.Controls
{
    /// <summary>
    /// Defines a tree control for displaying the progress of data set processing.
    /// </summary>
    public sealed class ProgressTree : TreeViewBase<IDataSetViewModel>
    {
        // Make this a filtered one? --> Then we can just link it to the project and filter out the non-active nodes


        public override ICollection<IDataSetViewModel> GetChildItems(IDataSetViewModel parent)
        {
            throw new NotImplementedException();
        }

        public override string GetItemKey(IDataSetViewModel item)
        {
            throw new NotImplementedException();
        }

        public override IDataSetViewModel GetParentItem(IDataSetViewModel item)
        {
            throw new NotImplementedException();
        }
    }
}
