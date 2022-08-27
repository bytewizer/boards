using GHIElectronics.TinyCLR.Drivers.Sitronix.ST7735;

namespace Bytewizer.TinyCLR.Hosting
{
    public class DisplaySettings 
    {
        public int BacklightPin { get; set; }
        public ST7735Controller Controller { get; set; }
    }
}