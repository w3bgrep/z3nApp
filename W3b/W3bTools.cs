using Newtonsoft.Json.Linq;
using Solnet.Wallet;
using Solnet.Wallet.Bip39;
using Solnet.Wallet.Utilities;
using SimpleBase;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Chaos.NaCl;

namespace z3nApp
{





   
    public class CoinGecco
    {
        private readonly string _apiKey = "CG-TJ3DRjP93bTSCto6LiPbMgaV";
        public async Task<string> CoinInfo(string CGid = "ethereum")
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage
            {
                Method = System.Net.Http.HttpMethod.Get,
                RequestUri = new Uri($"https://api.coingecko.com/api/v3/coins/{CGid}"),
                Headers =
                {
                    { "accept", "application/json" },
                    { "x-cg-demo-api-key", _apiKey },
                },
            };

            using (var response = await client.SendAsync(request))
            {
                response.EnsureSuccessStatusCode();
                var body = await response.Content.ReadAsStringAsync();
                return body;
            }
        }
        public static async Task<decimal> CGPrice(string CGid = "ethereum", [CallerMemberName] string callerName = "")
        {
            try
            {
                string result = await new CoinGecco().CoinInfo(CGid);
                var json = JObject.Parse(result);
                JToken usdPriceToken = json["market_data"]?["current_price"]?["usd"];

                if (usdPriceToken == null)
                {
                    return 0m;
                }

                decimal usdPrice = usdPriceToken.Value<decimal>();
                return usdPrice;
            }
            catch (Exception ex)
            {
                var stackFrame = new System.Diagnostics.StackFrame(1);
                var callingMethod = stackFrame.GetMethod();
                string method = string.Empty;
                if (callingMethod != null)
                    method = $"{callingMethod.DeclaringType.Name}.{callerName}";
                throw new Exception(ex.Message + $"\n{method}");
            }
        }

    }

    public class DexScreener
    {

        public async Task<string> CoinInfo(string contract, string chain)
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage
            {
                Method = System.Net.Http.HttpMethod.Get,
                RequestUri = new Uri($"https://api.dexscreener.com/tokens/v1/{chain}/{contract}"),
                Headers =
                {
                    { "accept", "application/json" },
                },
            };

            using (var response = await client.SendAsync(request))
            {
                response.EnsureSuccessStatusCode();
                var body = await response.Content.ReadAsStringAsync();
                return body;
            }
        }

    }


    public static class W3bTools 
    
    {

        public static decimal EvmNative(string rpc, string address)
        {
            return new EvmTools().GetEvmBalance(rpc, address).GetAwaiter().GetResult();
        }
        public static decimal ERC20(string tokenContract, string rpc, string address, string tokenDecimal = "18")
        {
            return new EvmTools().GetErc20Balance(tokenContract, rpc, address, tokenDecimal).GetAwaiter().GetResult();
        }
        public static decimal ERC721(string tokenContract, string rpc, string address)
        {
            return new EvmTools().GetErc721Balance(tokenContract, rpc, address).GetAwaiter().GetResult();
        }
        public static decimal ERC1155(string tokenContract, string tokenId, string rpc, string address)
        {
            return new EvmTools().GetErc1155Balance(tokenContract, tokenId, rpc, address).GetAwaiter().GetResult();
        }

        public static decimal SolNative(string address, string rpc = "https://api.mainnet-beta.solana.com")
        {
            return new SolTools().GetSolanaBalance(rpc, address).GetAwaiter().GetResult();
        }
        public static decimal SPL(string tokenMint, string walletAddress, string rpc = "https://api.mainnet-beta.solana.com")
        {
            return new SolTools().GetSplTokenBalance(rpc, walletAddress, tokenMint).GetAwaiter().GetResult();
        }
        public static decimal SolTxFee(string transactionHash, string rpc = null, string tokenDecimal = "9")
        {
            return new SolTools().SolFeeByTx(transactionHash, rpc, tokenDecimal).GetAwaiter().GetResult();
        }


        public static decimal CGPrice(string CGid = "ethereum",[CallerMemberName] string callerName = "")
        {
            try
            {
                string result = new CoinGecco().CoinInfo(CGid).GetAwaiter().GetResult();

                var json = JObject.Parse(result);
                JToken usdPriceToken = json["market_data"]?["current_price"]?["usd"];

                if (usdPriceToken == null)
                {
                    return 0m;
                }

                decimal usdPrice = usdPriceToken.Value<decimal>();
                return usdPrice;
            }
            catch (Exception ex)
            {
                var stackFrame = new System.Diagnostics.StackFrame(1);
                var callingMethod = stackFrame.GetMethod();
                string method = string.Empty;
                if (callingMethod != null)
                    method = $"{callingMethod.DeclaringType.Name}.{callerName}";
                throw new Exception(ex.Message + $"\n{method}");
            }
        }
        public static decimal DSPrice(string contract = "So11111111111111111111111111111111111111112", string chain = "solana",[CallerMemberName] string callerName = "")
        {
            try
            {
                string result = new DexScreener().CoinInfo(contract, chain).GetAwaiter().GetResult();

                var json = JArray.Parse(result);
                JToken priceToken = json.FirstOrDefault()?["priceNative"];

                if (priceToken == null)
                {
                    return 0m;
                }

                return priceToken.Value<decimal>();
            }
            catch (Exception ex)
            {
                var stackFrame = new System.Diagnostics.StackFrame(1);
                var callingMethod = stackFrame.GetMethod();
                string method = string.Empty;
                if (callingMethod != null)
                    method = $"{callingMethod.DeclaringType.Name}.{callerName}";
                throw new Exception(ex.Message + $"\n{method}");
            }
        }

    }

}
