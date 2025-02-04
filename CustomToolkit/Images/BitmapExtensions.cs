using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace CustomToolkit.Images
{
    /// <summary>
    /// Методы расширения для класса Bitmap и массивов с данными объектами.
    /// </summary>
    public static class BitmapExtensions
    {
        private static void CheckDir(string dir)
        {
            if(!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
        }


        /// <summary>
        /// Сохраняет все изображения последовательности в указанном каталоге 
        /// (если не существует, будет создан);
        /// в формате JPG, каждое в отдельный файл.
        /// Файлы будут названы image1, image2 и т.д., по количеству
        /// битмапов в исходной последовательности.
        /// </summary>
        /// <param name="sourcePicts"></param>
        /// <param name="folderPath"></param>
        public static void SaveJPG(this IEnumerable<Bitmap> sourcePicts, string folderPath)
        {
            CheckDir(folderPath);

            int index = 1;
            foreach (var pict in sourcePicts)
                pict.Save($"{folderPath}\\image{index++}.jpg");
        }

        /// <summary>
        /// Сохраняет все изображения последовательности в указанном каталоге 
        /// (если не существует, будет создан);
        /// в формате PNG, каждое в отдельный файл.
        /// Файлы будут названы image1, image2 и т.д., по количеству
        /// битмапов в исходной последовательности.
        /// </summary>
        /// <param name="sourcePicts"></param>
        /// <param name="folderPath"></param>
        public static void SavePNG(this IEnumerable<Bitmap> sourcePicts, string folderPath)
        {
            CheckDir(folderPath);

            int index = 1;
            foreach (var pict in sourcePicts)
                pict.Save($"{folderPath}\\image{index++}.png");
        }

        /// <summary>
        /// Возвращает поток, содержащий это изображение в его исходном формате.
        /// </summary>
        /// <param name="pict"></param>
        /// <returns></returns>
        public static Stream ToStream(this Bitmap pict)
        {
            var format = pict.RawFormat;
            var stream = new MemoryStream();

            pict.Save(stream, format);
            return stream;
        }
    }
}
