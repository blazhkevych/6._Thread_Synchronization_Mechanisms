using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace task;

public partial class MainWindow : Window
{
    private const int MaxCopies = 3;

    private const string Guid = "{84079a08-eb1c-4045-941e-08a5f337d471}";
    private static readonly Semaphore MainAppSemaphore = new(MaxCopies, MaxCopies, Guid);
    private readonly Task[] _arraytasks = new Task[3];
    private readonly SynchronizationContext _uiContext;
    private readonly Random rnd = new();
    private Mutex _mutex = new();

    public MainWindow()
    {
        if (WaitOne(0, 3))
        {
            InitializeComponent();
            _uiContext = SynchronizationContext.Current;
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
        }
        else
        {
            MessageBox.Show("Вы пытаетесь открыть 4 окно ! Так делать нельзя, ай-яй-яй, но-но-но !");
            Close();
        }
    }

    private static bool WaitOne(int ms, int count)
    {
        for (var i = 0; i < count; i++)
            if (MainAppSemaphore.WaitOne(ms))
                return true;
        return false;
    }

    private void Start_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            Thread1Message.Text = "";
            Thread2Message.Text = "";
            Thread3Message.Text = "";
            // Запуск 1 потока.
            _arraytasks[0] = Task.Factory.StartNew(Thread1Function);
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
        }
    }

    private void Window_Closing(object sender, CancelEventArgs e)
    {
        MainAppSemaphore.Release();
    }

    #region 1 поток

    private void GeneratorOfNumbers()
    {
        try
        {
            var file = new FileStream("../../../garbage/array.txt", FileMode.Create, FileAccess.Write);
            var writer = new StreamWriter(file);
            var range = rnd.Next(10000);
            for (var i = 0; i < 10000; i++)
            {
                var n = rnd.Next(range).ToString();
                writer.Write(n + " ");
            }

            writer.Close();
            file.Close();
            _uiContext.Send(d => Thread1Message.Text = "Файл с числовыми данными создан!", null);
        }
        catch (Exception e)
        {
            MessageBox.Show(e.Message);
        }
    }

    private void Thread1Function()
    {
        bool CreatedNew;
        // Создаём мьютекс 
        _mutex = new Mutex(false, "84079a08-eb1c-4045-941e-08a5f337d471", out CreatedNew);
        _mutex.WaitOne();
        // Запуск 2 потока, который будет ожидать завершения работы первого потока.
        _arraytasks[1] = Task.Factory.StartNew(Thread2Function);
        _uiContext.Send(d => Thread1Message.Text = "1 поток захватил мьютекс!", null);
        Thread.Sleep(2000);
        GeneratorOfNumbers();
        _mutex.ReleaseMutex();
    }

    #endregion

    #region 2 поток

    private void Thread2Function()
    {
        bool CreatedNew;
        // Создаём мьютекс 
        _mutex = new Mutex(false, "84079a08-eb1c-4045-941e-08a5f337d471", out CreatedNew);
        _mutex.WaitOne();
        // Запуск 3 потока, который будет ожидать завершения работы первого потока.
        _arraytasks[2] = Task.Factory.StartNew(Thread3Function);
        _uiContext.Send(d => Thread2Message.Text = "2 поток захватил мьютекс!", null);
        Thread.Sleep(2000);
        ParseFileForPrimes();
        _mutex.ReleaseMutex();
    }

    private void ParseFileForPrimes()
    {
        var inputFile = "../../../garbage/array.txt";
        var outputFile = "../../../garbage/arrayPrimesOnly.txt";
        var primes = new List<int>();
        using (var sr = new StreamReader(inputFile))
        {
            while (sr.ReadLine() is { } line)
            {
                // Split the line into individual numbers
                var numbers = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                // Parse each number and check if it's prime
                foreach (var number in numbers)
                {
                    int n;
                    if (int.TryParse(number, out n) && IsPrime(n)) primes.Add(n);
                }
            }
        }

        // Write the primes to the output file
        using (var sw = new StreamWriter(outputFile))
        {
            foreach (var prime in primes) sw.Write(prime + " ");
        }

        _uiContext.Send(d => Thread2Message.Text = "Файл с простыми числами создан!", null);
    }

    // Helper function to check if a number is prime
    private static bool IsPrime(int n)
    {
        if (n <= 1) return false;

        for (var i = 2; i <= Math.Sqrt(n); i++)
            if (n % i == 0)
                return false;

        return true;
    }

    #endregion

    #region 3 поток

    private void Thread3Function()
    {
        bool CreatedNew;
        // Создаём мьютекс 
        _mutex = new Mutex(false, "84079a08-eb1c-4045-941e-08a5f337d471", out CreatedNew);
        _mutex.WaitOne();
        _uiContext.Send(d => Thread3Message.Text = "3 поток захватил мьютекс!", null);
        Thread.Sleep(2000);
        CreateFileWithNumbersEndingInSeven();
        _mutex.ReleaseMutex();
    }

    private void CreateFileWithNumbersEndingInSeven()
    {
        var inputFile = "../../../garbage/arrayPrimesOnly.txt";
        var outputFile = "../../../garbage/arrayPrimesOnlyEndingInSeven.txt";
        var numbersEndingInSeven = new List<int>();
        using (var sr = new StreamReader(inputFile))
        {
            while (sr.ReadLine() is { } line)
            {
                // Split the line into individual numbers
                var numbers = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                // Parse each number and check if it ends in 7
                foreach (var number in numbers)
                {
                    int n;
                    if (int.TryParse(number, out n) && n % 10 == 7) numbersEndingInSeven.Add(n);
                }
            }
        }

        // Write the numbers ending in 7 to the output file
        using (var sw = new StreamWriter(outputFile))
        {
            foreach (var numberEndingInSeven in numbersEndingInSeven) sw.Write(numberEndingInSeven + " ");
        }

        _uiContext.Send(
            d => Thread3Message.Text = "Файл с простыми числами, у которых последняя цифра равна 7, создан!", null);
    }

    #endregion
}