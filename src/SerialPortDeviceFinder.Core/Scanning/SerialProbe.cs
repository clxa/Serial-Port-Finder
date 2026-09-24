using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using SerialPortDeviceFinder.Core.Models;
using SerialPortDeviceFinder.Core.Protocol;

namespace SerialPortDeviceFinder.Core.Scanning
{
    /// <summary>
    /// 在一组给定串口参数下执行一次无状态探测。无论结果如何，探测结束都会释放串口。
    /// </summary>
    public sealed class SerialProbe : ISerialProbe
    {
        private const int ReadIntervalMilliseconds = 20;
        private readonly ISerialSessionFactory _sessionFactory;

        public SerialProbe()
            : this(new SerialPortSessionFactory())
        {
        }

        public SerialProbe(ISerialSessionFactory sessionFactory)
        {
            _sessionFactory = sessionFactory ?? throw new ArgumentNullException(nameof(sessionFactory));
        }

        public SerialProbe(Func<ISerialSession> createSession)
            : this(new DelegateSerialSessionFactory(createSession))
        {
        }

        public ScanResult Probe(
            string portName,
            DeviceProfile profile,
            SerialPortSettings settings,
            CancellationToken cancellationToken)
        {
            var responseBytes = new List<byte>();
            ISerialSession? session = null;

            try
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    return CreateResult(portName, profile, settings, ScanStatus.Cancelled, responseBytes, "搜索已取消。");
                }

                session = _sessionFactory.Create();
                session.Open(portName, settings);
                session.ClearBuffers();
                session.Write(ProtocolCodec.CreateCommandBytes(profile));

                var timeoutMilliseconds = profile.TimeoutMilliseconds;
                var stopwatch = Stopwatch.StartNew();
                while (stopwatch.ElapsedMilliseconds < timeoutMilliseconds)
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        return CreateResult(portName, profile, settings, ScanStatus.Cancelled, responseBytes, "搜索已取消。");
                    }

                    var currentBytes = session.ReadAvailable();
                    if (currentBytes?.Length > 0)
                    {
                        responseBytes.AddRange(currentBytes);
                        if (ResponseMatcher.IsMatch(profile, responseBytes.ToArray()))
                        {
                            return CreateResult(portName, profile, settings, ScanStatus.Matched, responseBytes, "响应匹配成功。");
                        }
                    }

                    var remainingMilliseconds = timeoutMilliseconds - (int)stopwatch.ElapsedMilliseconds;
                    if (remainingMilliseconds > 0)
                    {
                        Thread.Sleep(Math.Min(ReadIntervalMilliseconds, remainingMilliseconds));
                    }
                }

                return responseBytes.Count == 0
                    ? CreateResult(portName, profile, settings, ScanStatus.Timeout, responseBytes, "等待响应超时。")
                    : CreateResult(portName, profile, settings, ScanStatus.NotMatched, responseBytes, "收到响应但不匹配。");
            }
            catch (UnauthorizedAccessException exception)
            {
                return CreateResult(portName, profile, settings, ScanStatus.PortUnavailable, responseBytes, exception.Message);
            }
            catch (IOException exception)
            {
                return CreateResult(portName, profile, settings, ScanStatus.CommunicationError, responseBytes, exception.Message);
            }
            catch (ArgumentException exception)
            {
                return CreateResult(portName, profile, settings, ScanStatus.CommunicationError, responseBytes, exception.Message);
            }
            catch (Exception exception)
            {
                return CreateResult(portName, profile, settings, ScanStatus.CommunicationError, responseBytes, exception.Message);
            }
            finally
            {
                if (session != null)
                {
                    try
                    {
                        session.Close();
                    }
                    catch
                    {
                    }
                    finally
                    {
                        try
                        {
                            session.Dispose();
                        }
                        catch
                        {
                        }
                    }
                }
            }
        }

        private static ScanResult CreateResult(
            string portName,
            DeviceProfile profile,
            SerialPortSettings settings,
            ScanStatus status,
            List<byte> responseBytes,
            string message)
        {
            return new ScanResult
            {
                PortName = portName ?? string.Empty,
                ProfileName = profile?.Name ?? string.Empty,
                PortSettings = settings,
                Status = status,
                ResponseBytes = responseBytes.ToArray(),
                Message = message,
                OccurredAt = DateTimeOffset.Now
            };
        }

        private sealed class DelegateSerialSessionFactory : ISerialSessionFactory
        {
            private readonly Func<ISerialSession> _createSession;

            public DelegateSerialSessionFactory(Func<ISerialSession> createSession)
            {
                _createSession = createSession ?? throw new ArgumentNullException(nameof(createSession));
            }

            public ISerialSession Create()
            {
                return _createSession();
            }
        }
    }
}
