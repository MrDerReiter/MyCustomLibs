using FactoryManagementCore.Production;

namespace FactoryManagementCore.Extensions
{
    public static class StringExtensions
    {
        public static string Translate(this string str)
        {
            return ProductionManager.NameTranslator.Translate(str);
        }
    }
}
