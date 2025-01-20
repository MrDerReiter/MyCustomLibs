using FactoryManagementCore.Elements;
using FactoryManagementCore.Production;

namespace FactoryManagementCore.Extensions
{
    /// <summary>
    /// Методы расширения для класса String
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Возвращает переведённую версию строки, у которой был вызван данный метод.
        /// Для перевода используется текущая реализация интерфейса INameTranslator,
        /// вызываемая через свойство класса ProductionManager.NameTranslator.
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string Translate(this string str)
        {
            return ProductionManager.NameTranslator.Translate(str);
        }
        /// <summary>
        /// Возвращает объекет ResourceStream, созданный из данной строки (если это возможно).
        /// Аналогичен вызову конструктора new ResourceStream(string).
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static ResourceStream ToStream(this string str)
        {
            return new ResourceStream(str);
        }
    }
}
