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
    internal class MedianFilter : EffectBase
    {
        public MedianFilter()
        {

        }
        public MedianFilter(IImageProvider source)
        {
            Source = source;
        }

        public override IImageProvider2 Clone()
        {
            return new MedianFilter(((IImageProvider2)Source).Clone());
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
                        //Almacenamos los pixels alrededor del actual en un array ordenado para coger la mediana
                        //var c = sourcePixelRegion.ImagePixels[index + i];
                        List<uint> array = new List<uint>();

                        var pixel = index + i;
                        array.Add(GetPixelOrDefault(sourcePixelRegion, pixel - width - 1));
                        array.Add(GetPixelOrDefault(sourcePixelRegion, pixel - width));
                        array.Add(GetPixelOrDefault(sourcePixelRegion, pixel - width + 1));
                                                                      
                        array.Add(GetPixelOrDefault(sourcePixelRegion, pixel - 1));
                        array.Add(GetPixelOrDefault(sourcePixelRegion, pixel));
                        array.Add(GetPixelOrDefault(sourcePixelRegion, pixel + 1));
                                                                      
                        array.Add(GetPixelOrDefault(sourcePixelRegion, pixel + width - 1));
                        array.Add(GetPixelOrDefault(sourcePixelRegion, pixel + width));
                        array.Add(GetPixelOrDefault(sourcePixelRegion, pixel + width + 1));
                        
                        targetPixelRegion.ImagePixels[pixel] = array.OrderBy(it=>it).ToList()[4];
                    }

                });
            }

            private uint GetPixelOrDefault(PixelRegion pixelRegion, int pixel)
            {
                if(pixel<0 || pixel>pixelRegion.ImagePixels.Length)
                    return (uint)(0xff000000);
                else
                    return pixelRegion.ImagePixels[pixel];                
            }
        }
    }
}
