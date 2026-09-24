using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SerialPortDeviceFinder.Core.Models;

namespace SerialPortDeviceFinder.Core.Scanning
{
    /// <summary>
    /// 以端口、设备模板、串口参数的固定顺序执行探测。
    /// 任一模板匹配端口后，立即停止该端口的后续探测。
    /// </summary>
    public sealed class SequentialScanScheduler : IScanScheduler
    {
        private readonly ISerialPortCatalog _portCatalog;
        private readonly ISerialProbe _serialProbe;

        public SequentialScanScheduler(ISerialPortCatalog portCatalog, ISerialProbe serialProbe)
        {
            _portCatalog = portCatalog ?? throw new ArgumentNullException(nameof(portCatalog));
            _serialProbe = serialProbe ?? throw new ArgumentNullException(nameof(serialProbe));
        }

        public Task<IReadOnlyList<ScanResult>> ScanAsync(
            IReadOnlyList<DeviceProfile> profiles,
            CancellationToken cancellationToken,
            IProgress<ScanResult>? progress)
        {
            if (profiles == null)
            {
                throw new ArgumentNullException(nameof(profiles));
            }

            var portNames = SnapshotAndSortPortNames(_portCatalog.GetPortNames());
            var enabledProfiles = SnapshotEnabledProfiles(profiles);

            return Task.Run<IReadOnlyList<ScanResult>>(
                () => Scan(portNames, enabledProfiles, cancellationToken, progress));
        }

        private IReadOnlyList<ScanResult> Scan(
            IReadOnlyList<string> portNames,
            IReadOnlyList<DeviceProfile> profiles,
            CancellationToken cancellationToken,
            IProgress<ScanResult>? progress)
        {
            var results = new List<ScanResult>();

            foreach (var portName in portNames)
            {
                var portMatched = false;
                foreach (var profile in profiles)
                {
                    foreach (var settings in profile.PortSettings)
                    {
                        if (settings?.Enabled != true)
                        {
                            continue;
                        }

                        if (cancellationToken.IsCancellationRequested)
                        {
                            return results;
                        }

                        var result = Probe(portName, profile, settings, cancellationToken);
                        results.Add(result);
                        progress?.Report(result);

                        if (cancellationToken.IsCancellationRequested || result.Status == ScanStatus.Cancelled)
                        {
                            return results;
                        }

                        if (result.Status == ScanStatus.Matched)
                        {
                            portMatched = true;
                            break;
                        }
                    }

                    if (portMatched)
                    {
                        break;
                    }
                }
            }

            return results;
        }

        private ScanResult Probe(
            string portName,
            DeviceProfile profile,
            SerialPortSettings settings,
            CancellationToken cancellationToken)
        {
            try
            {
                return _serialProbe.Probe(portName, profile, settings, cancellationToken);
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                return CreateResult(portName, profile, settings, ScanStatus.Cancelled, "搜索已取消。");
            }
            catch (Exception exception)
            {
                return CreateResult(portName, profile, settings, ScanStatus.CommunicationError, exception.Message);
            }
        }

        private static IReadOnlyList<string> SnapshotAndSortPortNames(IReadOnlyList<string> portNames)
        {
            var snapshot = (portNames ?? Array.Empty<string>())
                .Where(portName => !string.IsNullOrWhiteSpace(portName))
                .ToList();
            snapshot.Sort(ComPortNameComparer.Instance);
            return snapshot;
        }

        private static IReadOnlyList<DeviceProfile> SnapshotEnabledProfiles(IReadOnlyList<DeviceProfile> profiles)
        {
            return profiles
                .Where(profile => profile?.IsEnabled == true)
                .Select(CloneProfile)
                .ToList();
        }

        private static DeviceProfile CloneProfile(DeviceProfile profile)
        {
            return new DeviceProfile
            {
                Name = profile.Name,
                IsEnabled = profile.IsEnabled,
                CommandFormat = profile.CommandFormat,
                CommandContent = profile.CommandContent,
                TextTerminator = profile.TextTerminator,
                ResponseFormat = profile.ResponseFormat,
                ExpectedResponse = profile.ExpectedResponse,
                MatchMode = profile.MatchMode,
                TextEncoding = profile.TextEncoding,
                TimeoutMilliseconds = profile.TimeoutMilliseconds,
                LastScanPortName = profile.LastScanPortName,
                PortSettings = (profile.PortSettings ?? new List<SerialPortSettings>())
                    .Where(settings => settings != null)
                    .Select(CloneSettings)
                    .ToList()
            };
        }

        private static SerialPortSettings CloneSettings(SerialPortSettings settings)
        {
            return new SerialPortSettings
            {
                BaudRate = settings.BaudRate,
                DataBits = settings.DataBits,
                Parity = settings.Parity,
                StopBits = settings.StopBits,
                Handshake = settings.Handshake,
                Enabled = settings.Enabled
            };
        }

        private static ScanResult CreateResult(
            string portName,
            DeviceProfile profile,
            SerialPortSettings settings,
            ScanStatus status,
            string message)
        {
            return new ScanResult
            {
                PortName = portName,
                ProfileName = profile.Name,
                PortSettings = settings,
                Status = status,
                Message = message,
                OccurredAt = DateTimeOffset.Now
            };
        }

        private sealed class ComPortNameComparer : IComparer<string>
        {
            public static ComPortNameComparer Instance { get; } = new ComPortNameComparer();

            public int Compare(string? left, string? right)
            {
                if (ReferenceEquals(left, right))
                {
                    return 0;
                }

                if (left == null)
                {
                    return -1;
                }

                if (right == null)
                {
                    return 1;
                }

                if (TryGetComPortNumber(left, out var leftNumber) && TryGetComPortNumber(right, out var rightNumber))
                {
                    var numberComparison = leftNumber.CompareTo(rightNumber);
                    if (numberComparison != 0)
                    {
                        return numberComparison;
                    }
                }

                return StringComparer.OrdinalIgnoreCase.Compare(left, right);
            }

            private static bool TryGetComPortNumber(string portName, out int number)
            {
                number = 0;
                return portName.Length > 3 &&
                       portName.StartsWith("COM", StringComparison.OrdinalIgnoreCase) &&
                       int.TryParse(
                           portName.Substring(3),
                           NumberStyles.None,
                           CultureInfo.InvariantCulture,
                           out number);
            }
        }
    }
}
