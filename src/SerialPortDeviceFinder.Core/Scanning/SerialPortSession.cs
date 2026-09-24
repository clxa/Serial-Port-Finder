using System;
using System.IO.Ports;
using SerialPortDeviceFinder.Core.Models;

namespace SerialPortDeviceFinder.Core.Scanning
{
    /// <summary>
    /// 对 System.IO.Ports.SerialPort 的最小包装，方便将硬件 I/O 与探测逻辑分离。
    /// </summary>
    public sealed class SerialPortSession : ISerialSession
    {
        private readonly SerialPort _serialPort = new SerialPort();

        public void Open(string portName, SerialPortSettings settings)
        {
            if (string.IsNullOrWhiteSpace(portName))
            {
                throw new ArgumentException("串口名称不能为空。", nameof(portName));
            }

            if (settings == null)
            {
                throw new ArgumentNullException(nameof(settings));
            }

            _serialPort.PortName = portName;
            _serialPort.BaudRate = settings.BaudRate;
            _serialPort.DataBits = settings.DataBits;
            _serialPort.Parity = settings.Parity;
            _serialPort.StopBits = settings.StopBits;
            _serialPort.Handshake = settings.Handshake;
            _serialPort.Open();
        }

        public void ClearBuffers()
        {
            _serialPort.DiscardInBuffer();
            _serialPort.DiscardOutBuffer();
        }

        public void Write(byte[] bytes)
        {
            if (bytes == null)
            {
                throw new ArgumentNullException(nameof(bytes));
            }

            _serialPort.Write(bytes, 0, bytes.Length);
        }

        public byte[] ReadAvailable()
        {
            var count = _serialPort.BytesToRead;
            if (count <= 0)
            {
                return Array.Empty<byte>();
            }

            var buffer = new byte[count];
            var readCount = _serialPort.Read(buffer, 0, buffer.Length);
            if (readCount == buffer.Length)
            {
                return buffer;
            }

            var result = new byte[readCount];
            Buffer.BlockCopy(buffer, 0, result, 0, readCount);
            return result;
        }

        public void Close()
        {
            if (_serialPort.IsOpen)
            {
                _serialPort.Close();
            }
        }

        public void Dispose()
        {
            _serialPort.Dispose();
        }
    }
}
