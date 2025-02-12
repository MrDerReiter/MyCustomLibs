using System;
using System.IO;

namespace CustomToolkit.Text
{
    /// <summary>
    /// Простой статический логгер, записывающий лог в текстовый файл
    /// в текущей директории приложения (поведение по умолчанию);
    /// либо использующий для записи заданную функцию.
    /// Поведение можно настроить при вызове метода BeginLog().
    /// </summary>
    public static class Logger
    {
        private static readonly string _path =
            Environment.CurrentDirectory + "\\log.txt";

        private static bool LogIsNotBegan => _writerFunc is null && _writer is null;
        private static bool UsingDelegate => _writerFunc != null;
        private static StreamWriter _writer;
        private static Action<string> _writerFunc;
        private static int _lineCounter;


        /// <summary>
        /// Инициализирует счётчик записей; открывает поток для записи в текстовый файл.
        /// Данный метод должен быть вызван перед использованием всех остальных.
        /// Вызов любого метода логгера без предварительного вызова BeginLog()
        /// ничего не делает.
        /// </summary>
        public static void BeginLog()
        {
            _writer = new StreamWriter(_path, false);

            _writer.WriteLine("Начало логирования");
            AddSpacing();

            _lineCounter = 1;
        }

        /// <summary>
        /// Альтернативная версия инициализации логгера, позволяющая
        /// использовать для логирования заданную функцию записи;
        /// если реализация по умолчанию (запись в текстовый файл) не подходит.
        /// Также как и реализация по умолчанию должна быть вызвана 
        /// до любых других методов логгера.
        /// </summary>
        /// <param name="writeHandler">Делегат, реализующий запись в лог</param>
        public static void BeginLog(Action<string> writeHandler)
        {
            _writerFunc = writeHandler;

            _writerFunc("Начало логирования");
            AddSpacing();

            _lineCounter = 1;
        }

        /// <summary>
        /// Завершает логирование и освобождает поток записи.
        /// Рекомендуется вызвать в конце работы логгера,
        /// если работа самой программы на этом не закачивается.
        /// </summary>
        public static void EndLog()
        {
            if (LogIsNotBegan) return;

            AddSpacing();
            if(UsingDelegate)
            {
                _writerFunc("Логирование завершено");
                _writerFunc = null;
            }
            else
            {
                _writer.WriteLine("Логирование завершено");
                _writer.Close();
                _writer = null;
            }
        }

        /// <summary>
        /// Добаляет запись в лог (ведётся автоматическая нумерация записей).
        /// </summary>
        /// <param name="message"></param>
        public static void Log(string message)
        {
            if (LogIsNotBegan) return;

            if (UsingDelegate) _writerFunc(_lineCounter + ". " + message);
            else _writer.WriteLine(_lineCounter + ". " + message);

            _lineCounter++;
        }

        /// <summary>
        /// Добавляет пустую строку в лог; чтобы создать отступ между записями.
        /// </summary>
        public static void AddSpacing()
        {
            if (LogIsNotBegan) return;

            if (UsingDelegate) _writerFunc("\n");
            else _writer.WriteLine();
        }
    }
}
