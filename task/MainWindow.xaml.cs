using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace task;

/// <summary>
///     Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private const int MaxCopies = 3;

    private const string Guid = "{84079a08-eb1c-4045-941e-08a5f337d471}";
    private static readonly Semaphore MainAppSemaphore = new(MaxCopies, MaxCopies, Guid);
    private readonly Task[] _arraytasks = new Task[2];
    private readonly SynchronizationContext _uiContext;
    private readonly Random rnd = new();
    private Mutex _mutex = new();

    public MainWindow()
    {
        if (MainAppSemaphore.WaitOne(0))
        {
            InitializeComponent();
            _uiContext = SynchronizationContext.Current;
        }
        else
        {
            MessageBox.Show("Вы пытаетесь открыть 4 окно ! Так делать нельзя, ай-яй-яй, но-но-но !");
            Close();
        }
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
            var range = rnd.Next(1000);
            for (var i = 0; i < 1000; i++)
            {
                var n = rnd.Next(range).ToString();
                writer.WriteLine(n);
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
        // Создаём мьютекс 
        _mutex = new Mutex(true, "{84079a08-eb1c-4045-941e-08a5f337d472}");
        _mutex.WaitOne();
        // Запуск 2 потока, который будет ожидать завершения работы первого потока.
        _arraytasks[1] = Task.Factory.StartNew(Thread2Function);
        _uiContext.Send(d => Thread1Message.Text = "1 поток захватил мьютекс!", null);
        Thread.Sleep(1000);
        GeneratorOfNumbers();
        _mutex.ReleaseMutex();
    }

    #endregion

    #region 2 поток

    private void Thread2Function()
    {
        _mutex.WaitOne(0);
        _uiContext.Send(d => Thread1Message.Text = "2 поток захватил мьютекс!", null);
        Thread.Sleep(1000);
        ParseFileForPrimes();
        _mutex.ReleaseMutex();
    }

    public static void ParseFileForPrimes()
    {
        var inputFile = "../../../garbage/array.txt";
        var outputFile = "../../../garbage/arrayPrimesOnly.txt";
        var primes = new List<int>();
        using (var sr = new StreamReader(inputFile))
        {
            string line;
            while ((line = sr.ReadLine()) != null)
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
    }

    // Helper function to check if a number is prime
    public static bool IsPrime(int n)
    {
        if (n <= 1) return false;

        for (var i = 2; i <= Math.Sqrt(n); i++)
            if (n % i == 0)
                return false;

        return true;
    }

    #endregion
}