using System;
using System.IO;

namespace CustomToolkit.Text
{
    /// <summary>
    /// Простой статический логгер, записывающий лог в текстовый файл
    /// в текущей директории приложения.
    /// </summary>
    public static class Logger
    {
        private static readonly string _path = 
            Environment.CurrentDirectory + "\\log.txt";

        private static bool LogIsNotBegan => _writer is null;
        private static StreamWriter _writer;
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
        /// Завершает логирование и освобождает поток записи.
        /// Рекомендуется вызвать в конце работы логгера,
        /// если работа самой программы на этом не закачивается.
        /// </summary>
        public static void EndLog()
        {
            if (LogIsNotBegan) return;

            AddSpacing();
            _writer.WriteLine("Логирование завершено");
            _writer.Close();
            _writer = null;
        }

        /// <summary>
        /// Добаляет запись в лог (ведётся автоматическая нумерация записей).
        /// </summary>
        /// <param name="message"></param>
        public static void Log(string message)
        {
            if (LogIsNotBegan) return;

            _writer.WriteLine(_lineCounter + ". " + message);
            _lineCounter++;
        }

        /// <summary>
        /// Добавляет пустую строку в лог; чтобы создать отступ между записями.
        /// </summary>
        public static void AddSpacing()
        {
            if (LogIsNotBegan) return;

            _writer.WriteLine();
        }
    }
}
