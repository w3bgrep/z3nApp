using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace z3nApp.ViewModels
{
    
    
    
    internal partial class DbManagerModel : ObservableObject
    {
        private readonly Sql _database;
        //DEFAULT_SET
        public DbManagerModel(Page page)
        {
            _page = page; // Сохраняем страницу
            _database = new Sql();
            LoadWallets();
        }
        private readonly Page _page;
        [ObservableProperty]
        private string inputEdit = null;
        [ObservableProperty]
        private string output = null;
        [RelayCommand]
        private async void GoBack()
        {
            await Shell.Current.GoToAsync("//MainPage");
        }

        [RelayCommand]
        private async Task CopyResult()
        {
            if (!string.IsNullOrEmpty(Output))
            {
                await Clipboard.SetTextAsync(Output);
                await _page.DisplayAlert("Success", "Result copied to clipboard!", "OK");
            }
            else
            {
                await _page.DisplayAlert("Error", "No result to copy!", "OK");
            }
        }
        //OTHER
        [ObservableProperty]
        private ObservableCollection<string> importTypes = new ObservableCollection<string>
        {
            "Wallets",
            "Proxys",
            "Twitters",
            "Discords",
            "Googles",
            "GitHubs",
            "Native",
            "Balance",
        };

        [ObservableProperty]
        private string importType = null;


        [ObservableProperty]
        private ObservableCollection<string> operations = new ObservableCollection<string>
        {
            "update",
            "create&fill",
        };

        [ObservableProperty]
        private string operation = null;

        [ObservableProperty]
        private ObservableCollection<Wallets> allWallets;

        [RelayCommand]
        private async void LoadWallets()
        {
            var wallets = await _database.GetWallets();
            AllWallets = new ObservableCollection<Wallets>();


        }

    }
}
