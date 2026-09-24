using NUnit.Framework;
using SerialPortDeviceFinder.Core.Models;
using SerialPortDeviceFinder.Core.Protocol;

namespace SerialPortDeviceFinder.Core.Tests.Protocol
{
    [TestFixture]
    public sealed class ResponseMatcherTests
    {
        [Test]
        public void IsMatch_HexContains_FindsBytesInsideLongerResponse()
        {
            var profile = new DeviceProfile
            {
                ResponseFormat = PayloadFormat.Hex,
                ExpectedResponse = "AA 55",
                MatchMode = ResponseMatchMode.Contains
            };

            Assert.That(ResponseMatcher.IsMatch(profile, new byte[] { 0x01, 0xAA, 0x55, 0x02 }), Is.True);
        }

        [Test]
        public void IsMatch_HexExact_RequiresTheSameBytesAndLength()
        {
            var profile = new DeviceProfile
            {
                ResponseFormat = PayloadFormat.Hex,
                ExpectedResponse = "AA 55",
                MatchMode = ResponseMatchMode.Exact
            };

            Assert.That(ResponseMatcher.IsMatch(profile, new byte[] { 0xAA, 0x55 }), Is.True);
            Assert.That(ResponseMatcher.IsMatch(profile, new byte[] { 0xAA, 0x55, 0x00 }), Is.False);
        }

        [Test]
        public void IsMatch_TextContains_UsesOrdinalCaseSensitiveComparison()
        {
            var profile = new DeviceProfile
            {
                ResponseFormat = PayloadFormat.Text,
                ExpectedResponse = "OK",
                MatchMode = ResponseMatchMode.Contains,
                TextEncoding = TextEncodingKind.Ascii
            };

            Assert.That(ResponseMatcher.IsMatch(profile, new byte[] { 0x2B, 0x4F, 0x4B, 0x2B }), Is.True);
            Assert.That(ResponseMatcher.IsMatch(profile, new byte[] { 0x2B, 0x6F, 0x6B, 0x2B }), Is.False);
        }

        [Test]
        public void IsMatch_TextExact_IgnoresOnlyBoundaryCrLf()
        {
            var profile = new DeviceProfile
            {
                ResponseFormat = PayloadFormat.Text,
                ExpectedResponse = "OK",
                MatchMode = ResponseMatchMode.Exact,
                TextEncoding = TextEncodingKind.Ascii
            };

            Assert.That(ResponseMatcher.IsMatch(profile, new byte[] { 0x0D, 0x0A, 0x4F, 0x4B, 0x0D, 0x0A }), Is.True);
            Assert.That(ResponseMatcher.IsMatch(profile, new byte[] { 0x20, 0x4F, 0x4B, 0x20 }), Is.False);
        }

        [Test]
        public void IsMatch_EmptyResponse_ReturnsFalse()
        {
            var profile = new DeviceProfile
            {
                ResponseFormat = PayloadFormat.Text,
                ExpectedResponse = "OK"
            };

            Assert.That(ResponseMatcher.IsMatch(profile, new byte[0]), Is.False);
        }
    }
}
