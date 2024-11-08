using FactoryManagementCore.Elements;
using System.Collections.Generic;

namespace FactoryManagementCore.Production
{
    /// <summary>
    /// Базовый класс для создания производственного цеха;
    /// наследники определяют реализацию для конкретного фабричного симулятора.
    /// </summary>
    public abstract class ProductionUnit
    {
        /// <summary>
        /// Массив запросов на входные ресурсы цеха.
        /// </summary>
        protected ResourceRequest[] _inputs;
        /// <summary>
        /// Массив потоков выходных ресурсов цеха.
        /// Как правило первым идёт основной продукт, выпускаемый цехом,
        /// а далее побочные продукты.
        /// </summary>
        protected ResourceStream[] _outputs;

        /// <summary>
        /// Возвращает рецепт, используемый станками цеха для производства продукции.
        /// </summary>
        public virtual Recipe Recipe { get; }
        /// <summary>
        /// Возвращает название конкретного типа станков,
        /// используемых цехом для производства продукции.
        /// </summary>
        public string Machine { get => Recipe.Machine; }
        /// <summary>
        /// Возвращает минимальное необходимое количество станков,
        /// требуемое для выполнения производственного запроса.
        /// </summary>
        public double MachinesCount { get => GetMachinesCount(); }

        /// <summary>
        /// Возвращает роизводственный запрос цеха,
        /// для удовлетворения которого он был создан.
        /// Один из выходных потоков цеха (как правило первый) должен соответствовать этому запросу.
        /// Задаётся при создании цеха, и не может быть изменён вызывающим кодом
        /// (но может быть изменён в классах-наследниках).
        /// </summary>
        public ResourceRequest ProductionRequest { get; protected set; }
        /// <summary>
        /// Возвращает список (только для чтения) всех
        /// запросов цеха на входные ресурсы.
        /// </summary>
        public IReadOnlyList<ResourceRequest> Inputs { get => _inputs; }
        /// <summary>
        /// Возвращает список (только для чтения) всех выходных
        /// потоков ресурсов цеха.
        /// </summary>
        public IReadOnlyList<ResourceStream> Outputs { get => _outputs; }


        /// <summary>
        /// Обновляет все входные запросы для их соответствия
        /// производственному запросу цеха. Рекомендуется переопределять данный метод в
        /// классах-наследниках с учётом специфики их реализации.
        /// </summary>
        protected virtual void UpdateIO()
        {
            for (int i = 0; i < _inputs.Length; i++)
                _inputs[i].CountPerMinute = Recipe.Inputs[i].CountPerMinute * MachinesCount;

            for (int i = 0; i < _outputs.Length; i++)
                _outputs[i] = Recipe.Outputs[i] * MachinesCount;
        }

        /// <summary>
        /// Вычисляет количество станков, необходимое для
        /// удовлетворения производственного запроса.
        /// Необходимо реализовать в классах-наследниках.
        /// </summary>
        /// <returns></returns>
        protected abstract double GetMachinesCount();


        /// <summary>
        /// Удаляет внешние связи производственного цеха 
        /// (прекращает отслеживание изменений производственного запроса, обнуляет входные запросы).
        /// </summary>
        public virtual void Dispose()
        {
            ProductionRequest.RequestChanged -= UpdateIO;
            if (ProductionRequest is CombinedResourceRequest request) request.Dispose();
            foreach (var input in _inputs) input.CountPerMinute = 0;
        }
    }
}
