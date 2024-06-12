namespace FactoryManagementCore.Elements
{
    public abstract class Recipe
    {
        public int Id { get; set; } //ID в базе данных
        public string Name { get; }
        public string Machine { get; }
        public ResourceStream[] Inputs { get; }
        public ResourceStream[] Outputs { get; }

        public Recipe
            (string name, string machine,
            ResourceStream[] inputs, ResourceStream[] outputs)
        {
            Name = name;
            Machine = machine;
            Inputs = inputs;
            Outputs = outputs;
        }

        public override string ToString() => Name;

        public override bool Equals(object obj)
        {
            return obj is Recipe other &&
                   Name == other.Name;
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }
    }
}
