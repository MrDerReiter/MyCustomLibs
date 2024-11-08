using FactoryManagementCore.Production;
using System.Collections.Generic;

namespace FactoryManagementCore.Interfaces
{
    /// <summary>
    /// Представляет обьект, реализующий логику сохранения и зарузки фабрики между сессиями.
    /// </summary>
    public interface IFactorySaveLoadManager
    {
        /// <summary>
        /// Возвращает ранее сохранённую фабрику в виде списка производственных линий.
        /// </summary>
        /// <returns></returns>
        List<ProductionLine> LoadFactory();

        /// <summary>
        /// Сохраняет фабрику, чтобы её можно было использвоать в следующей сессии.
        /// </summary>
        void SaveFactory();
    }
}
