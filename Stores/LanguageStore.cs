using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace XO.Stores;

public class LanguageStore : INotifyPropertyChanged
{
    private string _currentLanguage;

    public string CurrentLanguage
    {
        get => _currentLanguage;
        set
        {
            _currentLanguage = value;
            OnPropertyChanged(nameof(CurrentLanguage));
        }
    }
    
    public event PropertyChangedEventHandler PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}