using System;
using System.Globalization;

namespace FactoryManagementCore.Elements
{
    public readonly struct ResourceStream
    {
        public string Resource { get; }
        public double CountPerMinute { get; }


        public ResourceStream(string resource, double countPerMinute)
        {
            Resource = resource;
            CountPerMinute = countPerMinute;
        }


        public bool HasSameResource(ResourceStream other)
        {
            return Resource == other.Resource;
        }


        public static ResourceStream operator +(ResourceStream left, ResourceStream right)
        {
            if (left.HasSameResource(right))
                return new ResourceStream(left.Resource, left.CountPerMinute + right.CountPerMinute);
            else throw new InvalidOperationException("Нельзя применять операцию сложения для потоков с разными ресурсами.");
        }

        public static ResourceStream operator -(ResourceStream left, ResourceStream right)
        {
            if (left.HasSameResource(right))
                return new ResourceStream(left.Resource, left.CountPerMinute - right.CountPerMinute);
            else throw new InvalidOperationException("Нельзя применять операцию вычитания для потоков с разными ресурсами.");
        }

        public static ResourceStream operator *(ResourceStream left, double right)
        {
            return new ResourceStream(left.Resource, left.CountPerMinute * right);
        }

        public static bool operator ==(ResourceStream left, ResourceStream right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(ResourceStream left, ResourceStream right)
        {
            return !left.Equals(right);
        }

        public static bool operator ==(ResourceStream left, ResourceRequest right)
        {
            return left.ToString() == right.ToString();
        }

        public static bool operator !=(ResourceStream left, ResourceRequest right)
        {
            return left.ToString() != right.ToString();
        }


        public ResourceRequest ToRequest()
        {
            return new ResourceRequest(Resource, CountPerMinute);
        }

        public override string ToString()
        {
            return Resource + " " + CountPerMinute.ToString(CultureInfo.InvariantCulture);
        }

        public override bool Equals(object obj)
        {
            return obj is ResourceStream other &&
                   ToString() == other.ToString();
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }
    }
}
