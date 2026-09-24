using System;
using System.IO;
using System.Threading;
using NUnit.Framework;
using SerialPortDeviceFinder.Core.Models;
using SerialPortDeviceFinder.Core.Scanning;
using SerialPortDeviceFinder.Core.Tests.Scanning.Fakes;

namespace SerialPortDeviceFinder.Core.Tests.Scanning
{
    [TestFixture]
    public sealed class SerialProbeTests
    {
        [Test]
        public void Probe_MatchedResponse_ClosesAndDisposesSession()
        {
            var session = new FakeSerialSession(new byte[] { 0x4F, 0x4B, 0x0D, 0x0A });
            var profile = TestProfiles.Text("设备A", "AT", "OK");

            var result = new SerialProbe(() => session).Probe("COM7", profile, SerialPortSettings.Default9600(), CancellationToken.None);

            Assert.That(result.Status, Is.EqualTo(ScanStatus.Matched));
            Assert.That(session.WasClosed, Is.True);
            Assert.That(session.WasDisposed, Is.True);
            Assert.That(session.WrittenBytes, Is.EqualTo(new byte[] { 0x41, 0x54, 0x0D, 0x0A }));
        }

        [Test]
        public void Probe_OpenSucceeded_ClearsBuffersBeforeWritingCommand()
        {
            var session = new FakeSerialSession(new byte[] { 0x4F, 0x4B });
            var profile = TestProfiles.Text("设备A", "AT", "OK");

            var result = new SerialProbe(() => session).Probe("COM7", profile, SerialPortSettings.Default9600(), CancellationToken.None);

            Assert.That(result.Status, Is.EqualTo(ScanStatus.Matched));
            Assert.That(session.WereBuffersCleared, Is.True);
            Assert.That(session.OperationLog, Is.EqualTo(new[] { "Open", "ClearBuffers", "Write" }));
        }

        [Test]
        public void Probe_ClearBuffersThrowsIOException_ReturnsCommunicationErrorAndReleasesSession()
        {
            var session = new FakeSerialSession(Array.Empty<byte>())
            {
                ClearBuffersException = new IOException("清空缓冲失败")
            };
            var profile = TestProfiles.Text("设备A", "AT", "OK");

            var result = new SerialProbe(() => session).Probe("COM7", profile, SerialPortSettings.Default9600(), CancellationToken.None);

            Assert.That(result.Status, Is.EqualTo(ScanStatus.CommunicationError));
            Assert.That(session.WereBuffersCleared, Is.True);
            Assert.That(session.WrittenBytes, Is.Empty);
            Assert.That(session.WasClosed, Is.True);
            Assert.That(session.WasDisposed, Is.True);
        }

        [Test]
        public void Probe_NoResponse_ReturnsTimeoutAndClosesSession()
        {
            var session = new FakeSerialSession(Array.Empty<byte>());
            var profile = TestProfiles.Text("设备A", "AT", "OK");

            var result = new SerialProbe(() => session).Probe("COM7", profile, SerialPortSettings.Default9600(), CancellationToken.None);

            Assert.That(result.Status, Is.EqualTo(ScanStatus.Timeout));
            Assert.That(session.WasClosed, Is.True);
            Assert.That(session.WasDisposed, Is.True);
        }

        [Test]
        public void Probe_OpenThrowsUnauthorizedAccess_ReturnsPortUnavailableAndDisposesSession()
        {
            var session = new FakeSerialSession(Array.Empty<byte>())
            {
                OpenException = new UnauthorizedAccessException("端口被占用")
            };
            var profile = TestProfiles.Text("设备A", "AT", "OK");

            var result = new SerialProbe(() => session).Probe("COM7", profile, SerialPortSettings.Default9600(), CancellationToken.None);

            Assert.That(result.Status, Is.EqualTo(ScanStatus.PortUnavailable));
            Assert.That(session.WasClosed, Is.True);
            Assert.That(session.WasDisposed, Is.True);
        }

        [Test]
        public void Probe_AlreadyCancelled_DoesNotCreateSessionAndReturnsCancelled()
        {
            var session = new FakeSerialSession(Array.Empty<byte>());
            var factory = new FakeSerialSessionFactory(() => session);
            var profile = TestProfiles.Text("设备A", "AT", "OK");
            using (var cancellationSource = new CancellationTokenSource())
            {
                cancellationSource.Cancel();

                var result = new SerialProbe(factory).Probe("COM7", profile, SerialPortSettings.Default9600(), cancellationSource.Token);

                Assert.That(result.Status, Is.EqualTo(ScanStatus.Cancelled));
                Assert.That(factory.CreateCallCount, Is.EqualTo(0));
                Assert.That(session.WasOpened, Is.False);
            }
        }

        [Test]
        public void Probe_CleanupThrows_PreservesResultAndStillDisposesSession()
        {
            var session = new FakeSerialSession(new byte[] { 0x4F, 0x4B })
            {
                CloseException = new InvalidOperationException("关闭失败"),
                DisposeException = new InvalidOperationException("释放失败")
            };
            var profile = TestProfiles.Text("设备A", "AT", "OK");
            ScanResult? result = null;

            Assert.DoesNotThrow(() =>
                result = new SerialProbe(() => session).Probe("COM7", profile, SerialPortSettings.Default9600(), CancellationToken.None));

            Assert.That(result!.Status, Is.EqualTo(ScanStatus.Matched));
            Assert.That(session.WasClosed, Is.True);
            Assert.That(session.WasDisposed, Is.True);
        }
    }
}
