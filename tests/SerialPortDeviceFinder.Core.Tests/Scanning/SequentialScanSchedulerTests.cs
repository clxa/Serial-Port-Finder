using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using SerialPortDeviceFinder.Core.Models;
using SerialPortDeviceFinder.Core.Scanning;
using SerialPortDeviceFinder.Core.Tests.Scanning.Fakes;

namespace SerialPortDeviceFinder.Core.Tests.Scanning
{
    [TestFixture]
    public sealed class SequentialScanSchedulerTests
    {
        [Test]
        public async Task ScanAsync_MatchedPort_SkipsRemainingProfilesForThatPort()
        {
            var catalog = new FakeSerialPortCatalog("COM3");
            var probe = new FakeSerialProbe(ScanStatus.Matched);
            var scheduler = new SequentialScanScheduler(catalog, probe);

            var results = await scheduler.ScanAsync(
                new[] { TestProfiles.Text("A", "A", "OK"), TestProfiles.Text("B", "B", "OK") },
                CancellationToken.None,
                progress: null);

            Assert.That(probe.Calls.Select(call => call.Profile.Name), Is.EqualTo(new[] { "A" }));
            Assert.That(results.Count(result => result.Status == ScanStatus.Matched), Is.EqualTo(1));
        }

        [Test]
        public async Task ScanAsync_SortsComPortsNaturally()
        {
            var catalog = new FakeSerialPortCatalog("COM10", "COM2", "COM1");
            var probe = new FakeSerialProbe(ScanStatus.Timeout);
            var scheduler = new SequentialScanScheduler(catalog, probe);

            await scheduler.ScanAsync(new[] { TestProfiles.Text("A", "A", "OK") }, CancellationToken.None, null);

            Assert.That(probe.Calls.Select(call => call.PortName), Is.EqualTo(new[] { "COM1", "COM2", "COM10" }));
        }

        [Test]
        public async Task ScanAsync_ProfileWithTwoSettings_ProbesBothInConfiguredOrder()
        {
            var profile = TestProfiles.Text("A", "A", "OK");
            profile.PortSettings.Add(new SerialPortSettings { BaudRate = 115200 });
            var probe = new FakeSerialProbe(ScanStatus.NotMatched);
            var scheduler = new SequentialScanScheduler(new FakeSerialPortCatalog("COM3"), probe);

            await scheduler.ScanAsync(new[] { profile }, CancellationToken.None, null);

            Assert.That(probe.Calls.Select(call => call.Settings.BaudRate), Is.EqualTo(new[] { 9600, 115200 }));
        }

        [Test]
        public async Task ScanAsync_FailedPort_DoesNotPreventLaterPorts()
        {
            var probe = new FakeSerialProbe(ScanStatus.CommunicationError);
            var scheduler = new SequentialScanScheduler(new FakeSerialPortCatalog("COM4", "COM3"), probe);

            var results = await scheduler.ScanAsync(new[] { TestProfiles.Text("A", "A", "OK") }, CancellationToken.None, null);

            Assert.That(probe.Calls.Select(call => call.PortName), Is.EqualTo(new[] { "COM3", "COM4" }));
            Assert.That(results, Has.Count.EqualTo(2));
        }

        [Test]
        public async Task ScanAsync_ProbeThrowsIOException_RecordsCommunicationErrorAndScansNextPort()
        {
            var probe = new FakeSerialProbe(call => throw new IOException("模拟串口读写失败"));
            var scheduler = new SequentialScanScheduler(new FakeSerialPortCatalog("COM3", "COM4"), probe);

            var results = await scheduler.ScanAsync(new[] { TestProfiles.Text("A", "A", "OK") }, CancellationToken.None, null);

            Assert.That(probe.Calls.Select(call => call.PortName), Is.EqualTo(new[] { "COM3", "COM4" }));
            Assert.That(results.Select(result => result.PortName), Is.EqualTo(new[] { "COM3", "COM4" }));
            Assert.That(results.Select(result => result.Status), Is.EqualTo(new[]
            {
                ScanStatus.CommunicationError,
                ScanStatus.CommunicationError
            }));
        }

        [Test]
        public async Task ScanAsync_CancelledAfterFirstResult_DoesNotStartNextTask()
        {
            using (var cancellationSource = new CancellationTokenSource())
            {
                var probe = new FakeSerialProbe(call =>
                {
                    cancellationSource.Cancel();
                    return CreateResult(call, ScanStatus.Cancelled);
                });
                var scheduler = new SequentialScanScheduler(new FakeSerialPortCatalog("COM3", "COM4"), probe);

                var results = await scheduler.ScanAsync(
                    new[] { TestProfiles.Text("A", "A", "OK") },
                    cancellationSource.Token,
                    null);

                Assert.That(probe.Calls, Has.Count.EqualTo(1));
                Assert.That(results, Has.Count.EqualTo(1));
            }
        }

        [Test]
        public async Task ScanAsync_ProbeCancelsAndThrowsOperationCanceledException_ReportsCancelledAndStops()
        {
            using (var cancellationSource = new CancellationTokenSource())
            {
                var probe = new FakeSerialProbe(call =>
                {
                    cancellationSource.Cancel();
                    throw new OperationCanceledException(cancellationSource.Token);
                });
                var scheduler = new SequentialScanScheduler(new FakeSerialPortCatalog("COM3", "COM4"), probe);

                var results = await scheduler.ScanAsync(
                    new[] { TestProfiles.Text("A", "A", "OK") },
                    cancellationSource.Token,
                    null);

                Assert.That(probe.Calls, Has.Count.EqualTo(1));
                Assert.That(results, Has.Count.EqualTo(1));
                Assert.That(results.Single().PortName, Is.EqualTo("COM3"));
                Assert.That(results.Single().Status, Is.EqualTo(ScanStatus.Cancelled));
            }
        }

        [Test]
        public async Task ScanAsync_DisabledSettings_AreSkipped()
        {
            var profile = TestProfiles.Text("A", "A", "OK");
            profile.PortSettings.Add(new SerialPortSettings { BaudRate = 115200, Enabled = false });
            var probe = new FakeSerialProbe(ScanStatus.NotMatched);
            var scheduler = new SequentialScanScheduler(new FakeSerialPortCatalog("COM3"), probe);

            await scheduler.ScanAsync(new[] { profile }, CancellationToken.None, null);

            Assert.That(probe.Calls.Select(call => call.Settings.BaudRate), Is.EqualTo(new[] { 9600 }));
        }

        [Test]
        public async Task ScanAsync_DisabledProfile_DoesNotCallProbe()
        {
            var profile = TestProfiles.Text("A", "A", "OK");
            profile.IsEnabled = false;
            var probe = new FakeSerialProbe(ScanStatus.Matched);
            var scheduler = new SequentialScanScheduler(new FakeSerialPortCatalog("COM3"), probe);

            var results = await scheduler.ScanAsync(new[] { profile }, CancellationToken.None, null);

            Assert.That(probe.Calls, Is.Empty);
            Assert.That(results, Is.Empty);
        }

        [Test]
        public async Task ScanAsync_ReportsEveryProbeResultOnce()
        {
            var progress = new RecordingProgress();
            var probe = new FakeSerialProbe(ScanStatus.Timeout);
            var scheduler = new SequentialScanScheduler(new FakeSerialPortCatalog("COM3"), probe);

            var results = await scheduler.ScanAsync(
                new[] { TestProfiles.Text("A", "A", "OK"), TestProfiles.Text("B", "B", "OK") },
                CancellationToken.None,
                progress);

            Assert.That(progress.Results, Is.EqualTo(results));
        }

        [Test]
        public async Task ScanAsync_SnapshotsEnabledProfilesAndSettingsBeforeFirstProbe()
        {
            var profile = TestProfiles.Text("A", "A", "OK");
            profile.PortSettings.Add(new SerialPortSettings { BaudRate = 115200 });
            var probe = new FakeSerialProbe(call =>
            {
                if (call.PortName == "COM3" && call.Settings.BaudRate == 9600)
                {
                    profile.IsEnabled = false;
                    profile.PortSettings.Clear();
                }

                return CreateResult(call, ScanStatus.Timeout);
            });
            var scheduler = new SequentialScanScheduler(new FakeSerialPortCatalog("COM3", "COM4"), probe);

            await scheduler.ScanAsync(new[] { profile }, CancellationToken.None, null);

            Assert.That(
                probe.Calls.Select(call => call.PortName + ":" + call.Settings.BaudRate),
                Is.EqualTo(new[] { "COM3:9600", "COM3:115200", "COM4:9600", "COM4:115200" }));
        }

        private static ScanResult CreateResult(ProbeCall call, ScanStatus status)
        {
            return new ScanResult
            {
                PortName = call.PortName,
                ProfileName = call.Profile.Name,
                PortSettings = call.Settings,
                Status = status
            };
        }

        private sealed class RecordingProgress : IProgress<ScanResult>
        {
            public List<ScanResult> Results { get; } = new List<ScanResult>();

            public void Report(ScanResult value)
            {
                Results.Add(value);
            }
        }
    }
}
