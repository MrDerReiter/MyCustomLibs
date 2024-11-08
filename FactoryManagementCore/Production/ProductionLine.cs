using FactoryManagementCore.Elements;
using System.Collections.Generic;
using System.Linq;

namespace FactoryManagementCore.Production
{
    /// <summary>
    /// Вполняет роль контейнера для нескольких производственных блоков,
    /// и позволяет отслеживать суммы их входных/выходных ресурсов.
    /// </summary>
    public class ProductionLine
    {
        private readonly List<ProductionBlock> _productionBlocks = new List<ProductionBlock>();

        /// <summary>
        /// Вохвращает контрольный список (только для чтения) всех производственных блоков,
        /// размещённых в этой линии.
        /// </summary>
        public IReadOnlyList<ProductionBlock> ProductionBlocks { get => _productionBlocks; }
        /// <summary>
        /// Возвращает первый блок производственной линии. Он является контрольным,
        /// с него начинается процесс создания производственной линии.
        /// </summary>
        public ProductionBlock MainProductionBlock { get => _productionBlocks[0]; }
        /// <summary>
        /// Возвращает список всех входных ресурсов производственной линии
        /// в виде объектов ResourceStream. Идентичные входные потоки ресурсов объединяются.
        /// </summary>
        public List<ResourceStream> Inputs { get; } = new List<ResourceStream>();
        /// <summary>
        /// Возвращает список всех выходных ресурсов производственной линии
        /// в виде списка объектов ResourceStream. Идентичные выходные потоки ресурсов
        /// объединяются. Идентичные потоки выходных и входных ресурсов взаимно компенсируются.
        /// </summary>
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

            //удаление продуктов которые должны быть равны нулю, но не равны из-за погрешности рассчётов в формате double
            for (int i = 0; i < Outputs.Count; i++)
                if (Outputs[i].CountPerMinute < 0.001)
                    Outputs.RemoveAt(i--);
        }


        /// <summary>
        /// Добавляет новый производственный блок в контрольный список,
        /// используя указанный цех в качестве его основного цеха.
        /// </summary>
        /// <param name="unit"></param>
        public void AddProductionBlock(ProductionUnit unit)
        {
            var block = new ProductionBlock(unit);
            AddProductionBlock(block);
        }
        /// <summary>
        /// Добавляет указанный производственный блок в контролльный список.
        /// </summary>
        /// <param name="prodBlock"></param>
        public void AddProductionBlock(ProductionBlock prodBlock)
        {
            _productionBlocks.Add(prodBlock);
            prodBlock.IOChanged += UpdateIO;

            UpdateIO();
        }
        /// <summary>
        /// Удаляет указанный производственный цех из контрольного списка.
        /// </summary>
        /// <param name="prodBlock"></param>
        public void RemoveProductionBlock(ProductionBlock prodBlock)
        {
            prodBlock.IOChanged -= UpdateIO;
            _productionBlocks.Remove(prodBlock);
            UpdateIO();
        }
    }
}
