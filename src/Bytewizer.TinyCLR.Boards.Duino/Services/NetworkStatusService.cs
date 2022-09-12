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

            _led = GpioController.GetDefault().OpenPin(FEZDuino.GpioPin.Led);
            _led.SetDriveMode(GpioPinDriveMode.Output);

            var networkServices = (INetworkService)services.GetService(typeof(INetworkService));
            if (networkServices != null)
            {
                networkServices.Enable();
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