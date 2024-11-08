using FactoryManagementCore.Elements;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FactoryManagementCore.Production
{
    /// <summary>
    /// Инкапсулирует группу производственных цехов,
    /// обьединённых общим производственным запросом, который они ступенчато реализуют,
    /// от более сложных компонентов к более простым.
    /// </summary>
    public class ProductionBlock
    {
        private readonly List<ResourceRequest> _inputs = [];
        private readonly List<ResourceStream> _outputs = [];
        private readonly List<ProductionUnit> _productionUnits = [];

        /// <summary>
        /// Возвращает контрольный список (только для чтения) цехов данного блока,
        /// включая основной (он всегда будет первым в списке).
        /// </summary>
        public IReadOnlyList<ProductionUnit> ProductionUnits { get => _productionUnits; }
        /// <summary>
        /// Возвращает основной цех производственного блока, который непосредственно
        /// удовлетворяет производственный запрос блока (он будет общим и для блока,
        /// и для основного цеха). С него начинается всё дерево запросов производственного блока.
        /// Этот цех нельзя удалить, т.к. без него весь производственный блок не имеет смысла.
        /// </summary>
        public ProductionUnit MainProductionUnit { get => _productionUnits[0]; }
        /// <summary>
        /// Возвращает производственный запрос блока
        /// (т.е. запрос, который он должен удовлетворять, выдавая эквивалентный продукт на выходе).
        /// </summary>
        public ResourceRequest ProductionRequest { get; }
        /// <summary>
        /// Возвращает список всех производственных запросов блока, которые не были
        /// удовлетворены непосредственно в нём (т.е. в блоке ещё нет цехов, которые-бы выполняли этот запрос).
        /// </summary>
        public IReadOnlyList<ResourceRequest> Inputs { get => _inputs; }
        /// <summary>
        /// Возвращает список (только для чтения) всей продукции,
        /// производимых в данном блоке.
        /// Первая позиция в списке всегда эквивалентна запросу производственного блока.
        /// </summary>
        public IReadOnlyList<ResourceStream> Outputs { get => _outputs; }

        /// <summary>
        /// Происходит при обновлении данных производственного блока,
        /// когда изменяются входные значения запросов одного или более цехов,
        /// и требуется обновить все производственные рассчёты.
        /// </summary>
        public event Action IOChanged;


        /// <summary>
        /// Создаёт новый производственный блок, используя указанный цех в
        /// качестве основного. Цех не создаётся в конструкторе производственного блока,
        /// т.к. конкретная реализация производственного цеха должна быть
        /// определена вызывающим кодом.
        /// </summary>
        /// <param name="unit"></param>
        public ProductionBlock(ProductionUnit unit)
        {
            ProductionRequest = unit.ProductionRequest;
            ProductionRequest.RequestChanged += UpdateIO;

            _productionUnits.Add(unit);

            UpdateIO();
        }


        private void UpdateIO()
        {
            _inputs.Clear();

            _inputs.AddRange
                (_productionUnits
                .SelectMany(pb => pb.Inputs)
                .Where(input => !input.IsSatisfied));
            OptimizeInputs();

            _outputs.Clear();
            _outputs.AddRange(MainProductionUnit.Outputs);
            _outputs.AddRange
                (_productionUnits.Skip(1)
                .Where(unit => unit.Outputs.Count > 1)
                .SelectMany(unit => unit.Outputs.Skip(1)));
            OptimizeOutputs();

            IOChanged?.Invoke();
        }

        private void OptimizeInputs()
        {
            for (int i = 0; i < _inputs.Count - 1; i++)
                for (int j = i + 1; j < _inputs.Count; j++)
                    if (_inputs[i].HasSameResource(_inputs[j]))
                    {
                        _inputs[i] = _inputs[i] + _inputs[j];
                        _inputs.RemoveAt(j--);
                    }
        }

        private void OptimizeOutputs()
        {
            for (int i = 0; i < _outputs.Count - 1; i++)
                for (int j = i + 1; j < _outputs.Count; j++)
                    if (_outputs[i].HasSameResource(_outputs[j]))
                    {
                        _outputs[i] = _outputs[i] + _outputs[j];
                        _outputs.RemoveAt(j--);
                    }
        }


        /// <summary>
        /// Добавляет указанный производственный цех в контрольный список
        /// и производит необходимые операции привязки.
        /// </summary>
        /// <param name="unit"></param>
        public void AddProductionUnit(ProductionUnit unit)
        {
            unit.ProductionRequest.IsSatisfied = true;
            _productionUnits.Add(unit);

            UpdateIO();
        }

        /// <summary>
        /// Удаляет указанный производственный цех из контрольного списка. Выбрасывает исключение,
        /// если указанного цеха нет в списке, или если этот цех является основным
        /// (т.к. без него производственный блок теряет всякий смысл).
        /// </summary>
        /// <param name="unit"></param>
        /// <exception cref="InvalidOperationException"></exception>
        public void RemoveProductionUnit(ProductionUnit unit)
        {
            if (!_productionUnits.Contains(unit))
                throw new InvalidOperationException
                    ("Недопустимая операция. Попытка удалить из списка цех, которого не было в списке.");

            if (unit == MainProductionUnit)
                throw new InvalidOperationException
                    ("Нельзя удалить или преобразовать главный цех производственного блока.");

            unit.ProductionRequest.IsSatisfied = false;
            _productionUnits.Remove(unit);
            unit.Dispose();

            UpdateIO();
        }

        #region Вспомогательные методы
        /// <summary>
        /// Вспомогательный метод для внешнего вызова события IOChanged.
        /// В большинстве случаев оно вызывается само, когда это нужно;
        /// используйте внешний вызов только в крайнем случае
        /// (например, если вызывающий код реализует специфический функционал,
        /// который не может быть размещён в самом классе ProductionBlock,
        /// но требует вызов этого события, и не может быть реализован иначе).
        /// </summary>
        public void RaiseIOChanged()
        {
            IOChanged?.Invoke();
        }

        /// <summary>
        /// Вспомогательный метод для внешнего принудительного обновления
        /// входа/выхода ресурсов производственного блока.
        /// В большинстве случаев оно происходит само, когда это нужно;
        /// используйте внешний вызов только в крайнем случае
        /// (например, если вызывающий код реализует специфический функционал,
        /// который не может быть размещён в самом классе ProductionBlock,
        /// но требует вызов приватного метода UpdateIO, и не может быть реализован иначе).
        /// </summary>
        public void ForceUpdate()
        {
            UpdateIO();
        }
        #endregion
    }
}
