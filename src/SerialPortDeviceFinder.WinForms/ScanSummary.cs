using System;
using System.Collections.Generic;
using System.Linq;
using SerialPortDeviceFinder.Core.Models;

namespace SerialPortDeviceFinder.WinForms
{
    /// <summary>
    /// 汇总一次完整扫描，避免窗体用单条日志推断端口级结果。
    /// </summary>
    public sealed class ScanSummary
    {
        private ScanSummary(int matchedPortCount, int unmatchedPortCount, int errorCount)
        {
            MatchedPortCount = matchedPortCount;
            UnmatchedPortCount = unmatchedPortCount;
            ErrorCount = errorCount;
        }

        public int MatchedPortCount { get; }

        public int UnmatchedPortCount { get; }

        public int ErrorCount { get; }

        public static ScanSummary From(IReadOnlyList<ScanResult> results)
        {
            if (results == null)
            {
                throw new ArgumentNullException(nameof(results));
            }

            var validResults = results.Where(result => result != null).ToList();
            var matchedPorts = new HashSet<string>(
                validResults.Where(result => result.Status == ScanStatus.Matched).Select(result => result.PortName),
                StringComparer.OrdinalIgnoreCase);
            var attemptedPorts = new HashSet<string>(
                validResults.Select(result => result.PortName).Where(portName => !string.IsNullOrWhiteSpace(portName)),
                StringComparer.OrdinalIgnoreCase);
            var errorCount = validResults.Count(result => result.Status == ScanStatus.PortUnavailable || result.Status == ScanStatus.CommunicationError);

            return new ScanSummary(matchedPorts.Count, attemptedPorts.Count - matchedPorts.Count, errorCount);
        }

        public string ToDisplayText()
        {
            return $"搜索完成：匹配 {MatchedPortCount} 个端口；未匹配 {UnmatchedPortCount} 个端口；端口/通信错误 {ErrorCount} 次。";
        }

        /// <summary>
        /// 把匹配结果整理成"设备名称-串口号-波特率-数据位-校验位"文本行（每行一台设备），
        /// 供扫描完成后只读汇总框显示；无匹配时返回空串。
        /// </summary>
        public static string FormatMatchedDevices(IReadOnlyList<ScanResult> results)
        {
            if (results == null)
            {
                throw new ArgumentNullException(nameof(results));
            }

            return string.Join(
                Environment.NewLine,
                results
                    .Where(result => result != null && result.Status == ScanStatus.Matched)
                    .Select(result =>
                        $"{result.ProfileName}-{result.PortName}-{result.PortSettings.BaudRate}-{result.PortSettings.DataBits}-{result.PortSettings.Parity}"));
        }
    }
}
