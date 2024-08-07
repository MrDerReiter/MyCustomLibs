﻿using System.Windows.Input;

namespace MVVMToolkit
{
    public class Command : ICommand
    {
        private Action _action;

        public event EventHandler CanExecuteChanged;


        public Command(Action action)
        {
            _action = action;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            _action();
        }
    }
}
