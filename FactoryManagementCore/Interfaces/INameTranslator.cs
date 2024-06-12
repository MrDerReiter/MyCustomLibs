namespace FactoryManagementCore.Interfaces
{
    public interface INameTranslator
    {
        /// <summary>
        /// Переводит строку названия станка, рецепта или ресурса из внутреннего программного представления
        /// в представление для показа в GUI</summary>
        /// <param name="name"></param>
        /// <returns></returns>
        string Translate(string name);
    }
}
