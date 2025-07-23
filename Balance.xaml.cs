namespace z3nApp;
using z3nApp.ViewModels;

public partial class Balance : ContentPage
{
	public Balance()
	{
		InitializeComponent();
        BindingContext = new MainViewModel(this);
    }

    private async void GoBack(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//MainPage");
    }
    private void OnRpcSelectedIndexChanged(object sender, EventArgs e)
    {
        var viewModel = (MainViewModel)BindingContext;
        viewModel.RpcChanged();
    }
}