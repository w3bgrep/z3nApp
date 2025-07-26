namespace z3nApp;
using z3nApp.ViewModels;
public partial class DbManager : ContentPage
{
	public DbManager()
	{
		InitializeComponent();
        BindingContext = new DbManagerModel(this);
    }
}