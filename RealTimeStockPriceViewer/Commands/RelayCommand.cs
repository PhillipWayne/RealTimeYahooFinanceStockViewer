using System;
using System.Windows.Input;

namespace Commands
{
    public class RelayCommand: ICommand
    {
        #region Fields
        readonly Action<object> _execute; readonly Predicate<object> _canExecute;
        #endregion

        #region Constructor
        public RelayCommand(Action<object> execute) : this(execute, null) { }

        public RelayCommand(Action<object> execute, Predicate<object> canExecute)
        {
            if (execute == null) throw new ArgumentNullException("execute");
            _execute = execute;
            _canExecute = canExecute;
        }
        #endregion

        #region Properties
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove
            {
                CommandManager.RequerySuggested -= value;
            }
        }
        #endregion

        #region Methods
        public void Execute(object parameter)
        {
            _execute(parameter);
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute == null || _canExecute(parameter);
        }
        #endregion
    }
}
