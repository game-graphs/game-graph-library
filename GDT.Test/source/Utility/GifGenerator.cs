using System.Drawing;
using AnimatedGif;
using GDT.Algorithm.WFC.Implementation;
using GDT.Test.Tests.WFC;

namespace GDT.Utility.Visualization
{
    public class GifGenerator
    {
        private AnimatedGifCreator _gif;

        public GifGenerator(string fileName, int delay = 600)
        {
            _gif = AnimatedGif.AnimatedGif.Create(fileName, delay);
        }

        public void AddFrame(Image frame)
        {
            _gif.AddFrame(frame, delay: -1, quality: GifQuality.Bit8);
        }
    }
}