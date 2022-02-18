using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ZPC.Phone.Command
{
    public class CommandBase : ICommand
    {
        public event EventHandler CanExecuteChanged;


        public CommandBase(Action<object> doExecute)
        {
            DoExecute = doExecute;
        }

        public CommandBase(Func<object, bool> doCanExecute)
        {
            DoCanExecute = doCanExecute;
        }

        public bool CanExecute(object parameter)
        {
            return DoCanExecute?.Invoke(parameter) == true;
        }

        public void Execute(object parameter)
        {
            DoExecute?.Invoke(parameter);
        }

        public Action<object> DoExecute { get; set; }

        public Func<object, bool> DoCanExecute { get; set; }
    }
}
