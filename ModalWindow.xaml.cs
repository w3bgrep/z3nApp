namespace z3nApp;

public partial class ModalWindow : ContentPage
{
	public ModalWindow()
	{
		InitializeComponent();

	}


    private async void OnClose(object sender, EventArgs e)
    {
        //await Shell.Current.GoToAsync("//MainPage");
        //await Navigation.PopAsync();
        await Navigation.PopModalAsync();
    }
}