using GHIElectronics.TinyCLR.Devices.Network;

namespace Bytewizer.TinyCLR.Hosting
{
    public abstract class NetworkSettings
    {
        public int EnablePin { get; set; }
        public string Controller { get; set; }
        public NetworkInterfaceSettings InterfaceSettings { get; set; }
        public NetworkCommunicationInterfaceSettings CommunicationSettings { get; set; }
    }
}