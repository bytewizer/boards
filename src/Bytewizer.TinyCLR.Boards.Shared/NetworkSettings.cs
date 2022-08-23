using GHIElectronics.TinyCLR.Devices.Network;

namespace Bytewizer.TinyCLR.Hosting
{
    public class NetworkSettings : IWirelessSettings, IEthernetSettings
    {
        public int EnablePin { get; set; }
        public string Controller { get; set; }
        public NetworkInterfaceSettings InterfaceSettings { get; set; }
        public NetworkCommunicationInterfaceSettings CommunicationSettings { get; set; }
    }

    public interface IWirelessSettings
    {
        int EnablePin { get; set; }
        string Controller { get; set; }
        NetworkInterfaceSettings InterfaceSettings { get; set; }
        NetworkCommunicationInterfaceSettings CommunicationSettings { get; set; }
    }

    public interface IEthernetSettings
    {
        int EnablePin { get; set; }
        string Controller { get; set; }
        NetworkInterfaceSettings InterfaceSettings { get; set; }
        NetworkCommunicationInterfaceSettings CommunicationSettings { get; set; }
    }
}