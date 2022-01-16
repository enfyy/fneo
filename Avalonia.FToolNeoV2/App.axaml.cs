using System.Threading.Tasks;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.FToolNeoV2.Services;
using Avalonia.Markup.Xaml;
using Avalonia.FToolNeoV2.ViewModels;
using Avalonia.FToolNeoV2.Views;

namespace Avalonia.FToolNeoV2
{
    public class App : Application
    {
        
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.Exit += async (_, _) => await OnApplicationEnd();
                
                desktop.MainWindow = new MainWindow
                {
                    DataContext = new MainWindowViewModel(),
                };
            }

            base.OnFrameworkInitializationCompleted();
        }

        private async Task OnApplicationEnd()
        {
            SpamHotkeyService.CleanUp();
            await PersistenceManager.Instance.SaveApplicationStateAsync();
        }
        
    }
}