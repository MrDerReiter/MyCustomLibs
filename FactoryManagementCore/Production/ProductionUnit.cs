using FactoryManagementCore.Elements;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FactoryManagementCore.Production
{
    public abstract class ProductionUnit
    {
        protected ResourceRequest[] _inputs;
        protected ResourceStream[] _outputs;

        public int Id { get; set; } //ID в базе данных
        public virtual Recipe Recipe { get; }
        public string Machine { get => Recipe.Machine; }
        public double MachinesCount { get => GetMachinesCount(); }

        public ResourceRequest ProductionRequest { get; protected set; }
        public IReadOnlyList<ResourceRequest> Inputs { get => _inputs; }
        public IReadOnlyList<ResourceStream> Outputs { get => _outputs; }


        protected virtual void UpdateIO()
        {
            for (int i = 0; i < _inputs.Length; i++)
                _inputs[i].CountPerMinute = Recipe.Inputs[i].CountPerMinute * MachinesCount;

            for (int i = 0; i < _outputs.Length; i++)
                _outputs[i] = Recipe.Outputs[i] * MachinesCount;
        }

        protected abstract double GetMachinesCount();
    }
}
