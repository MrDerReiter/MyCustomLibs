using System;
using System.Collections.Generic;
using System.Linq;

namespace FactoryManagementCore.Elements
{
    public class CombinedResourceRequest : ResourceRequest
    {
        private List<ResourceRequest> _sourceRequests;

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


        public ResourceRequest ToSingleRequest()
        {
            return new ResourceRequest(Resource, CountPerMinute);
        }

        public static CombinedResourceRequest operator +(CombinedResourceRequest left, ResourceRequest right)
        {
            var requestPool = left._sourceRequests.Append(right);
            return new CombinedResourceRequest(requestPool.ToArray());
        }

        public static CombinedResourceRequest operator +(ResourceRequest left, CombinedResourceRequest right)
        {
            var requestPool = right._sourceRequests.Append(left);
            return new CombinedResourceRequest(requestPool.ToArray());
        }

        public static CombinedResourceRequest operator +(CombinedResourceRequest left, CombinedResourceRequest right)
        {
            var requestPool = left._sourceRequests.Concat(right._sourceRequests);

            return new CombinedResourceRequest(requestPool.ToArray());
        }
    }
}
