namespace z3nApp;
using z3nApp.ViewModels;

public partial class ToDo : ContentPage
{
	public ToDo()
	{
		InitializeComponent();
        BindingContext = new TodoModel(this);
    }
}