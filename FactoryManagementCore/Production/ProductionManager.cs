using FactoryManagementCore.Interfaces;
using FactoryManagementCore.Elements;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace FactoryManagementCore.Production
{
    public static class ProductionManager
    {
        private static IFactorySaveLoadManager _saveLoadManager;
        private static ProductionLine _activeLine;
        private static BindingList<ProductionLine> _productionLines;
        private static bool IsDependenciesInjected;

        public static IReadOnlyList<ProductionLine> ProductionLines
        {
            get
            {
                CheckDependencies();
                return _productionLines;
            }
        }
        public static IRecipeProvider<Recipe> RecipeProvider { get; private set; }
        public static INameTranslator NameTranslator { get; private set; }
        public static ProductionLine LastLine { get => _productionLines.Last(); }
        public static ProductionLine ActiveLine
        {
            get => _activeLine;
            set
            {
                if (_productionLines.Contains(value) || value == null) _activeLine = value;
                else throw new InvalidOperationException
                        ("Попытка использовать в качестве активной линию, которой нет в списке менеджера.");
            }
        }


        private static void CheckDependencies()
        {
            if (!IsDependenciesInjected)
                throw new InvalidOperationException
                    ("Менеджер производства не инициализирован, " +
                    "сначала вызовите метод Initialize и передайте ему нужные зависимости");
        }


        public static void Initialize<TRecipeProvider, TSaveLoadManager, TNameTranslator>()
            where TRecipeProvider : IRecipeProvider<Recipe>, new()
            where TSaveLoadManager : IFactorySaveLoadManager, new()
            where TNameTranslator : INameTranslator, new()
        {
            RecipeProvider = new TRecipeProvider();
            _saveLoadManager = new TSaveLoadManager();
            NameTranslator = new TNameTranslator();

            var savedData = _saveLoadManager.LoadFactory();
            _productionLines = new BindingList<ProductionLine>(savedData);

            IsDependenciesInjected = true;
        }

        public static void AddProductionLine(ProductionLine line)
        {
            CheckDependencies();
            _productionLines.Add(line);
        }

        public static void RemoveActiveLine()
        {
            CheckDependencies();
            _productionLines.Remove(ActiveLine);
        }

        public static void MoveActiveLineLeft()
        {
            var index = _productionLines.IndexOf(ActiveLine);

            if (index <= 0) return;

            var temp = _productionLines[index - 1];
            _productionLines[index - 1] = _productionLines[index];
            _productionLines[index] = temp;
        }

        public static void MoveActiveLineRight()
        {
            var index = _productionLines.IndexOf(ActiveLine);

            if (index == _productionLines.Count - 1) return;

            var temp = _productionLines[index + 1];
            _productionLines[index + 1] = _productionLines[index];
            _productionLines[index] = temp;
        }

        public static void SaveFactory()
        {
            _saveLoadManager.SaveFactory();
        }
    }
}
