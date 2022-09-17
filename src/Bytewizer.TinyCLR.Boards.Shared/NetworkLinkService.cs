using System;
using System.Threading;
using System.Collections;

using Bytewizer.TinyCLR.Boards;
using Bytewizer.TinyCLR.DependencyInjection;

using GHIElectronics.TinyCLR.Devices.Network;

namespace Bytewizer.TinyCLR.Hosting
{
    /// <summary>
    /// Base class for implementing a network connected <see cref="IHostedService"/>.
    /// </summary>
    public abstract class NetworkLinkService : IHostedService
    {
        private readonly IServiceProvider _services;

        private object[] _networkServices;

        /// <summary>
        /// Gets or sets the amount of time to wait after the <see cref="NetworkLinkConnectedChanged"/> fires.
        /// </summary>
        protected int LinkDelay { get; set; } = 100;

        public NetworkLinkService(IServiceProvider services)
        {
            _services = services;
        }

        /// <inheritdoc />
        public virtual void Start()
        {
            _networkServices = _services.GetServices(typeof(INetworkService));

            ArrayList exceptions = null;

            for (int index = 0; index < _networkServices.Length; index++)
            {
                try
                {
                    var controller = ((INetworkService)_networkServices[index]).Controller;
                    controller.NetworkLinkConnectedChanged += NetworkLinkConnectedChanged;
                }
                catch (Exception ex)
                {
                    exceptions ??= new ArrayList();
                    exceptions.Add(ex);
                }
            }

            if (exceptions != null)
            {
                throw new AggregateException(string.Empty, exceptions);
            }
        }

        private void NetworkLinkConnectedChanged(NetworkController sender, NetworkLinkConnectedChangedEventArgs args)
        {
            Thread.Sleep(LinkDelay);

            var ipProperties = sender.GetIPProperties();
            var address = ipProperties.Address.GetAddressBytes();

            if (address[0] != 0)
            {
                LinkConnected(sender, args);
            }
            else
            {
                LinkDisconnected(sender, args);
            }
        }

        /// <inheritdoc />
        public virtual void Stop()
        {
            ArrayList exceptions = null;

            for (int index = _networkServices.Length - 1; index >= 0; index--)
            {
                try
                {
                    var controller = ((INetworkService)_networkServices[index]).Controller;
                    controller.NetworkLinkConnectedChanged -= NetworkLinkConnectedChanged;
                }
                catch (Exception ex)
                {
                    exceptions ??= new ArrayList();
                    exceptions.Add(ex);
                }
            }

            if (exceptions != null)
            {
                throw new AggregateException(string.Empty, exceptions);
            }
        }

        protected abstract void LinkConnected(NetworkController sender, NetworkLinkConnectedChangedEventArgs args);
        
        protected abstract void LinkDisconnected(NetworkController sender, NetworkLinkConnectedChangedEventArgs args);
    }
}
