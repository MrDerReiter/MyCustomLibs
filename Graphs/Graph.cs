using System;
using System.Collections.Generic;
using System.Linq;

namespace Graphs
{
    /// <summary>
    /// Представляет логическое отображение графа.
    /// При создании нового экземляра можно сразу сгенерировать набор непараметризованных вершин в конструкторе. 
    /// После создания экземпляра можно добавить новые вершины (в том числе именованные) и соедининять вершины рёбрами.
    /// Поддерживается обращение к вершинам по индексу, и рёбрам по индексам их инцидентных вершин.
    /// Поддерживается именование вершин, как при добавлении так и постфактум, через прямое обращение по индексу.
    /// Индексы присваиваются вершинам автоматически при их добавлении, в порядке их создания и неизменяемы вручную.
    /// Поддерживается создание взвешенных и направленных графов (см. описания методов и конструкторов).
    /// Не поддерживается удаление или изменение (кроме переименования) вершин и рёбер во избежании нарушения целостности данных.
    /// Если нужно перепроектировать граф опустошите его с помощью метода Clear или создайте новый.
    /// </summary>
    public class Graph
    {
        private readonly List<Node> _nodes;
        /// <summary>
        /// Возвращает перечисление всех вершин в графе.
        /// </summary>
        public IEnumerable<Node> Nodes { get => _nodes; }
        /// <summary>
        /// Возвращает перечисление имён всех именованных вершин в графе.
        /// </summary>
        public IEnumerable<string> NodesList { get => _nodes.Select(n => n.ToString()); }
        /// <summary>
        /// Возвращает перечисление всех рёбер в графе.
        /// </summary>
        public IEnumerable<Edge> Edges { get => Nodes.SelectMany(n => n.IncidentEdges).Distinct(); }
        /// <summary>
        /// Возвращает вершину с указанным индексом.
        /// </summary>
        /// <param name="index">Индекс вершины</param>
        /// <returns></returns>
        public Node this[int index] { get => _nodes[index]; }
        /// <summary>
        /// Возвращает ребро, направленное от вершины с первым индексом к вершине со вторым индексом.
        /// При отсутствии такового будет выброшено исключение.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public Edge this[int start, int end] { get => Edges.First(e => e.First.Id == start && e.Second.Id == end); }

        /// <summary>
        /// Возвращает пустой граф
        /// </summary>
        public Graph()
        {
            _nodes = new List<Node>();
        }

        /// <summary>
        /// Возвращает граф с указанным количеством вершин.
        /// Вершины будут проиндексированы начиная с нуля.
        /// У вершин изначально не будет ни имён, ни ребёр.
        /// </summary>
        /// <param name="nodesCount">Количество вершин</param>
        public Graph(int nodesCount)
        {
            _nodes = Enumerable
                    .Range(0, nodesCount)
                    .Select((n) => new Node(n))
                    .ToList();
        }

        /// <summary>
        /// Создаёт новое ребро между двумя указанными по индексу вершинами.
        /// Ребро будет направлено от первой вершины ко второй.
        /// При необходимости можно указать вес ребра.
        /// </summary>
        /// <param name="id1">Id первой вершины</param>
        /// <param name="id2">Id второй вершины</param>
        /// <param name="weight">Вес ребра (необязательно)</param>
        public void Connect(int id1, int id2, int weight = default) => _nodes[id1].ConnectTo(_nodes[id2], weight);

        /// <summary>
        /// Создаёт новое ребро между двумя указанными по имени вершинами.
        /// Ребро будет направлено от первой вершины ко второй.
        /// При необходимости можно указать вес ребра.
        /// </summary>
        /// <param name="name1">Имя первой вершины</param>
        /// <param name="name2">Имя второй вершины</param>
        /// <param name="weight">Вес ребра (необязательно)</param>
        /// <exception cref="ArgumentException">В списке вершин графа не найдена вершина с указнным именем</exception>
        public void Connect(string name1, string name2, int weight = default)
        {
            var names = _nodes.Select(n => n.Name);
            if (names.Contains(name1) && names.Contains(name2))
                _nodes.First(n => n.Name == name1).ConnectTo(_nodes.First(n => n.Name == name2), weight);
            else throw new ArgumentException();
        }

        /// <summary>
        /// Добавляет новую вершину по следующему свободному индексу
        /// </summary>
        public void Add()
        {
            _nodes.Add(new Node(_nodes.Count));
        }

        /// <summary>
        /// Добавляет новую именованную вершину по следующему свободному индексу
        /// </summary>
        /// <param name="name">Имя для новой вершины</param>
        public void Add(string name)
        {
            _nodes.Add(new Node(_nodes.Count, name));
        }

        /// <summary>
        /// Удаляет из графа все вершины и рёбра
        /// </summary>
        public void Clear()
        {
            _nodes.Clear();
        }
    }
}
