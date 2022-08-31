namespace Bytewizer.TinyCLR.Hosting
{
    public class EthernetSettings : NetworkSettings
    {
        public static readonly string DhcpEnable = "ethernet:dhcp-enable";
        public static readonly string DdnsEnable = "ethernet:ddns-enable";
        public static readonly string MdnsEnable = "ethernet:mdns-enable";
        public static readonly string MacAddress = "ethernet:mac";
        public static readonly string IpAddress = "ethernet:ip";
        public static readonly string GatewayAddress = "ethernet:gateway";
        public static readonly string SubnetMask = "ethernet:subnet";
        public static readonly string DnsAddresses = "ethernet:dns";
    }
}