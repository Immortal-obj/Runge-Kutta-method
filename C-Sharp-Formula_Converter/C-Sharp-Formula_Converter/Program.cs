using System;
using System.Text.RegularExpressions;

class DifferentialEquationConverter
{
    static void Main()
    {
        Console.WriteLine("Программа преобразования дифференциальных уравнений в C# функции");
        Console.WriteLine("===============================================================");
        Console.WriteLine("Введите уравнение в формате: y' + 2y/x = x^2");
        Console.WriteLine("Или введите 'exit' для выхода\n");

        while (true)
        {
            Console.Write("> ");
            string? input = Console.ReadLine()?.Trim();

            if (input?.ToLower() == "exit") break;
            if (string.IsNullOrEmpty(input)) continue;

            try
            {
                input = Regex.Replace(input, @"\\[\(\)\[\]]", "").Replace(" ", "");
                var result = ProcessEquation(input);

                Console.WriteLine("\nРезультат преобразования:");
                Console.WriteLine($"Стандартная форма: dy/dx = {result.StandardForm}");
                Console.WriteLine($"Функция на C#:     {result.CSharpFunction}");
                Console.WriteLine($"Примечания:       {result.Notes}\n");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
                Console.WriteLine("Попробуйте снова. Пример: y' + 2y/x = x^2\n");
            }
        }
    }

    static (string StandardForm, string CSharpFunction, string Notes) ProcessEquation(string equation)
    {
        if (equation.Contains("dx") && equation.Contains("dy"))
        {
            return ProcessDifferentialForm(equation);
        }

        string[] parts = equation.Split('=');
        if (parts.Length != 2) throw new ArgumentException("Уравнение должно содержать ровно один знак '='");

        string leftPart = parts[0];
        string rightPart = parts[1];

        if (leftPart == "y'" || leftPart == "dy/dx")
        {
            return (
                StandardForm: rightPart,
                CSharpFunction: ConvertToCSharp(rightPart),
                Notes: GenerateNotes(rightPart)
            );
        }

        var coeffMatch = Regex.Match(leftPart, @"^([+-]?\d*\.?\d*)");
        string coefficient = coeffMatch.Success ? coeffMatch.Value : "1";

        coefficient = coefficient switch
        {
            "+" => "1",
            "-" => "-1",
            "" => "1",
            _ => coefficient
        };

        int skipLength = leftPart.Contains("dy/dx") ? 5 : 2;
        string remainingTerms = leftPart.Substring(coeffMatch.Length + skipLength);

        string form = rightPart;
        if (!string.IsNullOrEmpty(remainingTerms))
        {
            form = remainingTerms[0] switch
            {
                '+' => $"{form} - {remainingTerms.Substring(1)}",
                '-' => $"{form} + {remainingTerms.Substring(1)}",
                _ => $"{form} - {remainingTerms}"
            };
        }

        if (coefficient != "1")
            form = $"({form})/{coefficient}";

        form = form.Replace("+-", "-").Replace("--", "+")
                 .Replace("(+", "(").Replace("(-", "(-");

        return (
            StandardForm: form,
            CSharpFunction: ConvertToCSharp(form),
            Notes: GenerateNotes(form)
        );
    }

    static (string StandardForm, string CSharpFunction, string Notes) ProcessDifferentialForm(string equation)
    {
        string[] parts = equation.Split('=');
        if (parts.Length != 2 || parts[1] != "0")
            throw new ArgumentException("Дифференциальная форма должна быть равна 0");

        string leftPart = parts[0];

        // Разбиваем на члены с учетом знаков
        var terms = Regex.Split(leftPart, @"([+-])");
        if (terms.Length == 0) terms = new[] { leftPart };

        string dyTerm = "", dxTerm = "";
        bool isNegative = false;

        foreach (string term in terms)
        {
            if (string.IsNullOrEmpty(term)) continue;
            if (term == "+") { isNegative = false; continue; }
            if (term == "-") { isNegative = true; continue; }

            if (term.Contains("dy"))
            {
                dyTerm = (isNegative ? "-" : "") + term.Replace("dy", "");
            }
            else if (term.Contains("dx"))
            {
                dxTerm = (isNegative ? "-" : "") + term.Replace("dx", "");
            }
        }

        if (string.IsNullOrEmpty(dyTerm) || string.IsNullOrEmpty(dxTerm))
            throw new ArgumentException("Не найдены dy или dx компоненты");

        // Упрощаем выражения
        dyTerm = SimplifyTerm(dyTerm);
        dxTerm = SimplifyTerm(dxTerm);

        string standardForm = $"-({dxTerm})/({dyTerm})";
        standardForm = standardForm.Replace("--", "");

        return (
            StandardForm: standardForm,
            CSharpFunction: ConvertToCSharp(standardForm),
            Notes: GenerateNotes(standardForm)
        );
    }

    static string SimplifyTerm(string term)
    {
        // Удаляем лишние * в конце
        term = term.TrimEnd('*');

        // Упрощаем выражения вида a*b*... к a*b*...
        term = Regex.Replace(term, @"\*+", "*");
        term = term.Trim('*');

        return term;
    }

    static string ConvertToCSharp(string expression)
    {
        // Сначала упрощаем выражение
        expression = SimplifyTerm(expression);

        // Заменяем тригонометрические функции
        string result = expression
            .Replace("sin(", "Math.Sin(")
            .Replace("cos(", "Math.Cos(")
            .Replace("tan(", "Math.Tan(");

        // Заменяем степени
        result = Regex.Replace(result, @"([a-zA-Z][a-zA-Z0-9]*)\^(\d+)", m =>
            m.Groups[2].Value switch
            {
                "2" => $"{m.Groups[1].Value}*{m.Groups[1].Value}",
                "3" => $"{m.Groups[1].Value}*{m.Groups[1].Value}*{m.Groups[1].Value}",
                _ => $"Math.Pow({m.Groups[1].Value},{m.Groups[2].Value})"
            });

        // Удаляем лишние * перед скобками
        result = Regex.Replace(result, @"\*+(?=[\(])", "");

        return $"(x, y) => {result}";
    }

    static string GenerateNotes(string expression)
    {
        var notes = "";
        if (expression.Contains("/x")) notes += "Избегайте x = 0 в начальных условиях. ";
        if (expression.Contains("Math.Pow")) notes += "Для степеней используется Math.Pow. ";
        if (expression.Contains("Math.Sin") || expression.Contains("Math.Cos") || expression.Contains("Math.Tan"))
            notes += "Тригонометрические функции работают в радианах. ";
        if (expression.Contains("/y")) notes += "Избегайте y = 0 в начальных условиях. ";

        return string.IsNullOrEmpty(notes) ? "Нет особых замечаний" : notes.Trim();
    }
}