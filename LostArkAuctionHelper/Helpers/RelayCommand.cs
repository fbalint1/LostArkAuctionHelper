using System;
using System.Windows.Input;

namespace LostArkAuctionHelper.Helpers
{
  public class RelayCommand : ICommand
  {
    public event EventHandler CanExecuteChanged
    {
      add
      {
        CommandManager.RequerySuggested += value;
        CanExecuteChangedInternal += value;
      }
      remove
      {
        CommandManager.RequerySuggested -= value;
        CanExecuteChangedInternal -= value;
      }
    }

    private event EventHandler CanExecuteChangedInternal;

    Action<object> execute_function; //void(object)
    Predicate<object> canexecute_function; //bool(object)

    public RelayCommand(Action<object> execute_function,
        Predicate<object> canexecute_function)
    {
      this.execute_function = execute_function ??
          throw new ArgumentException("execute function not defined!");

      this.canexecute_function = canexecute_function ??
          throw new ArgumentException("can execute function not defined!");
    }

    public RelayCommand(Action<object> execute_function)
        : this(execute_function, t => true)
    {
    }

    public bool CanExecute(object parameter)
    {
      return this.canexecute_function != null
          && this.canexecute_function(parameter);
    }

    public void Execute(object parameter)
    {
      execute_function?.Invoke(parameter);
    }

    public void OnCanExecuteChanged()
    {
      EventHandler handler = this.CanExecuteChangedInternal;

      handler?.Invoke(this, EventArgs.Empty);
    }

    public void Destroy()
    {
      canexecute_function = t => false;
      execute_function = t => { return; };
    }
  }
}
