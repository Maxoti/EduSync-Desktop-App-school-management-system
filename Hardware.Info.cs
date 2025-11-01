using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace EduSync
{
    internal class Hardware
    {
      
        /// <summary>
        /// Generates a stable hardware fingerprint using multiple hardware components
        /// </summary>
        public static class HardwareInfo
        {
            /// <summary>
            /// Gets a stable hardware ID by combining multiple hardware identifiers
            /// </summary>
            public static string GetHardwareId()
            {
                try
                {
                    // Collect multiple hardware identifiers
                    string cpuId = GetCpuId();
                    string motherboardId = GetMotherboardId();
                    string diskId = GetDiskId();
                    string machineGuid = GetMachineGuid();

                    // Combine them into a single fingerprint
                    string combined = $"{cpuId}|{motherboardId}|{diskId}|{machineGuid}";

                    // Generate a hash to create a consistent, shorter ID
                    return GenerateHash(combined);
                }
                catch (Exception ex)
                {
                    // Log the error for debugging
                    System.Diagnostics.Debug.WriteLine($"HardwareInfo Error: {ex.Message}");

                    // Fallback to a less reliable but still functional ID
                    return GetFallbackId();
                }
            }

            /// <summary>
            /// Gets CPU processor ID
            /// </summary>
            private static string GetCpuId()
            {
                try
                {
                    using (var searcher = new ManagementObjectSearcher("SELECT ProcessorId FROM Win32_Processor"))
                    {
                        foreach (ManagementObject mo in searcher.Get())
                        {
                            string processorId = mo["ProcessorId"]?.ToString();
                            if (!string.IsNullOrEmpty(processorId) && processorId != "0")
                            {
                                return processorId;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"CPU ID Error: {ex.Message}");
                }

                return "CPU_UNKNOWN";
            }

            /// <summary>
            /// Gets motherboard serial number
            /// </summary>
            private static string GetMotherboardId()
            {
                try
                {
                    using (var searcher = new ManagementObjectSearcher("SELECT SerialNumber FROM Win32_BaseBoard"))
                    {
                        foreach (ManagementObject mo in searcher.Get())
                        {
                            string serialNumber = mo["SerialNumber"]?.ToString()?.Trim();
                            if (!string.IsNullOrEmpty(serialNumber) &&
                                !serialNumber.Equals("None", StringComparison.OrdinalIgnoreCase) &&
                                !serialNumber.Equals("To be filled by O.E.M.", StringComparison.OrdinalIgnoreCase))
                            {
                                return serialNumber;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Motherboard ID Error: {ex.Message}");
                }

                return "MB_UNKNOWN";
            }

            /// <summary>
            /// Gets primary disk drive serial number
            /// </summary>
            private static string GetDiskId()
            {
                try
                {
                    using (var searcher = new ManagementObjectSearcher("SELECT SerialNumber FROM Win32_DiskDrive WHERE MediaType='Fixed hard disk media'"))
                    {
                        foreach (ManagementObject mo in searcher.Get())
                        {
                            string serialNumber = mo["SerialNumber"]?.ToString()?.Trim();
                            if (!string.IsNullOrEmpty(serialNumber))
                            {
                                return serialNumber;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Disk ID Error: {ex.Message}");
                }

                return "DISK_UNKNOWN";
            }

            /// <summary>
            /// Gets Windows machine GUID from registry
            /// </summary>
            private static string GetMachineGuid()
            {
                try
                {
                    using (var key = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Cryptography"))
                    {
                        string guid = key?.GetValue("MachineGuid")?.ToString();
                        if (!string.IsNullOrEmpty(guid))
                        {
                            return guid;
                        }
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Machine GUID Error: {ex.Message}");
                }

                return "GUID_UNKNOWN";
            }

            /// <summary>
            /// Generates a SHA256 hash of the combined hardware info
            /// </summary>
            private static string GenerateHash(string input)
            {
                using (var sha256 = SHA256.Create())
                {
                    byte[] hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
                    return Convert.ToBase64String(hash).Replace("+", "").Replace("/", "").Replace("=", "").Substring(0, 16);
                }
            }

            /// <summary>
            /// Fallback ID generation when WMI fails
            /// </summary>
            private static string GetFallbackId()
            {
                try
                {
                    // Use environment variables and system info as fallback
                    string computerName = Environment.MachineName ?? "UNKNOWN";
                    string userName = Environment.UserName ?? "UNKNOWN";
                    string osVersion = Environment.OSVersion.ToString();
                    string processorCount = Environment.ProcessorCount.ToString();

                    string fallback = $"{computerName}|{userName}|{osVersion}|{processorCount}";
                    return GenerateHash(fallback);
                }
                catch
                {
                    // Ultimate fallback - generate a semi-random but consistent ID
                    return "FALLBACK_" + DateTime.Now.ToString("yyyyMMdd");
                }
            }

            /// <summary>
            /// Gets detailed hardware information for debugging
            /// </summary>
            public static HardwareDetails GetHardwareDetails()
            {
                return new HardwareDetails
                {
                    CpuId = GetCpuId(),
                    MotherboardId = GetMotherboardId(),
                    DiskId = GetDiskId(),
                    MachineGuid = GetMachineGuid(),
                    ComputerName = Environment.MachineName,
                    ProcessorCount = Environment.ProcessorCount,
                    OSVersion = Environment.OSVersion.ToString(),
                    FinalHardwareId = GetHardwareId()
                };
            }

            /// <summary>
            /// Validates that the current hardware ID matches the stored one with some tolerance
            /// </summary>
            public static bool ValidateHardwareId(string storedId, out string reason)
            {
                reason = "";

                try
                {
                    string currentId = GetHardwareId();

                    if (currentId == storedId)
                    {
                        reason = "Hardware ID matches exactly";
                        return true;
                    }

                    reason = $"Hardware ID mismatch. Current: {currentId}, Stored: {storedId}";
                    return false;
                }
                catch (Exception ex)
                {
                    reason = $"Error validating hardware ID: {ex.Message}";
                    return false;
                }
            }
        }

        /// <summary>
        /// Detailed hardware information for debugging
        /// </summary>
        public class HardwareDetails
        {
            public string CpuId { get; set; }
            public string MotherboardId { get; set; }
            public string DiskId { get; set; }
            public string MachineGuid { get; set; }
            public string ComputerName { get; set; }
            public int ProcessorCount { get; set; }
            public string OSVersion { get; set; }
            public string FinalHardwareId { get; set; }

            public override string ToString()
            {
                return $"CPU: {CpuId}\n" +
                       $"Motherboard: {MotherboardId}\n" +
                       $"Disk: {DiskId}\n" +
                       $"Machine GUID: {MachineGuid}\n" +
                       $"Computer: {ComputerName}\n" +
                       $"Processors: {ProcessorCount}\n" +
                       $"OS: {OSVersion}\n" +
                       $"Final ID: {FinalHardwareId}";
            }
        }
    }




}

