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
            "Addresses",
            "Wallets",
            "Proxy",
            "Twitter",
            "Discord",
            "Google",
            "GitHub",
            "Native",
            "Balance",
            "Mail",
        };

        [ObservableProperty]
        private string importType = null;


        [ObservableProperty]
        private ObservableCollection<string> operations = new ObservableCollection<string>
        {
            "read",
            "import",
            "update",
            "show",

        };

        [ObservableProperty]
        private string operation = null;

        [ObservableProperty]
        private string operationInfo = null;

        internal void setInfo()
        {
            switch (Operation)
            {
                case "import":
                    OperationInfo = "import multiple accounts";
                    return;
                default:
                    return;
            }
        }




        [RelayCommand]
        private async Task Process()
        {
            new Sql(ImportType.ToLower());
        }

    }
}
