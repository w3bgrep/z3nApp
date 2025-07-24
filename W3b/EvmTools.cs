using Newtonsoft.Json.Linq;
using System.Globalization;
using System.Numerics;
using System.Text;

using Nethereum.HdWallet;
using Nethereum.Web3.Accounts;

namespace z3nApp
{
    public class EvmTools
    {

        public async Task<decimal> GetEvmBalance(string rpc, string address)
        {
            string jsonBody = $@"{{ ""jsonrpc"": ""2.0"", ""method"": ""eth_getBalance"", ""params"": [""{address}"", ""latest""], ""id"": 1 }}";

            using (var client = new HttpClient())
            {
                var request = new HttpRequestMessage
                {
                    Method = HttpMethod.Post,
                    RequestUri = new Uri(rpc),
                    Content = new StringContent(jsonBody, Encoding.UTF8, "application/json")
                };

                using (var response = await client.SendAsync(request))
                {
                    response.EnsureSuccessStatusCode();
                    var body = await response.Content.ReadAsStringAsync();

                    var json = JObject.Parse(body);
                    string hexBalance = json["result"]?.ToString().Replace("0x", "") ?? "0";

                    if (string.IsNullOrEmpty(hexBalance) || hexBalance == "0")
                    {
                        return 0m;
                    }
                    BigInteger balanceWei = BigInteger.Parse("0" + hexBalance, NumberStyles.AllowHexSpecifier);
                    decimal balance = (decimal)balanceWei / 1000000000000000000m;

                    //BigInteger balanceWei = BigInteger.Parse(hexBalance, NumberStyles.AllowHexSpecifier);
                    //decimal balance = (decimal)balanceWei / 1_000_000_000_000_000_000m;
                    return balance;
                }
            }
        }
        public async Task<decimal> GetErc20Balance(string tokenContract, string rpc, string address, string tokenDecimal = "18")
        {
            string data = "0x70a08231000000000000000000000000" + address.Replace("0x", "");
            string jsonBody = $@"{{ ""jsonrpc"": ""2.0"", ""method"": ""eth_call"", ""params"": [{{ ""to"": ""{tokenContract}"", ""data"": ""{data}"" }}, ""latest""], ""id"": 1 }}";

            using (var client = new HttpClient())
            {
                var request = new HttpRequestMessage
                {
                    Method = System.Net.Http.HttpMethod.Post,
                    RequestUri = new Uri(rpc),
                    Content = new System.Net.Http.StringContent(jsonBody, Encoding.UTF8, "application/json")
                };

                using (var response = await client.SendAsync(request))
                {
                    response.EnsureSuccessStatusCode();
                    var body = await response.Content.ReadAsStringAsync();

                    var json = JObject.Parse(body);
                    string hexBalance = json["result"]?.ToString().Replace("0x", "") ?? "0";
                    BigInteger balanceRaw = BigInteger.Parse(hexBalance, NumberStyles.AllowHexSpecifier);
                    decimal decimals = (decimal)Math.Pow(10, double.Parse(tokenDecimal, CultureInfo.InvariantCulture));
                    decimal balance = (decimal)balanceRaw / decimals;
                    return balance;
                }
            }
        }

        public async Task<decimal> GetErc721Balance(string tokenContract, string rpc, string address)
        {
            string data = "0x70a08231000000000000000000000000" + address.Replace("0x", "").ToLower();
            string jsonBody = $@"{{ ""jsonrpc"": ""2.0"", ""method"": ""eth_call"", ""params"": [{{ ""to"": ""{tokenContract}"", ""data"": ""{data}"" }}, ""latest""], ""id"": 1 }}";

            using (var client = new HttpClient())
            {
                var request = new HttpRequestMessage
                {
                    Method = System.Net.Http.HttpMethod.Post,
                    RequestUri = new Uri(rpc),
                    Content = new System.Net.Http.StringContent(jsonBody, Encoding.UTF8, "application/json")
                };

                using (var response = await client.SendAsync(request))
                {
                    response.EnsureSuccessStatusCode();
                    var body = await response.Content.ReadAsStringAsync();

                    var json = JObject.Parse(body);
                    string hexBalance = json["result"]?.ToString().Replace("0x", "") ?? "0";
                    BigInteger balanceRaw = BigInteger.Parse(hexBalance, NumberStyles.AllowHexSpecifier);
                    return (decimal)balanceRaw;
                }
            }
        }

        public async Task<decimal> GetErc1155Balance(string tokenContract, string tokenId, string rpc, string address)
        {
            string data = "0x00fdd58e" + address.Replace("0x", "").ToLower().PadLeft(64, '0') + BigInteger.Parse(tokenId).ToString("x").PadLeft(64, '0');
            string jsonBody = $@"{{ ""jsonrpc"": ""2.0"", ""method"": ""eth_call"", ""params"": [{{ ""to"": ""{tokenContract}"", ""data"": ""{data}"" }}, ""latest""], ""id"": 1 }}";

            using (var client = new HttpClient())
            {
                var request = new HttpRequestMessage
                {
                    Method = System.Net.Http.HttpMethod.Post,
                    RequestUri = new Uri(rpc),
                    Content = new System.Net.Http.StringContent(jsonBody, Encoding.UTF8, "application/json")
                };

                using (var response = await client.SendAsync(request))
                {
                    response.EnsureSuccessStatusCode();
                    var body = await response.Content.ReadAsStringAsync();

                    var json = JObject.Parse(body);
                    string hexBalance = json["result"]?.ToString().Replace("0x", "") ?? "0";
                    BigInteger balanceRaw = BigInteger.Parse(hexBalance, NumberStyles.AllowHexSpecifier);
                    return (decimal)balanceRaw;
                }
            }
        }

        public async Task<string> KeyFromSeed(string mnemonic)
        {
            Wallet wallet = new Wallet(mnemonic, null);
            Account account = wallet.GetAccount(0);
            return account.PrivateKey;
        }

        public async Task<string> AddressFromSeed(string mnemonic)
        {
            Wallet wallet = new Wallet(mnemonic, null);
            Account account = wallet.GetAccount(0);
            return account.Address;
        }

        public async Task<string> AddressFromKey(string privateKey)
        {
            Account account = new Account(privateKey);
            return account.Address;
        }

        public async Task<string[]> AccFromSeed(string mnemonic)
        {
            Wallet wallet = new Wallet(mnemonic, null);
            Account account = wallet.GetAccount(0);
            return new string[] { account.PrivateKey, account.Address };
        }


    }
}
