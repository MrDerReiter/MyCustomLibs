using System;
using System.Collections.Generic;
using System.Linq;

namespace Graphs
{
    /// <summary>
    /// Представляет вершину в графе. Имеет большое количество публичных свойств, но изменить вручную можно только имя (по умолчанию все вершины анонимны).
    /// Для соединения с другими вершинами и прочих операций используйте методы графа.
    /// </summary>
    public class Node
    {
        private readonly List<Edge> _incidentEdges = new List<Edge>();
        /// <summary>
        /// Возвращает перечисление всех нцидентных вершин.
        /// </summary>
        public IEnumerable<Node> IncidentNodes { get => _incidentEdges.Select(e => e.OtherNode(this)); }
        /// <summary>
        /// Возвращает перечисление вершин, в которые ведёт ребро из этой вершины.
        /// </summary>
        public IEnumerable<Node> OutgoingNodes { get => _incidentEdges.Where(e => e.First == this).Select(e => e.Second); }
        /// <summary>
        /// Возвращает перечисление вершин, из которых идёт ребро в эту вершину.
        /// </summary>
        public IEnumerable<Node> IcomingNodes { get => _incidentEdges.Where(e => e.Second == this).Select(e => e.First); }
        /// <summary>
        /// Возваращает перечисление всех инцидентных рёбер (независимо от их направления).
        /// </summary>
        public IEnumerable<Edge> IncidentEdges { get => _incidentEdges; }
        /// <summary>
        /// Возвращает перечисление всех рёбер, исходящих из этой вершины.
        /// </summary>
        public IEnumerable<Edge> OutgoingEdges { get => _incidentEdges.Where(e => e.First == this); }
        /// <summary>
        /// Возвращает перечисление всех рёбер, приходящих в эту вершину.
        /// </summary>
        public IEnumerable<Edge> IncomungEdges { get => _incidentEdges.Where(e => e.Second == this); }
        /// <summary>
        /// Возвращает или задаёт имя вершины в формате строки
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Возвращает индекс вершины в родительском графе.
        /// </summary>
        public int Id { get; }

        /// <summary>
        /// Создаёт новую вершину с указанным идексом и именем (необязательно).
        /// </summary>
        /// <param name="id">Id вершины</param>
        /// <param name="name">Имя вершины</param>
        public Node(int id, string name = default)
        { 
            Id = id; 
            Name = name;
        }

        public override string ToString()
        {
            return Name == null ? Id.ToString() : Id.ToString() + " " + Name;
        }

        /// <summary>
        /// Соединяет вершину с другой вершиной ребром.
        /// Ребро будет направлено от этой вершины к другой.
        /// Можно задать вес ребра (необязательно).
        /// </summary>
        /// <param name="anotherNode">Вершина, к которой нужно присоединиться</param>
        /// <param name="weight">Вес для создаваемого ребра</param>
        public void ConnectTo(Node anotherNode, int weight = default)
        {
            var edge = new Edge(this, anotherNode, weight);
            _incidentEdges.Add(edge);
            anotherNode._incidentEdges.Add(edge);
        }
    }
}
