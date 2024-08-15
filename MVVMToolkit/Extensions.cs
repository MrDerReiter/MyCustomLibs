using System.ComponentModel;

namespace MVVMToolkit
{
    public static class Extensions
    {
        public static void AddRange<T>(this BindingList<T> list, IEnumerable<T> range)
        {
            foreach (var item in range)
                list.Add(item);
        }
    }
}
