using System;
using System.Globalization;

class Program
{
    static void Main()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        double current = 0.0;          // текущее значение
        double memory = 0.0;          // память
        string? pendingOp = null;      // операции с двумя числами (бинарные): "+", "-", "*", "/", "%"
        bool expectNumber = true;      // ожидаем число после выбора бинарной операции

        Console.WriteLine("Простой калькулятор. Введите число или операцию. Команды: + - * / % sqrt 1/x x^2 m+ m- mr mc c exit");
        while (true)
        {
            Console.Write("Введите число/операцию: ");
            string? token = Console.ReadLine();
            if (token == null) break;
            token = token.Trim().ToLowerInvariant();
            if (token.Length == 0) continue;

            try
            {
                // Попытка распознать число
                if (double.TryParse(token, out double num))
                {
                    if (pendingOp == null)
                    {
                        // Это новое текущее значение, если бинарной операции нет
                        current = num;
                    }
                    else
                    {
                        // Применяем отложенную бинарную операцию
                        current = ApplyBinary(pendingOp, current, num);
                        pendingOp = null;
                        expectNumber = true; // снова ждём число/операцию
                    }
                    Print(current, memory);
                    continue;
                }

                // Не число => операция
                switch (token)
                {
                    case "exit": return;
                    case "c":
                    case "clear":
                        current = 0.0; pendingOp = null; expectNumber = true;
                        Console.WriteLine("Сброс. Текущее = 0");
                        Print(current, memory);
                        break;

                    // бинарные операции просто запоминаем, дальше ждём число
                    case "+":
                    case "-":
                    case "*":
                    case "/":
                    case "%":
                        pendingOp = token;
                        expectNumber = true;
                        // сразу просим число следующей строкой
                        break;

                    // унарные операции применяем немедленно к current
                    case "sqrt":
                        if (current < 0) throw new ArithmeticException("sqrt для отрицательного числа не определён");
                        current = Math.Sqrt(current);
                        Print(current, memory);
                        break;

                    case "1/x":
                    case "inv": // альтернативное имя
                        if (current == 0) throw new DivideByZeroException("1/x при x=0 не определено");
                        current = 1.0 / current;
                        Print(current, memory);
                        break;

                    case "x^2":
                    case "sqr": // альтернативное имя
                        current = current * current;
                        Print(current, memory);
                        break;

                    // память: работает с текущим значением
                    case "m+":
                        memory += current;
                        Console.WriteLine("Память увеличена на текущее значение");
                        Print(current, memory);
                        break;

                    case "m-":
                        memory -= current;
                        Console.WriteLine("Память уменьшена на текущее значение");
                        Print(current, memory);
                        break;

                    case "mr":
                        current = memory;
                        Console.WriteLine("Память считана в текущее");
                        Print(current, memory);
                        break;

                    case "mc":
                        memory = 0.0;
                        Console.WriteLine("Память очищена");
                        Print(current, memory);
                        break;

                    default:
                        Console.WriteLine("Неизвестная операция. Доступно: + - * / % sqrt 1/x x^2 m+ m- mr mc c exit");
                        break;
                }
            }
            catch (DivideByZeroException ex)
            {
                Console.WriteLine("Ошибка: " + ex.Message);
                pendingOp = null; // сбрасываем состояние
            }
            catch (ArithmeticException ex)
            {
                Console.WriteLine("Арифметическая ошибка: " + ex.Message);
                pendingOp = null;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Неожиданная ошибка: " + ex.Message);
                pendingOp = null;
            }
        }
    }

    // Остаток/деление с проверками
    static double ApplyBinary(string op, double a, double b)
    {
        return op switch
        {
            "+" => a + b,
            "-" => a - b,
            "*" => a * b,
            "/" => b == 0 ? throw new DivideByZeroException("Деление на ноль") : a / b,
            "%" => b == 0 ? throw new DivideByZeroException("Остаток от деления на ноль") : a % b,
            _ => throw new InvalidOperationException("Неизвестная бинарная операция")
        };
    }

    static void Print(double current, double memory)
    {
        Console.WriteLine("Вывод: " + current.ToString() + $" (память: {memory.ToString()})");
    }
}
