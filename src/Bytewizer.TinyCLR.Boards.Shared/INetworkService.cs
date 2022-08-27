using System;

using GHIElectronics.TinyCLR.Devices.Network;

namespace Bytewizer.TinyCLR.Boards
{
    public interface INetworkService : IDisposable
    {
        NetworkController Controller { get; }
        bool LinkConnected { get; }
        void Enable();
        void Disable();
    }
}