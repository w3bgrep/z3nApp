
using SQLite;

namespace z3nApp
{
    public class Wallets
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string Seed { get; set; }
        public string EvmKey { get; set; }
        public string EvmAddress { get; set; }




    }

    public class Sql 
    {
        private readonly SQLiteAsyncConnection _connection;
        public Sql()
        {
            string path = Path.Combine(FileSystem.AppDataDirectory, "default.db");
            _connection = new SQLiteAsyncConnection(path);
            _connection.CreateTableAsync<Wallets>().Wait();
        
        }


        public Task<List<Wallets>> GetWallets()
        {
            return _connection.Table<Wallets>().ToListAsync();        
        }

        public Task<int> ImportWallet(Wallets wallet)
        {
            if (wallet.Id == 0)
                return _connection.InsertAsync(wallet);
            else
                return _connection.UpdateAsync(wallet);
        }

        public Task<int> DeleteWallet(Wallets wallet) 
        {
           return _connection.DeleteAsync(wallet);
        }
        public Task<int> Raw(int id, string newName)
        {
            return _connection.ExecuteAsync("UPDATE Wallets SET Name = ? WHERE Id = ?", newName, id);
        }




    }








}
