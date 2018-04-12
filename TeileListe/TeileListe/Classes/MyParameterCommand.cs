using System;
using System.Windows.Input;

namespace TeileListe.Classes
{
    public class MyParameterCommand<TParameterType> : ICommand
    {
        private readonly Action<TParameterType> _execute;

        public MyParameterCommand(Action<TParameterType> onExecute)
        {
            _execute = onExecute;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            _execute((TParameterType) parameter);
        }
    }
}
