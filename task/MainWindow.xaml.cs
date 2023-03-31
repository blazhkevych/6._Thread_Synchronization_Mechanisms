using System;
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
    private static readonly Semaphore? MainAppSemaphore = new(MaxCopies, MaxCopies, Guid);
    private readonly Task[] arraytasks = new Task[2];

    private readonly Random rnd = new();
    public SynchronizationContext uiContext;

    public MainWindow()
    {
        if (MainAppSemaphore != null && MainAppSemaphore.WaitOne(0))
        {
            try
            {
                InitializeComponent();
                WindowStartupLocation = WindowStartupLocation.CenterScreen;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
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
            arraytasks[3] = Task.Factory.StartNew(Thread1Function);
            //arraytasks[4] = arraytasks[3].ContinueWith(ThreadFunction5);
            //arraytasks[5] = arraytasks[3].ContinueWith(ThreadFunction6);
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
            var file = new FileStream("../../garbage/array.txt", FileMode.Create, FileAccess.Write);
            var writer = new BinaryWriter(file);
            var range = rnd.Next(1000);
            for (var i = 0; i < 1000; i++)
            {
                var n = rnd.Next(range);
                writer.Write(n);
            }

            writer.Close();
            file.Close();
            uiContext.Send(d => Thread1Message.Text = "Файл с числовыми данными создан!", null);
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
        var mutex = new Mutex(false, "DB744E26-72C1-4F2A-8BF8-5C31980953C7", out CreatedNew);
        mutex.WaitOne();
        uiContext.Send(d => Thread1Message.Text = "Поток захватил мьютекс!", null);
        GeneratorOfNumbers();
        mutex.ReleaseMutex();
    }

    #endregion
}