using System;
using System.Globalization;

namespace FactoryManagementCore.Elements
{
    public class ResourceRequest
    {
        protected double _countPerMinute;

        public int Id { get; set; } //ID в базе данных
        public string Resource { get; }
        public virtual double CountPerMinute
        {
            get => _countPerMinute;
            set
            {
                if (value < 0) throw new InvalidOperationException("Значение запроса ресурса не может быть отрицательным");
                _countPerMinute = value;
                RequestChanged?.Invoke();
            }
        }
        public bool IsSatisfied;

        public event Action RequestChanged;


        public ResourceRequest(string resource, double countPerMinute)
        {
            Resource = resource;
            _countPerMinute = countPerMinute;
        }


        protected void RaiseRequestChanged()
        {
            RequestChanged?.Invoke();
        }


        public bool HasSameResource(ResourceRequest other)
        {
            return Resource == other.Resource;
        }

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

        public static bool operator ==(ResourceRequest left, ResourceRequest right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(ResourceRequest left, ResourceRequest right)
        {
            return !left.Equals(right);
        }

        public ResourceStream ToStream()
        {
            return new ResourceStream(Resource, CountPerMinute);
        }

        public override string ToString()
        {
            return Resource + " " + CountPerMinute.ToString(CultureInfo.InvariantCulture);
        }

        public override bool Equals(object obj)
        {
            return obj is ResourceRequest otherRequest &&
                   ToString() == otherRequest.ToString();
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }
    }
}

