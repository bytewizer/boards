namespace Bytewizer.TinyCLR.Hosting
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

    public static partial class BoardSettings
    {        
        public static readonly string EthernetDhcpEnable = "ethernet:dhcp-enable";
        public static readonly string EthernetDdnsEnable = "ethernet:ddns-enable";
        public static readonly string EthernetMdnsEnable = "ethernet:mdns-enable";
        public static readonly string EthernetMacAddress = "ethernet:mac";
        public static readonly string EthernetIpAddress = "ethernet:ip";
        public static readonly string EthernetGatewayAddress = "ethernet:gateway";
        public static readonly string EthernetSubnetMask = "ethernet:subnet";
        public static readonly string EthernetDnsAddresses = "ethernet:dns";
    }
}