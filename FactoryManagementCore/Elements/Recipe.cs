﻿using System.Collections.Generic;

namespace FactoryManagementCore.Elements
{
    /// <summary>
    /// Базовый абстрактный класс рецепта для изготовления продукции.
    /// Конкретная реализация определяется классом-наследником для
    /// определённой фабрики.
    /// </summary>
    public abstract class Recipe
    {
        /// <summary>
        /// Возвращает строку с названием рецепта.
        /// </summary>
        public string Name { get; }
        /// <summary>
        /// Возвращает в виде строки тип станков, на которых производится продукция
        /// по этому рецепту.
        /// </summary>
        public string Machine { get; }
        /// <summary>
        /// Возвращает список обьектов ResourceStream,
        /// соответствующих входным ресурам рецепта, необходимым для одной итерации
        /// работы производственного цеха.
        /// </summary>
        public List<ResourceStream> Inputs { get; init; }
        /// <summary>
        /// Возвращает список обьектов ResourceStream,
        /// соответствующих выходным продуктам рецепта, произведённым в ходе одной итерации
        /// работы производственного цеха.
        /// </summary>
        public List<ResourceStream> Outputs { get; init; }

        /// <summary>
        /// Базовый конструктор, используемый классами-наследниками.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="machine"></param>
        /// <param name="inputs"></param>
        /// <param name="outputs"></param>
        protected Recipe
            (string name, string machine,
            List<ResourceStream> inputs, List<ResourceStream> outputs)
        {
            Name = name;
            Machine = machine;
            Inputs = inputs;
            Outputs = outputs;
        }

        /// <summary>
        /// Специальный конструктор для EntityFramework,
        /// оставляющий возможность донастроить вход/выход рецепта уже после его создания.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="machine"></param>
        protected Recipe(string name, string machine)
        {
            Name = name;
            Machine = machine;
        }

        /// <summary>
        /// Возвращает название рецепта в качестве его строкового представления.
        /// </summary>
        /// <returns></returns>
        public override string ToString() => Name;

        /// <summary>
        /// Определяет равенство двух экземпляров-наследиков Recipe.
        /// Объекты будут считаться равными, если у них одно и то-же название рецепта.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            return obj is Recipe other &&
                   Name == other.Name;
        }

        /// <summary>
        /// Возвращает хэш-код строки с названием рецепта.
        /// Таким образом, у рецептов с идентичным названием
        /// будет один и тот-же хэш-код.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }
    }
}
