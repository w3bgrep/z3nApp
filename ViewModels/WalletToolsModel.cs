
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Diagnostics;
using z3nApp;


namespace z3nApp.ViewModels
{
    public partial class WalletToolsModel : ObservableObject
    {
        //DEFAULT_SET
        private readonly Page _page;
        [ObservableProperty]
        private string inputEdit = null;
        [ObservableProperty]
        private string output = null;
        [RelayCommand]
        private async void GoMain()
        {
            await Shell.Current.GoToAsync("//MainPage");
        }
        [RelayCommand]
        private async void GoBack()
        {
            await Shell.Current.GoToAsync("//MainPage");
        }
        public WalletToolsModel(Page page)
        {
            _page = page; // Сохраняем страницу
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
        private string chainType = "not set";
        [ObservableProperty]
        private string dirrection = null;

        [ObservableProperty]
        private ObservableCollection<string> chainTypes = new ObservableCollection<string>
        {
            "Evm",
            "Solana",
            "Cosmos"

        };
        [ObservableProperty]
        private ObservableCollection<string> dirrections = new ObservableCollection<string>
        {
            "seed-pk-address",
            "seed-pk",
            "pk-address",
            "seed-address"

        };

        [RelayCommand]
        private async Task PrivateFromSeed()
        {
            Output = null;


            if (!InputEdit.Contains("\r") && !InputEdit.Contains(","))
            {
                Output = await ProcessKey(InputEdit);
                return;
            }


            else
            {
                var list = new List<string>();
                if (InputEdit.Contains("\r"))
                    list = InputEdit.Split('\r', StringSplitOptions.RemoveEmptyEntries).ToList();
                else if (InputEdit.Contains(","))
                    list = InputEdit.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList();

                foreach (var item in list)
                {
                    Output += await ProcessKey(item) + "\n";
                }

            }

        }

        private async Task<string> ProcessKey(string input)
        {
            string type = ChainType;
            string output = Dirrection;


            string seed = "";
            string key = "";
            string address = "";

            if (type == "Solana")
            {
                if (output.Contains("seed"))
                {
                    try
                    {
                        var account = await Task.Run(() => new SolTools().AccFromSeed(input).Result);
                        seed = input;
                        key = account[0];
                        address = account[1];
                    }
                    catch (Exception ex)
                    {
                        return ex.Message;
                    }
                }
                else
                {
                    try
                    {
                        key = input;
                        address = await Task.Run(() => new SolTools().AddressFromKey(key).Result);
                    }
                    catch (Exception ex)
                    {
                        return ex.Message;
                    }
                }
            }
            else if (type == "Evm")
            {
                if (output.Contains("seed"))
                {
                    try
                    {
                        var account = await Task.Run(() => new EvmTools().AccFromSeed(input).Result);
                        seed = input;
                        key = account[0];
                        address = account[1];
                    }
                    catch (Exception ex)
                    {
                        return ex.Message;
                    }
                }
                else
                {
                    try
                    {
                        key = input;
                        address = await Task.Run(() => new EvmTools().AddressFromKey(key).Result);
                    }
                    catch (Exception ex)
                    {
                        return ex.Message;
                    }
                }

            }



                string result = "";
            switch (output) 
            {
                case "seed-pk-address":
                    result = $"{seed}\t{key}\t{address}"; 
                    break;
                case "seed-pk":
                    result = $"{seed}\t{key}";
                    break;
                case "pk-address":
                    result = $"{key}\t{address}";
                    break;
                case "seed-address":
                    result = $"{seed}\t{address}";
                    break;
                default:
                    result = $"output format undefined";
                    break;       
            }
            return result;

        }

    }
}
