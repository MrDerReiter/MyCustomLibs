using FactoryManagementCore.Interfaces;
using FactoryManagementCore.Elements;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using FactoryManagementCore.Services;

namespace FactoryManagementCore.Production
{
    /// <summary>
    /// Статический класс, используемый в качестве центразованного средства управления
    /// производственными линиями.
    /// Использует ряд встроенных методов для управления списком производственных линий
    /// и предоставляет вспомогательные сервисы.
    /// </summary>
    public static class ProductionManager
    {
        private static IFactorySaveLoadManager _saveLoadManager;
        private static ProductionLine _activeLine;
        private static BindingList<ProductionLine> _productionLines;
        private static bool IsDependenciesInjected;

        /// <summary>
        /// Возвращает контрольный список производственных линий, предназначенный только для чтения.
        /// Чтобы добавить или убрать линию, используйте соответствующие методы класса Production Manager.
        /// </summary>
        public static IReadOnlyList<ProductionLine> ProductionLines
        {
            get
            {
                CheckDependencies();
                return _productionLines;
            }
        }
        /// <summary>
        /// Возвращает экземпляр сервиса IRecipeProvider,
        /// предоставляющего рецепты для использования в производственных блоках и цехах.
        /// </summary>
        public static IRecipeProvider<Recipe> RecipeProvider { get; private set; }
        /// <summary>
        /// Возвращает экземпляр сервиса INameTranslator,
        /// используемого для отображения внутренник строк на пользовательский интерфейс. 
        /// </summary>
        public static INameTranslator NameTranslator { get; private set; }
        /// <summary>
        /// Возвращает последнюю производственную линию в контрольном списке.
        /// </summary>
        public static ProductionLine LastLine { get => _productionLines.Last(); }
        /// <summary>
        /// Возвращает или задаёт текущую выбранную производственную линию.
        /// При попытке задать в качестве активной линию, которой нет в контрольном списке выбросит исключение.
        /// </summary>
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


        /// <summary>
        /// Задаёт зависимости, указывая конкретные реализации для менеджера рецептов
        /// и менеджера сохранения/загрузки. Обязательно должен быть вызван перед любым другим обращением к классу,
        /// в противном случае будет выброшено исключение.
        /// </summary>
        /// <typeparam name="TRecipeProvider"></typeparam>
        /// <typeparam name="TSaveLoadManager"></typeparam>
        public static void Initialize<TRecipeProvider, TSaveLoadManager>()
            where TRecipeProvider : IRecipeProvider<Recipe>, new()
            where TSaveLoadManager : IFactorySaveLoadManager, new()
        {
            RecipeProvider = new TRecipeProvider();
            _saveLoadManager = new TSaveLoadManager();
            NameTranslator = new NameTranslator();

            var savedData = _saveLoadManager.LoadFactory();
            _productionLines = new BindingList<ProductionLine>(savedData);

            IsDependenciesInjected = true;
        }
        
        /// <summary>
        /// Добавляет указанную производственную линию в контрольный список
        /// (экземпляр предоставляется вызывающим кодом).
        /// </summary>
        /// <param name="line"></param>
        public static void AddProductionLine(ProductionLine line)
        {
            CheckDependencies();
            _productionLines.Add(line);
        }

        /// <summary>
        /// Удаляет текущую выбранную производственную линию из контрольного списка.
        /// </summary>
        public static void RemoveActiveLine()
        {
            CheckDependencies();
            _productionLines.Remove(ActiveLine);
        }

        /// <summary>
        /// Перемещает выбранную линию на одну позицию к началу списка.
        /// Если она уже в начале списка, ничего не делает.
        /// </summary>
        public static void MoveActiveLineLeft()
        {
            var index = _productionLines.IndexOf(ActiveLine);

            if (index <= 0) return;

            var temp = _productionLines[index - 1];
            _productionLines[index - 1] = _productionLines[index];
            _productionLines[index] = temp;
        }

        /// <summary>
        /// Перемещает выбранную линию на одну позицию к концу списка.
        /// Если она уже в конце списка, ничего не делает.
        /// </summary>
        public static void MoveActiveLineRight()
        {
            var index = _productionLines.IndexOf(ActiveLine);

            if (index == _productionLines.Count - 1) return;

            var temp = _productionLines[index + 1];
            _productionLines[index + 1] = _productionLines[index];
            _productionLines[index] = temp;
        }

        /// <summary>
        /// Сохраняет фабрику. Реализация процесса сохранения делегируется экземпляру
        /// ISaveLoadManager, полученному в виде зависимлсти при инициализации класса.
        /// </summary>
        public static void SaveFactory()
        {
            _saveLoadManager.SaveFactory();
        }
    }
}
