using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Security.Cryptography;
using System.Text;

namespace z3nApp
{

    public static class FunctionStorage
    {
        public static ConcurrentDictionary<string, object> Functions = new ConcurrentDictionary<string, object>();
    }


    public class SAFU
    {
        public string id { get; set; }
        private readonly string _cfgPin;
        private readonly string _hwId;

        public SAFU(string cfgPin)
        {
            _cfgPin = cfgPin;
            _hwId = GetHWId();
            RegisterFunctions();

        }

        public string Encode(string input)
        {
            var encodeFunc = FunctionStorage.Functions["SAFU_Encode"] as Func<string, string>;
            return encodeFunc?.Invoke(input) ?? string.Empty;
        }

        public string Decode(string input)
        {
            var decodeFunc = FunctionStorage.Functions["SAFU_Decode"] as Func<string, string>;
            return decodeFunc?.Invoke(input) ?? string.Empty;
        }

        public string HWPass()
        {
            var hwPassFunc = FunctionStorage.Functions["SAFU_HWPass"] as Func<string>;
            return hwPassFunc?.Invoke() ?? string.Empty;
        }

        public void RegisterFunctions()
        {
            var encodeFunc = new Func<string, string>((toEncrypt) =>
            {
                if (string.IsNullOrEmpty(toEncrypt))
                {
                    Debug.WriteLine("Encode: Input string is empty");
                    return string.Empty;
                }
                string combinedKey = _hwId + _cfgPin + _id;
                Debug.WriteLine("Encode: Generating key for encryption");
                return AES.EncryptAES(toEncrypt, combinedKey, true);
            });
            FunctionStorage.Functions.AddOrUpdate("SAFU_Encode", encodeFunc, (key, oldValue) => encodeFunc);

            var decodeFunc = new Func<string, string>((toDecrypt) =>
            {
                if (string.IsNullOrEmpty(toDecrypt))
                {
                    Debug.WriteLine("Decode: Input string is empty");
                    return string.Empty;
                }
                string combinedKey = _hwId + _cfgPin + _id;
                Debug.WriteLine("Decode: Generating key for decryption");
                return AES.DecryptAES(toDecrypt, combinedKey, true);
            });
            FunctionStorage.Functions.AddOrUpdate("SAFU_Decode", decodeFunc, (key, oldValue) => decodeFunc);

            var hwPassFunc = new Func<string>(() =>
            {
                string comboInput = _hwId + _id + _cfgPin;
                Debug.WriteLine("HWPass: Generating password hash");
                using (var sha256 = SHA256.Create())
                {
                    byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(comboInput));
                    string hash = BitConverter.ToString(hashBytes).Replace("-", "").ToLower();

                    StringBuilder password = new StringBuilder();
                    string lowerChars = "abcdefghijklmnopqrstuvwxyz";
                    string upperChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
                    string digits = "0123456789";
                    string specialChars = "!@#$%^&*()";

                    for (int i = 0; i < 20; i++)
                    {
                        int index = Convert.ToInt32(hash.Substring(i * 2, 2), 16);
                        char c;
                        if (i % 4 == 0) c = lowerChars[index % 26];
                        else if (i % 4 == 1) c = upperChars[index % 26];
                        else if (i % 4 == 2) c = digits[index % 10];
                        else c = specialChars[index % 10];
                        password.Append(c);
                    }
                    return password.ToString();
                }
            });
            FunctionStorage.Functions.AddOrUpdate("SAFU_HWPass", hwPassFunc, (key, oldValue) => hwPassFunc);
        }

        private string GetHWId()
        {
            string cpuInfo = string.Empty;
            string hddInfo = string.Empty;
            string baseboardInfo = string.Empty;

            try
            {
                using (var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_Processor"))
                {
                    foreach (ManagementObject mo in searcher.Get())
                    {
                        cpuInfo = mo["ProcessorId"]?.ToString() ?? "";
                        Debug.WriteLine($"GetHWId: CPU Info = {cpuInfo}");
                        break;
                    }
                }

                using (var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_DiskDrive"))
                {
                    foreach (ManagementObject mo in searcher.Get())
                    {
                        hddInfo = mo["SerialNumber"]?.ToString() ?? "";
                        Debug.WriteLine($"GetHWId: HDD Info = {hddInfo}");
                        break;
                    }
                }

                using (var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_BaseBoard"))
                {
                    foreach (ManagementObject mo in searcher.Get())
                    {
                        baseboardInfo = mo["SerialNumber"]?.ToString() ?? "";
                        Debug.WriteLine($"GetHWId: Baseboard Info = {baseboardInfo}");
                        break;
                    }
                }

                string combinedInfo = cpuInfo + hddInfo + baseboardInfo;
                using (var sha256 = SHA256.Create())
                {
                    byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(combinedInfo));
                    string result = BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
                    Debug.WriteLine($"GetHWId: Combined hash = {result}");
                    return result;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"GetHWId: Error - {ex.Message}");
                return string.Empty;
            }
        }
    
    }
}
