using Dapper;
using Microsoft.Data.Sqlite;
using System.Diagnostics;
using System.Net.NetworkInformation;
using static SQLite.TableMapping;


namespace z3nApp
{
    public class Sql
    {
        private readonly SqliteConnection _connection;
        private readonly string _tableName;
        public Sql()
        {
            string path = Path.Combine(FileSystem.AppDataDirectory, "default.db");
            Debug.WriteLine(path);
            _connection = new SqliteConnection($"Data Source={path}");
            _connection.Open();

        }

        public Sql(string tableName = "")
        {
            string path = Path.Combine(FileSystem.AppDataDirectory, "default.db");
            Debug.WriteLine(path);
            _connection = new SqliteConnection($"Data Source={path}");
            _tableName = tableName;
            _connection.Open();
            TblCreate(tableName);
        }

        public async Task<int> Raw(string rawQuery)
        {
            return await _connection.ExecuteAsync(rawQuery);
        }

        //public async Task<string> Get(string columns, string acc)
        //{

        //    string query = $"SELECT {columns} FROM TableName WHERE acc = @acc";
        //    return await _connection.ExecuteScalarAsync<string>(query, new { acc });
        //}

        public async Task<int> Upd(string toUpd, object id, string tableName = null, string where = null, bool last = false)
        {
            var parameters = new DynamicParameters();
            if (tableName == null) tableName = _tableName;
            if (tableName == null) throw new Exception("TableName is null");

            toUpd = QuoteName(toUpd, true);
            tableName = QuoteName(tableName);

            if (last)
            {
                toUpd += ", last = @lastTime";
                parameters.Add("lastTime", DateTime.UtcNow.ToString("MM-ddTHH:mm"));
            }

            string query;
            if (string.IsNullOrEmpty(where))
            {
                query = $"UPDATE {tableName} SET {toUpd} WHERE id = @id";
                parameters.Add("id", $"{id}");
            }
            else
            {
                query = $"UPDATE {tableName} SET {toUpd} WHERE {where}";
            }

            try
            {
                return await _connection.ExecuteAsync(query, parameters, commandType: System.Data.CommandType.Text);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error: {ex.Message}");
                throw;
            }
        }
        public async Task Upd(List<string> toWrite, string tableName = null, string where = null, bool last = false)
        {
           int id = 0;
            foreach (var item in toWrite)
            {
                await Upd(item, id, tableName, where, last);
                id++;
            }
        }
  
        public async Task<int> UpdAES(string column, string item, object id, string pin, string tableName = null, string where = null)
        {
            var parameters = new DynamicParameters();
            if (tableName == null) tableName = _tableName;
            if (tableName == null) throw new Exception("TableName is null");

            column = QuoteName(column, true);
            tableName = QuoteName(tableName);
            SAFU safu = new SAFU(pin);
            safu.id = id.ToString();
            string encodedItem = safu.Encode(item);

            string query;
            if (string.IsNullOrEmpty(where))
            {
                query = $"UPDATE {tableName} SET {column} = @item WHERE id = @id";
                parameters.Add("item", $"{item}");
                parameters.Add("id", $"{id}");
            }
            else
            {
                query = $"UPDATE {tableName} SET {column} = @item WHERE {where}";
                parameters.Add("item", $"{item}");
            }

            try
            {
                return await _connection.ExecuteAsync(query, parameters, commandType: System.Data.CommandType.Text);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error: {ex.Message}");
                throw;
            }
        }
        public async Task UpdAES(string column, List<string> items, string pin, string tableName = null, string where = null)
        {
            SAFU safu = new SAFU(pin);
            int id = 0;
            foreach (var item in items)
            {
                safu.id = id.ToString();
                string encodedItem = safu.Encode(item);
                await UpdAES(column, item, id, pin, tableName, where);
                id++;
            }
        }
   
        public async Task<string> Get(string toGet, string id, string tableName = null, string where = null)
        {
            var parameters = new DynamicParameters();
            if (tableName == null) tableName = _tableName;
            if (tableName == null) throw new Exception("TableName is null");

            toGet = QuoteName(toGet, true); 
            tableName = QuoteName(tableName);



            string query;
            if (string.IsNullOrEmpty(where))
            {
                query = $"SELECT {toGet} FROM {tableName} WHERE id = @id";
                parameters.Add("id", id); 
            }
            else
            {
                query = $"SELECT {toGet} FROM {tableName} WHERE {where}";
            }

            try
            {
                return await _connection.ExecuteScalarAsync<string>(query, parameters, commandType: System.Data.CommandType.Text);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error: {ex.Message}");
                throw;
            }
        }
        
        
        
        
        
        private string QuoteName(string name, bool isColumnList = false)
        {
            if (isColumnList)
            {
                name = name.Trim().TrimEnd(',');
                var parts = name.Split(',').Select(p => p.Trim()).ToList();
                var result = new List<string>();

                foreach (var part in parts)
                {
                    int equalsIndex = part.IndexOf('=');
                    if (equalsIndex > 0)
                    {
                        string columnName = part.Substring(0, equalsIndex).Trim();
                        string valuePart = part.Substring(equalsIndex).Trim();
                        result.Add($"\"{columnName}\" {valuePart}");
                    }
                    else
                    {
                        result.Add($"\"{part}\"");
                    }
                }
                return string.Join(", ", result);
            }
            return $"\"{name.Replace("\"", "\"\"")}\"";
        }
        private void TblCreate(string tableName)
        {
            if (string.IsNullOrEmpty(tableName)) return;
            tableName = tableName.ToLower();

            string rawQuery = tableName switch
            {
                "twitter" or "discord" or "google" or "github" => $@"
                    CREATE TABLE IF NOT EXISTS {QuoteName(tableName)} (
                        id INTEGER PRIMARY KEY,
                        login TEXT DEFAULT '',
                        password TEXT DEFAULT '',
                        token TEXT DEFAULT '',
                        secret TEXT DEFAULT '',
                        backup TEXT DEFAULT '',
                        extra TEXT DEFAULT ''
                    )",
                "proxy" => $@"
                    CREATE TABLE IF NOT EXISTS {QuoteName(tableName)} (
                        id INTEGER PRIMARY KEY,
                        proxy TEXT DEFAULT '',
                        protocol TEXT DEFAULT '',
                        login TEXT DEFAULT '',
                        pass TEXT DEFAULT '',
                        ip TEXT DEFAULT '',
                        port TEXT DEFAULT ''
                    )",
                "wallets" => $@"
                    CREATE TABLE IF NOT EXISTS {QuoteName(tableName)} (
                        id INTEGER PRIMARY KEY,
                        bip39 TEXT DEFAULT '',
                        secp256k1 TEXT DEFAULT '',
                        base58 TEXT DEFAULT ''
                    )",
                "addresses" => $@"
                    CREATE TABLE IF NOT EXISTS {QuoteName(tableName)} (
                        id INTEGER PRIMARY KEY,
                        evm TEXT DEFAULT '',
                        solana TEXT DEFAULT '',
                        sui TEXT DEFAULT '',
                        xion TEXT DEFAULT ''
                    )",
                "api" => $@"
                    CREATE TABLE IF NOT EXISTS {QuoteName(tableName)} (
                        id TEXT PRIMARY KEY,
                        key TEXT DEFAULT '',
                        secret TEXT DEFAULT '',
                        pass TEXT DEFAULT '',
                        wl TEXT DEFAULT ''
                    )",
                _ => ""
            };

            if (!string.IsNullOrEmpty(rawQuery))
            {
                _connection.Execute(rawQuery);
            }
        }


    }
}