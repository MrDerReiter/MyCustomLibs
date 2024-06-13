using FactoryManagementCore.Elements;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FactoryManagementCore.Production
{
    public class ProductionBlock
    {
        private readonly List<ResourceRequest> _inputs = new List<ResourceRequest>();
        private readonly List<ResourceStream> _outputs = new List<ResourceStream>();
        private readonly List<ProductionUnit> _productionUnits = new List<ProductionUnit>();

        public int Id { get; set; } //ID в базе данных
        public IReadOnlyList<ProductionUnit> ProductionUnits { get => _productionUnits; }
        public ProductionUnit MainProductionUnit { get => _productionUnits[0]; }
        public ResourceRequest ProductionRequest { get; }
        public IReadOnlyList<ResourceRequest> Inputs { get => _inputs; }
        public IReadOnlyList<ResourceStream> Outputs { get => _outputs; }

        public event Action IOChanged;


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


        public void AddProductionUnit(ProductionUnit unit)
        {
            unit.ProductionRequest.IsSatisfied = true;
            _productionUnits.Add(unit);

            UpdateIO();
        }

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
    }
}
