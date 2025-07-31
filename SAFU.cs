using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Security.Cryptography;
using System.Text;

namespace z3nApp
{

    public class SAFU
    {
        public string id { get; set; }
        private readonly string _cfgPin;
        private readonly string _hwId;

        public SAFU(string cfgPin)
        {
            _cfgPin = cfgPin;
            _hwId = GetHWId();
        }



        public string Encode(string toEncrypt, string currentId)
        {
            id = currentId;
            if (string.IsNullOrEmpty(toEncrypt))
            {
                Debug.WriteLine("Encode: Input string is empty");
                return string.Empty;
            }
            string combinedKey = _hwId + _cfgPin + id;
            Debug.WriteLine("Encode: Generating key for encryption");
            try
            {
                return AES.EncryptAES(toEncrypt, combinedKey, true);
            }
            catch (Exception ex)
            {
                new Logger(true).Send($"W [{ex.Message}]");
                return null;
            }
        }

        public string Decode(string toDecrypt, string currentId)
        {
            id = currentId;
            if (string.IsNullOrEmpty(toDecrypt))
            {
                Debug.WriteLine("Decode: Input string is empty");
                return string.Empty;
            }
            string combinedKey = _hwId + _cfgPin + id;
            Debug.WriteLine("Decode: Generating key for decryption");
            return AES.DecryptAES(toDecrypt, combinedKey, true);
        }
        public string HWPass()
        {
            string comboInput = _hwId + _cfgPin + id;
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
