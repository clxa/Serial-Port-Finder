namespace SerialPortDeviceFinder.Core.Scanning
{
    /// <summary>
    /// 为每次探测创建独立会话，避免跨端口或跨参数复用打开的串口。
    /// </summary>
    public interface ISerialSessionFactory
    {
        ISerialSession Create();
    }
}
