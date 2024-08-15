using System;
using System.Collections.Generic;
using System.Linq;

namespace GoogleServicesToolkit
{
    /// <summary>
    /// Инкапсулирует ответ на запрос получения данных из таблицы Google. 
    /// Представляет неизменяемую таблицу со строковыми данными, 
    /// и соответствующие методы доступа к ним. 
    /// Её экземпляр нельзя создать напрямую, только получить в ответ на запрос.
    /// </summary>
    public class TableObject
    {
        /// <summary>
        /// Инкапсулирует внутренний двухуровневый список строковых значений, для более удобного
        /// обращения к нему через публичные методы и индексаторы как к таблице.
        /// </summary>
        protected readonly List<List<string>> _content;

        /// <summary>
        /// Возвращает true, если в таблице есть хотя-бы одна строка и хотя-бы один столбец,
        /// в противном случае возвращает false.
        /// </summary>
        public bool IsNotEmpty
        {
            get
            {
                if (_content.Count == 0) return false;
                else
                {
                    foreach (var item in _content)
                        if (item.Count > 0) return true;
                }
                return false;
            }
        }
        /// <summary>
        /// Количество строк в таблице.
        /// </summary>
        public int RowsCount => _content.Count;
        /// <summary>
        /// Количество столбцов в таблице.
        /// </summary>
        public int ColumnsCount => IsNotEmpty ? MaxRowLength() : 0;
        /// <summary>
        /// Возвращает все ряды таблицы в виде последовательности ReadOnly-списков.
        /// </summary>
        public IEnumerable<IReadOnlyList<string>> Rows
        {
            get
            {
                foreach (var item in _content)
                    yield return item;
            }
        }
        /// <summary>
        /// Возвращает все столбцы таблицы в виде последовательности ReadOnly-списков.
        /// </summary>
        public IEnumerable<IReadOnlyList<string>> Columns 
        { 
            get
            {
                for (int i = 0; i < MaxRowLength(); i++)
                    yield return GetColumn(i);
            }
        }

        /// <summary>
        /// Возвращает содержимое указанной ячеки 
        /// (в формате [номер строки, номер столбца]) в виде строки.
        /// Если ячейка была пустой, возвращает пуcтую строку. 
        /// Если хотя-бы один из индексов находится за пределами диапазона ячеек таблицы, 
        /// будет выброшено исключение.
        /// </summary>
        /// <param name="row"></param>
        /// <param name="column"></param>
        /// <returns></returns>
        public string this[int row, int column]
        {
            get
            {
                if (row < 0 || row >= RowsCount ||
                    column < 0 || column >= ColumnsCount)
                    throw new ArgumentException("Индекс выходит за пределы таблицы");

                if (HasValue(row, column)) return _content[row][column];
                else return string.Empty;
            }
        }
        /// <summary>
        /// Возвращает целиком указанную строку таблицы в виде списка значений (только для чтения).
        /// Пустые ячейки в конце строки будут отброшены; но при наличии пустых ячеек между ячейками 
        /// с данными, в список вместо них будут добавлены пустые строки.
        /// Выбрасывает исключение, если в таблице нет строки с таким индексом.
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        public IReadOnlyList<string> this[int row]
        {
            get => GetRow(row);
        }


        /// <summary>
        /// Создаёт новую таблицу TableOject из обьекта IList(IList(object)), 
        /// который обычно возвращает GoogleAPI в ответ на запрос.
        /// Предназначен только для внутреннего использования в классе GoogleSheetsHelper.
        /// </summary>
        /// <param name="sourceSheet"></param>
        internal TableObject(IList<IList<object>> sourceSheet)
        {
            _content = new List<List<string>>(sourceSheet.Count);

            for (int i = 0; i < sourceSheet.Count; i++)
            {
                _content.Add(new List<string>(sourceSheet[i].Count));

                for (int j = 0; j < sourceSheet[i].Count; j++)
                    _content[i].Add(sourceSheet[i][j].ToString());
            }
        }

        /// <summary>
        /// Технический конструктор, предназначенный только для удобства наследования. 
        /// Недоступен для прямого вызова.
        /// </summary>
        protected internal TableObject()
        {
            _content = new List<List<string>>();
        }


        /// <summary>
        /// Возвращает true, если указанные координаты [строка/столбец] 
        /// действительно представленны элементом внутреннего списка 
        /// (при обращении к ним не произойдёт IndexOutOfRangeExeption).
        /// </summary>
        /// <param name="row"></param>
        /// <param name="column"></param>
        /// <returns></returns>
        protected bool HasValue(int row, int column)
        {
            return row >= 0 && row < _content.Count &&
                column >= 0 && column < _content[row].Count;
        }

        private bool IsValidRow(int row)
        {
            return row >= 0 && row < _content.Count;
        }

        private bool IsValidColumn(int column)
        {
            return column >= 0 && column < MaxRowLength();
        }

        /// <summary>
        /// Возвращает длину самого длинного внутреннего списка, которую можно интерпретировать 
        /// как ширину таблицы в столбцах. Значение столбца больше или равное этому 
        /// однозначно будет находиться за пределами таблицы. Меньшее значение 
        /// может быть интерпретировано как находящееся в пределах таблицы, но фактически в 
        /// некоторых списках элемента соответствующего этому столбцу может и не быть.
        /// </summary>
        /// <returns></returns>
        protected int MaxRowLength()
        {
            return _content.Select(item => item.Count).Max();
        }


        /// <summary>
        /// Возвращает целиком указанную строку таблицы в виде списка значений (только для чтения). 
        /// Пустые ячейки в конце строки будут отброшены; но при наличии пустых ячеек между ячейками 
        /// с данными, в список вместо них будут добавлены пустые строки.
        /// Выбрасывает исключение, если в таблице нет строки с таким индексом.
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        public IReadOnlyList<string> GetRow(int row)
        {
            if (!IsValidRow(row)) 
                throw new ArgumentOutOfRangeException("Строки с таким индексом нет в таблице.");

            return new List<string>(_content[row]);
        }

        /// <summary>
        /// Возвращает целиком указанный столбец таблицы в виде списка значений (только для чтения).
        /// Пустые ячейки внизу столбца будут отброшены; но при наличии пустых ячеек 
        /// между ячейками с данными, в список вместо них будут добавлены пустые строки.
        /// Если в таблице нет такого столбца, будет выброшено исключение.
        /// </summary>
        /// <param name="column"></param>
        /// <returns></returns>
        public IReadOnlyList<string> GetColumn(int column)
        {
            if (!IsValidColumn(column))
                throw new ArgumentOutOfRangeException("Столбца с таким индексом нет в таблице.");

            var result = new List<string>();

            foreach (var row in _content)
            {
                if (HasValue(_content.IndexOf(row), column))
                    result.Add(row[column]);
                else result.Add(string.Empty);
            }

            //Обрезка лишних пустых строк конце списка, с охранением таковых внутри него
            for (int i = result.Count - 1; i >= 0; i--)
            {
                if (string.IsNullOrEmpty(result[i]))
                    result.RemoveAt(i);
                else break;
            }

            return result;
        }
    }
}
