using System;
using System.Collections.Generic;
using System.Linq;

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
        /// Возвращает индекс вершины в родительском графе.
        /// </summary>
        public int ID { get; }
        /// <summary>
        /// Возвращает или задаёт имя вершины в формате строки
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
        public Node(int id, string name = default)
        {
            ID = id;
            Name = name;
        }


        private void Disconnect(Node otherNode)
        {
            var targetEdge = _incidentEdges
                .First(edge => edge.Nodes.Contains(otherNode));

            otherNode._incidentEdges.Remove(targetEdge);
            _incidentEdges.Remove(targetEdge);
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

        /// <summary>
        /// Обрывает связи узла с другими узлами (удаляет все связывающие его с ними рёбра).
        /// </summary>
        public void Dispose()
        {
            foreach (var node in IncidentNodes)
                Disconnect(node);
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
