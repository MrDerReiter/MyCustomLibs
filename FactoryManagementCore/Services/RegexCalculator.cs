using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace FactoryManagementCore.Services
{
    public static class RegexCalculator
    {
        public static string Calculate(string expression)
        {
            var leftNumber = double.Parse(Regex.Match(expression, @"^\s*\d*\.?\d*").Value, CultureInfo.InvariantCulture);
            var rightNumber = double.Parse(Regex.Match(expression, @"\d*\.?\d*\s*$").Value, CultureInfo.InvariantCulture);
            var operation = Regex.Match(expression, @"[-+*/]").Value;

            var total = operation switch
            {
                "+" => leftNumber + rightNumber,
                "-" => leftNumber - rightNumber,
                "*" => leftNumber * rightNumber,
                "/" => rightNumber != 0 ? leftNumber / rightNumber : throw new DivideByZeroException(),
                _ => throw new InvalidOperationException()
            };

            return total.ToString("0.###", CultureInfo.InvariantCulture);
        }
    }
}
