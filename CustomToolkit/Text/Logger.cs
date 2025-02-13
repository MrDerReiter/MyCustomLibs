using System;
using System.IO;

namespace CustomToolkit.Text
{
    /// <summary>
    /// Универсальный статический логгер. Поддерживает лгирование в консоль, 
    /// в текстовый файл; а также логирование с использованием пользовательской функции.
    /// Поведение можно (и нужно) настроить с помощью FluentAPI,
    /// начав с вызова метода BeginLog().
    /// </summary>
    public static class Logger
    {
        /// <summary>
        /// Настроечный обьект,позволяющий настроить по цепи способ логирования для 
        /// класса Logger, после вызова метода BeginLog().
        /// </summary>
        public class LoggerConfigurator
        {
            internal LoggerConfigurator() { }


            /// <summary>
            /// Настраивает логгер для записи в консоль.
            /// </summary>
            public void ToConsole()
            {
                _writeFunc = Console.WriteLine;
                Init();
            }

            /// <summary>
            /// Настраивает логгер для записи в файл. Опиционально можно указать путь к файлу.
            /// По умолчанию создаётся/перезаписывается файл Log.txt в текущей директории приложения.
            /// </summary>
            /// <param name="path">Путь к файлу для записи лога</param>
            public void ToFile(string path = "Log.txt")
            {
                _writer = new StreamWriter(path, false);
                _writeFunc = Console.WriteLine;
                Init();
            }

            /// <summary>
            /// Настраивает логгер для записи с использованием заданной функции.
            /// </summary>
            /// <param name="writeFunc">Делегат, реализующий запись</param>
            public void WithDelegate(Action<string> writeFunc)
            {
                _writeFunc = writeFunc;
                Init();
            }
        }


        private static bool _logIsBegan;
        private static bool LogIsNotBegan => !_logIsBegan;
        private static StreamWriter _writer;
        private static Action<string> _writeFunc;
        private static int _lineCounter;


        private static void Init()
        {
            _writeFunc("Начало логирования");
            AddSpacing();

            _lineCounter = 1;
            _logIsBegan = true;
        }

        private static void Dispose()
        {
            _writer?.Close();
            _writeFunc = null;
            _logIsBegan = false;
            _lineCounter = 0;
        }


        /// <summary>
        /// Активирует логгер и возвращает объект LoggerConfigurator для 
        /// дальнейшей настройки в стиле Method Chaining.
        /// Данный метод должен быть вызван перед использованием всех остальных.
        /// Вызов любого метода логгера без предварительного вызова BeginLog()
        /// ничего не делает; ровно как и вызов этого метода без использования дальнейшей настройки.
        /// </summary>
        public static LoggerConfigurator BeginLog()
        {
            return new LoggerConfigurator();
        }

        /// <summary>
        /// Завершает логирование, освобождает поток записи (если он был использован) 
        /// и сбрасывает все параметры. Рекомендуется вызвать в конце работы логгера,
        /// если работа самой программы на этом не закачивается.
        /// Для продолжения логирования после вызова этого метода, необходима 
        /// повторная инициализация через BeginLog().
        /// </summary>
        public static void EndLog()
        {
            if (LogIsNotBegan) return;

            AddSpacing();
            _writeFunc("Логирование завершено");

            Dispose();
        }

        /// <summary>
        /// Добаляет запись в лог (ведётся автоматическая нумерация записей).
        /// </summary>
        /// <param name="message"></param>
        public static void Log(string message)
        {
            if (LogIsNotBegan) return;
            _writeFunc($"{_lineCounter++}. {message}");
        }

        /// <summary>
        /// Добавляет пустую строку в лог; чтобы создать отступ между записями.
        /// </summary>
        public static void AddSpacing()
        {
            if (LogIsNotBegan) return;
            _writeFunc(Environment.NewLine);
        }
    }
}
