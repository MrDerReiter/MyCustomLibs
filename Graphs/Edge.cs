using System;

namespace Graphs
{
    /// <summary>
    /// Представляет ребро в графе.
    /// Иммутабельный обьект, создаётся автоматически при соединении вершин внутри графа с помощью его методов и не подлежит изменению.
    /// Все свойства публичны, но доступны только для чтения.
    /// </summary>
    public class Edge
    {
        /// <summary>
        /// Возвращает вес ребра.
        /// </summary>
        public readonly int Weight;
        /// <summary>
        /// Хранит вершину, из которой исходит это ребро.
        /// </summary>
        public readonly Node First;
        /// <summary>
        /// Хранит вершину, в которую ведёт это ребро.
        /// </summary>
        public readonly Node Second;

        /// <summary>
        /// Возвращает новое ребро, ведущее из первой указанной вершины во вторую.
        /// Для взвешеннных графов можно также указать вес ребра (если не указан, будет равен нулю).
        /// </summary>
        /// <param name="first">Вершина исхода</param>
        /// <param name="second">Вершина прихода</param>
        /// <param name="weight">Вес ребра</param>
        public Edge(Node first, Node second, int weight = default) 
        { 
            First = first; 
            Second = second; 
            Weight = weight; 
        }

        /// <summary>
        /// Возвращает true если указанная вершина инцидентна этому ребру.
        /// Иначе возвращает false.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public bool IsIncident(Node node) 
        { 
            return node == First || node == Second; 
        }

        /// <summary>
        /// Возвращает true, если ребро приходит в указанную вершину.
        /// Иначе возвращает false.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public bool IsGoingTo(Node node) 
        { 
            return node == Second;
        }

        /// <summary>
        /// Возвращает true, если ребро исходит из указанной вершины.
        /// Иначе возвращает false.
        /// </summary>
        /// <param name="node">Вершина для проверки</param>
        /// <returns></returns>
        public bool IsGoingFrom(Node node) 
        {
            return node == First;
        }

        /// <summary>
        /// Возвращает вершину, находящуюся на противоположном конце ребра от заданной вершины.
        /// Если заданная вершина не инцидентна ребру, будет выброшено исключение.
        /// </summary>
        /// <param name="node">Вершина для поиска противоположной</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">Указанная вершина не инцидентна этому ребру</exception>
        public Node OtherNode(Node node)
        {
            if (node == First) return Second;
            else if (node == Second) return First;
            else throw new ArgumentException();
        }
    }
}
