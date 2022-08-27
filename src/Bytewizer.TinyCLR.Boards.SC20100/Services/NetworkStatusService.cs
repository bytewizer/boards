using System;

using Bytewizer.TinyCLR.Hosting;

using GHIElectronics.TinyCLR.Pins;
using GHIElectronics.TinyCLR.Devices.Gpio;

namespace Bytewizer.TinyCLR.Boards
{
    public class NetworkStatusService : SchedulerService
    {  
        private readonly GpioPin _led;
        private readonly IConfiguration _configuration;

        public NetworkStatusService(IServiceProvider services, IConfiguration configuration)
            : base(TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(2))
        {
            _configuration = configuration;

            _led = GpioController.GetDefault().OpenPin(SC20100.GpioPin.PB0);
            _led.SetDriveMode(GpioPinDriveMode.Output);

            var networkServices = services.GetService(new Type[] { typeof(IEthernetService), typeof(IWirelessService) });
            foreach (INetworkService service in networkServices)
            {
                service.Enable();
            }
        }

        protected override void ExecuteAsync()
        {
            var connected = (bool)_configuration[BoardSettings.NetworkConnected];

            if (!connected)
            {
                _led.Toggle();
            }
            else 
            { 
                if(GpioPinValue.High == _led.Read())
                {
                    _led.Write(GpioPinValue.Low);
                }
            }
        }
    }
}