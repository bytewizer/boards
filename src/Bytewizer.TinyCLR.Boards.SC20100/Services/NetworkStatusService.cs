using System;
using System.Threading;

using Bytewizer.TinyCLR.Hosting;

using GHIElectronics.TinyCLR.Pins;
using GHIElectronics.TinyCLR.Devices.Gpio;
using GHIElectronics.TinyCLR.Devices.Network;

namespace Bytewizer.TinyCLR.Boards
{
    public class NetworkStatusService : NetworkLinkService
    {
        private Timer _executeTimer;

        private readonly GpioPin _led;

        public TimeSpan Time { get; set; }
        public TimeSpan Interval { get; set; }

        public NetworkStatusService(IServiceProvider services)
            : base(services)
        {
            _led = GpioController.GetDefault().OpenPin(SC20100.GpioPin.PB0);
            _led.SetDriveMode(GpioPinDriveMode.Output);

            Time = TimeSpan.Zero;
            Interval = TimeSpan.FromSeconds(2);

            var networkServices = services.GetService(new Type[] { typeof(INetworkService) });

            foreach (INetworkService service in networkServices)
            {
                service.Enable();
            }
        }

        protected override void LinkConnected(NetworkController sender, NetworkLinkConnectedChangedEventArgs args)
        {
            _led.Write(GpioPinValue.Low);

            if (_executeTimer == null)
            {
                return;
            }

            try
            {
                _executeTimer.Change(Timeout.Infinite, 0);
            }
            finally
            {
                _executeTimer = null;
            }
        }

        protected override void LinkDisconnected(NetworkController sender, NetworkLinkConnectedChangedEventArgs args)
        {
            _executeTimer = new Timer(state =>
            {
                _led.Toggle();
            }, null, Time, Interval);
        }

        public virtual void Dispose()
        {
            _executeTimer?.Dispose();
        }
    }
}