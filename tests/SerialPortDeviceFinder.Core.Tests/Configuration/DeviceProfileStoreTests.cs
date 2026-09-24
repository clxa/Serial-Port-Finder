using System;
using System.IO;
using System.Linq;
using NUnit.Framework;
using SerialPortDeviceFinder.Core.Configuration;
using SerialPortDeviceFinder.Core.Models;

namespace SerialPortDeviceFinder.Core.Tests.Configuration
{
    [TestFixture]
    public sealed class DeviceProfileStoreTests
    {
        [Test]
        public void Save_ValidProfiles_CanBeLoadedWithoutLoss()
        {
            using (var directory = new TemporaryDirectory())
            {
                var store = new DeviceProfileStore(directory.Path);
                var original = new[] { TestProfiles.Text("流量计", "READ", "VALUE") };

                store.Save(original);

                var loaded = store.Load();
                Assert.That(loaded.Single().Name, Is.EqualTo("流量计"));
                Assert.That(loaded.Single().CommandContent, Is.EqualTo("READ"));
                Assert.That(loaded.Single().ExpectedResponse, Is.EqualTo("VALUE"));
                Assert.That(loaded.Single().PortSettings.Single().BaudRate, Is.EqualTo(9600));
            }
        }

        [Test]
        public void Save_LastScanPortName_IsPersistedAndRestored()
        {
            using (var directory = new TemporaryDirectory())
            {
                var store = new DeviceProfileStore(directory.Path);
                var profile = TestProfiles.Text("流量计", "READ", "VALUE");
                profile.LastScanPortName = "COM3";

                store.Save(new[] { profile });

                Assert.That(store.Load().Single().LastScanPortName, Is.EqualTo("COM3"));
            }
        }

        [Test]
        public void Load_OldJsonWithoutLastScanPortName_DefaultsToEmpty()
        {
            using (var directory = new TemporaryDirectory())
            {
                var filePath = Path.Combine(directory.Path, "devices.json");
                File.WriteAllText(filePath,
                    "[{\"Name\":\"老模板\",\"CommandContent\":\"READ\",\"ExpectedResponse\":\"OK\","
                    + "\"PortSettings\":[{\"BaudRate\":9600,\"DataBits\":8,\"Parity\":0,\"StopBits\":1}]}]");

                var loaded = new DeviceProfileStore(directory.Path).Load().Single();

                Assert.That(loaded.LastScanPortName, Is.EqualTo(string.Empty));
            }
        }

        [Test]
        public void Load_FileDoesNotExist_ReturnsEmptyList()
        {
            using (var directory = new TemporaryDirectory())
            {
                var loaded = new DeviceProfileStore(directory.Path).Load();

                Assert.That(loaded, Is.Empty);
            }
        }

        [Test]
        public void Load_MalformedJson_ThrowsWithPathAndKeepsOriginalFile()
        {
            using (var directory = new TemporaryDirectory())
            {
                var filePath = Path.Combine(directory.Path, "devices.json");
                const string originalContent = "{ this is not valid json";
                File.WriteAllText(filePath, originalContent);
                var store = new DeviceProfileStore(directory.Path);

                var exception = Assert.Throws<InvalidDataException>(() => store.Load());

                Assert.That(exception!.Message, Does.Contain(filePath));
                Assert.That(File.ReadAllText(filePath), Is.EqualTo(originalContent));
            }
        }

        [Test]
        public void Load_NullJsonModel_ThrowsWithPath()
        {
            using (var directory = new TemporaryDirectory())
            {
                var filePath = Path.Combine(directory.Path, "devices.json");
                File.WriteAllText(filePath, "null");

                var exception = Assert.Throws<InvalidDataException>(() => new DeviceProfileStore(directory.Path).Load());

                Assert.That(exception!.Message, Does.Contain(filePath));
            }
        }

        [Test]
        public void Load_ContainsNullProfile_ThrowsWithPath()
        {
            using (var directory = new TemporaryDirectory())
            {
                var filePath = Path.Combine(directory.Path, "devices.json");
                File.WriteAllText(filePath, "[null]");

                var exception = Assert.Throws<InvalidDataException>(() => new DeviceProfileStore(directory.Path).Load());

                Assert.That(exception!.Message, Does.Contain(filePath));
            }
        }

        [Test]
        public void Save_InvalidProfiles_ThrowsAndKeepsExistingFile()
        {
            using (var directory = new TemporaryDirectory())
            {
                var store = new DeviceProfileStore(directory.Path);
                store.Save(new[] { TestProfiles.Text("原有设备", "READ", "VALUE") });

                var filePath = Path.Combine(directory.Path, "devices.json");
                var originalContent = File.ReadAllText(filePath);
                var invalidProfile = TestProfiles.Text(string.Empty, "READ", "VALUE");

                Assert.Throws<InvalidOperationException>(() => store.Save(new[] { invalidProfile }));

                Assert.That(File.ReadAllText(filePath), Is.EqualTo(originalContent));
            }
        }

        [Test]
        public void Load_JsonWithoutEnabledField_TreatsRowsAsEnabled()
        {
            using (var directory = new TemporaryDirectory())
            {
                var filePath = Path.Combine(directory.Path, "devices.json");
                File.WriteAllText(
                    filePath,
                    "[{\"Name\":\"旧设备\",\"CommandFormat\":0,\"CommandContent\":\"AT\",\"TextTerminator\":0,\"ResponseFormat\":0,\"ExpectedResponse\":\"OK\",\"MatchMode\":0,\"TextEncoding\":0,\"TimeoutMilliseconds\":800,\"PortSettings\":[{\"BaudRate\":9600,\"DataBits\":8,\"Parity\":0,\"StopBits\":1,\"Handshake\":0}]}]");

                var loaded = new DeviceProfileStore(directory.Path).Load();

                Assert.That(loaded.Single().PortSettings.Single().Enabled, Is.True);
            }
        }

        [Test]
        public void Save_ExistingFile_ReplacesWithNewProfilesAndRemovesTemporaryFile()
        {
            using (var directory = new TemporaryDirectory())
            {
                var store = new DeviceProfileStore(directory.Path);
                store.Save(new[] { TestProfiles.Text("设备A", "READ_A", "VALUE_A") });

                store.Save(new[] { TestProfiles.Text("设备B", "READ_B", "VALUE_B") });

                var loaded = store.Load();
                Assert.That(loaded.Select(profile => profile.Name), Is.EqualTo(new[] { "设备B" }));
                Assert.That(loaded.Single().CommandContent, Is.EqualTo("READ_B"));
                Assert.That(Directory.EnumerateFiles(directory.Path, "devices.*.tmp.json"), Is.Empty);
            }
        }

        private sealed class TemporaryDirectory : IDisposable
        {
            public TemporaryDirectory()
            {
                Path = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "SerialPortDeviceFinder.Tests", Guid.NewGuid().ToString("N"));
                Directory.CreateDirectory(Path);
            }

            public string Path { get; }

            public void Dispose()
            {
                if (Directory.Exists(Path))
                {
                    Directory.Delete(Path, true);
                }
            }
        }
    }
}
