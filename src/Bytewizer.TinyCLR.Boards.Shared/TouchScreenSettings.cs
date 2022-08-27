using GHIElectronics.TinyCLR.Devices.Display;
using GHIElectronics.TinyCLR.Drivers.FocalTech.FT5xx6;

namespace Bytewizer.TinyCLR.Hosting
{
    public class TouchScreenSettings 
    {
        public int BacklightPin { get; set; }
        public FT5xx6Controller TouchController { get; set; }
        public DisplayController DisplayController { get; set; }
    }
}