using System;
using NUnit.Framework;
using SerialPortDeviceFinder.Core.Models;
using SerialPortDeviceFinder.Core.Protocol;

namespace SerialPortDeviceFinder.Core.Tests.Protocol
{
    [TestFixture]
    public sealed class ProtocolCodecTests
    {
        [Test]
        public void CreateCommandBytes_TextWithCrLf_AppendsCrLf()
        {
            var profile = new DeviceProfile
            {
                CommandFormat = PayloadFormat.Text,
                CommandContent = "AT",
                TextTerminator = TextTerminator.CrLf,
                TextEncoding = TextEncodingKind.Ascii
            };

            Assert.That(ProtocolCodec.CreateCommandBytes(profile), Is.EqualTo(new byte[] { 0x41, 0x54, 0x0D, 0x0A }));
        }

        [Test]
        public void CreateCommandBytes_TextTerminators_AppendOnlyTheSelectedTerminator()
        {
            Assert.That(CreateTextCommand(TextTerminator.None), Is.EqualTo(new byte[] { 0x41, 0x54 }));
            Assert.That(CreateTextCommand(TextTerminator.Cr), Is.EqualTo(new byte[] { 0x41, 0x54, 0x0D }));
            Assert.That(CreateTextCommand(TextTerminator.Lf), Is.EqualTo(new byte[] { 0x41, 0x54, 0x0A }));
        }

        [Test]
        public void CreateCommandBytes_TextUtf8_UsesUtf8Encoding()
        {
            var profile = new DeviceProfile
            {
                CommandFormat = PayloadFormat.Text,
                CommandContent = "温",
                TextTerminator = TextTerminator.None,
                TextEncoding = TextEncodingKind.Utf8
            };

            Assert.That(ProtocolCodec.CreateCommandBytes(profile), Is.EqualTo(new byte[] { 0xE6, 0xB8, 0xA9 }));
        }

        [Test]
        public void CreateCommandBytes_TextGbk_UsesGbkEncoding()
        {
            var profile = new DeviceProfile
            {
                CommandFormat = PayloadFormat.Text,
                CommandContent = "中",
                TextTerminator = TextTerminator.None,
                TextEncoding = TextEncodingKind.Gbk
            };

            Assert.That(ProtocolCodec.CreateCommandBytes(profile), Is.EqualTo(new byte[] { 0xD6, 0xD0 }));
        }

        [Test]
        public void TryParseHex_SpacedBytes_ReturnsBytes()
        {
            Assert.That(ProtocolCodec.TryParseHex("AA 55 01", out var bytes), Is.True);
            Assert.That(bytes, Is.EqualTo(new byte[] { 0xAA, 0x55, 0x01 }));
        }

        [TestCase("A")]
        [TestCase("GG")]
        [TestCase("")]
        [TestCase("   ")]
        public void TryParseHex_InvalidInput_ReturnsFalseAndEmptyBytes(string input)
        {
            Assert.That(ProtocolCodec.TryParseHex(input, out var bytes), Is.False);
            Assert.That(bytes, Is.Empty);
        }

        [TestCase("AA55")]
        [TestCase("AA\t55")]
        [TestCase("AA\r\n55")]
        [TestCase("A A")]
        [TestCase("AA  55")]
        [TestCase(" AA 55")]
        [TestCase("AA 55 ")]
        public void TryParseHex_BytesNotSeparatedByOneAsciiSpace_ReturnsFalseAndEmptyBytes(string input)
        {
            Assert.That(ProtocolCodec.TryParseHex(input, out var bytes), Is.False);
            Assert.That(bytes, Is.Empty);
        }

        [Test]
        public void CreateCommandBytes_InvalidHex_ThrowsArgumentException()
        {
            var profile = new DeviceProfile
            {
                CommandFormat = PayloadFormat.Hex,
                CommandContent = "GG"
            };

            Assert.That(() => ProtocolCodec.CreateCommandBytes(profile), Throws.TypeOf<ArgumentException>());
        }

        private static byte[] CreateTextCommand(TextTerminator terminator)
        {
            return ProtocolCodec.CreateCommandBytes(new DeviceProfile
            {
                CommandFormat = PayloadFormat.Text,
                CommandContent = "AT",
                TextTerminator = terminator,
                TextEncoding = TextEncodingKind.Ascii
            });
        }
    }
}
