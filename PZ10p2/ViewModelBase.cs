using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace PZ10p2;

public class ViewModelBase : INotifyPropertyChanged, INotifyPropertyChanging {
    public event PropertyChangedEventHandler? PropertyChanged;
    public event PropertyChangingEventHandler? PropertyChanging;

    /// <summary>
    /// Меняет значение, если нужно, и вызывает PropertyChanging и PropertyChanged
    /// </summary>
    /// <param name="field">ссылка на поле</param>
    /// <param name="value">значение</param>
    /// <param name="propertyName">имя свойства</param>
    /// <typeparam name="T"></typeparam>
    /// <returns>true - если изменено, иначе false</returns>
    protected bool SetPropertyIfChanged<T>(ref T field, T value, [CallerMemberName] string propertyName = "") {
        if (Equals(field, value)) {
            return false;
        }
        OnPropertyChanging(propertyName);
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }

    protected void OnPropertyChanged(string propertyName) {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected void OnPropertyChanging(string propertyName) {
        PropertyChanging?.Invoke(this, new PropertyChangingEventArgs(propertyName));
    }
}