using System.Collections.Generic;
using SerialPortDeviceFinder.Core.Models;

namespace SerialPortDeviceFinder.Core.Tests
{
    internal static class TestProfiles
    {
        public static DeviceProfile Text(string name, string command, string expectedResponse)
        {
            return new DeviceProfile
            {
                Name = name,
                CommandFormat = PayloadFormat.Text,
                CommandContent = command,
                TextTerminator = TextTerminator.CrLf,
                ResponseFormat = PayloadFormat.Text,
                ExpectedResponse = expectedResponse,
                MatchMode = ResponseMatchMode.Contains,
                TextEncoding = TextEncodingKind.Ascii,
                TimeoutMilliseconds = 100,
                PortSettings = new List<SerialPortSettings> { SerialPortSettings.Default9600() }
            };
        }
    }
}
