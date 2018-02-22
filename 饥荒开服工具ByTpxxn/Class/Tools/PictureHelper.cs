using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace 饥荒开服工具ByTpxxn.Class.Tools
{
    internal class PictureHelper
    {
        [DllImport("gdi32.dll", SetLastError = true)]
        private static extern bool DeleteObject(IntPtr hObject);
        /// <summary>  
        /// 从bitmap转换成ImageSource  
        /// </summary>  
        /// <param name="bitmap"></param>  
        /// <returns></returns>  
        public static ImageSource ChangeBitmapToImageSource(Bitmap bitmap)
        {
            //Bitmap bitmap = icon.ToBitmap();  
            IntPtr hBitmap = bitmap.GetHbitmap();
            ImageSource wpfBitmap = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                hBitmap,
                IntPtr.Zero,
                Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions());
            if (!DeleteObject(hBitmap))
            {
                throw new System.ComponentModel.Win32Exception();
            }
            return wpfBitmap;
        }

    }
}
