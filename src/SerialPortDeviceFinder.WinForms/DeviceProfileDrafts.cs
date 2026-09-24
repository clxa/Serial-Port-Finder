using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using SerialPortDeviceFinder.Core.Models;

namespace SerialPortDeviceFinder.WinForms
{
    /// <summary>
    /// 集中处理模板新增与复制的默认值，避免窗体事件中混入数据创建规则。
    /// </summary>
    public static class DeviceProfileDrafts
    {
        /// <summary>
        /// 常用串口组合目录（穷举平时常用的波特率 × 校验 × 停止位组合），
        /// 全部满足 DeviceProfileValidator 的规则。目录顺序即面板展示顺序。
        /// </summary>
        public static IReadOnlyList<ComboOption> CommonSerialPortCombos { get; } = new[]
        {
            Combo(1200, 8, Parity.None, StopBits.One),
            Combo(2400, 8, Parity.None, StopBits.One),
            Combo(4800, 8, Parity.None, StopBits.One),
            Combo(9600, 8, Parity.None, StopBits.One),
            Combo(14400, 8, Parity.None, StopBits.One),
            Combo(19200, 8, Parity.None, StopBits.One),
            Combo(38400, 8, Parity.None, StopBits.One),
            Combo(57600, 8, Parity.None, StopBits.One),
            Combo(115200, 8, Parity.None, StopBits.One),
            Combo(230400, 8, Parity.None, StopBits.One),
            Combo(460800, 8, Parity.None, StopBits.One),
            Combo(921600, 8, Parity.None, StopBits.One),
            Combo(9600, 8, Parity.Even, StopBits.One),
            Combo(19200, 8, Parity.Even, StopBits.One),
            Combo(38400, 8, Parity.Even, StopBits.One),
            Combo(57600, 8, Parity.Even, StopBits.One),
            Combo(115200, 8, Parity.Even, StopBits.One),
            Combo(9600, 8, Parity.Odd, StopBits.One),
            Combo(19200, 8, Parity.Odd, StopBits.One),
            Combo(115200, 8, Parity.Odd, StopBits.One),
            Combo(9600, 8, Parity.None, StopBits.Two),
            Combo(115200, 8, Parity.None, StopBits.Two),
            Combo(9600, 7, Parity.Even, StopBits.One),
            Combo(19200, 7, Parity.Even, StopBits.One),
            Combo(9600, 7, Parity.Odd, StopBits.One),
            Combo(19200, 7, Parity.Odd, StopBits.One)
        };

        /// <summary>
        /// 新建模板默认勾选的组合（更常用的一组）。由目录按显示文本派生，
        /// 找不到即静态初始化失败，防止默认集与目录漂移。
        /// </summary>
        private static readonly IReadOnlyList<ComboOption> DefaultCheckedCombos = new[]
        {
            "9600 8N1", "19200 8N1", "38400 8N1", "57600 8N1", "115200 8N1",
            "9600 8E1", "9600 8O1", "115200 8N2"
        }
        .Select(text => CommonSerialPortCombos.Single(option => option.DisplayText == text))
        .ToList();

        public static DeviceProfile CreateDefault(IEnumerable<string> existingNames)
        {
            return new DeviceProfile
            {
                Name = CreateAvailableName("新设备", existingNames),
                IsEnabled = true,
                CommandFormat = PayloadFormat.Text,
                TextTerminator = TextTerminator.CrLf,
                ResponseFormat = PayloadFormat.Text,
                MatchMode = ResponseMatchMode.Contains,
                TextEncoding = TextEncodingKind.Ascii,
                TimeoutMilliseconds = 800,
                PortSettings = CommonSerialPortCombos
                    .Select(option =>
                    {
                        var settings = option.CreateSettings();
                        settings.Enabled = DefaultCheckedCombos.Contains(option);
                        return settings;
                    })
                    .ToList()
            };
        }

        public static DeviceProfile Clone(DeviceProfile source, IEnumerable<string> existingNames)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            return new DeviceProfile
            {
                Name = CreateAvailableName(source.Name, existingNames),
                IsEnabled = source.IsEnabled,
                CommandFormat = source.CommandFormat,
                CommandContent = source.CommandContent,
                TextTerminator = source.TextTerminator,
                ResponseFormat = source.ResponseFormat,
                ExpectedResponse = source.ExpectedResponse,
                MatchMode = source.MatchMode,
                TextEncoding = source.TextEncoding,
                TimeoutMilliseconds = source.TimeoutMilliseconds,
                LastScanPortName = source.LastScanPortName,
                PortSettings = (source.PortSettings ?? new List<SerialPortSettings>())
                    .Where(settings => settings != null)
                    .Select(CloneSettings)
                    .ToList()
            };
        }

        public static SerialPortSettings Create8N1(int baudRate)
        {
            return new SerialPortSettings
            {
                BaudRate = baudRate,
                DataBits = 8,
                Parity = Parity.None,
                StopBits = StopBits.One,
                Handshake = Handshake.None
            };
        }

        private static ComboOption Combo(int baudRate, int dataBits, Parity parity, StopBits stopBits)
        {
            return new ComboOption(
                $"{baudRate} {dataBits}{ToParityShorthand(parity)}{ToStopShorthand(stopBits)}",
                baudRate,
                dataBits,
                parity,
                stopBits);
        }

        private static char ToParityShorthand(Parity parity)
        {
            switch (parity)
            {
                case Parity.None: return 'N';
                case Parity.Even: return 'E';
                case Parity.Odd: return 'O';
                default: return '?';
            }
        }

        private static char ToStopShorthand(StopBits stopBits)
        {
            switch (stopBits)
            {
                case StopBits.One: return '1';
                case StopBits.Two: return '2';
                default: return '?';
            }
        }

        private static string CreateAvailableName(string preferredName, IEnumerable<string> existingNames)
        {
            var baseName = string.IsNullOrWhiteSpace(preferredName) ? "新设备" : preferredName.Trim();
            var names = new HashSet<string>(existingNames ?? Array.Empty<string>(), StringComparer.Ordinal);
            if (!names.Contains(baseName))
            {
                return baseName;
            }

            for (var index = 2; ; index++)
            {
                var candidate = baseName + " " + index;
                if (!names.Contains(candidate))
                {
                    return candidate;
                }
            }
        }

        private static SerialPortSettings CloneSettings(SerialPortSettings source)
        {
            return new SerialPortSettings
            {
                BaudRate = source.BaudRate,
                DataBits = source.DataBits,
                Parity = source.Parity,
                StopBits = source.StopBits,
                Handshake = source.Handshake,
                Enabled = source.Enabled
            };
        }
    }
}
