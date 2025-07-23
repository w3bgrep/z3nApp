namespace z3nApp;

public partial class Balances : ContentPage
{
	public Balances()
	{
		InitializeComponent();
	}

    private async void GoBack(object sender, EventArgs e)
    {
        //await Shell.Current.GoToAsync("//MainPage");
        await Navigation.PopAsync();
    }
}