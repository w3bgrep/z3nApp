using z3nApp.ViewModels;


namespace z3nApp
{
    public partial class MainPage : ContentPage
    {
        int count = 0;

        public MainPage()
        {
            InitializeComponent();
            BindingContext = new MainViewModel();
        }
        private async void GoToKiller(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//Killer");
        }

        private async void OnModelOpen(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new ModalWindow());
        }

    }

}
