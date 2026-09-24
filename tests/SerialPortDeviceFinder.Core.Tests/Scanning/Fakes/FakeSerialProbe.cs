using System;
using System.Collections.Generic;
using System.Threading;
using SerialPortDeviceFinder.Core.Models;
using SerialPortDeviceFinder.Core.Scanning;

namespace SerialPortDeviceFinder.Core.Tests.Scanning.Fakes
{
    internal sealed class FakeSerialProbe : ISerialProbe
    {
        private readonly Func<ProbeCall, ScanResult> _resultFactory;

        public FakeSerialProbe(ScanStatus status)
            : this(call => CreateResult(call, status))
        {
        }

        public FakeSerialProbe(Func<ProbeCall, ScanResult> resultFactory)
        {
            _resultFactory = resultFactory ?? throw new ArgumentNullException(nameof(resultFactory));
        }

        public List<ProbeCall> Calls { get; } = new List<ProbeCall>();

        public ScanResult Probe(
            string portName,
            DeviceProfile profile,
            SerialPortSettings settings,
            CancellationToken cancellationToken)
        {
            var call = new ProbeCall(portName, profile, settings, cancellationToken);
            Calls.Add(call);
            return _resultFactory(call);
        }

        private static ScanResult CreateResult(ProbeCall call, ScanStatus status)
        {
            return new ScanResult
            {
                PortName = call.PortName,
                ProfileName = call.Profile.Name,
                PortSettings = call.Settings,
                Status = status
            };
        }
    }

    internal sealed class ProbeCall
    {
        public ProbeCall(
            string portName,
            DeviceProfile profile,
            SerialPortSettings settings,
            CancellationToken cancellationToken)
        {
            PortName = portName;
            Profile = profile;
            Settings = settings;
            CancellationToken = cancellationToken;
        }

        public string PortName { get; }

        public DeviceProfile Profile { get; }

        public SerialPortSettings Settings { get; }

        public CancellationToken CancellationToken { get; }
    }
}
