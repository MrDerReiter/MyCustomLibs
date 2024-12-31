using System;
using System.Collections.Generic;

namespace CustomToolkit.Validation
{
    /// <summary>
    /// Универсальный обобщённый валидатор с поддержкой FluentAPI. 
    /// Может быть использован в качестве базового класса для специальных валидаторов 
    /// с предустановленной логикой;
    /// или использоваться как есть, с кастомизацией через пользовательские делегаты 
    /// в клиентском коде.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Validator<T>
    {
        private readonly Dictionary<string, Func<T, bool>> _validations;
        private T _savedValue;

        /// <summary>
        /// Возвращает результат всех предыдущих проверок методом Validate(). Если все проверки пройдены успешно,
        /// вернёт true. Если хотя-бы одна проверка не прошла или ни одной проверки 
        /// ещё не было проведено, вернёт false.
        /// </summary>
        public bool IsCorrect { get; protected set; }


        /// <summary>
        /// Базовый конструктор без параметров.
        /// </summary>
        public Validator()
        {
            _validations = new Dictionary<string, Func<T, bool>>();
            OnAddingValidations();
        }

        /// <summary>
        /// Альтернативный конструктор для вызова в классах-наследниках. 
        /// Позволяет не инициализировать контрольный словарь с делегатами 
        /// (указать false в качестве параметра), 
        /// если он не нужен для реализации специфической 
        /// логики конкретного валидатора.
        /// </summary>
        /// <param name="createDictionary"></param>
        protected Validator(bool createDictionary)
        {
            if (createDictionary)
            {
                _validations = new Dictionary<string, Func<T, bool>>();
                OnAddingValidations();
            }
        }


        /// <summary>
        /// Метод для переопределения в классах-наследниках. Вызывается в конструкторе 
        /// базового класса при создании экземпляра.
        /// Можно использовать для предварительного добавления делегатов 
        /// в контрольный словарь с помощью метода AddValidation(), 
        /// чтобы не перегружать ими клиентский код.
        /// Реализация по умолчанию ничего не делает.
        /// </summary>
        protected virtual void OnAddingValidations() { }


        /// <summary>
        /// Проверяет указанный обьект, используя именованную функцию проверки.
        /// Функция проверки должна быть предварительно добавлена через метод AddValidation(). 
        /// Поддерживает MethodChaining с помощью дополнительных перегрузок.
        /// Если соответствующая функция не найдена, будет выброшено исключение.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="funcName"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public Validator<T> Validate(T value, string funcName)
        {
            if (!_validations.ContainsKey(funcName))
                throw new ArgumentException
                    ($"Отсутствует делегат для указанной проверки: {funcName}");

            IsCorrect = _validations[funcName].Invoke(value);
            _savedValue = value;
            return this;
        }

        /// <summary>
        /// Продолжает проверку изначального объекта в стиле MethodChaining, 
        /// используя следующую именованную функцию проверки.
        /// Данная перегрузка метода должна быть вызвана строго после перегрузки, 
        /// которая принимает объект типа Т в качестве параметра (иначе проверять будет нечего).
        /// Выбрасывает исключение, если нет объекта проверки или не найдена 
        /// соответствующая функция проверки.
        /// </summary>
        /// <param name="funcName"></param>
        /// <returns></returns>
        /// <exception cref="NullReferenceException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public Validator<T> Validate(string funcName)
        {
            if (_savedValue == null)
                throw new NullReferenceException
                    ("Нет объекта для проверки. Проверка должна начинаться с перегрузки метода " +
                     $"Validate(), который принимает объект типа {typeof(T)} в качестве параметра.");

            else if (!_validations.ContainsKey(funcName))
                throw new ArgumentException
                    ($"Отсутствует делегат для указанной проверки: {funcName}");

            if (!IsCorrect) return this;

            IsCorrect = _validations[funcName].Invoke(_savedValue);
            return this;
        }

        /// <summary>
        /// Добавляет именованную реализацию определённой проверки обьекта типа T, 
        /// чтобы её можно было вызвать в методе Validate. 
        /// Реализация должна быть делегатом, принимающим параметр типа T и 
        /// возвращающим bool (true если проверка пройдена, иначе false).
        /// </summary>
        /// <param name="funcName"></param>
        /// <param name="validation"></param>
        public void AddValidation(string funcName, Func<T, bool> validation)
        {
            _validations.Add(funcName, validation);
        }
    }
}