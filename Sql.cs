using Dapper;
using Microsoft.Data.Sqlite;
using System;
using System.Diagnostics;
using System.Net.NetworkInformation;
using static SQLite.TableMapping;


namespace z3nApp
{
    public class Sql
    {
        private readonly SqliteConnection _connection;
        private readonly string _tableName;
        private readonly Logger _logger;
        public Sql(bool log = false)
        {
            string path = Path.Combine(FileSystem.AppDataDirectory, "default.db");
            Debug.WriteLine(path);
            _connection = new SqliteConnection($"Data Source={path}");
            _connection.Open();
            _logger = new Logger(log);
        }

        public Sql(string tableName, bool log = false )
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


        public async Task<int> Upd(string toUpd, object id, string tableName = null, string where = null, bool last = false)
        {
            _logger.Send(toUpd);
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
                query = $"UPDATE {tableName} SET {toUpd} WHERE id = {id}";
               
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
                string formattedQuery = query;
                foreach (var param in parameters.ParameterNames)
                {
                    var value = parameters.Get<dynamic>(param)?.ToString() ?? "NULL";
                    formattedQuery = formattedQuery.Replace($"@{param}", $"'{value}'");
                }

                Debug.WriteLine($"Error: {ex.Message}");
                Debug.WriteLine($"Executed query: {formattedQuery}");
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

        public async Task AddRange(int range, string tableName = null)
        {
            if (tableName == null) tableName = _tableName;
            if (tableName == null) throw new Exception("TableName is null");

            string query = $@"SELECT COALESCE(MAX(id), 0) FROM {tableName};";

            int current = await _connection.ExecuteScalarAsync<int>(query, commandType: System.Data.CommandType.Text);
            _logger.Send($"{query}-{current}");
            
            for (int currentId = current + 1; currentId <= range; currentId++)
            {
                _connection.ExecuteAsync($@"INSERT INTO {tableName} (id) VALUES ({currentId}) ON CONFLICT DO NOTHING;");
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