using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using MaterialDesignColors;
using MaterialDesignThemes.Wpf;
using XO.Commands;
using XO.Stores;
using XO.ViewModels;

namespace XO;

/// <summary>
///     Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private NavigationStore _navigationStore;
    private readonly LanguageStore _languageStore;


    public MainWindow()
    {
        
        _navigationStore = new NavigationStore();
        _languageStore = new LanguageStore();
        InitializeComponent();
        _navigationStore.CurrentViewModel = new MainMenuViewModel(_navigationStore, _languageStore);
        DataContext = new
        {
            NavigationStore = _navigationStore,
            ChangeThemeCommand = new ActionCommand<object>(ChangeTheme)
        };
    }
    
    private void ChangeTheme(object parameter)
    {
        var paletteHelper = new PaletteHelper();
        Theme theme = paletteHelper.GetTheme();

        if (theme.GetBaseTheme() == BaseTheme.Dark)
        {
            // Switch to Light theme with Orange/DeepOrange
            theme.SetBaseTheme(BaseTheme.Light);
            theme.SetPrimaryColor(SwatchHelper.Lookup[MaterialDesignColor.Orange]);
            theme.SetSecondaryColor(SwatchHelper.Lookup[MaterialDesignColor.DeepOrange]);

            if (parameter is Button button)
            {
                button.Content = new Image { 
                    Source = new BitmapImage(new Uri("pack://application:,,,/XO;component/Resources/icons/dark_theme_icon.png")) 
                };
            }
        }
        else
        {
            theme.SetBaseTheme(BaseTheme.Dark);
            theme.SetPrimaryColor(SwatchHelper.Lookup[MaterialDesignColor.Green]);
            theme.SetSecondaryColor(SwatchHelper.Lookup[MaterialDesignColor.Teal]);
            theme.SecondaryMid = new ColorPair(System.Windows.Media.Colors.White, System.Windows.Media.Colors.Black);
            
            if (parameter is Button button)
            {
                button.Content = new Image { 
                    Source = new BitmapImage(new Uri("pack://application:,,,/XO;component/Resources/icons/light_theme_icon.png")) 
                };
            }
        }

        paletteHelper.SetTheme(theme);
    }
    
    private void UpdateResourceDictionary(string resourcePath, Func<ResourceDictionary, bool> condition)
    {
        var mergedDictionaries = Application.Current.Resources.MergedDictionaries;
        
        for (int i = mergedDictionaries.Count - 1; i >= 0; i--)
        {
            var dict = mergedDictionaries[i];
            if (condition(dict))
            {
                mergedDictionaries.RemoveAt(i);
            }
        }
        
        var resourceUri = new Uri(resourcePath, UriKind.Relative);
        var resourceDict = new ResourceDictionary { Source = resourceUri };
        mergedDictionaries.Add(resourceDict);
    }

    private void ChangeLanguage(string languageCode)
    {
       
        bool IsLanguageDictionary(ResourceDictionary dict) => 
            dict.Source != null && dict.Source.ToString().Contains("Resources/");
        UpdateResourceDictionary($"Resources/locale.{languageCode}.xaml", IsLanguageDictionary);
    }
    
    private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (sender is ComboBox comboBox)
        {
            if (comboBox.SelectedItem is ComboBoxItem selectedItem)
            {
                string languageCode = selectedItem.Tag.ToString();
                _languageStore.CurrentLanguage = languageCode;
                ChangeLanguage(languageCode);
            }
        }
    }
}