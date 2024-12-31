using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace CustomToolkit.Text
{
    /// <summary>
    /// Сервисный статический класс, осуществляющий проверку и вычисление
    /// строковых выражений бинарной математической операции.
    /// </summary>
    public static class RegexCalculator
    {
        /// <summary>
        /// Принимает в качестве аргумента строку с простым математическим выражением
        /// сложения, вычитания, деления или умножения (x + y, x / y и т.п.)
        /// и возвращает строку с результатом вычисления этого выражения,
        /// если это возможно. Выбрасывает исключение при передаче синтаксически некорректного
        /// выражения, а также выражения деления на ноль.
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        /// <exception cref="DivideByZeroException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        public static string Calculate(string expression)
        {
            double leftNumber = double.Parse
                (Regex.Match(expression, @"^\s*\d*\.?\d*").Value, CultureInfo.InvariantCulture);

            double rightNumber = double.Parse
                (Regex.Match(expression, @"\d*\.?\d*\s*$").Value, CultureInfo.InvariantCulture);

            string operation = Regex.Match(expression, @"[-+*/]").Value;


            double total;
            switch (operation)
            {
                case "+":
                    total = leftNumber + rightNumber; break;
                case "-":
                    total = leftNumber - rightNumber; break;
                case "*":
                    total = leftNumber * rightNumber; break;
                case "/":
                    total = rightNumber != 0 ? leftNumber / rightNumber
                        : throw new DivideByZeroException(); break;

                default: throw new InvalidOperationException("Неопознанная операция в выражении.");
            }

            return total.ToString("0.###", CultureInfo.InvariantCulture);
        }
    }
}
