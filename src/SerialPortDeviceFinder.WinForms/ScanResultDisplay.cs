using System;
using System.Linq;
using SerialPortDeviceFinder.Core.Models;

namespace SerialPortDeviceFinder.WinForms
{
    internal sealed class ScanResultDisplay
    {
        public string DeviceName { get; set; } = string.Empty;
        public string PortName { get; set; } = string.Empty;
        public string Parameters { get; set; } = string.Empty;
        public string Response { get; set; } = string.Empty;
        public string Time { get; set; } = string.Empty;

        public static ScanResultDisplay From(ScanResult result)
        {
            return new ScanResultDisplay
            {
                DeviceName = result.ProfileName,
                PortName = result.PortName,
                Parameters = $"{result.PortSettings.BaudRate}, {result.PortSettings.DataBits}{result.PortSettings.Parity}, {result.PortSettings.StopBits}",
                Response = ToHex(result.ResponseBytes),
                Time = result.OccurredAt.LocalDateTime.ToString("yyyy-MM-dd HH:mm:ss")
            };
        }

        public static string ToHex(byte[]? bytes)
        {
            return bytes == null || bytes.Length == 0
                ? "（无）"
                : string.Join(" ", bytes.Select(value => value.ToString("X2")));
        }
    }
}
