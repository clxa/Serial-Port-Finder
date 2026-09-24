using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using SerialPortDeviceFinder.Core.Models;
using SerialPortDeviceFinder.Core.Validation;

namespace SerialPortDeviceFinder.Core.Configuration
{
    public sealed class DeviceProfileStore
    {
        private readonly string _filePath;

        public DeviceProfileStore(string applicationDirectory)
        {
            if (string.IsNullOrWhiteSpace(applicationDirectory))
            {
                throw new ArgumentException("应用程序目录不能为空。", nameof(applicationDirectory));
            }

            _filePath = Path.Combine(applicationDirectory, "devices.json");
        }

        public IReadOnlyList<DeviceProfile> Load()
        {
            if (!File.Exists(_filePath))
            {
                return Array.Empty<DeviceProfile>();
            }

            try
            {
                var content = File.ReadAllText(_filePath, Encoding.UTF8);
                var profiles = JsonConvert.DeserializeObject<List<DeviceProfile>>(content);
                if (profiles == null)
                {
                    throw new InvalidDataException($"配置文件内容为空：{_filePath}");
                }

                for (var index = 0; index < profiles.Count; index++)
                {
                    if (profiles[index] == null)
                    {
                        throw new InvalidDataException($"配置文件包含空设备模板：{_filePath}");
                    }
                }

                return profiles;
            }
            catch (JsonException exception)
            {
                throw new InvalidDataException($"无法加载配置文件：{_filePath}", exception);
            }
        }

        public void Save(IReadOnlyList<DeviceProfile> profiles)
        {
            var errors = DeviceProfileValidator.ValidateProfiles(profiles);
            if (errors.Count > 0)
            {
                throw new InvalidOperationException(string.Join(Environment.NewLine, errors));
            }

            var directory = Path.GetDirectoryName(_filePath);
            if (string.IsNullOrEmpty(directory))
            {
                throw new InvalidOperationException($"无法确定配置文件目录：{_filePath}");
            }

            Directory.CreateDirectory(directory);
            var temporaryPath = Path.Combine(directory, $"devices.{Guid.NewGuid():N}.tmp.json");

            try
            {
                WriteJsonFile(temporaryPath, profiles);

                if (File.Exists(_filePath))
                {
                    File.Replace(temporaryPath, _filePath, null);
                }
                else
                {
                    File.Move(temporaryPath, _filePath);
                }
            }
            finally
            {
                if (File.Exists(temporaryPath))
                {
                    File.Delete(temporaryPath);
                }
            }
        }

        private static void WriteJsonFile(string path, IReadOnlyList<DeviceProfile> profiles)
        {
            var json = JsonConvert.SerializeObject(profiles, Formatting.Indented);
            using (var stream = new FileStream(path, FileMode.CreateNew, FileAccess.Write, FileShare.None))
            using (var writer = new StreamWriter(stream, new UTF8Encoding(encoderShouldEmitUTF8Identifier: false)))
            {
                writer.Write(json);
            }
        }
    }
}
