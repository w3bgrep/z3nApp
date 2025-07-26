using SQLite;
using System.Diagnostics;
using z3nApp.Models;

namespace z3nApp.Data
{
    public class TodoDatabase
    {
        private readonly SQLiteAsyncConnection _connection;

        public TodoDatabase()
        {
            string dbPath = Path.Combine(FileSystem.AppDataDirectory, "Todo.db");
            Debug.WriteLine(dbPath);
            _connection = new SQLiteAsyncConnection(dbPath);
            _connection.CreateTableAsync<TodoItem>().Wait();
        }

        public Task<List<TodoItem>> GetItems()
        {
            return _connection.Table<TodoItem>().ToListAsync();
        }
        public Task<int> SaveItem(TodoItem item)
        {
            if (item.Id == 0)
                return _connection.InsertAsync(item);
            else
                return _connection.UpdateAsync(item);
        }
        public Task<int> DeleteItem(TodoItem item)
        {
            return _connection.DeleteAsync(item);
        }

    }
}
