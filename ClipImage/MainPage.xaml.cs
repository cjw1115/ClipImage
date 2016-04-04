using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;
//“空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409 上有介绍

namespace ClipImage
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    
    public sealed partial class MainPage : Page
    {
        CompositeTransform trans = new CompositeTransform();
        public MainPage()
        {
            InitializeComponent();
            //img.RenderTransform = trans;
            //img.ManipulationMode = ManipulationModes.TranslateX | ManipulationModes.TranslateY | ManipulationModes.Scale;
            //img.ManipulationDelta += Img_ManipulationDelta;
            Load();
        }

        

        public async void Load()
        {
            WriteableBitmap bitmap = new WriteableBitmap(1, 1);
            var file=await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///assets//cjw.jpg"));
            var ras = await file.OpenAsync(FileAccessMode.ReadWrite);
            bitmap.SetSource(ras);
            this.img.Source = bitmap;
            img.Width = bitmap.PixelWidth;
            img.Height = bitmap.PixelHeight;
            
        }
        public async void clip(ImageSource imageSource,double x,double y,double width,double height)
        {
            try
            {
                WriteableBitmap bitmap = imageSource as WriteableBitmap;
                var stream = bitmap.PixelBuffer.AsStream();
                byte[] buffer = new byte[stream.Length];

                InMemoryRandomAccessStream ras = new InMemoryRandomAccessStream();
                await stream.ReadAsync(buffer, 0, buffer.Length);
                BitmapEncoder encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.JpegEncoderId, ras);
                encoder.SetPixelData(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Ignore, (uint)bitmap.PixelWidth, (uint)bitmap.PixelHeight, 96.0, 96.0, buffer);


                //图片控件实际尺寸
                //Size imageSize = new Size() { Height = img.Height* trans.ScaleY, Width = img.Width* trans.ScaleX };
                Size imageSize = new Size() { Height = img.Height, Width = img.Width };

                var px = (x ) / imageSize.Width;
                var py= (y ) / imageSize.Height;
                var pwidth=width/ imageSize.Width;
                var pheight = height / imageSize.Height;

                encoder.BitmapTransform.Bounds = new BitmapBounds()
                {
                    X = (uint)(px * bitmap.PixelWidth),
                    Y = (uint)(py * bitmap.PixelHeight),
                    Width = (uint)(pwidth * bitmap.PixelWidth),
                    Height = (uint)(pheight * bitmap.PixelHeight)
                };
                
                await encoder.FlushAsync();
                WriteableBitmap wb = new WriteableBitmap((int)encoder.BitmapTransform.Bounds.Width, (int)encoder.BitmapTransform.Bounds.Height);
                wb.SetSource(ras);
                this.img.Source = wb;

                if (wb.PixelHeight >= wb.PixelWidth)
                {
                    img.Height = 300;
                    img.Width = wb.PixelWidth * img.Height / wb.PixelHeight;
                }
                else
                {
                    img.Width = 300;
                    img.Height = wb.PixelHeight * img.Width / wb.PixelWidth;
                }
            }
            catch
            {

            }
            
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            clip(this.img.Source,
                this.cliprect.ClipRect.X ,
                this.cliprect.ClipRect.Y ,
                this.cliprect.ClipRect.Width ,
                this.cliprect.ClipRect.Height);
        }
    }
    
    
}
