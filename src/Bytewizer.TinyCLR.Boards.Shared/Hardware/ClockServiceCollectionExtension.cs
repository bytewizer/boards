﻿using System;

using Bytewizer.TinyCLR.DependencyInjection;

using GHIElectronics.TinyCLR.Native;
using GHIElectronics.TinyCLR.Devices.Rtc;

namespace Bytewizer.TinyCLR.Boards
{
    public static class ClockServiceCollectionExtension
    {
        public static IServiceCollection AddClock(this IServiceCollection services, int timezoneOffset)
        {
            if (services == null)
            {
                throw new ArgumentNullException();
            }

            services.Replace(
                new ServiceDescriptor(
                    typeof(IClockService),
                    new ClockService(timezoneOffset)
                ));

            return services;
        }
    }

    public interface IClockService : IDisposable
    {
        RtcController Controller { get; }
        DateTime UtcNow { get; }
        DateTime Now { get; }
        void SetTime(DateTime value);
    }

    public class ClockService : IClockService
    {
        private int _timeZoneOffset;

        public RtcController Controller { get; private set; }
        
        public DateTime UtcNow { get => Controller.Now; }
        
        public DateTime Now { get => Controller.Now.AddSeconds(_timeZoneOffset); }

        public ClockService(int offsetSeconds)
        {
            _timeZoneOffset = offsetSeconds;

            Controller = RtcController.GetDefault();
            Controller.SetChargeMode(BatteryChargeMode.Fast);

            if (Controller.IsValid)
            {
                SystemTime.SetTime(Now);
            }
        }

        public void SetTimeZone(int offsetSeconds)
        {
            _timeZoneOffset = offsetSeconds;
        }

        public void SetTime(DateTime value)
        {
            Controller.SetTime(RtcDateTime.FromDateTime(value));
            SystemTime.SetTime(value.AddSeconds(_timeZoneOffset));
        }

        public void Dispose()
        {
            Controller?.Dispose();
        }
    }
}