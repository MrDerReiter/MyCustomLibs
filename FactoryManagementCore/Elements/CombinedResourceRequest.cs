using System;
using System.Collections.Generic;
using System.Linq;

namespace FactoryManagementCore.Elements
{
    /// <summary>
    /// Представляет обьект, включающий себя два или более
    /// запросов, но ведёт себя как обычный запрос, суммирующий исходные. Отслеживает состояние
    /// включённых в него запросов и изменяется соответственно.
    /// </summary>
    public class CombinedResourceRequest : ResourceRequest
    {
        private List<ResourceRequest> _sourceRequests;

        /// <summary>
        /// Возвращает сумму всех значений CountPerMinute
        /// включённых запросов.
        /// Выбрасывает исключение при попытке задать это значение непосредственно.
        /// </summary>
        public override double CountPerMinute
        {
            get => _countPerMinute;
            set
            {
                throw new InvalidOperationException
                    ("Недопустимая операция. Нельзя напрямую задать значение для комбинированного запроса; " +
                     "оно может быть задано только изменением значений вложенных запросов.");
            }
        }
        /// <summary>
        /// Возвращает или задаёт логическое значение, указывающее,
        /// существует ли производственный цех, который удовлетворяет этот запрос.
        /// Также задаёт это значение для всех включённых запросов
        /// (поскольку цех, удовлетворяющий этот запрос очевидно удоветворяет
        /// и все включённые в него).
        /// </summary>
        public override bool IsSatisfied 
        { 
            get => base.IsSatisfied;
            set
            {
                base.IsSatisfied = value;
                foreach (var request in _sourceRequests) 
                    request.IsSatisfied = value;
            } 
        }


        /// <summary>
        /// Создаёт новый объект CombinedResourceRequest на
        /// основе переданого массива/перечисления исходных запросов
        /// (они тоже могут быть комбинированными).
        /// </summary>
        /// <param name="sourceRequests"></param>
        public CombinedResourceRequest(params ResourceRequest[] sourceRequests) : base(sourceRequests[0].Resource, 0)
        { 
            _sourceRequests = new List<ResourceRequest>(sourceRequests);
            _countPerMinute = _sourceRequests.Select(request => request.CountPerMinute).Sum();
            foreach (var request in _sourceRequests) request.RequestChanged += Update;
        }


        private void Update()
        {
            _countPerMinute = _sourceRequests.Select(x => x.CountPerMinute).Sum();
            RaiseRequestChanged();
        }


        /// <summary>
        /// Удаляет все связи исходными запросами и прекращает
        /// отслеживать их изменения.
        /// </summary>
        public void Dispose()
        {
            foreach (var request in _sourceRequests)
                request.RequestChanged -= Update;

            _sourceRequests.Clear();
        }

        /// <summary>
        /// Возвращает новый обычный запрос, с идентичным исходному
        /// значением CountPerMinute. Этот запрос не будет привязан к исходным
        /// включённым запросам и не будет отслеживать их изменения.
        /// </summary>
        /// <returns></returns>
        public ResourceRequest ToSingleRequest()
        {
            return new ResourceRequest(Resource, CountPerMinute);
        }

        /// <summary>
        /// Возвращает новый комбинированный запрос,
        /// в список включённых запросов которого добавлен указанный справа запрос
        /// (помимо уже имеющихся в исходном комбинированном запросе слева).
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static CombinedResourceRequest operator +(CombinedResourceRequest left, ResourceRequest right)
        {
            var requestPool = left._sourceRequests.Append(right);
            return new CombinedResourceRequest(requestPool.ToArray());
        }

        /// <summary>
        /// Возвращает новый комбинированный запрос,
        /// в список включённых запросов которого добавлен указанный слева запрос
        /// (помимо уже имеющихся в исходном комбинированном запросе справа).
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static CombinedResourceRequest operator +(ResourceRequest left, CombinedResourceRequest right)
        {
            var requestPool = right._sourceRequests.Append(left);
            return new CombinedResourceRequest(requestPool.ToArray());
        }

        /// <summary>
        /// Возвращает новый комбинированный запрос,
        /// список включённых запросов которого объединяет все включённые
        /// запросы как правого, так и левого комбинированных запросов.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static CombinedResourceRequest operator +(CombinedResourceRequest left, CombinedResourceRequest right)
        {
            var requestPool = left._sourceRequests.Concat(right._sourceRequests);

            return new CombinedResourceRequest(requestPool.ToArray());
        }
    }
}
