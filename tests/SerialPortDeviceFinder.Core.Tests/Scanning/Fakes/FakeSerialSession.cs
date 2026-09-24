using System;
using System.Collections.Generic;
using SerialPortDeviceFinder.Core.Models;
using SerialPortDeviceFinder.Core.Scanning;

namespace SerialPortDeviceFinder.Core.Tests.Scanning.Fakes
{
    internal sealed class FakeSerialSession : ISerialSession
    {
        private readonly byte[] _response;
        private bool _responseWasRead;

        public FakeSerialSession(byte[] response)
        {
            _response = response ?? Array.Empty<byte>();
        }

        public Exception? OpenException { get; set; }

        public Exception? CloseException { get; set; }

        public Exception? ClearBuffersException { get; set; }

        public Exception? DisposeException { get; set; }

        public bool WasOpened { get; private set; }

        public bool WasClosed { get; private set; }

        public bool WasDisposed { get; private set; }

        public bool WereBuffersCleared { get; private set; }

        public IList<string> OperationLog { get; } = new List<string>();

        public byte[] WrittenBytes { get; private set; } = Array.Empty<byte>();

        public void Open(string portName, SerialPortSettings settings)
        {
            OperationLog.Add("Open");
            if (OpenException != null)
            {
                throw OpenException;
            }

            WasOpened = true;
        }

        public void ClearBuffers()
        {
            OperationLog.Add("ClearBuffers");
            WereBuffersCleared = true;
            if (ClearBuffersException != null)
            {
                throw ClearBuffersException;
            }
        }

        public void Write(byte[] bytes)
        {
            OperationLog.Add("Write");
            WrittenBytes = bytes ?? Array.Empty<byte>();
        }

        public byte[] ReadAvailable()
        {
            if (_responseWasRead)
            {
                return Array.Empty<byte>();
            }

            _responseWasRead = true;
            return _response;
        }

        public void Close()
        {
            WasClosed = true;
            if (CloseException != null)
            {
                throw CloseException;
            }
        }

        public void Dispose()
        {
            WasDisposed = true;
            if (DisposeException != null)
            {
                throw DisposeException;
            }
        }
    }
}
