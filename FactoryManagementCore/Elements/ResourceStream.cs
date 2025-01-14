using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;

namespace FactoryManagementCore.Elements
{
    /// <summary>
    /// Простой value object инкапсулирующий поток определённого ресурса
    /// (тип и количество поступления в минуту). Логически связан с классом
    /// ResourceRequest и может сравниваться с экземплярами этого класса и
    /// конвертироваться в них.
    /// </summary>
    public class ResourceStream
    {
        /// <summary>
        /// ID для использования в качестве первичного ключа в базе данных.
        /// </summary>
        ///
        public int ID { get; set; }
        /// <summary>
        /// Возвращает строку с названием ресурса.
        /// </summary>
        public string Resource { get; }
        /// <summary>
        /// Возвращает количество поступающего ресурса (ед. в минуту).
        /// </summary>
        public double CountPerMinute { get; }


        /// <summary>
        /// Создаёт новый объект ResourceStream с указанным ресурсом
        /// и количеством в минуту.
        /// </summary>
        /// <param name="resource"></param>
        /// <param name="countPerMinute"></param>
        public ResourceStream(string resource, double countPerMinute)
        {
            if (countPerMinute < 0)
                throw new InvalidOperationException
                    ("Значение потока ресурса не может быть отрицательным.");

            Resource = resource;
            CountPerMinute = countPerMinute;
        }

        /// <summary>
        /// Создаёт новый объект ResourceStream на основе строки в формате
        /// [ресурс] [количество] с пробелом в качестве разделителя.
        /// Название ресурса не должно содержать пробелов, а количество должно быть
        /// выражено корректным числом (целое или дробное с точкой в качестве десятичного разделителя).
        /// </summary>
        /// <param name="parsebleContent"></param>
        /// <exception cref="FormatException"></exception>
        public ResourceStream(string parsebleContent)
        {
            try
            {
                var parts = parsebleContent.Split(' ');
                var resource = parts[0];
                var count = double.Parse(parts[1], CultureInfo.InvariantCulture);

                if (count < 0)
                    throw new ArgumentException
                        ("Значение потока ресурса не может быть отрицательным.");

                Resource = resource;
                CountPerMinute = count;
            }
            catch (ArgumentException) { throw; }
            catch (Exception)
            {
                throw new FormatException
                    ("Невозможно преобразовать данную строку в ResourceStream. " +
                     "Строка должна иметь формат [ресурс, без пробелов]пробел" +
                     "[количество(число, целое или с точкой)].");
            }
        }


        /// <summary>
        /// Проверяет, имеет ли указанный объект ResourceStream тот-же самый тип ресурса,
        /// что у исходного экземпляра. Возвращает true, если и у исходного объекта,
        /// и у объекта, переданного в качестве параметра, ресурс один и тот-же.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool HasSameResource(ResourceStream other)
        {
            return Resource == other.Resource;
        }


        /// <summary>
        /// Возвращает новый экземпляр ResourceStream, у которого
        /// свойство CountPerMinute будет суммой аналогичного свойства двух исходных обьектов.
        /// Выбросит исключение, если использованы потоки с разными ресурсами.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public static ResourceStream operator +(ResourceStream left, ResourceStream right)
        {
            if (left.HasSameResource(right))
                return new ResourceStream(left.Resource, left.CountPerMinute + right.CountPerMinute);
            else throw new InvalidOperationException
                    ("Нельзя применять операцию сложения для потоков с разными ресурсами.");
        }

        /// <summary>
        /// Возвращает новый экземпляр ResourceStream, у которого
        /// свойство CountPerMinute будет результатом вычитания CountPerMinute второго объекта
        /// из аналогичного свойства первого.
        /// Выбросит исключение, если были использованы потоки с разными ресурсами 
        /// или если в результате будет получен поток с нулевым или отрицательным количеством.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public static ResourceStream operator -(ResourceStream left, ResourceStream right)
        {
            if (left.HasSameResource(right) &&
                left.CountPerMinute > right.CountPerMinute)
                return new ResourceStream(left.Resource, left.CountPerMinute - right.CountPerMinute);
            else throw new InvalidOperationException
                    ("Нельзя применять операцию вычитания для потоков с разными ресурсами или " +
                     "вычитать больший поток из меньшего.");
        }

        /// <summary>
        /// Возвращает новый экземпляр ResourceStream, у которого
        /// свойство CountPerMinute будет произведением исходной величины на указанное число.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static ResourceStream operator *(ResourceStream left, double right)
        {
            return new ResourceStream(left.Resource, left.CountPerMinute * right);
        }

        /// <summary>
        /// Возвращает новый экземпляр ResourceStream, у которого
        /// свойство CountPerMinute будет результатом деления исходной величины на указанное число.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static ResourceStream operator /(ResourceStream left, double right)
        {
            if (right == 0)
                throw new DivideByZeroException("Деление на ноль невозможно.");

            return new ResourceStream(left.Resource, left.CountPerMinute / right);
        }

        /// <summary>
        /// Определяет равенство двух обьектов ResourceStream.
        /// Обьекты будут считаться равными, если у них совпадает и ресурс
        /// и количество поступления в минуту.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator ==(ResourceStream left, ResourceStream right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Определяет неравенство двух объектов ResourceStream.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator !=(ResourceStream left, ResourceStream right)
        {
            return !left.Equals(right);
        }

        /// <summary>
        /// Определяет равенство объекта ResourceStream и объекта RecourceRequest.
        /// Обьекты будут считаться равными, если у них совпадает и ресурс
        /// и количество поступления в минуту (несмотря на то, что они являются
        /// экземплярами разных классов).
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator ==(ResourceStream left, ResourceRequest right)
        {
            return left.ToString() == right.ToString();
        }

        /// <summary>
        /// Определяет неравенство объекта ResourceStream и объекта RecourceRequest.
        /// Обьекты будут считаться неравными, если у них не совпадает и ресурс
        /// и количество поступления в минуту (а не только потому, что они являются
        /// экземплярами разных классов).
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator !=(ResourceStream left, ResourceRequest right)
        {
            return left.ToString() != right.ToString();
        }


        /// <summary>
        /// Возвращает новый объект ResourceRequest с идентичными
        /// значениями свойств Resource и CountPerMinute.
        /// </summary>
        /// <returns></returns>
        public ResourceRequest ToRequest()
        {
            return new ResourceRequest(Resource, CountPerMinute);
        }

        /// <summary>
        /// Переопределённая версия базового метода. Возвращает строку в формате
        /// "[Resource] пробел [CountPerMinute]".
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Resource + " " + CountPerMinute.ToString(CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Переопределяет исходный метод.
        /// Вернёт true, если второй обьект является ResourceStream
        /// и у обоих объектов совпадает ресурс
        /// и количество поступления в минуту. Иначе вернёт false.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            return obj is ResourceStream other &&
                   ToString() == other.ToString();
        }

        /// <summary>
        /// Переопределяет исходный метод. Возвращает хэш-код строки,
        /// возвращаемой переопределённым методом ToString.
        /// У двух объектов ResourceStream, которые представляют одну и
        /// туже строку, будет один и тот-же хэш-код.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }
    }
}
