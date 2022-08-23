using GHIElectronics.TinyCLR.Devices.Display;
using GHIElectronics.TinyCLR.Drivers.FocalTech.FT5xx6;

namespace Bytewizer.TinyCLR.Hosting
{
    public class TouchScreenSettings 
    {
        public int BacklighPin { get; private set; }
        public FT5xx6Controller TouchController { get; private set; }
        public DisplayController DisplayController { get; private set; }

        public TouchScreenSettings(DisplayController displayController, FT5xx6Controller touchController, int backlighPin)
        {
            BacklighPin = backlighPin;
            TouchController = touchController;
            DisplayController = displayController;
        }
    }
}