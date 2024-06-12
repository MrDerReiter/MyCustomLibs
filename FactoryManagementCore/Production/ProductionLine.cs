using FactoryManagementCore.Elements;
using System.Collections.Generic;
using System.Linq;

namespace FactoryManagementCore.Production
{
    public class ProductionLine
    {
        private readonly List<ProductionBlock> _productionBlocks = new List<ProductionBlock>();

        public int Id { get; set; } //ID в базе данных
        public IReadOnlyList<ProductionBlock> ProductionBlocks { get => _productionBlocks; }
        public ProductionBlock MainProductionBlock { get => _productionBlocks[0]; }
        public List<ResourceStream> Inputs { get; } = new List<ResourceStream>();
        public List<ResourceStream> Outputs { get; } = new List<ResourceStream>();


        private void UpdateIO()
        {
            Inputs.Clear();
            Inputs.AddRange
                (_productionBlocks
                .SelectMany(pb => pb.Inputs)
                .Where(input => input.CountPerMinute > 0)
                .Select(input => input.ToStream()));

            Outputs.Clear();
            Outputs.AddRange
                (_productionBlocks
                .SelectMany(pb => pb.Outputs)
                .Where(output => output.CountPerMinute > 0));

            OptimizeIO();
        }

        private void OptimizeIO()
        {
            MergeExcessIO();
            BalanceIO();
        }

        private void MergeExcessIO()
        {
            for (int i = 0; i < Inputs.Count - 1; i++)
                for (int j = i + 1; j < Inputs.Count; j++)
                    if (Inputs[i].HasSameResource(Inputs[j]))
                    {
                        Inputs[i] = Inputs[i] + Inputs[j];
                        Inputs.RemoveAt(j--);
                    }

            for (int i = 0; i < Outputs.Count - 1; i++)
                for (int j = i + 1; j < Outputs.Count; j++)
                    if (Outputs[i].HasSameResource(Outputs[j]))
                    {
                        Outputs[i] = Outputs[i] + Outputs[j];
                        Outputs.RemoveAt(j--);
                    }
        }

        private void BalanceIO()
        {
            for (int i = 0; i < Outputs.Count; i++)
                for (int j = 0; j < Inputs.Count; j++)
                    if (Outputs[i].HasSameResource(Inputs[j]))
                    {
                        if (Outputs[i].CountPerMinute < Inputs[j].CountPerMinute)
                        {
                            Inputs[j] = Inputs[j] - Outputs[i];
                            Outputs.RemoveAt(i--);
                            break;
                        }
                        else if (Outputs[i].CountPerMinute > Inputs[j].CountPerMinute)
                        {
                            Outputs[i] = Outputs[i] - Inputs[j];
                            Inputs.RemoveAt(j--);
                        }
                        else
                        {
                            Outputs.RemoveAt(i--);
                            Inputs.RemoveAt(j);
                            break;
                        }
                    }
        }


        public void AddProductionBlock(ProductionUnit unit)
        {
            var block = new ProductionBlock(unit);
            AddProductionBlock(block);
        }

        public void AddProductionBlock(ProductionBlock prodBlock)
        {
            _productionBlocks.Add(prodBlock);
            prodBlock.IOChanged += UpdateIO;

            UpdateIO();
        }

        public void RemoveProductionBlock(ProductionBlock prodBlock)
        {
            prodBlock.IOChanged -= UpdateIO;
            _productionBlocks.Remove(prodBlock);
            UpdateIO();
        }
    }
}
