using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Org.BouncyCastle.Pqc.Crypto.Lms;
using SimpleBase;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.NetworkInformation;

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
            "addresses",
            "wallets",
            "proxy",
            "twitter",
            "discord",
            "google",
            "github",
            "native",
            "balance",
            "mail",
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

        [ObservableProperty]
        private ObservableCollection<string> walletTypes = new ObservableCollection<string>
        {
            "bip39",
            "secp256k1",
            "base58",
            "secp256k1 from bip39",
            "base58 from bip39",
        };

        [ObservableProperty]
        private string pin = null;

        [ObservableProperty]
        private string walletType = null;

        [ObservableProperty]
        private ObservableCollection<string> addressTypes = new ObservableCollection<string>
        {
            "EvmFromKey",
            "EvmFromSeed",
            "SolFromKey",
            "SolFromSeed",
        };
        [ObservableProperty]
        private string addressType = null;

        [ObservableProperty]
        private string socialMask = null;
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
            switch (Operation)
            {
                case "import":
                    await Import();
                    break;
                default:
                    return; 
            }

        }

        private async Task Import() 
        {
            var _logger = new Logger(true);
            var sql = new Sql(true);         
            var input = InputEdit.Trim().Split('\r').ToList();
            int id = 1;
            var sol = new SolTools();
            var evm = new EvmTools();
            switch (ImportType)
            {
                case "proxy":
                    await sql.AddRange(input.Count, "proxy");
                    foreach (var item in input)
                    {                     
                        await sql.Upd($"proxy = '{item}'", id, "proxy");
                        id++;
                    }
                    break;
                case "wallets":

                    await sql.AddRange(input.Count, "wallets");
                    if (WalletType == "bip39")
                    {
                        foreach (var item in input)
                        {
                            _logger.Send($"processing [{item}]");
                            string bip39 = item.Trim();
                            SAFU safu = new SAFU(Pin);
                            string encoded_bip39 = safu.Encode(bip39, id.ToString());
                            await sql.Upd($"bip39 = '{encoded_bip39}'", id, "wallets");

                            id++;
                        }
                    }
                    else if (WalletType == "secp256k1")
                    {
                        foreach (var item in input)
                        {
                            string secp256k1 = item;
                            SAFU safu = new SAFU(Pin);
                            string encoded_secp256k1 = safu.Encode(secp256k1, id.ToString());
                            await sql.Upd($"secp256k1 = '{encoded_secp256k1}'", id, "wallets");
                            id++;
                        }
                    }
                    else if (WalletType == "base58")
                    {
                        foreach (var item in input)
                        {
                            string base58 = item;
                            SAFU safu = new SAFU(Pin);
                            string encoded_base58 = safu.Encode(base58, id.ToString());
                            await sql.Upd($"base58 = '{encoded_base58}'", id, "wallets");
                            id++;
                        }
                    }
                    else if (WalletType == "secp256k1 from bip39")
                    {
                        
                    }
                    else if (WalletType == "base58 from bip39")
                    {

                    }
                    break;
                case "addresses":
                    await sql.AddRange(input.Count, "addresses");
                    if (AddressType == "EvmFromKey")
                    {
                        foreach (var item in input)
                        {
                            string secp256k1 = item;
                            string address = await evm.AddressFromKey(secp256k1);
                            await sql.Upd($"evm = '{address}'", id, "addresses");
                            id++;
                        }
                    }
                    else if (AddressType == "EvmFromSeed")
                    {
                        foreach (var item in input)
                        {
                            string bip39 = item;
                            string address = await evm.AddressFromSeed(bip39);
                            await sql.Upd($"evm = '{address}'", id, "addresses");
                            id++;
                        }
                    }
                    else if (AddressType == "SolFromKey")
                    {
                        foreach (var item in input)
                        {
                            string base58 = item;
                            string address = await sol.AddressFromKey(base58);
                            await sql.Upd($"solana = '{address}'", id, "addresses");
                            id++;
                        }
                    }
                    else if (AddressType == "SolFromSeed")
                    {
                        foreach (var item in input)
                        {
                            string bip39 = item;
                            string address = await sol.AddressFromSeed(bip39);
                            await sql.Upd($"solana = '{address}'", id, "addresses");
                            id++;
                        }
                    }
                    break;


                case "Twitter":
                case "Discord":
                case "Google":










                default:
                    return;

            }





        }








    }
}
