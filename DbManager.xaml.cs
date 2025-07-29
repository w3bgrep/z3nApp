namespace z3nApp;
using z3nApp.ViewModels;
public partial class DbManager : ContentPage
{
	public DbManager()
	{
		InitializeComponent();
        BindingContext = new DbManagerModel(this);
    }

    private void TablePicked(object sender, EventArgs e)
    {
        var viewModel = (DbManagerModel)BindingContext;
        new Sql(viewModel.ImportType);
        viewModel.setInfo();
    }
}