using System;
using System.Collections.Generic;
using System.Linq;

namespace CustomToolkit.Graphs
{
    /// <summary>
    /// Представляет логическое отображение графа.
    /// Поддерживается создание взвешенных и направленных графов (см. описания методов и конструкторов), 
    /// добавление/удаление/переименование вершин (но не рёбер, они управляются самим графом),
    /// а также полная очистка графа.
    /// </summary>
    public class Graph
    {
        private readonly List<Node> _nodes;

        /// <summary>
        /// Возвращает список (только для чтения) всех вершин в графе.
        /// </summary>
        public IReadOnlyList<Node> Nodes => _nodes;
        /// <summary>
        /// Возвращает список имён всех именованных вершин в графе.
        /// </summary>
        public List<string> NodeNames => _nodes
                                         .Select(node => node.ToString())
                                         .ToList();
        /// <summary>
        /// Возвращает перечисление всех рёбер в графе.
        /// </summary>
        public IEnumerable<Edge> Edges => _nodes
                                          .SelectMany(node => node.IncidentEdges)
                                          .Distinct();
        /// <summary>
        /// Возвращает вершину с указанным индексом.
        /// </summary>
        /// <param name="id">Индекс вершины</param>
        /// <returns></returns>
        public Node this[int id] => _nodes.First(node => node.ID == id);
        /// <summary>
        /// Возвращает ребро, направленное от вершины с первым индексом к вершине со вторым индексом.
        /// При отсутствии такового будет выброшено исключение.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public Edge this[int start, int end] => Edges
                                               .First(edge => edge.First.ID == start &&
                                                              edge.Second.ID == end);


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
                    .Select((index) => new Node(index))
                    .ToList();
        }


        private int GetProperID()
        {
            if (_nodes.Count == 0) return 0;

            else return _nodes
                   .Select(node => node.ID)
                   .Max() + 1;
        }


        /// <summary>
        /// Создаёт новое ребро между двумя указанными по индексу вершинами.
        /// Ребро будет направлено от первой вершины ко второй.
        /// При необходимости можно указать вес ребра.
        /// </summary>
        /// <param name="id1">Id первой вершины</param>
        /// <param name="id2">Id второй вершины</param>
        /// <param name="weight">Вес ребра (необязательно)</param>
        public void Connect(int id1, int id2, int weight = default)
        {
            _nodes[id1].ConnectTo(_nodes[id2], weight);
        }

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
            try
            {
                _nodes.First(node => node.Name == name1)
                      .ConnectTo(_nodes.First(node => node.Name == name2), weight);
            }
            catch
            {
                throw new ArgumentException
                    ("В графе нет вершин с указанными именами, " +
                     "либо есть только одна подходящая; соединение невозможно.");
            }
        }

        /// <summary>
        /// Добавляет новую вершину и назначает ей следующий свободный индекс.
        /// Можно указать имя для новой вершины (необязательно).
        /// </summary>
        /// <param name="name">Имя для новой вершины</param>
        public void AddNode(string name = default)
        {
            _nodes.Add(new Node(GetProperID(), name));
        }

        /// <summary>
        /// Удаляет указанный узел и все рёбра, связывающие его с другими узлами.
        /// Если указанный узел отсутсвует, будет выброшено исключение.
        /// </summary>
        /// <param name="node"></param>
        public void RemoveNode(Node node)
        {
            if (_nodes.Contains(node))
                throw new ArgumentException("Узел не найден.");

            node.Dispose();
            _nodes.Remove(node);
        }

        /// <summary>
        /// Удаляет узел с указанным именем и все рёбра, связывающие его с другими узлами.
        /// Если узел с указанным именем отсутствует, будет выброшено исключение.
        /// </summary>
        /// <param name="name"></param>
        public void RemoveNode(string name)
        {
            if (!NodeNames.Contains(name))
                throw new ArgumentException("Не найден узел с таким именем.");

            var targetNode = _nodes.First(node => node.Name == name);
            RemoveNode(targetNode);
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
