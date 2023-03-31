using System.Threading;
using System.Windows;

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
    }
}