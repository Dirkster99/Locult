namespace LocultApp.Behaviours
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Input;

    /// <summary>
    /// Class atatches to a selector and executes a command if the new
    /// selecteditem when the selection has changed. This behaviour supports
    /// single selection only.
    /// </summary>
    public static class SelectedItemChanged
    {
        #region fields
        // Using a DependencyProperty as the backing store for SelectedItemChanged command.
        public static readonly DependencyProperty CommandProperty =
                DependencyProperty.RegisterAttached("Command",
                typeof(ICommand),
                typeof(SelectedItemChanged),
                new PropertyMetadata(null,
                SelectedItemChanged.OnCommand));
        #endregion fields

        #region methods
        #region getter setter methods for static properties
        public static ICommand GetCommand(DependencyObject obj)
        {
            return (ICommand)obj.GetValue(CommandProperty);
        }

        public static void SetCommand(DependencyObject obj, ICommand value)
        {
            obj.SetValue(CommandProperty, value);
        }
        #endregion getter setter methods for static properties

        /// <summary>
        /// Method executes when attached dependency property changes through binding.
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        private static void OnCommand(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var fwElement = d as Selector;

            if (fwElement == null)
                return;

            // Remove the handler if it exist to avoid memory leaks
            if (fwElement != null)
                fwElement.SelectionChanged -= fwElement_SelectionChanged;

            var command = e.NewValue as ICommand;
            if (command != null)
            {
                // the property is attached so we attach the event handler
                fwElement.SelectionChanged += fwElement_SelectionChanged;
            }
        }

        /// <summary>
        /// Method executes when the selection of the currently
        /// selected item changes in the <seealso cref="Selector"/> control.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void fwElement_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
			// Sender should be this class or a descendent of it
			var fwElement = sender as Selector;

			// Sanity check just in case this was somehow send by something else
			if (fwElement == null)
				return;

			// Handle right mouse click event if there is a command attached for this
			ICommand command = SelectedItemChanged.GetCommand(fwElement);

            object commandParameter = null;
            var listParameters = e.AddedItems;

            if (listParameters != null)
            {
                if (listParameters.Count > 0)
                {
                    commandParameter = listParameters[0];
                }
            }

			if (command != null)
			{
				// Check whether this attached behaviour is bound to a RoutedCommand
				if (command is RoutedCommand)
				{
					// Execute the routed command
                    (command as RoutedCommand).Execute(commandParameter, fwElement);
					e.Handled = true;
				}
				else
				{
					// Execute the Command as bound delegate
                    command.Execute(commandParameter);
					e.Handled = true;
				}
			}
        }
        #endregion methods
    }
}
