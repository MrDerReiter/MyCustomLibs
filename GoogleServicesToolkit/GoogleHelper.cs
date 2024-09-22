using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Http;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Util.Store;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;


namespace GoogleServicesToolkit
{
    /// <summary>
    /// Класс-обёртка для API-сервисов Google.
    /// Инкапсулирует процессы авторизации, доступа к таблицам и диску, чтения и записи данных, 
    /// позволяя абстрагироваться от большого количества сервисного кода 
    /// и работать с полученными данными более удобными методами.
    /// </summary>
    public class GoogleHelper
    {
        /// <summary>
        /// Рабочее имя приложения, через которое идёт обращение к API.
        /// Задаётся только через конструктор, как правило используется
        /// реальное имя приложения, которое использует API.
        /// </summary>
        public string AppName { get; }

        private readonly DriveService _driveService;
        private readonly SheetsService _sheetsService;
        private static readonly string[] _scopes = new string[]
        {
            DriveService.Scope.Drive,
            SheetsService.Scope.Spreadsheets
        };
        private IConfigurableHttpClientInitializer _credentials;
        private string _activeSpreadsheetID;
        private string _activeSheetName;
        private AuthType _authType;


        /// <summary>
        /// Создаёт новый обьект GoogleSheetsHelper с заданным названием приложения
        /// (переменная, необходимая для внутреннего использования API)
        /// и загружающий данные для авторизации из заданного файла .json.
        /// Также нужно указать тип авторизации для приложения (реквизиты авторизации в файле
        /// должны соответстовать данному типу авторизации).
        /// </summary>
        /// <param name="appName"></param>
        /// <param name="credentialFile"></param>
        /// <param name="authType"></param>
        public GoogleHelper(string appName, FileInfo credentialFile, AuthType authType)
        {
            AppName = appName;
            _authType = authType;
            _credentials = GetCredentialFromJsonFileAsync(credentialFile).Result;
            _driveService = InitializeDriveService();
            _sheetsService = InitializeSheetsService();
        }

        /// <summary>
        /// Создаёт новый обьект GoogleSheetsHelper с заданным названием приложения
        /// (переменная, необходимая для внутреннего использования API)
        /// и загружающий данные для авторизации из строки в формате .json.
        /// Также нужно указать тип авторизации для приложения (реквизиты авторизации в строке
        /// должны соответстовать данному типу авторизации).
        /// </summary>
        /// <param name="appName"></param>
        /// <param name="jsonContent"></param>
        /// <param name="authType"></param>
        public GoogleHelper(string appName, string jsonContent, AuthType authType)
        {
            AppName = appName;
            _authType = authType;
            _credentials = GetCredentialFromJsonAsync(jsonContent).Result;
            _driveService = InitializeDriveService();
            _sheetsService = InitializeSheetsService();
        }


        private DriveService InitializeDriveService()
        {
            var service = new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = _credentials,
                ApplicationName = AppName
            });

            return service;
        }

        private SheetsService InitializeSheetsService()
        {
            var service = new SheetsService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = _credentials,
                ApplicationName = AppName
            });

            return service;
        }

        private async Task<IConfigurableHttpClientInitializer> GetCredentialFromJsonFileAsync(FileInfo jsonFile)
        {
            switch (_authType)
            {
                case AuthType.AsServiceAccount:
                    using (var stream = jsonFile.OpenRead())
                    {
                        var accountCredential = GoogleCredential.FromStream(stream).CreateScoped(_scopes);
                        return accountCredential;
                    }

                case AuthType.AsUser:
                    var userCredential =
                        await GoogleWebAuthorizationBroker.AuthorizeAsync(
                        GoogleClientSecrets.FromFile(jsonFile.FullName).Secrets, _scopes, "user",
                        CancellationToken.None, new FileDataStore("CredentialData"));
                    return userCredential;

                default:
                    throw new ArgumentException($"Недопустимое значение параметра AuthType: {_authType}");
            }
        }

        private async Task<IConfigurableHttpClientInitializer> GetCredentialFromJsonAsync(string jsonContent)
        {
            switch (_authType)
            {
                case AuthType.AsServiceAccount:

                    var accountCredential = GoogleCredential.FromJson(jsonContent).CreateScoped(_scopes);
                    return accountCredential;

                case AuthType.AsUser:

                    using (var stream = new MemoryStream())
                    {
                        var writer = new StreamWriter(stream);
                        writer.Write(jsonContent);
                        writer.Flush();

                        var userCredential =
                            await GoogleWebAuthorizationBroker.AuthorizeAsync(
                            GoogleClientSecrets.FromStream(stream).Secrets, _scopes, "user",
                            CancellationToken.None, new FileDataStore("CredentialData"));

                        return userCredential;
                    }

                default:
                    throw new ArgumentException($"Недопустимое значение параметра AuthType: {_authType}");
            }
        }

        private void CheckInitialized(bool checkSheet)
        {
            if (checkSheet)
            {
                if (string.IsNullOrEmpty(_activeSheetName) || string.IsNullOrEmpty(_activeSpreadsheetID))
                    throw new InvalidOperationException(
                        "Не установлен ID таблицы и/или название вкладки. " +
                        "Сначала проинициализируйте обьект GoogleHelper методами SetSpreadsheet/SetSheet");
            }
            else if (string.IsNullOrEmpty(_activeSpreadsheetID))
                throw new InvalidOperationException(
                        "Не установлен ID таблицы. " +
                        "Сначала проинициализируйте обьект GoogleHelper методом SetSpreadsheet");
        }

        private DataTable ParseRange(IList<IList<object>> range)
        {
            var table = new DataTable();
            var columnsCount = range.Select(list => list.Count()).Max();

            for (int i = 0; i < columnsCount; i++)
                table.Columns.Add();

            foreach (var list in range)
                table.Rows.Add(list);

            return table;
        }


        /// <summary>
        /// Устанавливает таблицу, которая будет использоваться в последующих запросах. 
        /// Таблица определяется по её видимому названию, под которым она хранится на GoogleDrive. 
        /// Не работает при авторизации через сервисный аккаунт (будет выброшено исключение).
        /// Поддерживает Method Chaining.
        /// </summary>
        /// <param name="spreadsheetName"></param>
        /// <returns></returns>
        public GoogleHelper SetSpreadsheet(string spreadsheetName)
        {
            if (_authType != AuthType.AsUser)
                throw new InvalidOperationException
                    ("Невозможно найти таблицу по её названию при текущем типе авторизации, " +
                    "т.к. нет доступа к GoogleDrive конкретного клиента. " +
                    "Требуется тип авторизации AuthType.AsUser либо установка таблицы по её ID " +
                    "через соответсвующий метод.");

            var response = _driveService.Files.List().Execute();

            _activeSpreadsheetID =
                response.Files
                .First(file => file.Name == spreadsheetName).Id;
            return this;
        }

        /// <summary>
        /// Устанавливает таблицу, которая будет использоваться в последующих запросах. 
        /// Таблица определяется по её ID (отображается, например, в адресной строке браузера), 
        /// под которым она хранится на GoogleDrive владельца.
        /// Данный метод используется как альтернатива SetSpreadsheet при работе
        /// через сервисный аккаунт, когда нет доступа к GoogleDrive клиента.
        /// Поддерживает Method Chaining.
        /// </summary>
        /// <param name="spreadsheetID"></param>
        /// <returns></returns>
        public GoogleHelper SetSpreadsheetID(string spreadsheetID)
        {
            _activeSpreadsheetID = spreadsheetID;
            return this;
        }

        /// <summary>
        /// Устанавливает лист таблицы, который будет использоваться в последующих запросах. 
        /// Лист определяется по его видимому названию в таблице. Поддерживает Method Chaining.
        /// </summary>
        /// <param name="sheetName"></param>
        /// <returns></returns>
        public GoogleHelper SetSheet(string sheetName)
        {
            _activeSheetName = sheetName;
            return this;
        }

        /// <summary>
        /// Получает данные из прямоугольного диапазона ячеек таблицы, заданного 
        /// индексами верхней левой и правой нижней ячейки диапазона в том-же формате, что используется 
        /// непосредственно в таблице (A1, B4 и т.п.). 
        /// Данные возвращаются в виде объекта System.Data.DataTable. 
        /// Перед запросом обязательно нужно выбрать таблицу и лист, 
        /// из которых нужно получить данные (методы SetSpreadsheet/SetSheet), 
        /// в противном случае метод выбросит исключение. Также исключение будет выброшено при 
        /// некорректных вводных данных.
        /// </summary>
        /// <param name="startCellID"></param>
        /// <param name="endCellID"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public DataTable GetFromRange(string startCellID, string endCellID)
        {
            CheckInitialized(true);

            string range = $"{_activeSheetName}!{startCellID}:{endCellID}";
            var request = _sheetsService.Spreadsheets.Values.Get(_activeSpreadsheetID, range);

            var response = request.Execute();

            return ParseRange(response.Values);
        }

        /// <summary>
        /// Возвращает содержимое указанной ячейки в виде строкового значения. 
        /// Индекс ячейки указывается в стандартом формате, непосредственно 
        /// отображаемом в Google-таблицах (например A1, B4 и т.п.).
        /// Перед запросом обязательно нужно выбрать таблицу и лист, 
        /// из которых нужно получить данные (методы SetSpreadsheet/SetSheet), 
        /// в противном случае метод выбросит исключение. Также исключение будет выброшено при 
        /// некорректных вводных данных.
        /// </summary>
        /// <param name="cellID"></param>
        /// <returns></returns>
        public string GetFromCell(string cellID)
        {
            CheckInitialized(true);

            string range = $"{_activeSheetName}!{cellID}";
            var request = _sheetsService.Spreadsheets.Values.Get(_activeSpreadsheetID, range);
            var response = request.Execute();

            return response.Values[0][0].ToString();
        }

        /// <summary>
        /// Записывает строку в указанную ячейку и возвращает true в случае успешной записи. 
        /// Индекс ячейки указывается в стандартом формате, непосредственно 
        /// отображаемом в Google-таблицах (например A1, B4 и т.п.).
        /// Перед запросом обязательно нужно выбрать таблицу и лист, 
        /// в ячейку которого нужно записать данные (методы SetSpreadsheet/SetSheet), 
        /// в противном случае метод выбросит исключение. Также исключение будет выброшено при 
        /// некорректном индексе яейки.
        /// </summary>
        /// <param name="content"></param>
        /// <param name="cellID"></param>
        public bool Insert(string content, string cellID)
        {
            CheckInitialized(true);

            string range = $"{_activeSheetName}!{cellID}";
            var list = new List<List<object>>() { new List<object>() { content } };
            var body = new ValueRange() { Values = new List<IList<object>>(list) };

            var request = _sheetsService.Spreadsheets.Values.Update(body, _activeSpreadsheetID, range);
            request.ValueInputOption =
                SpreadsheetsResource
                .ValuesResource
                .UpdateRequest
                .ValueInputOptionEnum.USERENTERED;

            var response = request.Execute();

            return response.UpdatedCells == 1;
        }

        /// <summary>
        /// Заносит данные в указанный дипазон таблицы используя указанный объект
        /// TableBuilder соответсвующий подмножеству ячеек этого диапазона и возвращает 
        /// true, если GoogleAPI подтвердил добавление в таблицу искомого количества значений 
        /// (пустые ячейки в конце любого ряда не считаются).
        /// Диапазон задаётся  индексами верхней левой и правой нижней ячейки диапазона 
        /// в том-же формате, что используется непосредственно в таблице (A1, B4 и т.п.). 
        /// Объект TableBuilder должен соответствовать указанному диапазону 
        /// (иметь то-же количество/длинну строк и столбцов), в противном случае при выполнении 
        /// запроса к GoogleAPI будет выброшено исключение.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="startCellID"></param>
        /// <param name="endCellID"></param>
        /// <returns></returns>
        public bool InsertRange(DataTable data, string startCellID, string endCellID)
        {
            CheckInitialized(true);

            string range = $"{_activeSheetName}!{startCellID}:{endCellID}";
            var list = new List<List<object>>();
            int counter = 0;
            foreach (DataRow item in data.Rows)
            {
                list.Add(new List<object>(item.ItemArray));
                counter += item.ItemArray.Length;
            }

            var body = new ValueRange() { Values = new List<IList<object>>(list) };

            var request = _sheetsService.Spreadsheets.Values.Update(body, _activeSpreadsheetID, range);
            request.ValueInputOption =
                SpreadsheetsResource
                .ValuesResource
                .UpdateRequest
                .ValueInputOptionEnum.USERENTERED;

            var response = request.Execute();

            return response.UpdatedCells == counter;
        }

        /// <summary>
        /// Сохраняет выбранную таблицу целиком в локальный файл .xlsx,
        /// по указанному пути (с расширением файла). 
        /// Если указано только имя файла, он
        /// будет сохранён в исполняемом каталоге приложения.
        /// </summary>
        /// <param name="path"></param>
        public void DownloadToFile(string path)
        {
            CheckInitialized(false);

            var fileMetadata = _driveService.Files.Get(_activeSpreadsheetID).Execute();
            using (var fileStream = new FileStream(path, FileMode.Create, FileAccess.Write))
            {
                if (fileMetadata.MimeType == "application/vnd.google-apps.spreadsheet")
                {
                    // Если обычная таблица Google
                    var request = _driveService.Files.Export(_activeSpreadsheetID, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
                    request.Download(fileStream);
                }
                else if (fileMetadata.MimeType == "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
                {
                    // Если файл в формате XLSX
                    var request = _driveService.Files.Get(_activeSpreadsheetID);
                    request.Download(fileStream);
                }
                else throw new InvalidOperationException("Неопознанный формат загружаемой таблицы.");

                fileStream.Flush();
            }
        }

        /// <summary>
        /// Сохраняет таблицу целиком в формате .xlsx в MemoryStream и возвращает целевой поток.
        /// </summary>
        /// <returns></returns>
        public MemoryStream DownloadToStream()
        {
            CheckInitialized(false);

            var fileMetadata = _driveService.Files.Get(_activeSpreadsheetID).Execute();
            var memoryStream = new MemoryStream();


            if (fileMetadata.MimeType == "application/vnd.google-apps.spreadsheet")
            {
                // Если обычная таблица Google
                var request = _driveService.Files.Export(_activeSpreadsheetID, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
                request.Download(memoryStream);
            }
            else if (fileMetadata.MimeType == "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
            {
                // Если файл в формате XLSX
                var request = _driveService.Files.Get(_activeSpreadsheetID);
                request.Download(memoryStream);
            }
            else throw new InvalidOperationException("Неопознанный формат загружаемой таблицы.");

            return memoryStream;
        }
    }
}
