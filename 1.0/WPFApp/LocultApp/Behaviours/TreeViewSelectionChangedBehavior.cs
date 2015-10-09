namespace LocultApp.Behaviours
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Media;

    /// <summary>
    /// Source:
    /// http://stackoverflow.com/questions/1034374/drag-and-drop-in-mvvm-with-scatterview
    /// http://social.msdn.microsoft.com/Forums/de-DE/wpf/thread/21bed380-c485-44fb-8741-f9245524d0ae
    /// 
    /// Attached behaviour to implement the SelectionChanged command/event via delegate command binding or routed commands.
    /// </summary>
    public static class TreeViewSelectionChangedBehavior
    {
        #region fields
        /// <summary>
        /// Field of attached ICommand property
        /// </summary>
        private static readonly DependencyProperty ChangedCommandProperty = DependencyProperty.RegisterAttached(
            "ChangedCommand",
            typeof(ICommand),
            typeof(TreeViewSelectionChangedBehavior),
            new PropertyMetadata(null, OnSelectionChangedCommandChange));

        /// <summary>
        /// Implement backing store for UndoSelection dependency proeprty to indicate whether selection should be
        /// cancelled via MessageBox query or not.
        /// </summary>
        public static readonly DependencyProperty UndoSelectionProperty =
            DependencyProperty.RegisterAttached("UndoSelection",
            typeof(bool),
            typeof(TreeViewSelectionChangedBehavior),
            new PropertyMetadata(false, OnUndoSelectionChanged));
        #endregion fields

        #region methods
        #region ICommand changed methods
        /// <summary>
        /// Setter method of the attached ChangedCommand <seealso cref="ICommand"/> property
        /// </summary>
        /// <param name="source"></param>
        /// <param name="value"></param>
        public static void SetChangedCommand(DependencyObject source, ICommand value)
        {
            source.SetValue(ChangedCommandProperty, value);
        }

        /// <summary>
        /// Getter method of the attached ChangedCommand <seealso cref="ICommand"/> property
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static ICommand GetChangedCommand(DependencyObject source)
        {
            return (ICommand)source.GetValue(ChangedCommandProperty);
        }
        #endregion ICommand changed methods

        #region UndoSelection methods
        public static bool GetUndoSelection(DependencyObject obj)
        {
            return (bool)obj.GetValue(UndoSelectionProperty);
        }

        public static void SetUndoSelection(DependencyObject obj, bool value)
        {
            obj.SetValue(UndoSelectionProperty, value);
        }
        #endregion UndoSelection methods

        /// <summary>
        /// This method is hooked in the definition of the <seealso cref="ChangedCommandProperty"/>.
        /// It is called whenever the attached property changes - in our case the event of binding
        /// and unbinding the property to a sink is what we are looking for.
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        private static void OnSelectionChangedCommandChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TreeView uiElement = d as TreeView;  // Remove the handler if it exist to avoid memory leaks

            if (uiElement != null)
            {
                uiElement.SelectedItemChanged -= Selection_Changed;

                var command = e.NewValue as ICommand;
                if (command != null)
                {
                    // the property is attached so we attach the Drop event handler
                    uiElement.SelectedItemChanged += Selection_Changed;
                }
            }
        }

        /// <summary>
        /// This method is called when the selection changed event occurs. The sender should be the control
        /// on which this behaviour is attached - so we convert the sender into a <seealso cref="UIElement"/>
        /// and receive the Command through the <seealso cref="GetChangedCommand"/> getter listed above.
        /// 
        /// The <paramref name="e"/> parameter contains the standard EventArgs data,
        /// which is unpacked and reales upon the bound command.
        /// 
        /// This implementation supports binding of delegate commands and routed commands.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void Selection_Changed(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var uiElement = sender as TreeView;

            // Sanity check just in case this was somehow send by something else
            if (uiElement == null)
                return;

            ICommand changedCommand = TreeViewSelectionChangedBehavior.GetChangedCommand(uiElement);

            // There may not be a command bound to this after all
            if (changedCommand == null)
                return;

            ////if ((e.NewValue == null && e.NewValue == null))
            ////  return;
            ////else
            ////{
            ////  // Actual value did not really change
            ////  if ((e.NewValue != null && e.NewValue != null))
            ////  {
            ////    if ((e.NewValue == e.NewValue))
            ////    return;
            ////  }
            ////}

            // Check whether this attached behaviour is bound to a RoutedCommand
            if (changedCommand is RoutedCommand)
            {
                // Execute the routed command
                (changedCommand as RoutedCommand).Execute(e.NewValue, uiElement);
            }
            else
            {
                // Execute the Command as bound delegate
                changedCommand.Execute(e.NewValue);
            }
        }

        /// <summary>
        /// Executes when the bound boolean property indicates that a user should be asked
        /// about changing a treeviewitem selection instead of just performing it.
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        private static void OnUndoSelectionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TreeView uiElement = d as TreeView;  // Remove the handler if it exist to avoid memory leaks

            if (uiElement != null)
            {
                uiElement.PreviewMouseDown -= uiElement_PreviewMouseDown;

                var command = (bool)e.NewValue;
                if (command == true)
                {
                    // the property is attached so we attach the Drop event handler
                    uiElement.PreviewMouseDown += uiElement_PreviewMouseDown;
                }
            }
        }

        /// <summary>
        /// Based on the solution proposed here:
        /// Source: http://stackoverflow.com/questions/20244916/wpf-treeview-selection-change
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void uiElement_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            // first did the user click on a tree node?
            var source = e.OriginalSource as DependencyObject;
            while (source != null && !(source is TreeViewItem))
                source = VisualTreeHelper.GetParent(source);

            var itemSource = source as TreeViewItem;
            if (itemSource == null)
                return;

            var treeView = sender as TreeView;
            if (treeView == null)
                return;

            bool undoSelection = TreeViewSelectionChangedBehavior.GetUndoSelection(treeView);
            if (undoSelection == false)
                return;

            // Cancel the attempt to select an item.
            var result = MessageBox.Show("The current document has unsaved data. Do you want to continue without saving data?", "Are you really sure?",
                                         MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No);

            if (result == MessageBoxResult.No)
            {
                // Cancel the attempt to select a differnet item.
                e.Handled = true;
            }
            else
            {
                // Lets disable this for a moment, otherwise, we'll get into an event "recursion"
                treeView.PreviewMouseDown -= uiElement_PreviewMouseDown;

                // Select the new item - make sure a SelectedItemChanged event is fired in any case
                // Even if this means that we have to deselect/select the one and the same item
                if (itemSource.IsSelected == true)
                    itemSource.IsSelected = false;

                itemSource.IsSelected = true;

                // Lets enable this to get back to business for next selection
                treeView.PreviewMouseDown += uiElement_PreviewMouseDown;
            }
        }
        #endregion methods
    }
}
