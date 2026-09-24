using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using SerialPortDeviceFinder.Core.Models;
using SerialPortDeviceFinder.Core.Protocol;

namespace SerialPortDeviceFinder.Core.Validation
{
    public static class DeviceProfileValidator
    {
        public static IReadOnlyList<string> ValidateProfiles(IReadOnlyList<DeviceProfile> profiles)
        {
            if (profiles == null)
            {
                throw new ArgumentNullException(nameof(profiles));
            }

            var errors = new List<string>();
            var names = new HashSet<string>(StringComparer.Ordinal);

            for (var index = 0; index < profiles.Count; index++)
            {
                var profile = profiles[index];
                if (profile == null)
                {
                    errors.Add($"第 {index + 1} 个设备模板不能为空。");
                    continue;
                }

                errors.AddRange(Validate(profile));

                if (!string.IsNullOrWhiteSpace(profile.Name) && !names.Add(profile.Name))
                {
                    errors.Add($"设备模板名称不能重复：{profile.Name}。");
                }
            }

            return errors;
        }

        public static IReadOnlyList<string> Validate(DeviceProfile profile)
        {
            if (profile == null)
            {
                throw new ArgumentNullException(nameof(profile));
            }

            var errors = new List<string>();

            if (!Enum.IsDefined(typeof(PayloadFormat), profile.CommandFormat))
            {
                errors.Add("命令格式无效。");
            }

            if (!Enum.IsDefined(typeof(PayloadFormat), profile.ResponseFormat))
            {
                errors.Add("响应格式无效。");
            }

            if (!Enum.IsDefined(typeof(ResponseMatchMode), profile.MatchMode))
            {
                errors.Add("响应匹配方式无效。");
            }

            if ((profile.CommandFormat == PayloadFormat.Text || profile.ResponseFormat == PayloadFormat.Text)
                && !Enum.IsDefined(typeof(TextEncodingKind), profile.TextEncoding))
            {
                errors.Add("文本编码无效。");
            }

            if (profile.CommandFormat == PayloadFormat.Text
                && !Enum.IsDefined(typeof(TextTerminator), profile.TextTerminator))
            {
                errors.Add("文本结束符无效。");
            }

            if (string.IsNullOrWhiteSpace(profile.Name))
            {
                errors.Add("设备名称不能为空。");
            }

            if (string.IsNullOrWhiteSpace(profile.CommandContent))
            {
                errors.Add("命令不能为空。");
            }
            else if (profile.CommandFormat == PayloadFormat.Hex
                     && !ProtocolCodec.TryParseHex(profile.CommandContent, out _))
            {
                errors.Add("十六进制命令格式无效。");
            }

            if (string.IsNullOrWhiteSpace(profile.ExpectedResponse))
            {
                errors.Add("预期响应不能为空。");
            }
            else if (profile.ResponseFormat == PayloadFormat.Hex
                     && !ProtocolCodec.TryParseHex(profile.ExpectedResponse, out _))
            {
                errors.Add("十六进制预期响应格式无效。");
            }

            if (profile.TimeoutMilliseconds < 100 || profile.TimeoutMilliseconds > 10000)
            {
                errors.Add("超时时间必须在 100 到 10000 毫秒之间。");
            }

            if (profile.PortSettings == null || profile.PortSettings.Count == 0)
            {
                errors.Add("至少需要一组串口参数。");
                return errors;
            }

            for (var index = 0; index < profile.PortSettings.Count; index++)
            {
                var settings = profile.PortSettings[index];
                var settingNumber = index + 1;

                if (settings == null)
                {
                    errors.Add($"第 {settingNumber} 组串口参数不能为空。");
                    continue;
                }

                if (settings.BaudRate <= 0)
                {
                    errors.Add($"第 {settingNumber} 组串口参数的波特率必须大于 0。");
                }

                if (settings.DataBits < 5 || settings.DataBits > 8)
                {
                    errors.Add($"第 {settingNumber} 组串口参数的数据位必须在 5 到 8 之间。");
                }

                if (!Enum.IsDefined(typeof(Parity), settings.Parity))
                {
                    errors.Add($"第 {settingNumber} 组串口参数的校验位无效。");
                }

                if (!Enum.IsDefined(typeof(StopBits), settings.StopBits)
                    || (settings.StopBits != StopBits.One
                        && settings.StopBits != StopBits.OnePointFive
                        && settings.StopBits != StopBits.Two))
                {
                    errors.Add($"第 {settingNumber} 组串口参数的停止位无效。");
                }

                if (settings.Handshake != Handshake.None)
                {
                    errors.Add($"第 {settingNumber} 组串口参数的握手方式必须为 None。");
                }
            }

            if (!profile.PortSettings.Any(settings => settings?.Enabled == true))
            {
                errors.Add("至少需要一组启用的串口参数。");
            }

            return errors;
        }
    }
}
