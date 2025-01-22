using System;
using System.Collections.Generic;
using System.Linq;

namespace CustomToolkit.Graphs
{
    /// <summary>
    /// Представляет логическое отображение графа.
    /// Поддерживается создание взвешенных и направленных графов (см. описания методов и конструкторов), 
    /// добавление/удаление/переименование вершин, добавление/удаление рёбер,
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
        /// Возвращает вершину с указанным ID 
        /// (назначается графом атоматически при её создании).
        /// </summary>
        /// <param name="id">Индекс вершины</param>
        /// <returns></returns>
        public Node this[int id] => _nodes.First(node => node.ID == id);
        /// <summary>
        /// Возвращает первую найденную вершину с указанным именем.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Node this[string name] => _nodes.First(node => node.Name == name);
        /// <summary>
        /// Возвращает ребро, направленное от вершины с первым ID к вершине со вторым ID.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public Edge this[int start, int end] => Edges
                                               .First(edge => edge.First.ID == start &&
                                                              edge.Second.ID == end);
        /// <summary>
        /// Возвращает ребро, направленное от вершины с первым именем к вершине со вторым, 
        /// если таковое имеется.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public Edge this[string start, string end] => Edges
                                               .First(edge => edge.First.Name == start &&
                                                              edge.Second.Name == end);


        /// <summary>
        /// Возвращает пустой граф
        /// </summary>
        public Graph()
        {
            _nodes = new List<Node>();
        }

        /// <summary>
        /// Возвращает граф с указанным количеством вершин.
        /// Вершинам будут автоматически назначены ID, начиная с нуля.
        /// У вершин изначально не будет ни имён, ни ребёр.
        /// </summary>
        /// <param name="nodesCount">Количество вершин</param>
        public Graph(int nodesCount)
        {
            _nodes = Enumerable
                    .Range(0, nodesCount)
                    .Select((id) => new Node(id))
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
            _nodes.First(node => node.Name == name1)
                  .ConnectTo(_nodes.First(node => node.Name == name2), weight);
        }

        /// <summary>
        /// Удаляет указанное ребро, отсоединяя друг от друга его вершины.
        /// </summary>
        /// <param name="edge"></param>
        public void Disconnect(Edge edge)
        {
            edge.Dispose();
        }

        /// <summary>
        /// Удаляет рёбро между вершинами с указанными ID, если таковое имеется 
        /// (с учётом направления, от первого ко второму).
        /// </summary>
        /// <param name="id1"></param>
        /// <param name="id2"></param>
        public void Disconnect(int id1, int id2)
        {
            this[id1, id2].Dispose();
        }

        /// <summary>
        /// Удаляет рёбро между вершинами с указанными именами, если таковые имеются 
        /// (с учётом направления, от первого ко второму).
        /// </summary>
        /// <param name="name1"></param>
        /// <param name="name2"></param>
        public void Disconnect(string name1, string name2)
        {
            this[name1, name2].Dispose();
        }

        /// <summary>
        /// Добавляет новую вершину и назначает ей указанный ID (не должен повторять уже имеющиеся).
        /// Можно указать имя для новой вершины (необязательно).
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        public void AddNode(int id, string name = default)
        {
            if (_nodes.Select(node => node.ID).Contains(id))
                throw new ArgumentException("В графе уже есть вершина с таким ID");

            _nodes.Add(new Node(id, name));
        }

        /// <summary>
        /// Добавляет новую вершину и автоматически назначает ей подходящий индекс.
        /// Можно указать имя для новой вершины (необязательно).
        /// </summary>
        /// <param name="name">Имя для новой вершины</param>
        public void AddNode(string name = default)
        {
            _nodes.Add(new Node(GetProperID(), name));
        }

        /// <summary>
        /// Удаляет указанную вершину и все рёбра, связывающие её с другими вершинами.
        /// Если данная вершина в графе отсутствует, будет выброшено исключение.
        /// </summary>
        /// <param name="node"></param>
        public void RemoveNode(Node node)
        {
            node.Dispose();
            _nodes.Remove(node);
        }

        /// <summary>
        /// Удаляет вершину с указанным именем и все рёбра, связывающие её с другими узлами.
        /// Если вершина с указанным именем отсутствует, будет выброшено исключение.
        /// </summary>
        /// <param id="id"></param>
        public void RemoveNode(int id)
        {
            RemoveNode(this[id]);
        }

        /// <summary>
        /// Удаляет вершину с указанным именем и все рёбра, связывающие её с другими узлами.
        /// Если вершина с указанным именем отсутствует, будет выброшено исключение.
        /// </summary>
        /// <param name="name"></param>
        public void RemoveNode(string name)
        {
            RemoveNode(this[name]);
        }

        /// <summary>
        /// Удаляет из графа все вершины и рёбра.
        /// </summary>
        public void Clear()
        {
            _nodes.Clear();
        }
    }
}
