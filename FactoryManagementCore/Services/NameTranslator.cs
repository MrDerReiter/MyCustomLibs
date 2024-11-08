using FactoryManagementCore.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace FactoryManagementCore.Services
{
    /// <summary>
    /// Базовая реализация интерфейса INameTranslator,
    /// использующая словарь в формате текстового файла,
    /// размещаемого в исполняемом каталоге.
    /// </summary>
    public class NameTranslator : INameTranslator
    {
        private readonly Dictionary<string, string> _dict = InitializeDictionary("Dictionary.cfg");


        private static Dictionary<string, string> InitializeDictionary(string dictFilePath)
        {
            var dict = new Dictionary<string, string>();
            var entriesToAdd = File.ReadAllLines(dictFilePath, Encoding.Unicode);

            try
            {
                foreach (var entry in entriesToAdd)
                {
                    if (!entry.StartsWith('#') && !string.IsNullOrEmpty(entry))
                    {
                        var pair = entry.Split(" => ");
                        dict.Add(pair[0], pair[1]);
                    }
                }
            }
            catch(Exception ex)
            {
                throw new InvalidDataException
                    ("Ошибка при построении словаря для перевода строк. " +
                    "Проверьте корректность данных в файле .cfg", ex);
            }
            

            return dict;
        }

        /// <summary>
        /// Переводит строку названия станка, рецепта или ресурса из внутреннего программного представления
        /// в представление для показа в GUI
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public string Translate(string name)
        {
            return _dict[name];
        }
    }
}
