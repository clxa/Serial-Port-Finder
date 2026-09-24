using System;
using System.Collections.Generic;
using System.IO.Ports;

namespace SerialPortDeviceFinder.Core.Scanning
{
    /// <summary>
    /// 从当前 Windows 系统读取可见 COM 端口的一次快照。
    /// </summary>
    public sealed class SystemSerialPortCatalog : ISerialPortCatalog
    {
        public IReadOnlyList<string> GetPortNames()
        {
            return SerialPort.GetPortNames() ?? Array.Empty<string>();
        }
    }
}
