using System;
using SerialPortDeviceFinder.Core.Scanning;

namespace SerialPortDeviceFinder.Core.Tests.Scanning.Fakes
{
    internal sealed class FakeSerialSessionFactory : ISerialSessionFactory
    {
        private readonly Func<ISerialSession> _create;

        public FakeSerialSessionFactory(Func<ISerialSession> create)
        {
            _create = create;
        }

        public int CreateCallCount { get; private set; }

        public ISerialSession Create()
        {
            CreateCallCount++;
            return _create();
        }
    }
}
