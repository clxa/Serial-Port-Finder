using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SerialPortDeviceFinder.Core.Models;

namespace SerialPortDeviceFinder.Core.Scanning
{
    /// <summary>
    /// 按设备模板执行串口搜索的调度器。实现可替换，以便后续增加并行搜索策略。
    /// </summary>
    public interface IScanScheduler
    {
        Task<IReadOnlyList<ScanResult>> ScanAsync(
            IReadOnlyList<DeviceProfile> profiles,
            CancellationToken cancellationToken,
            IProgress<ScanResult>? progress);
    }
}
