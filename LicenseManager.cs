using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using static EduSync.Hardware;

namespace EduSync
{
    public static class LicenseManager
    {
        private const string LicenseFilePath = "license.txt";
        private static readonly string[] ValidKeyPrefixes = { "EDUSYNC", "EDU2025" };

        public static bool IsLicenseValid()
        {
            try
            {
                if (!File.Exists(LicenseFilePath))
                    return false;

                string[] lines = File.ReadAllLines(LicenseFilePath);
                string key = "", machineId = "", expiry = "";

                foreach (var line in lines)
                {
                    if (line.StartsWith("Key=")) key = line.Substring(4).Trim();
                    if (line.StartsWith("MachineID=")) machineId = line.Substring(10).Trim();
                    if (line.StartsWith("ExpiresOn=")) expiry = line.Substring(10).Trim();
                }

                if (string.IsNullOrWhiteSpace(key) || string.IsNullOrWhiteSpace(machineId) || string.IsNullOrWhiteSpace(expiry))
                    return false;

                if (!IsValidLicenseKey(key))
                    return false;

                if (!DateTime.TryParse(expiry, out DateTime expiryDate))
                    return false;

                if (DateTime.Now > expiryDate)
                    return false;

                // Use enhanced hardware validation
                return HardwareInfo.ValidateHardwareId(machineId, out string reason);
            }
            catch
            {
                return false;
            }
        }

        public static bool ActivateLicense(string key, DateTime expiryDate, out string errorMessage)
        {
            errorMessage = "";

            try
            {
                if (!IsValidLicenseKey(key))
                {
                    errorMessage = "Invalid license key format.";
                    return false;
                }

                if (DateTime.Now > expiryDate)
                {
                    errorMessage = "License has already expired.";
                    return false;
                }

                SaveLicense(key, expiryDate);
                return true;
            }
            catch (Exception ex)
            {
                errorMessage = $"Failed to activate license: {ex.Message}";
                return false;
            }
        }

        private static void SaveLicense(string key, DateTime expiryDate)
        {
            string machineId = HardwareInfo.GetHardwareId();
            string[] lines =
            {
                $"Key={key}",
                $"MachineID={machineId}",
                $"ExpiresOn={expiryDate:yyyy-MM-dd}"
            };
            File.WriteAllLines(LicenseFilePath, lines);
        }

        private static bool IsValidLicenseKey(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                return false;

            if (key.Length < 10 || !key.Contains("-"))
                return false;

            bool hasValidPrefix = false;
            foreach (var prefix in ValidKeyPrefixes)
            {
                if (key.StartsWith(prefix))
                {
                    hasValidPrefix = true;
                    break;
                }
            }

            if (!hasValidPrefix)
                return false;

            return ValidateKeyChecksum(key);
        }

        private static bool ValidateKeyChecksum(string key)
        {
            try
            {
                if (key.Length < 8)
                    return false;

                string mainPart = key.Substring(0, key.Length - 4);
                string providedChecksum = key.Substring(key.Length - 4);
                string expectedChecksum = CalculateSimpleChecksum(mainPart);

                return providedChecksum.Equals(expectedChecksum, StringComparison.OrdinalIgnoreCase);
            }
            catch
            {
                return false;
            }
        }

        private static string CalculateSimpleChecksum(string input)
        {
            using (var md5 = MD5.Create())
            {
                byte[] hash = md5.ComputeHash(Encoding.UTF8.GetBytes(input));
                return BitConverter.ToString(hash).Replace("-", "").Substring(0, 4);
            }
        }

        public static LicenseStatus GetLicenseStatus()
        {
            try
            {
                if (!File.Exists(LicenseFilePath))
                    return new LicenseStatus { IsValid = false, Message = "No license file found." };

                string[] lines = File.ReadAllLines(LicenseFilePath);
                string key = "", machineId = "", expiry = "";

                foreach (var line in lines)
                {
                    if (line.StartsWith("Key=")) key = line.Substring(4).Trim();
                    if (line.StartsWith("MachineID=")) machineId = line.Substring(10).Trim();
                    if (line.StartsWith("ExpiresOn=")) expiry = line.Substring(10).Trim();
                }

                if (string.IsNullOrWhiteSpace(key))
                    return new LicenseStatus { IsValid = false, Message = "Invalid license key." };

                if (!IsValidLicenseKey(key))
                    return new LicenseStatus { IsValid = false, Message = "Invalid license key format." };

                if (!DateTime.TryParse(expiry, out DateTime expiryDate))
                    return new LicenseStatus { IsValid = false, Message = "Invalid expiry date." };

                if (DateTime.Now > expiryDate)
                    return new LicenseStatus { IsValid = false, Message = "License has expired.", ExpiryDate = expiryDate };

                // Enhanced hardware validation with detailed reason
                if (!HardwareInfo.ValidateHardwareId(machineId, out string reason))
                    return new LicenseStatus { IsValid = false, Message = $"Hardware validation failed: {reason}" };

                return new LicenseStatus { IsValid = true, Message = "License is valid.", ExpiryDate = expiryDate };
            }
            catch (Exception ex)
            {
                return new LicenseStatus { IsValid = false, Message = $"Error checking license: {ex.Message}" };
            }
        }

        // Debug method to show hardware details
        public static string GetHardwareDebugInfo()
        {
            var details = HardwareInfo.GetHardwareDetails();
            return details.ToString();
        }

        // Helper method to generate valid test keys
        public static string GenerateTestKey(string baseName = "TEST")
        {
            string baseKey = $"EDUSYNC-2025-{baseName}-";
            string checksum = CalculateSimpleChecksum(baseKey);
            return baseKey + checksum;
        }
    }

    public class LicenseStatus
    {
        public bool IsValid { get; set; }
        public string Message { get; set; }
        public DateTime? ExpiryDate { get; set; }
    }
}