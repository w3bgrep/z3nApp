using z3nApp.ViewModels;


namespace z3nApp
{
    public partial class MainPage : ContentPage
    {
        int count = 0;

        public MainPage()
        {
            InitializeComponent();
            BindingContext = new MainViewModel(this);
        }
        private async void GoToKiller(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//Killer");
        }
        private async void GoToBalances(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//Balance");
        }
        private async void GoWalletTools(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//WalletTools");
        }

    }

}
