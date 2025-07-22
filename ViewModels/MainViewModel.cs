
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace z3nApp.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {

        [ObservableProperty]
        private string ethPriceLbl = "sup";
        [ObservableProperty]
        private string address = null;
        [ObservableProperty]
        private string native = null;
        [RelayCommand]

        private async void EthPrice()
        {
            var res = await CoinGecco.CGPrice();
            EthPriceLbl = res.ToString();

        }
        [RelayCommand]
        private async void GetNative()
        {

            Native = "Checking";
            var rpc = "https://eth.llamarpc.com";
            var res = await new EvmTools().GetEvmBalance(rpc, Address);
            Native = res.ToString();
            

        }

    }
}
