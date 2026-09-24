using System.IO.Ports;

namespace SerialPortDeviceFinder.Core.Models
{
    public sealed class SerialPortSettings
    {
        public int BaudRate { get; set; } = 9600;

        public int DataBits { get; set; } = 8;

        public Parity Parity { get; set; } = Parity.None;

        public StopBits StopBits { get; set; } = StopBits.One;

        public Handshake Handshake { get; set; } = Handshake.None;

        /// <summary>
        /// 是否参与扫描。默认启用；旧配置读取时缺省为 true，完全兼容。
        /// </summary>
        public bool Enabled { get; set; } = true;

        public static SerialPortSettings Default9600()
        {
            return new SerialPortSettings();
        }
    }
}
