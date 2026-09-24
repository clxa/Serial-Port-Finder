using System.Collections.Generic;

namespace SerialPortDeviceFinder.Core.Models
{
    public enum PayloadFormat
    {
        Text,
        Hex
    }

    public enum ResponseMatchMode
    {
        Contains,
        Exact
    }

    public enum TextTerminator
    {
        None,
        Cr,
        Lf,
        CrLf
    }

    public enum TextEncodingKind
    {
        Ascii,
        Utf8,
        Gbk
    }

    public sealed class DeviceProfile
    {
        public string Name { get; set; } = string.Empty;

        public bool IsEnabled { get; set; } = true;

        public PayloadFormat CommandFormat { get; set; } = PayloadFormat.Text;

        public string CommandContent { get; set; } = string.Empty;

        public TextTerminator TextTerminator { get; set; } = TextTerminator.CrLf;

        public PayloadFormat ResponseFormat { get; set; } = PayloadFormat.Text;

        public string ExpectedResponse { get; set; } = string.Empty;

        public ResponseMatchMode MatchMode { get; set; } = ResponseMatchMode.Contains;

        public TextEncodingKind TextEncoding { get; set; } = TextEncodingKind.Ascii;

        public int TimeoutMilliseconds { get; set; } = 800;

        public List<SerialPortSettings> PortSettings { get; set; } = new List<SerialPortSettings>();

        /// <summary>
        /// 上次扫描匹配到的串口号（如 "COM3"）；空串表示尚未搜索到过。
        /// 只读展示字段，由扫描结果回写，用户不可编辑。
        /// </summary>
        public string LastScanPortName { get; set; } = string.Empty;
    }
}
