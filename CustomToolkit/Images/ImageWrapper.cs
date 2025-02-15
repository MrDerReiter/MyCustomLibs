using System;
using System.Drawing;
using System.Linq;

namespace CustomToolkit.Images
{
    /// <summary>
    /// Статический класс для работы с изображениями.
    /// Предоставляет методы для выполнения часто используемых шаблонных операций.
    /// </summary>
    public static class ImageWrapper
    {
        internal static Bitmap[] SliceH(Bitmap source, int chunks)
        {
            var result = new Bitmap[chunks];
            int targetHeight = source.Height / chunks;

            for (int i = 0; i < chunks; i++)
            {
                var target = new Bitmap(source.Width, targetHeight);
                var targetRect = new Rectangle(new Point(0, 0), target.Size);
                var cropRect = new Rectangle
                    (new Point(0, targetHeight * i), target.Size);

                Graphics.FromImage(target)
                        .DrawImage(source, targetRect, cropRect, GraphicsUnit.Pixel);
                result[i] = target;
            }

            return result;
        }

        internal static Bitmap[] SliceV(Bitmap source, int chunks)
        {
            var result = new Bitmap[chunks];
            int targetWidth = source.Width / chunks;

            for (int i = 0; i < chunks; i++)
            {
                var target = new Bitmap(targetWidth, source.Height);
                var targetRect = new Rectangle(new Point(0, 0), target.Size);
                var cropRect = new Rectangle
                    (new Point(targetWidth * i, 0), target.Size);

                Graphics.FromImage(target)
                        .DrawImage(source, targetRect, cropRect, GraphicsUnit.Pixel);
                result[i] = target;
            }

            return result;
        }


        /// <summary>
        /// Разрезает изображение из указанного файла на равные по ширине и высоте части
        /// (по условной "сетке") и возвращает массив объектов Bitmap с этими изображениями.
        /// Параметры chunksH и chunksV определяют размеры условной сетки, количество рассечений 
        /// исходного изображения по горизонатали (ряды) и вертикали (колонки) соответственно 
        /// (1 - без рассечения в данной плоскости, 2 - рассечение пополам и т.д.; 
        /// если оба параметра равны 1, вернётся массив с одним исходным изображением;
        /// ноль и отрицательный значения недопустимы).
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="chunksH"></param>
        /// <param name="chunksV"></param>
        /// <returns></returns>
        public static Bitmap[] Slice(string filePath, int chunksH, int chunksV)
        {
            if (chunksH <= 0 || chunksV <= 0)
                throw new ArgumentException
                    ("Значения параметров chunksH/chunksV должны быть не меньше чем 1");

            int outCount = chunksH * chunksV;
            var result = new Bitmap[outCount];
            var source = new Bitmap(filePath);

            var index = 0;
            var partsH = SliceH(source, chunksH);
            for (int i = 0; i < chunksH; i++)
            {
                var partsV = SliceV(partsH[i], chunksV);
                for (int j = 0; j < chunksV; j++)
                {
                    result[index++] = partsV[j];
                }
            }

            return result;
        }

        /// <summary>
        /// Перегрузка метода для нарезки исходного избражения с получением
        /// выходных изображений разной ширины.
        /// Принимает в качестве второго аргумента последовательность/массив чисел.
        /// Изображение будет рассечено по горизонтали (ряды) в соответствии с количеством чисел
        /// в последовательности, после чего для каждого числа каждая часть 
        /// будет рассечена по вертикали на соответствующее этому числу количество частей.
        /// Последовательность не должна содержать нулей или отрицательных значений. 
        /// (Значение 1 допустимо, в этом случае фрагмент не будет рассечён по вертикали 
        /// и будет добавлен в выходной массив в исходном виде).
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="chunks"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static Bitmap[] Slice(string filePath, params int[] chunks)
        {
            foreach (var number in chunks)
                if (number <= 0) throw new ArgumentException
                        ("Значения параметров нарезки должны быть не меньше чем 1");

            int outCount = chunks.Sum();
            var result = new Bitmap[outCount];
            var source = new Bitmap(filePath);

            var index = 0;
            var partsH = SliceH(source, chunks.Length);
            for (int i = 0; i < partsH.Length; i++)
            {
                var partsV = SliceV(partsH[i], chunks[i]);
                for (int j = 0; j < partsV.Length; j++)
                {
                    result[index++] = partsV[j];
                }
            }

            return result;
        }
    }
}
