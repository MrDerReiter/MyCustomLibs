using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace CustomToolkit.Graphs
{
    /// <summary>
    /// Представляет вершину в графе. Имеет большое количество публичных свойств, но изменить вручную можно только имя (по умолчанию все вершины анонимны).
    /// Для соединения с другими вершинами и прочих операций используйте методы графа.
    /// </summary>
    public class Node
    {
        private readonly List<Edge> _incidentEdges = new List<Edge>();

        /// <summary>
        /// Возвращает ID вершины, позволяющий идентифицировать её в родительском графе.
        /// </summary>
        public int ID { get; }
        /// <summary>
        /// Возвращает или задаёт имя вершины в виде строки
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Возвращает перечисление всех нцидентных вершин.
        /// </summary>
        public IEnumerable<Node> IncidentNodes => _incidentEdges
                                                  .Select(edge => edge.OtherNode(this));
        /// <summary>
        /// Возвращает перечисление вершин, в которые ведёт ребро из этой вершины.
        /// </summary>
        public IEnumerable<Node> OutgoingNodes => _incidentEdges
                                                  .Where(edge => edge.First == this)
                                                  .Select(edge => edge.Second);
        /// <summary>
        /// Возвращает перечисление вершин, из которых идёт ребро в эту вершину.
        /// </summary>
        public IEnumerable<Node> IncomingNodes => _incidentEdges
                                                  .Where(edge => edge.Second == this)
                                                  .Select(edge => edge.First);
        /// <summary>
        /// Возваращает перечисление всех инцидентных рёбер (независимо от их направления).
        /// </summary>
        public IEnumerable<Edge> IncidentEdges => _incidentEdges;
        /// <summary>
        /// Возвращает перечисление всех рёбер, исходящих из этой вершины.
        /// </summary>
        public IEnumerable<Edge> OutgoingEdges => _incidentEdges
                                                  .Where(edge => edge.First == this);
        /// <summary>
        /// Возвращает перечисление всех рёбер, приходящих в эту вершину.
        /// </summary>
        public IEnumerable<Edge> IncomingEdges => _incidentEdges
                                                  .Where(edge => edge.Second == this);


        /// <summary>
        /// Создаёт новую вершину с указанным идексом и именем (необязательно).
        /// </summary>
        /// <param name="id">Id вершины</param>
        /// <param name="name">Имя вершины</param>
        internal Node(int id, string name = default)
        {
            ID = id;
            Name = name;
        }

        /// <summary>
        /// Удаляет ребро из исходной вершины в переданную.
        /// </summary>
        /// <param name="otherNode"></param>
        internal void Disconnect(Node otherNode)
        {
            var targetEdge = _incidentEdges
                .First(edge => edge.IsGoingTo(otherNode));

                otherNode._incidentEdges.Remove(targetEdge);
                _incidentEdges.Remove(targetEdge);
        }

        /// <summary>
        /// Соединяет вершину с другой вершиной ребром.
        /// Ребро будет направлено от этой вершины к другой.
        /// Можно задать вес ребра (необязательно).
        /// </summary>
        /// <param name="otherNode">Вершина, к которой нужно присоединиться</param>
        /// <param name="weight">Вес для создаваемого ребра</param>
        internal void ConnectTo(Node otherNode, int weight = default)
        {
            var edge = new Edge(this, otherNode, weight);
            _incidentEdges.Add(edge);
            otherNode._incidentEdges.Add(edge);
        }

        /// <summary>
        /// Обрывает связи вершины с другими вершинами (удаляет все связывающие их рёбра).
        /// </summary>
        internal void Dispose()
        {
            foreach (var node in OutgoingNodes)
                Disconnect(node);

            foreach(var node in IncomingNodes)
                node.Disconnect(this);
        }

        /// <summary>
        /// Возвращает строку в формате [ID узла][пробел][имя узла]. 
        /// Если имя узла не задано, возвращает только строку с ID узла.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.IsNullOrEmpty(Name) ? 
                          ID.ToString() : 
                          ID.ToString() + " " + Name;
        }
    }
}
