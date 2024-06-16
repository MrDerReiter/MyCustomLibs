using FactoryManagementCore.Elements;
using System.Collections.Generic;

namespace FactoryManagementCore.Interfaces
{
    /// <summary>
    /// Представляет класс, реализующий выдачу рецептов по запросу фабрики.
    /// </summary>
    public interface IRecipeProvider<TRecipe> : IRecipeProvider where TRecipe : Recipe
    {
        /// <summary>
        /// Возвращает рецепт с указанным именем, если таковой имеется.
        /// Иначе вызывает исключение.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        TRecipe GetRecipeByName(string name);

        /// <summary>
        /// Возвращает перечисление всех репептов из указанной категории.
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        IEnumerable<TRecipe> GetAllRecipiesOfCategory(string category);

        /// <summary>
        /// Возвращает перечисление всех рецептов, имеющих указанный ресурс в качестве основного или побочного продукта.
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        IEnumerable<TRecipe> GetAllRecipiesOfProduct(string product);

        /// <summary>
        /// Возвращает перечисление всех рецептов, которые реализуются указанным станком/экстрактором.
        /// </summary>
        /// <param name="machine"></param>
        /// <returns></returns>
        IEnumerable<TRecipe> GetAllRecipiesOfMachine(string machine);

        /// <summary>
        /// Возвращает перечисление всех рецептов, в которых указанный ресурс требуется в качестве ингредиента.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        IEnumerable<TRecipe> GetAllRecipiesOfInput(string input);
    }
}
