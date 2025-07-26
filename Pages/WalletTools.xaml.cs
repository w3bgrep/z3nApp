namespace z3nApp;
using z3nApp.ViewModels;

public partial class WalletTools : ContentPage
{
	public WalletTools()
	{
		InitializeComponent();
        BindingContext = new WalletToolsModel(this);
    }
}