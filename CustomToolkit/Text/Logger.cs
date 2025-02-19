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
            public LoggerConfigurator ToConsole()
            {
                _writeFunc = Console.WriteLine;
                _logInProgress = true;
                return this;
            }

            /// <summary>
            /// Настраивает логгер для записи в файл. Опиционально можно указать путь к файлу.
            /// По умолчанию создаётся/перезаписывается файл Log.txt в текущей директории приложения.
            /// </summary>
            /// <param name="path">Путь к файлу для записи лога</param>
            public LoggerConfigurator ToFile(string path = "Log.txt")
            {
                _writer = new StreamWriter(path, false);
                _writeFunc = Console.WriteLine;
                _logInProgress = true;
                return this;
            }

            /// <summary>
            /// Настраивает логгер для записи с использованием заданной функции.
            /// </summary>
            /// <param name="writeFunc">Делегат, реализующий запись</param>
            public LoggerConfigurator WithDelegate(Action<string> writeFunc)
            {
                _writeFunc = writeFunc;
                _logInProgress = true;
                return this;
            }

            /// <summary>
            /// Запускает или отключает счётчик строк в логе (добавление номера записи лога в её начале).
            /// По умолчанию строки не нумеруются. После вызова нумерация 
            /// всегда начинается с единицы. При передаче false в качестве аргумента 
            /// нумерация прекращается (если была активна), и при следующей активации 
            /// счётчик будет сброшен.
            /// </summary>
            /// <param name="isCounting">Указывает, нужно-ли включить нумерацию, или отключить её. 
            /// Если параметр не указан явно, будет true.</param>
            /// <returns></returns>
            public LoggerConfigurator CountLines(bool isCounting = true)
            {
                Logger.CountLines(isCounting);
                return this;
            }
        }


        private static bool _logInProgress;
        private static bool LogIsNotBegan => !_logInProgress;
        private static StreamWriter _writer;
        private static Action<string> _writeFunc;
        private static bool _isCounting;
        private static uint _lineCounter;


        private static void Dispose()
        {
            _writer?.Close();
            _writeFunc = default;
            _logInProgress = false;
            _lineCounter = default;
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
            Dispose();
        }

        /// <summary>
        /// Добаляет запись в лог (ведётся автоматическая нумерация записей).
        /// </summary>
        /// <param name="message"></param>
        public static void Log(string message)
        {
            if (LogIsNotBegan) return;
            _writeFunc($"{(_isCounting ? _lineCounter++ + ". " : "")}{message}");
        }

        /// <summary>
        /// Добавляет пустую строку в лог; чтобы создать отступ между записями.
        /// </summary>
        public static void AddSpacing()
        {
            if (LogIsNotBegan) return;
            _writeFunc(Environment.NewLine);
        }

        /// <summary>
        /// Запускает или отключает счётчик строк в логе (добавление номера записи лога в её начале).
        /// По умолчанию строки не нумеруются. После вызова нумерация 
        /// всегда начинается с единицы. При передаче false в качестве аргумента 
        /// нумерация прекращается (если была активна), и при следующей активации 
        /// счётчик будет сброшен.
        /// </summary>
        /// <param name="isCounting">Указывает, нужно-ли включить нумерацию, или отключить её. 
        /// Если параметр не указан явно, будет true.</param>
        public static void CountLines(bool isCounting = true)
        {
            _lineCounter = isCounting ? 1U : default;
            _isCounting = isCounting;
        }
    }
}
