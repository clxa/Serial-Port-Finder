using System.Collections.Generic;

namespace SerialPortDeviceFinder.Core.Scanning
{
    /// <summary>
    /// 提供当前计算机可见串口的快照。扫描调度器通过该接口隔离系统查询。
    /// </summary>
    public interface ISerialPortCatalog
    {
        IReadOnlyList<string> GetPortNames();
    }
}
