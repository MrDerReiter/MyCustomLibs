using FactoryManagementCore.Production;
using System.Collections.Generic;

namespace FactoryManagementCore.Interfaces
{
    public interface IFactorySaveLoadManager
    {
        List<ProductionLine> LoadFactory();

        void SaveFactory();
    }
}
