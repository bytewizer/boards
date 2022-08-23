using System;

using Bytewizer.TinyCLR.Hosting;
using Bytewizer.TinyCLR.DependencyInjection;

using GHIElectronics.TinyCLR.Native;
using GHIElectronics.TinyCLR.Devices.Rtc;

namespace Bytewizer.TinyCLR.Hosting
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

    public static partial class BoardSettings
    {
        // internal use only
        public static readonly string BoardType = "_board:type_";
        public static readonly string DefaultController = "_default:controller_";
        public static readonly string WirelessConnected = "_wireless:connected_";
        public static readonly string EthernetConnected = "_ethernet:connected_";
        
        // public configuration settings
        public static readonly string TimeZoneOffset = "timezone:offset";
        public static readonly string WirelessSsid = "wireless:ssid";
        public static readonly string WirelessPsk = "wireless:psk";
    }
}