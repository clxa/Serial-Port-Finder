namespace SerialPortDeviceFinder.Core.Scanning
{
    public sealed class SerialPortSessionFactory : ISerialSessionFactory
    {
        public ISerialSession Create()
        {
            return new SerialPortSession();
        }
    }
}
