﻿using System;

using Bytewizer.TinyCLR.Hosting;

using GHIElectronics.TinyCLR.Pins;
using GHIElectronics.TinyCLR.Devices.Gpio;

namespace Bytewizer.TinyCLR.Boards
{
    public class NetworkStatusService : SchedulerService
    {  
        private readonly GpioPin _led;
        private readonly IConfiguration _configuration;

        public NetworkStatusService(IConfiguration configuration)
            : base(TimeSpan.FromSeconds(2))
        {
            _configuration = configuration;
            _led = GpioController.GetDefault().OpenPin(SC20260.GpioPin.PH11);
            _led.SetDriveMode(GpioPinDriveMode.Output);
        }

        protected override void ExecuteAsync()
        {
            var connected = (bool)_configuration[BoardSettings.WirelessConnected] ||
                            (bool)_configuration[BoardSettings.EthernetConnected];

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