﻿
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Diagnostics;
using z3nApp;


namespace z3nApp.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        private readonly Page _page;

        [ObservableProperty]
        private string ethPriceLbl = "update";
        [ObservableProperty]
        private string address = null;
        [ObservableProperty]
        private string native = null;
        [ObservableProperty]
        private string output = null;
        [ObservableProperty]
        private string rpc = null;
        [ObservableProperty]
        private string selectedRpc = "not set";
        [ObservableProperty]
        private ObservableCollection<string> rpcOptions = new ObservableCollection<string>
        {
            "Ethereum",
            "Arbitrum",
            "Base",
            "Blast", 
            "Fantom", 
            "Linea",
            "Manta",
            "Optimism",
            "Scroll", 
            "Soneium", 
            "Taiko",
            "Unichain", 
            "Zero",
            "Zksync",
            "Zora",
            "Avalanche", 
            "Bsc",
            "Gravity",
            "Opbnb", 
            "Polygon", 

            "Sepolia", 
            "Monad",
            "Aptos",
            "Movement",

            "Solana", 
            "Solana_Devnet",
            "Solana_Testnet", 
        };

        public MainViewModel(Page page)
        {
            _page = page; // Сохраняем страницу
        }

        [RelayCommand]
        private async void EthPrice()
        {
            EthPriceLbl = "Checking...";
            var res = await CoinGecco.CGPrice();
            EthPriceLbl = res.ToString();

        }
        [RelayCommand]
        private async void GetNative()
        {
            if (!Address.Contains("\r") && !Address.Contains(","))
            {
                Debug.WriteLine($"Single address: {Address}");
                Output = $"Checking by {SelectedRpc}";
                decimal res = 0;
                if (SelectedRpc.Contains("solana"))
                    res = await new SolTools().GetSolanaBalance(SelectedRpc, Address.Trim());
                else
                    res = await new EvmTools().GetEvmBalance(SelectedRpc, Address.Trim());
                Output = res.ToString();
                Debug.WriteLine($"Result for single address: {res}");
            }
            else
            {
                var addresses = new List<string>();
                if (Address.Contains("\r"))
                    addresses = Address.Split('\r', StringSplitOptions.RemoveEmptyEntries).ToList();
                else if (Address.Contains(","))
                    addresses = Address.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList();
                else
                {
                    Output = $"unexpected divider [{Address}]";
                    Debug.WriteLine($"Unexpected divider: {Address}");
                    return;
                }

                
                Output = "";
                foreach (var address in addresses)
                {
                    var trimmedAddress = address.Trim();
                   
                    decimal res = 0;
                    if (SelectedRpc.Contains("solana"))
                    {
                        Debug.WriteLine($"Processing SOL address: {trimmedAddress}");
                        res = await new SolTools().GetSolanaBalance(SelectedRpc, trimmedAddress);
                    }
                    else
                    {
                        Debug.WriteLine($"Processing EVM address: {trimmedAddress}");
                        res = await new EvmTools().GetEvmBalance(SelectedRpc, trimmedAddress);
                    }
                        

                    //var res = await new EvmTools().GetEvmBalance(SelectedRpc, trimmedAddress);
                    Output += $"{trimmedAddress}\t {res.ToString()} \r";
                    Debug.WriteLine($"Result for {trimmedAddress}: {res}");
                }
                //await _page.DisplayAlert("Balance", Output, "OK");
            }
        }
        [RelayCommand]
        private async void GoMain()
        {
            await Shell.Current.GoToAsync("//MainPage");
        }

        [RelayCommand]
        private async void GoBack()
        {
            await Shell.Current.GoToAsync("//MainPage");
            //await Navigation.PopAsync();
        }


        public void RpcChanged()
        {
            SelectedRpc = z3nApp.Rpc.Get(Rpc);
            //Output = $"Selected RPC: {Rpc}";
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






    }
}
