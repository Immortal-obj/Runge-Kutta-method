using System;
using System.Collections.Generic;

class RungeKuttamethod
{
    // Функция, задающая дифференциальное уравнение: dy/dx = f(x, y)
    static double F(double x, double y)
    {
        return x * x - (2 * y) / x; //Формула 
    }

    // Метод Рунге-Кутты 4-го порядка
    static void МетодРунгеКутты(double x0, double y0, double шаг, int количествоШагов)
    {
        double x = x0;
        double y = y0;

        Console.WriteLine("\nРезультаты вычислений:");
        Console.WriteLine("Шаг\tx\t\ty (числ.)\t");
        Console.WriteLine("------------------------------------------------------------");

        for (int i = 0; i <= количествоШагов; i++)
        {

            Console.WriteLine($"{i}\t{x:F6}\t{y:F6}\t");

            if (i < количествоШагов)
            {
                double k1 = шаг * F(x, y);
                double k2 = шаг * F(x + шаг / 2, y + k1 / 2);
                double k3 = шаг * F(x + шаг / 2, y + k2 / 2);
                double k4 = шаг * F(x + шаг, y + k3);

                y += (k1 + 2 * k2 + 2 * k3 + k4) / 6;
                x += шаг;
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
        double шаг = double.Parse(Console.ReadLine());

        Console.Write("Введите количество шагов: ");
        int количествоШагов = int.Parse(Console.ReadLine());

        // Вычисление
        МетодРунгеКутты(x0, y0, шаг, количествоШагов);
    }
}

