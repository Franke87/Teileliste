using System;
using System.Windows.Input;

namespace TeileListe.Classes
{
    public class MyCommand : ICommand
    {
        private readonly Action _execute;

        public MyCommand(Action onExecute)
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
            _execute();
        }
    }
}
