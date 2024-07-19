using System.Windows.Input;

namespace MVVMToolkit
{
    public class ParametrizedCommand<TParameter> : ICommand
    {
        private Action<TParameter> _action;

        public event EventHandler CanExecuteChanged;


        public ParametrizedCommand(Action<TParameter> action)
        {
            _action = action;
        }


        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            _action((TParameter)parameter);
        }
    }
}
