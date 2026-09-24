using System.Threading;
using SerialPortDeviceFinder.Core.Models;

namespace SerialPortDeviceFinder.Core.Scanning
{
    public interface ISerialProbe
    {
        ScanResult Probe(
            string portName,
            DeviceProfile profile,
            SerialPortSettings settings,
            CancellationToken cancellationToken);
    }
}
