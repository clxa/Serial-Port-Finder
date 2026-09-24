using System;
using SerialPortDeviceFinder.Core.Models;

namespace SerialPortDeviceFinder.Core.Protocol
{
    public static class ResponseMatcher
    {
        public static bool IsMatch(DeviceProfile profile, byte[] response)
        {
            if (profile == null)
            {
                throw new ArgumentNullException(nameof(profile));
            }

            if (response == null || response.Length == 0)
            {
                return false;
            }

            switch (profile.ResponseFormat)
            {
                case PayloadFormat.Text:
                    return IsTextMatch(profile, response);
                case PayloadFormat.Hex:
                    return IsHexMatch(profile, response);
                default:
                    throw new ArgumentOutOfRangeException(nameof(profile), "响应格式无效。");
            }
        }

        private static bool IsTextMatch(DeviceProfile profile, byte[] response)
        {
            var responseText = ProtocolCodec.GetEncoding(profile.TextEncoding).GetString(response);
            var expectedResponse = profile.ExpectedResponse ?? string.Empty;

            if (profile.MatchMode == ResponseMatchMode.Contains)
            {
                return responseText.IndexOf(expectedResponse, StringComparison.Ordinal) >= 0;
            }

            if (profile.MatchMode == ResponseMatchMode.Exact)
            {
                return string.Equals(responseText.Trim('\r', '\n'), expectedResponse, StringComparison.Ordinal);
            }

            throw new ArgumentOutOfRangeException(nameof(profile), "响应匹配方式无效。");
        }

        private static bool IsHexMatch(DeviceProfile profile, byte[] response)
        {
            if (!ProtocolCodec.TryParseHex(profile.ExpectedResponse, out var expectedResponse))
            {
                return false;
            }

            if (profile.MatchMode == ResponseMatchMode.Contains)
            {
                return Contains(response, expectedResponse);
            }

            if (profile.MatchMode == ResponseMatchMode.Exact)
            {
                return BytesEqual(response, expectedResponse);
            }

            throw new ArgumentOutOfRangeException(nameof(profile), "响应匹配方式无效。");
        }

        private static bool Contains(byte[] response, byte[] expectedResponse)
        {
            if (expectedResponse.Length == 0 || expectedResponse.Length > response.Length)
            {
                return false;
            }

            for (var startIndex = 0; startIndex <= response.Length - expectedResponse.Length; startIndex++)
            {
                var matched = true;
                for (var offset = 0; offset < expectedResponse.Length; offset++)
                {
                    if (response[startIndex + offset] != expectedResponse[offset])
                    {
                        matched = false;
                        break;
                    }
                }

                if (matched)
                {
                    return true;
                }
            }

            return false;
        }

        private static bool BytesEqual(byte[] first, byte[] second)
        {
            if (first.Length != second.Length)
            {
                return false;
            }

            for (var index = 0; index < first.Length; index++)
            {
                if (first[index] != second[index])
                {
                    return false;
                }
            }

            return true;
        }
    }
}
