using System;
using System.Globalization;
using System.Text;
using SerialPortDeviceFinder.Core.Models;

namespace SerialPortDeviceFinder.Core.Protocol
{
    public static class ProtocolCodec
    {
        public static byte[] CreateCommandBytes(DeviceProfile profile)
        {
            if (profile == null)
            {
                throw new ArgumentNullException(nameof(profile));
            }

            if (profile.CommandFormat == PayloadFormat.Hex)
            {
                if (!TryParseHex(profile.CommandContent, out var bytes))
                {
                    throw new ArgumentException("十六进制命令格式无效。", nameof(profile));
                }

                return bytes;
            }

            if (profile.CommandFormat != PayloadFormat.Text)
            {
                throw new ArgumentOutOfRangeException(nameof(profile), "命令格式无效。");
            }

            var command = (profile.CommandContent ?? string.Empty) + GetTerminator(profile.TextTerminator);
            return GetEncoding(profile.TextEncoding).GetBytes(command);
        }

        public static bool TryParseHex(string text, out byte[] bytes)
        {
            bytes = Array.Empty<byte>();
            if (string.IsNullOrEmpty(text))
            {
                return false;
            }

            var byteTexts = text.Split(' ');
            var parsedBytes = new byte[byteTexts.Length];
            for (var index = 0; index < byteTexts.Length; index++)
            {
                var byteText = byteTexts[index];
                if (byteText.Length != 2
                    || !byte.TryParse(byteText, NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture, out parsedBytes[index]))
                {
                    bytes = Array.Empty<byte>();
                    return false;
                }
            }

            bytes = parsedBytes;
            return true;
        }

        internal static Encoding GetEncoding(TextEncodingKind encodingKind)
        {
            switch (encodingKind)
            {
                case TextEncodingKind.Ascii:
                    return Encoding.ASCII;
                case TextEncodingKind.Utf8:
                    return Encoding.UTF8;
                case TextEncodingKind.Gbk:
                    return Encoding.GetEncoding(936);
                default:
                    throw new ArgumentOutOfRangeException(nameof(encodingKind), "文本编码无效。");
            }
        }

        private static string GetTerminator(TextTerminator terminator)
        {
            switch (terminator)
            {
                case TextTerminator.None:
                    return string.Empty;
                case TextTerminator.Cr:
                    return "\r";
                case TextTerminator.Lf:
                    return "\n";
                case TextTerminator.CrLf:
                    return "\r\n";
                default:
                    throw new ArgumentOutOfRangeException(nameof(terminator), "文本结束符无效。");
            }
        }
    }
}
