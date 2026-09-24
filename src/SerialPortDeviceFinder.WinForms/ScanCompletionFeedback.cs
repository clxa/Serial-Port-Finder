using System;
using System.Collections.Generic;
using System.Linq;
using SerialPortDeviceFinder.Core.Models;

namespace SerialPortDeviceFinder.WinForms
{
    /// <summary>
    /// 区分正常完成与取消完成，防止取消后的部分结果被误报为完整扫描结果。
    /// </summary>
    public sealed class ScanCompletionFeedback
    {
        private ScanCompletionFeedback(bool isStopped, bool shouldShowNoMatchState, string message)
        {
            IsStopped = isStopped;
            ShouldShowNoMatchState = shouldShowNoMatchState;
            Message = message;
        }

        public bool IsStopped { get; }

        public bool ShouldShowNoMatchState { get; }

        public string Message { get; }

        public static ScanCompletionFeedback Create(IReadOnlyList<ScanResult> results, bool cancellationRequested)
        {
            if (results == null)
            {
                throw new ArgumentNullException(nameof(results));
            }

            var summary = ScanSummary.From(results);
            var isStopped = cancellationRequested || results.Any(result => result != null && result.Status == ScanStatus.Cancelled);
            if (isStopped)
            {
                return new ScanCompletionFeedback(
                    isStopped: true,
                    shouldShowNoMatchState: false,
                    message: $"搜索已停止，已保留 {results.Count} 条已完成任务的部分结果：匹配 {summary.MatchedPortCount} 个端口；未匹配 {summary.UnmatchedPortCount} 个端口；端口/通信错误 {summary.ErrorCount} 次。");
            }

            return new ScanCompletionFeedback(false, summary.MatchedPortCount == 0, summary.ToDisplayText());
        }
    }
}
