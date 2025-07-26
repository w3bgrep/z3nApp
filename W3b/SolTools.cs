using Newtonsoft.Json.Linq;
using SimpleBase;
using System.Globalization;
using System.Numerics;
using System.Text;
using Solnet.Wallet;
using Solnet.Wallet.Utilities;


namespace z3nApp
{
    public class SolTools
    {
        public async Task<decimal> GetSolanaBalance(string rpc, string address)
        {
            string jsonBody = $@"{{ ""jsonrpc"": ""2.0"", ""method"": ""getBalance"", ""params"": [""{address}""], ""id"": 1 }}";

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
                    BigInteger balanceLamports = json["result"]?["value"]?.ToObject<BigInteger>() ?? 0;
                    decimal balance = (decimal)balanceLamports / 1_000_000_000m; // Convert lamports to SOL
                    return balance;
                }
            }
        }

        public async Task<decimal> GetSplTokenBalance(string rpc, string walletAddress, string tokenMint)
        {
            string jsonBody = $@"{{ ""jsonrpc"": ""2.0"", ""method"": ""getTokenAccountsByOwner"", ""params"": [""{walletAddress}"", {{""mint"": ""{tokenMint}""}}, {{""encoding"": ""jsonParsed""}}], ""id"": 1 }}";

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

                    var accounts = json["result"]?["value"] as JArray;
                    if (accounts == null || accounts.Count == 0)
                        return 0m;

                    var tokenData = accounts[0]?["account"]?["data"]?["parsed"]?["info"];
                    if (tokenData == null)
                        return 0m;

                    string amount = tokenData["tokenAmount"]?["uiAmountString"]?.ToString();
                    if (string.IsNullOrEmpty(amount))
                        return 0m;

                    return decimal.Parse(amount, CultureInfo.InvariantCulture);
                }
            }
        }

        public async Task<decimal> SolFeeByTx(string transactionHash, string rpc = null, string tokenDecimal = "9")
        {
            if (string.IsNullOrEmpty(rpc)) rpc = "https://api.mainnet-beta.solana.com";

            string jsonBody = $@"{{ ""jsonrpc"": ""2.0"", ""method"": ""getTransaction"", ""params"": [""{transactionHash}"", {{""encoding"": ""jsonParsed"", ""maxSupportedTransactionVersion"": 0}}], ""id"": 1 }}";

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
                    string feeLamports = json["result"]?["meta"]?["fee"]?.ToString() ?? "0";
                    BigInteger balanceRaw = BigInteger.Parse(feeLamports);
                    decimal decimals = (decimal)Math.Pow(10, double.Parse(tokenDecimal, CultureInfo.InvariantCulture));
                    decimal balance = (decimal)balanceRaw / decimals;
                    return balance;
                }
            }
        }

        public async Task<string> KeyFromSeed(string mnemonic)
        {
            Wallet wallet = new Wallet(mnemonic);
            Account account = wallet.GetAccount(0);
            byte[] privateKeyBytes = account.PrivateKey;
            string privateKeyBase58 = Base58.Bitcoin.Encode(privateKeyBytes.AsSpan());
            string publicKeyBase58 = account.PublicKey.Key;
            return privateKeyBase58;
        }
        public async Task<string> AddressFromSeed(string mnemonic)
        {
            Wallet wallet = new Wallet(mnemonic);
            Account account = wallet.GetAccount(0);
            byte[] privateKeyBytes = account.PrivateKey;
            string privateKeyBase58 = Base58.Bitcoin.Encode(privateKeyBytes.AsSpan());
            string publicKeyBase58 = account.PublicKey.Key;
            return publicKeyBase58;
        }
        public async Task<string> AddressFromKey(string privateKeyBase58)
        {
            var b58 = privateKeyBase58;
            var fullKey = Encoders.Base58.DecodeData(b58);
            if (fullKey.Length != 64)
            {
                throw new Exception("Key must be 64 bytes!");
            }
            var pubKey = fullKey.Skip(32).Take(32).ToArray();
            var address = Encoders.Base58.EncodeData(pubKey);
            return address;
        }
        public async Task<string[]> AccFromSeed(string mnemonic)
        {
            Wallet wallet = new Wallet(mnemonic);
            Account account = wallet.GetAccount(0);
            byte[] privateKeyBytes = account.PrivateKey;
            string privateKeyBase58 = Base58.Bitcoin.Encode(privateKeyBytes.AsSpan());
            string publicKeyBase58 = account.PublicKey.Key;
            return new string[] { privateKeyBase58, publicKeyBase58 };
        }

    }
}
