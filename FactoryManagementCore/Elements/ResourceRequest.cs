using System;
using System.Globalization;

namespace FactoryManagementCore.Elements
{
    /// <summary>
    /// Инкапсулирует входной поток определённого ресурса
    /// (тип и количество поступления в минуту). Логически связан с классом
    /// ResourceStream и может сравниваться с экземплярами этого класса и
    /// конвертироваться в них.
    /// </summary>
    public class ResourceRequest
    {
        /// <summary>
        /// Хранит количество поступающего ресурса (ед. в минуту).
        /// </summary>
        protected double _countPerMinute;


        /// <summary>
        /// Возвращает строку с названием ресурса.
        /// </summary>
        public string Resource { get; }

        /// <summary>
        /// Возвращает или задаёт количество поступающего ресурса (ед. в минуту).
        /// </summary>
        public virtual double CountPerMinute
        {
            get => _countPerMinute;
            set
            {
                if (value < 0) throw new InvalidOperationException
                        ("Значение запроса ресурса не может быть отрицательным");

                _countPerMinute = value;
                RequestChanged?.Invoke();
            }
        }
        /// <summary>
        /// Возвращает или задаёт логическое значение, указывающее,
        /// существует ли производственный цех, который удовлетворяет этот запрос.
        /// </summary>
        public virtual bool IsSatisfied { get; set; }

        /// <summary>
        /// Происходит при изменении значения свойства CountPerMinute.
        /// </summary>
        public event Action RequestChanged;


        /// <summary>
        /// Создаёт новый экземпляр ResourceRequest с указанными
        /// значениями свойств Resource и CountPerMinute.
        /// Новый запрос всегда является неудовлетворённым, пока не будет
        /// задано обратное.
        /// </summary>
        /// <param name="resource"></param>
        /// <param name="countPerMinute"></param>
        public ResourceRequest(string resource, double countPerMinute)
        {
            Resource = resource;
            _countPerMinute = countPerMinute;
        }


        /// <summary>
        /// Метод для наследников, позволяющий им вызвать событие RequestChanged.
        /// </summary>
        protected void RaiseRequestChanged()
        {
            RequestChanged?.Invoke();
        }


        /// <summary>
        /// Проверяет, имеет ли указанный объект ResourceRequest тот-же самый тип ресурса,
        /// что у исходного экземпляра. Возвращает true, если и у исходного объекта,
        /// и у объекта, переданного в качестве параметра, ресурс один и тот-же.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool HasSameResource(ResourceRequest other)
        {
            return Resource == other.Resource;
        }

        /// <summary>
        /// Возвращает новый экземпляр ResourceRequest
        /// с идентичными исходному экземпляру значениями
        /// свойств Resource и CountPerMinute.
        /// Новый запрос всегда является неудовлетворённым, пока не будет
        /// задано обратное.
        /// </summary>
        /// <returns></returns>
        public ResourceRequest Clone()
        {
            return new ResourceRequest(Resource, CountPerMinute);
        }

        /// <summary>
        /// Возвращает новый экземпляр CombinedResourceRequest, на основе двух исходных запросов.
        /// Один из исходных запросов (или оба они)
        /// так-же может/могут быть комбинированным/комбинированными.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static CombinedResourceRequest operator +(ResourceRequest left, ResourceRequest right)
        {
            if(left is CombinedResourceRequest combinedLeft
                && right is CombinedResourceRequest combinedRight)
                return combinedLeft + combinedRight;

            else if(left is CombinedResourceRequest altCombinedLeft)
                return altCombinedLeft + right;

            else if(right is CombinedResourceRequest altCombinedRight) 
                return left + altCombinedRight;

            else return new CombinedResourceRequest(left, right);
        }

        /// <summary>
        /// Определяет равенство двух обьектов ResourceRequest.
        /// Обьекты будут считаться равными, если у них совпадает и ресурс
        /// и количество поступления в минуту.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator ==(ResourceRequest left, ResourceRequest right)
        {
            return left.Equals(right);
        }


        /// <summary>
        /// Определяет неравенство двух объектов ResourceRequest.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator !=(ResourceRequest left, ResourceRequest right)
        {
            return !left.Equals(right);
        }

        /// <summary>
        /// Возвращает структуру ResourceStream с идентичными
        /// значениями свойств Resource и CountPerMinute.
        /// </summary>
        /// <returns></returns>
        public ResourceStream ToStream()
        {
            return new ResourceStream(Resource, CountPerMinute);
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
        /// Вернёт true, если второй обьект является ResourceRequest
        /// и у обоих объектов совпадает ресурс
        /// и количество поступления в минуту. Иначе вернёт false.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            return obj is ResourceRequest otherRequest &&
                   ToString() == otherRequest.ToString();
        }

        /// <summary>
        /// Переопределяет исходный метод. Возвращает хэш-код строки,
        /// возвращаемой переопределённым методом ToString.
        /// У двух объектов ResourceRequest, которые представляют одну и
        /// туже строку, будет один и тот-же хэш-код.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }
    }
}

