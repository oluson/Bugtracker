using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Drawing.Imaging;

namespace Bugtracker.Models
{
    public class ImageUploadValidator
    {
        public static bool IsWebFriendly(HttpPostedFileBase file)
        {
            //check for actual object
            if (file == null)
            {
                return false;
            }

            if (file.ContentLength > 2 * 1024 * 1024 || file.ContentLength < 1024)
            {
                return false;
            }
            else
            {
                return true;
            }

        }

        public static bool IsImage(HttpPostedFileBase file)
        {

            try
            {
                using (var img = Image.FromStream(file.InputStream))
                {
                    return ImageFormat.Jpeg.Equals(img.RawFormat) ||
                           ImageFormat.Png.Equals(img.RawFormat) ||
                           ImageFormat.Gif.Equals(img.RawFormat);
                }
            }
            catch
            {
                return false;
            }

        }
    }
}