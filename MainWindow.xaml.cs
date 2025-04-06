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
    
    public MainWindow()
    {
        
        _navigationStore = new NavigationStore();
        InitializeComponent();
        _navigationStore.CurrentViewModel = new MainMenuViewModel(_navigationStore);
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
}