using System.Collections.Generic;
using System.IO.Ports;
using NUnit.Framework;
using SerialPortDeviceFinder.Core.Models;
using SerialPortDeviceFinder.Core.Validation;

namespace SerialPortDeviceFinder.Core.Tests.Validation
{
    [TestFixture]
    public sealed class DeviceProfileValidatorTests
    {
        [Test]
        public void Validate_EmptyExpectedResponse_ReturnsSpecificError()
        {
            var profile = CreateValidTextProfile();
            profile.ExpectedResponse = string.Empty;

            var errors = DeviceProfileValidator.Validate(profile);

            Assert.That(errors, Does.Contain("预期响应不能为空。"));
        }

        [Test]
        public void Validate_TimeoutBelowMinimum_ReturnsSpecificError()
        {
            var profile = CreateValidTextProfile();
            profile.TimeoutMilliseconds = 99;

            var errors = DeviceProfileValidator.Validate(profile);

            Assert.That(errors, Does.Contain("超时时间必须在 100 到 10000 毫秒之间。"));
        }

        [Test]
        public void Validate_TimeoutAboveMaximum_ReturnsSpecificError()
        {
            var profile = CreateValidTextProfile();
            profile.TimeoutMilliseconds = 10001;

            var errors = DeviceProfileValidator.Validate(profile);

            Assert.That(errors, Does.Contain("超时时间必须在 100 到 10000 毫秒之间。"));
        }

        [Test]
        public void Validate_NoPortSettings_ReturnsSpecificError()
        {
            var profile = CreateValidTextProfile();
            profile.PortSettings = new List<SerialPortSettings>();

            var errors = DeviceProfileValidator.Validate(profile);

            Assert.That(errors, Does.Contain("至少需要一组串口参数。"));
        }

        [Test]
        public void Validate_AllPortSettingsDisabled_ReturnsSpecificError()
        {
            var profile = CreateValidTextProfile();
            profile.PortSettings[0].Enabled = false;

            var errors = DeviceProfileValidator.Validate(profile);

            Assert.That(errors, Does.Contain("至少需要一组启用的串口参数。"));
        }

        [Test]
        public void Validate_DataBitsBelowMinimum_ReturnsSpecificError()
        {
            var profile = CreateValidTextProfile();
            profile.PortSettings[0].DataBits = 4;

            var errors = DeviceProfileValidator.Validate(profile);

            Assert.That(errors, Does.Contain("第 1 组串口参数的数据位必须在 5 到 8 之间。"));
        }

        [Test]
        public void Validate_NonPositiveBaudRate_ReturnsSpecificError()
        {
            var profile = CreateValidTextProfile();
            profile.PortSettings[0].BaudRate = 0;

            var errors = DeviceProfileValidator.Validate(profile);

            Assert.That(errors, Does.Contain("第 1 组串口参数的波特率必须大于 0。"));
        }

        [Test]
        public void Validate_HandshakeOtherThanNone_ReturnsSpecificError()
        {
            var profile = CreateValidTextProfile();
            profile.PortSettings[0].Handshake = Handshake.RequestToSend;

            var errors = DeviceProfileValidator.Validate(profile);

            Assert.That(errors, Does.Contain("第 1 组串口参数的握手方式必须为 None。"));
        }

        [Test]
        public void Validate_InvalidParity_ReturnsSpecificError()
        {
            var profile = CreateValidTextProfile();
            profile.PortSettings[0].Parity = (Parity)99;

            var errors = DeviceProfileValidator.Validate(profile);

            Assert.That(errors, Does.Contain("第 1 组串口参数的校验位无效。"));
        }

        [Test]
        public void Validate_InvalidStopBits_ReturnsSpecificError()
        {
            var profile = CreateValidTextProfile();
            profile.PortSettings[0].StopBits = (StopBits)99;

            var errors = DeviceProfileValidator.Validate(profile);

            Assert.That(errors, Does.Contain("第 1 组串口参数的停止位无效。"));
        }

        [Test]
        public void Validate_StopBitsNone_ReturnsSpecificError()
        {
            var profile = CreateValidTextProfile();
            profile.PortSettings[0].StopBits = StopBits.None;

            var errors = DeviceProfileValidator.Validate(profile);

            Assert.That(errors, Does.Contain("第 1 组串口参数的停止位无效。"));
        }

        [Test]
        public void Validate_EmptyName_ReturnsSpecificError()
        {
            var profile = CreateValidTextProfile();
            profile.Name = "  ";

            var errors = DeviceProfileValidator.Validate(profile);

            Assert.That(errors, Does.Contain("设备名称不能为空。"));
        }

        [Test]
        public void Validate_EmptyCommand_ReturnsSpecificError()
        {
            var profile = CreateValidTextProfile();
            profile.CommandContent = string.Empty;

            var errors = DeviceProfileValidator.Validate(profile);

            Assert.That(errors, Does.Contain("命令不能为空。"));
        }

        [Test]
        public void Validate_ValidTextProfile_ReturnsNoErrors()
        {
            var errors = DeviceProfileValidator.Validate(CreateValidTextProfile());

            Assert.That(errors, Is.Empty);
        }

        [Test]
        public void Validate_ValidHexProfile_ReturnsNoErrors()
        {
            var profile = CreateValidTextProfile();
            profile.CommandFormat = PayloadFormat.Hex;
            profile.CommandContent = "AA 55 01";
            profile.ResponseFormat = PayloadFormat.Hex;
            profile.ExpectedResponse = "01 02";

            var errors = DeviceProfileValidator.Validate(profile);

            Assert.That(errors, Is.Empty);
        }

        [Test]
        public void Validate_InvalidHexCommand_ReturnsSpecificError()
        {
            var profile = CreateValidTextProfile();
            profile.CommandFormat = PayloadFormat.Hex;
            profile.CommandContent = "GG";

            var errors = DeviceProfileValidator.Validate(profile);

            Assert.That(errors, Does.Contain("十六进制命令格式无效。"));
        }

        [Test]
        public void Validate_InvalidHexExpectedResponse_ReturnsSpecificError()
        {
            var profile = CreateValidTextProfile();
            profile.ResponseFormat = PayloadFormat.Hex;
            profile.ExpectedResponse = "GG";

            var errors = DeviceProfileValidator.Validate(profile);

            Assert.That(errors, Does.Contain("十六进制预期响应格式无效。"));
        }

        [Test]
        public void Validate_InvalidCommandFormat_ReturnsSpecificError()
        {
            var profile = CreateValidTextProfile();
            profile.CommandFormat = (PayloadFormat)99;

            var errors = DeviceProfileValidator.Validate(profile);

            Assert.That(errors, Does.Contain("命令格式无效。"));
        }

        [Test]
        public void Validate_InvalidResponseFormat_ReturnsSpecificError()
        {
            var profile = CreateValidTextProfile();
            profile.ResponseFormat = (PayloadFormat)99;

            var errors = DeviceProfileValidator.Validate(profile);

            Assert.That(errors, Does.Contain("响应格式无效。"));
        }

        [Test]
        public void Validate_InvalidMatchMode_ReturnsSpecificError()
        {
            var profile = CreateValidTextProfile();
            profile.MatchMode = (ResponseMatchMode)99;

            var errors = DeviceProfileValidator.Validate(profile);

            Assert.That(errors, Does.Contain("响应匹配方式无效。"));
        }

        [Test]
        public void Validate_InvalidTextEncodingForTextResponse_ReturnsSpecificError()
        {
            var profile = CreateValidTextProfile();
            profile.CommandFormat = PayloadFormat.Hex;
            profile.CommandContent = "AA 55";
            profile.ResponseFormat = PayloadFormat.Text;
            profile.TextEncoding = (TextEncodingKind)99;

            var errors = DeviceProfileValidator.Validate(profile);

            Assert.That(errors, Does.Contain("文本编码无效。"));
        }

        [Test]
        public void Validate_InvalidTextTerminatorForTextCommand_ReturnsSpecificError()
        {
            var profile = CreateValidTextProfile();
            profile.TextTerminator = (TextTerminator)99;

            var errors = DeviceProfileValidator.Validate(profile);

            Assert.That(errors, Does.Contain("文本结束符无效。"));
        }

        [Test]
        public void Validate_TextOnlySettingsForHexCommandAndResponse_AreIgnored()
        {
            var profile = CreateValidTextProfile();
            profile.CommandFormat = PayloadFormat.Hex;
            profile.CommandContent = "AA 55";
            profile.ResponseFormat = PayloadFormat.Hex;
            profile.ExpectedResponse = "01 02";
            profile.TextEncoding = (TextEncodingKind)99;
            profile.TextTerminator = (TextTerminator)99;

            var errors = DeviceProfileValidator.Validate(profile);

            Assert.That(errors, Is.Empty);
        }

        [Test]
        public void ValidateProfiles_DuplicateNonEmptyNames_ReturnsSpecificError()
        {
            var profiles = new List<DeviceProfile>
            {
                CreateValidTextProfile(),
                CreateValidTextProfile()
            };

            var errors = DeviceProfileValidator.ValidateProfiles(profiles);

            Assert.That(errors, Does.Contain("设备模板名称不能重复：温控器。"));
        }

        [Test]
        public void ValidateProfiles_NamesDifferingOnlyByCase_AreNotDuplicates()
        {
            var firstProfile = CreateValidTextProfile();
            firstProfile.Name = "DeviceA";
            var secondProfile = CreateValidTextProfile();
            secondProfile.Name = "devicea";

            var errors = DeviceProfileValidator.ValidateProfiles(new List<DeviceProfile> { firstProfile, secondProfile });

            Assert.That(errors, Is.Empty);
        }

        private static DeviceProfile CreateValidTextProfile()
        {
            return new DeviceProfile
            {
                Name = "温控器",
                CommandContent = "AT",
                ExpectedResponse = "OK",
                TimeoutMilliseconds = 800,
                PortSettings = new List<SerialPortSettings> { SerialPortSettings.Default9600() }
            };
        }
    }
}
