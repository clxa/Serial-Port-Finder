using System;
using System.Collections.Generic;
using System.Linq;
using SerialPortDeviceFinder.Core.Scanning;

namespace SerialPortDeviceFinder.Core.Tests.Scanning.Fakes
{
    internal sealed class FakeSerialPortCatalog : ISerialPortCatalog
    {
        private readonly IReadOnlyList<string> _portNames;

        public FakeSerialPortCatalog(params string[] portNames)
        {
            _portNames = (portNames ?? Array.Empty<string>()).ToArray();
        }

        public int GetPortNamesCallCount { get; private set; }

        public IReadOnlyList<string> GetPortNames()
        {
            GetPortNamesCallCount++;
            return _portNames;
        }
    }
}
