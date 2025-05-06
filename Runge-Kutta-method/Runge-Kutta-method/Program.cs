class RungeKuttamethod
{
    static void Main()
    {
        double x0 = 1.0d; //Начальная точка x
        double y0 = 1.0d; //Начальное значение y
        double h = 0.1d; //Шаг
        int колличество_шагов = 10; //Количество шагов

        МетодРунгеКутта(x0, y0, h, колличество_шагов); //Метод РунгеКутты
    }

    static double F(double x, double y)
    {
        //return -2 * y / x; //Формула xdy + 2ydx = 0
        return y / x + 1.0 / (x * x); //Формула y' - (1/x)*y = 1/x^2
    }

    static void МетодРунгеКутта(double x0, double y0, double h, int колличество_шагов)
    {
        double x = x0;
        double y = y0;

        Console.WriteLine("Шаг\t x\t\t y\t");
        Console.WriteLine($"0\t {x:F6}\t {y:F6}");
        for (int i = 1; i <= колличество_шагов; i++)
        {
            double k1 = h * F(x, y);
            double k2 = h * F(x + h / 2, y + k1 / 2);
            double k3 = h * F(x + h / 2, y + k2 / 2);
            double k4 = h * F(x + h, y + k2);
            y += (k1 + 2 * k2 + 2 + 2 * k3 + 2 + k4) / 6;
            x += h;
            Console.WriteLine($"{i}\t {x:F6}\t {y:F6}");
        }
    }
}
