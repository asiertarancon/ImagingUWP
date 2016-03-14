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
                SortedSet<uint> array = new SortedSet<uint>();

                //int width = Convert.ToInt32(sourcePixelRegion.ImageSize.Width);                
                
                targetPixelRegion.ForEachRow((index, width, position) =>
                {
                    for (int i = 0; i < width; i++)
                    {
                        var pixel = index + i;
                        array.Clear();
                        //var pixel = Convert.ToInt32(index + position.X + i);

                        var pixelCentral = GetPixelOrDefault(sourcePixelRegion.ImagePixels, pixel, 0);
                        array.Add(GetPixelOrDefault(sourcePixelRegion.ImagePixels, pixel - width - 1, pixelCentral));
                        array.Add(GetPixelOrDefault(sourcePixelRegion.ImagePixels, pixel - width, pixelCentral));
                        array.Add(GetPixelOrDefault(sourcePixelRegion.ImagePixels, pixel - width + 1, pixelCentral));

                        array.Add(GetPixelOrDefault(sourcePixelRegion.ImagePixels, pixel - 1, pixelCentral));
                        array.Add(pixelCentral);
                        array.Add(GetPixelOrDefault(sourcePixelRegion.ImagePixels, pixel + 1, pixelCentral));

                        array.Add(GetPixelOrDefault(sourcePixelRegion.ImagePixels, pixel + width - 1, pixelCentral));
                        array.Add(GetPixelOrDefault(sourcePixelRegion.ImagePixels, pixel + width, pixelCentral));
                        array.Add(GetPixelOrDefault(sourcePixelRegion.ImagePixels, pixel + width + 1, pixelCentral));

                        targetPixelRegion.ImagePixels[pixel] = sourcePixelRegion.ImagePixels[pixel];// array.ElementAt((array.Count- 1) /2);//.OrderBy(it => it).ToList()[4];
                    }
                });
                //for (; pixel<sourcePixelRegion.ImagePixels.Length-width-2; pixel++)
                //{

                //for (int i=0; i< targetPixelRegion.ImagePixels.Length-1; i++)
                //{

                //    //targetPixelRegion.ImagePixels[i];
                //    //targetPixelRegion.ForEachRow((index, width, position) =>
                //    //{

                //    //    //Parallel.For(0, width, (i) =>
                //    //    for (int i = 0; i < width; i++)
                //    //    {

                //    //array.Clear();
                //    ////var pixel = index + i;
                //    //array.Add(sourcePixelRegion.ImagePixels[pixel - width - 1]);
                //    //array.Add(sourcePixelRegion.ImagePixels[ pixel - width]);
                //    //array.Add(sourcePixelRegion.ImagePixels[ pixel - width + 1]);
                //    //array.Add(sourcePixelRegion.ImagePixels[ pixel - 1]);
                //    //array.Add(sourcePixelRegion.ImagePixels[ pixel]);
                //    //array.Add(sourcePixelRegion.ImagePixels[ pixel + 1]);
                //    //array.Add(sourcePixelRegion.ImagePixels[ pixel + width - 1]);
                //    //array.Add(sourcePixelRegion.ImagePixels[ pixel + width]);
                //    //array.Add(sourcePixelRegion.ImagePixels[pixel + width + 1]);

                //    targetPixelRegion.ImagePixels[pixel] = sourcePixelRegion.ImagePixels[pixel - 1];// GetPixelOrDefault(sourcePixelRegion, pixel);// array[4];
                //    }//  });
                    //for (int i = 0; i < width; ++i)
                    //{
                    //    //Almacenamos los pixels alrededor del actual en un array ordenado para coger la mediana
                    //    //var c = sourcePixelRegion.ImagePixels[index + i];
                    //    List<uint> array = new List<uint>();

                    //    var pixel = index + i;
                    //    array.Add(GetPixelOrDefault(sourcePixelRegion, pixel - width - 1));
                    //    array.Add(GetPixelOrDefault(sourcePixelRegion, pixel - width));
                    //    array.Add(GetPixelOrDefault(sourcePixelRegion, pixel - width + 1));
                                                                      
                    //    array.Add(GetPixelOrDefault(sourcePixelRegion, pixel - 1));
                    //    array.Add(GetPixelOrDefault(sourcePixelRegion, pixel));
                    //    array.Add(GetPixelOrDefault(sourcePixelRegion, pixel + 1));
                                                                      
                    //    array.Add(GetPixelOrDefault(sourcePixelRegion, pixel + width - 1));
                    //    array.Add(GetPixelOrDefault(sourcePixelRegion, pixel + width));
                    //    array.Add(GetPixelOrDefault(sourcePixelRegion, pixel + width + 1));
                        
                    //    targetPixelRegion.ImagePixels[pixel] = array.OrderBy(it=>it).ToList()[4];
                    //}

                //});
            }

            private uint GetPixelOrDefault(PixelRegion pixelRegion, int pixel)
            {
                if(pixel<0 || pixel>=pixelRegion.ImagePixels.Length)
                    return (uint)(0xff000000);
                else
                    return pixelRegion.ImagePixels[pixel];                
            }
            private uint GetPixelOrDefault(uint[] image, int pixel, uint pixelDefault)
            {
                if (pixel < 0 || pixel >= image.Length)
                    return pixelDefault;//(uint)(0xff000000);
                else
                    return image[pixel];
            }
        }
    }
}
