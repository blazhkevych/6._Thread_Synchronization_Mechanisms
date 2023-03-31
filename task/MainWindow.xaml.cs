using System.IO;
using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media.Imaging;

namespace task;

/// <summary>
///     Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private const int MAX_COPIES = 3;
    private static Semaphore? mainAppSemaphore;

    private readonly string[] Guids = new string[MAX_COPIES]
    {
        "{84079a08-eb1c-4045-941e-08a5f337d471}", "{84079a08-eb1c-4045-941e-08a5f337d472}",
        "{84079a08-eb1c-4045-941e-08a5f337d473}"
    };

    public MainWindow()
    {
        bool createdNew;
        for (var i = 0; i < MAX_COPIES; i++)
        {
            mainAppSemaphore = new Semaphore(MAX_COPIES, MAX_COPIES, Guids[i], out createdNew);
            if (!createdNew)
            {
                if (i == MAX_COPIES - 1)
                {
                    MessageBox.Show("Вы пытаетесь открыть 4 окно ! Так делать нельзя, ай-яй-яй, но-но-но !");
                    Close();
                    return;
                }
            }
            else
            {
                break;

            }
        }

        InitializeComponent();
        WindowStartupLocation = WindowStartupLocation.CenterScreen;
    }

    //void GeneratorOfNumbers()
    //{
    //    try
    //    {
    //        FileStream file2 = new FileStream("../../array.txt", FileMode.Create, FileAccess.Write);
    //        BinaryWriter writer = new BinaryWriter(file2);
    //        int range = rnd.Next(1000);
    //        for (int i = 0; i < 200000000; i++)
    //        {
    //            int n = rnd.Next(range);
    //            writer.Write(n);
    //        }
    //        writer.Close();
    //        file2.Close();
    //        uiContext.Send(d => label1.Text = "Файл с числовыми данными создан!", null);
    //    }
    //    catch (Exception e)
    //    {
    //        MessageBox.Show(e.Message);
    //    }
    //}
}