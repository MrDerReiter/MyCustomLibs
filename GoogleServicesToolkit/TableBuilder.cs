using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoogleServicesToolkit
{
    /// <summary>
    /// Представляет настраиваемую таблицу со строковыми данными, 
    /// в которую можно добавлять новые данные или удалять уже имеющиеся. 
    /// В остальном аналогична таблице TableObject и наследует весь её функционал для чтения.
    /// Используется для предварительной группировки данных перед отправкой 
    /// запроса через метод InsertRange класса GoogleHelper, чтобы минимизировать 
    /// количество запросов и связанные с ними издержки производительности.
    /// </summary>
    public sealed class TableBuilder : TableObject
    {
        /// <summary>
        /// Создаёт пустую таблицу, которую можно последовательно заполнять данными.
        /// </summary>
        public TableBuilder() { }

        /// <summary>
        /// Создаёт таблицу и сразу-же формирует в ней первый ряд и 
        /// соответствующие колонки переданными в параметры строками.
        /// </summary>
        /// <param name="initialStrings"></param>
        public TableBuilder(params string[] initialStrings)
            : this((IEnumerable<string>)initialStrings) { }

        /// <summary>
        /// Создаёт таблицу и сразу-же формирует в ней первый ряд и соответствующие колонки
        /// переданной в параметре строковой последовательностью.
        /// </summary>
        /// <param name="initialRow"></param>
        public TableBuilder(IEnumerable<string> initialRow)
        {
            _content.Add(initialRow.ToList());
        }


        /// <summary>
        /// Добавляет в таблицу новый ряд из указанной строковой последовательности. 
        /// Если длина последовательности превышает уже имеющееся количество колонок, 
        /// новые колонки будут добавлены автоматически, а уже имеющиеся ряды 
        /// будут добиты пустыми строками.
        /// </summary>
        /// <param name="list"></param>
        public void AddRow(IEnumerable<string> list)
        {
            _content.Add(list.ToList());
        }

        /// <summary>
        /// Добавляет в таблицу новый ряд из перечисленных строк. 
        /// Если длина последовательности превышает уже имеющееся количество колонок, 
        /// новые колонки будут добавлены автоматически, а уже имеющиеся ряды 
        /// будут добиты пустыми строками.
        /// </summary>
        /// <param name="strings"></param>
        public void AddRow(params string[] strings)
        {
            _content.Add(strings.ToList());
        }

        /// <summary>
        /// Удаляет из таблицы указанный ряд по индексу, если он сущетвует.
        /// Возвращает true, если удаление прошло успешно.
        /// </summary>
        /// <param name="row"></param>
        public bool RemoveRow(int row)
        {
            try
            {
                _content.RemoveAt(row);
                return true;
            }
            catch { return false; }
        }

        /// <summary>
        /// Очищает указанную ячейку таблицы, если она существует 
        /// (в ячейке после очистки будет пустая строка). Возвращает true 
        /// если данные были успешно удалены.
        /// </summary>
        /// <param name="row"></param>
        /// <param name="column"></param>
        /// <returns></returns>
        public bool ClearCell(int row, int column)
        {
            try
            {
                if (column == _content[row].Count - 1)
                {
                    _content[row].RemoveAt(column);
                    return true;
                }
                else if(HasValue(row, column))
                {
                    _content[row][column] = string.Empty;
                    return true;
                }
                else if(column < MaxRowLength()) return true;
                else return false;
            }
            catch { return false; }
        }

        /// <summary>
        /// Помещает указанное строковое значение в ячейку таблицы с указанными координатами, 
        /// если она есть. Возвращает true если запись прошла успешно.
        /// </summary>
        /// <param name="content"></param>
        /// <param name="row"></param>
        /// <param name="column"></param>
        /// <returns></returns>
        public bool Insert(string content, int row, int column)
        {
            try
            {
                if (HasValue(row, column))
                {
                    _content[row][column] = content;
                    return true;
                }
                else if (column < MaxRowLength())
                {
                    while (_content[row].Count < column)
                        _content[row].Add(string.Empty);
                    _content[row].Add(content);
                    return true;
                }
                else return false;
            }
            catch { return false; }
        }
    }
}
