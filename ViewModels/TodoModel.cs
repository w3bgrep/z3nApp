using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using z3nApp.Data;
using z3nApp.Models;

namespace z3nApp.ViewModels
{
    public partial class TodoModel : ObservableObject
    {
        private readonly TodoDatabase _database;

        [ObservableProperty]
        private string dirrection = null;

        [ObservableProperty]
        private ObservableCollection<TodoItem> todoItems;

        [ObservableProperty]
        private string newTaskTitle;

        private readonly Page _page;

        public TodoModel(Page page)
        {
            _page = page; // Сохраняем страницу
            _database = new TodoDatabase();
        }

        [RelayCommand]
        private async void LoadItems()
        {
            var items = await _database.GetItems();
            TodoItems = new ObservableCollection<TodoItem>(items);
        }
        [RelayCommand]
        private async void AddItem()
        {
            if (!string.IsNullOrEmpty(NewTaskTitle))
            {
                var newItem = new TodoItem { Title = NewTaskTitle, IsCompleted = false };
                await _database.SaveItem(newItem);
                NewTaskTitle = "";
                LoadItems();

            }
        
        }
        [RelayCommand]
        private async void DeleteItem(TodoItem item)
        {
            await _database.DeleteItem(item);
            LoadItems();
        
        }
        [RelayCommand]
        private async void GoBack()
        {
            await Shell.Current.GoToAsync("//MainPage");
        }
    }
}
