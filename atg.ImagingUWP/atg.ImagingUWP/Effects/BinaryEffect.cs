using Lumia.Imaging;
using Lumia.Imaging.Workers;
using Lumia.Imaging.Workers.Cpu;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace atg.ImagingUWP.Effects
{    
    internal class BinaryEffect : EffectBase
    {
        public BinaryEffect()
        {

        }
        public BinaryEffect(IImageProvider source)
        {
            Source = source;
        }

        public override IImageProvider2 Clone()
        {
            return new BinaryEffect(((IImageProvider2)Source).Clone());
        }

        public override RenderOptions SupportedRenderOptions
        {
            get
            {
                return RenderOptions.Cpu; // This example supports only CPU based rendering.
            }
        }

        public override IImageWorker CreateImageWorker(IImageWorkerRequest imageWorkerRequest)
        {
            if (imageWorkerRequest.RenderOptions == RenderOptions.Cpu)
            {
                return new BlockBasedWorker();
            }

            return null; // Unsupported requests get null as the return value.
        }

        // This is the image worker implementation, which performs actual processing.
        private class BlockBasedWorker : CpuImageWorkerBase
        {
            public BlockBasedWorker()
                : base(new[] { ColorMode.Bgra8888 }) // Supported color modes.
            {
            }

            protected override void OnProcess(PixelRegion sourcePixelRegion, PixelRegion targetPixelRegion)
            {
                targetPixelRegion.ForEachRow((index, width, position) =>
                {
                    for (int i = 0; i < width; ++i)
                    {
                        //var c = sourcePixelRegion.ImagePixels[index + i];

                        //c >>= 1;
                        //c &= 0x7F7F7F7F;

                        var pixel = sourcePixelRegion.ImagePixels[index + i];

                        uint red = (pixel >> 16) & 0x000000FF;
                        uint green = (pixel >> 8) & 0x000000FF;
                        uint blue = (pixel) & 0x000000FF;

                        int average = (int)(0.0722 * blue + 0.7152 * green + 0.2126 * red); // weighted average component
                        pixel = (uint)(0xff000000 | average | (average << 8) | (average << 16)); // use average for each color component	

                        if (average > 150)
                            pixel = (uint)(0xffffffff);
                        else
                            pixel = (uint)(0xff000000);

                        targetPixelRegion.ImagePixels[index + i] = pixel;
                    }

                });
            }
        }
    }
}
