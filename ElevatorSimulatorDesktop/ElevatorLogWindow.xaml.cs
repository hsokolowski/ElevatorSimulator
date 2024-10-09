using System.Windows;

namespace ElevatorSimulatorDesktop;

public partial class ElevatorLogWindow : Window
{
    public ElevatorLogWindow(int elevatorId)
    {
        InitializeComponent();
        Title = $"Logi Windy {elevatorId}";
    }

    public void AppendLog(string message)
    {
        Dispatcher.Invoke(() =>
        {
            LogTextBox.AppendText(message + Environment.NewLine);
            LogTextBox.ScrollToEnd();
        });
    }
}