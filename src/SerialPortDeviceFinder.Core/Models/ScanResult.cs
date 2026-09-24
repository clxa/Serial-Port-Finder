using System;

namespace SerialPortDeviceFinder.Core.Models
{
    public enum ScanStatus
    {
        Matched,
        NotMatched,
        Timeout,
        PortUnavailable,
        CommunicationError,
        Cancelled
    }

    public sealed class ScanResult
    {
        public string PortName { get; set; } = string.Empty;

        public string ProfileName { get; set; } = string.Empty;

        public SerialPortSettings PortSettings { get; set; } = SerialPortSettings.Default9600();

        public ScanStatus Status { get; set; }

        public byte[] ResponseBytes { get; set; } = Array.Empty<byte>();

        public string Message { get; set; } = string.Empty;

        public DateTimeOffset OccurredAt { get; set; } = DateTimeOffset.Now;
    }
}
