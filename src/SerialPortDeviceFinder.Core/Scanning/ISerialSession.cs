using System;
using SerialPortDeviceFinder.Core.Models;

namespace SerialPortDeviceFinder.Core.Scanning
{
    /// <summary>
    /// 一次串口探测使用的短生命周期会话。
    /// </summary>
    public interface ISerialSession : IDisposable
    {
        void Open(string portName, SerialPortSettings settings);

        /// <summary>
        /// 清空本次探测前遗留在串口驱动中的收发缓冲区。
        /// </summary>
        void ClearBuffers();

        void Write(byte[] bytes);

        byte[] ReadAvailable();

        void Close();
    }
}
