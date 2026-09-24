using System.IO.Ports;
using SerialPortDeviceFinder.Core.Models;

namespace SerialPortDeviceFinder.WinForms
{
    /// <summary>
    /// 常用串口组合目录项：显示文本（如 "9600 8N1"）+ 生成对应 SerialPortSettings 的工厂。
    /// </summary>
    public sealed class ComboOption
    {
        public ComboOption(string displayText, int baudRate, int dataBits, Parity parity, StopBits stopBits)
        {
            DisplayText = displayText;
            BaudRate = baudRate;
            DataBits = dataBits;
            Parity = parity;
            StopBits = stopBits;
        }

        public string DisplayText { get; }
        public int BaudRate { get; }
        public int DataBits { get; }
        public Parity Parity { get; }
        public StopBits StopBits { get; }

        public SerialPortSettings CreateSettings()
        {
            return new SerialPortSettings
            {
                BaudRate = BaudRate,
                DataBits = DataBits,
                Parity = Parity,
                StopBits = StopBits,
                Handshake = Handshake.None
            };
        }

        public override string ToString()
        {
            return DisplayText;
        }
    }
}
