using System;
using System.Collections.Generic;

class RungeKuttamethod
{
    // Функция, задающая дифференциальное уравнение: dy/dx = f(x, y)
    static double F(double x, double y)
    {
        return x; //Формула 
    }

    // Метод Рунге-Кутты 4-го порядка
    static void RungeKuttaMethod(double x0, double y0, double step, int NumberOfSteps)
    {
        double x = x0;
        double y = y0;

        Console.WriteLine("\nРезультаты вычислений:");
        Console.WriteLine("Шаг\tx\t\ty (числ.)\t");
        Console.WriteLine("------------------------------------------------------------");

        for (int i = 0; i <= NumberOfSteps; i++)
        {

            Console.WriteLine($"{i}\t{x:F6}\t{y:F6}\t");

            if (i < NumberOfSteps)
            {
                double k1 = step * F(x, y);
                double k2 = step * F(x + step / 2, y + k1 / 2);
                double k3 = step * F(x + step / 2, y + k2 / 2);
                double k4 = step * F(x + step, y + k3);

                y += (k1 + 2 * k2 + 2 * k3 + k4) / 6;
                x += step;
            }
        }
    }

    static void Main()
    {
        Console.WriteLine("Решение ОДУ методом Рунге-Кутты 4-го порядка");
        //Console.WriteLine("Уравнение: y' = x * y"); //Формула

        // Ввод начальных условий
        Console.Write("\nВведите начальное значение x0: ");
        double x0 = double.Parse(Console.ReadLine());

        Console.Write("Введите начальное значение y0: ");
        double y0 = double.Parse(Console.ReadLine());

        Console.Write("Введите шаг h: ");
        double step = double.Parse(Console.ReadLine());

        Console.Write("Введите количество шагов: ");
        int NumberOfSteps = int.Parse(Console.ReadLine());

        // Вычисление
        RungeKuttaMethod(x0, y0, step, NumberOfSteps);
    }
}

