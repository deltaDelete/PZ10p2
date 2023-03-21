using System;
using System.Windows.Input;

namespace PZ10p2; 

public class Command<TParameter> : ICommand {
    private Func<TParameter, bool> canExecute;
    private Action<TParameter> execute;
        
    public bool CanExecute(object? parameter) {
        return canExecute.Invoke((TParameter)parameter);
    }

    public void Execute(object? parameter) {
        execute.Invoke((TParameter)parameter);
    }

    public event EventHandler? CanExecuteChanged;

    public Command(Action<TParameter> execute, Func<TParameter, bool> canExecute) {
        this.execute = execute;
    }

    public Command(Action<TParameter> execute) : this(execute, x => true) {}
}

public class Command : ICommand {
    private Func<bool> canExecute;
    private Action execute;
    
    public bool CanExecute(object? parameter) {
        return canExecute.Invoke();
    }

    public void Execute(object? parameter) {
        execute.Invoke();
    }

    public event EventHandler? CanExecuteChanged;

    public Command(Action execute, Func<bool> canExecute) {
        this.execute = execute;
        this.canExecute = canExecute;
    }
    
    public Command(Action execute) : this(execute, () => true) {}
}