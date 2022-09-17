using System;
using System.Text;

using GHIElectronics.TinyCLR.Devices.Network;

namespace Bytewizer.TinyCLR.Boards
{
    internal class NetworkHelper 
    {
        internal static string GetNetworkInfo(NetworkController controller)
        {
            var ipProperties = controller.GetIPProperties();

            var sb = new StringBuilder();

            sb.Append($"Address: {ipProperties.Address} ");
            sb.Append($"Subnet: {ipProperties.SubnetMask} ");
            sb.Append($"Gateway: {ipProperties.GatewayAddress} ");

            for (int i = 0; i < ipProperties.DnsAddresses.Length; i++)
            {
                var address = ipProperties.DnsAddresses[i].GetAddressBytes();
                if (address[0] != 0)
                {
                    sb.Append($"DNS: {ipProperties.DnsAddresses[i]}");

                    if (i < ipProperties.DnsAddresses.Length - 1)
                    {
                        sb.Append(", ");
                    }
                }
                else
                {
                    sb.Append("0.0.0.0");
                }
            }

            return sb.ToString();
        }

        internal static string GetNetworkSettings(NetworkController controller)
        {
            var physicalAddress = BitConverter.ToString(controller.GetInterfaceProperties().MacAddress);

            var dhcpEnabled = controller.ActiveInterfaceSettings.DhcpEnable ? "Yes" : "No";
            var dnsEnabled = controller.ActiveInterfaceSettings.DynamicDnsEnable ? "Yes" : "No";
            var multDnsEnabled = controller.ActiveInterfaceSettings.MulticastDnsEnable ? "Yes" : "No";

            var sb = new StringBuilder();

            sb.Append($"Physical Address: {physicalAddress} ");
            sb.Append($"DHCP Enable: {dhcpEnabled} ");
            sb.Append($"DNS Enable: {dnsEnabled} ");
            sb.Append($"MDNS Enable: {multDnsEnabled} ");

            return sb.ToString();
        }
    }
}