namespace Bytewizer.TinyCLR.Hosting
{
    public class WirelessSettings : NetworkSettings
    {
        public static readonly string Ssid = "wireless:ssid";
        public static readonly string Psk = "wireless:psk";
        public static readonly string Channel = "wireless:channel";
        public static readonly string Mode = "wireless:mode";
        public static readonly string DhcpEnable = "wireless:dhcp-enable";
        public static readonly string DdnsEnable = "wireless:ddns-enable";
        public static readonly string MdnsEnable = "wireless:mdns-enable";
        public static readonly string MacAddress = "wireless:mac";
        public static readonly string IpAddress = "wireless:ip";
        public static readonly string GatewayAddress = "wireless:gateway";
        public static readonly string SubnetMask = "wireless:subnet";
        public static readonly string DnsAddresses = "wireless:dns";
    }
}