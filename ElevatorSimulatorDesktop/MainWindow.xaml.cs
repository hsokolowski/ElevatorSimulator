using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;
using ElevatorSimulator;
using ElevatorSimulator.Config;
using ElevatorSimulator.Model;
using ElevatorSimulator.Strategy;

namespace ElevatorSimulatorDesktop;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private ElevatorSystem _elevatorSystem;
    private DispatcherTimer _timer;
    private double _totalWaitTime = 0;
    private int _totalRequests = 0;
    
    private Dictionary<int, ElevatorLogWindow> _elevatorLogWindows = new Dictionary<int, ElevatorLogWindow>();

    private IElevatorAssignmentStrategy _currentStrategy;
    
    public MainWindow()
    {
        InitializeComponent();

        var configService = new ConfigurationService();
        
        _currentStrategy = new ClosestIdleElevatorStrategy();
        _elevatorSystem = new ElevatorSystem(configService, _currentStrategy);
        _elevatorSystem.InitializeSystem();
        _elevatorSystem.ElevatorRequestCompleted += ElevatorSystem_ElevatorRequestCompleted;
    
        InitializeElevatorLogs();
        InitializeFloorButtons();
        InitializeElevatorSelector();

        _timer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(500)
        };
        _timer.Tick += UpdateElevators;
        _timer.Start();
    }

    
    private void InitializeElevatorLogs()
    {
        foreach (var elevator in _elevatorSystem.GetElevators())
        {
            var logWindow = new ElevatorLogWindow(elevator.Id);
            _elevatorLogWindows[elevator.Id] = logWindow;
            elevator.ElevatorLog += Elevator_ElevatorLog;
            logWindow.Show();
        }
    }
    
    private void Elevator_ElevatorLog(object sender, string e)
    {
        var elevator = sender as IElevator;
        if (elevator != null && _elevatorLogWindows.ContainsKey(elevator.Id))
        {
            _elevatorLogWindows[elevator.Id].AppendLog(e);
        }
    }
    
    protected override void OnClosing(CancelEventArgs e)
    {
        _elevatorSystem.ShutdownSystem();
        base.OnClosing(e);
    }

    private void InitializeFloorButtons()
    {
        FloorPanel.Children.Clear();
        for (int i = _elevatorSystem.Configuration.NumberOfFloors - 1; i >= 0; i--)
        {
            Button floorButton = new Button
            {
                Content = $"Piętro {i}",
                Tag = i,
                Height = 30,
                Width = 100,
                Margin = new Thickness(0, 5, 0, 0)
            };
            floorButton.Click += FloorButton_Click;
            FloorPanel.Children.Add(floorButton);
        }
    }

    private void FloorButton_Click(object sender, RoutedEventArgs e)
    {
        int floor = (int)(sender as Button).Tag;
        _elevatorSystem.RequestElevator(floor);
        _totalRequests++;
    }

    private void UpdateElevators(object sender, EventArgs e)
    {
        ElevatorCanvas.Children.Clear();
        double canvasHeight = ElevatorCanvas.ActualHeight;
        double floorHeight = canvasHeight / _elevatorSystem.Configuration.NumberOfFloors;

        // Rysowanie pięter
        for (int i = 0; i < _elevatorSystem.Configuration.NumberOfFloors; i++)
        {
            Line floorLine = new Line
            {
                X1 = 0,
                Y1 = canvasHeight - (i * floorHeight),
                X2 = ElevatorCanvas.ActualWidth,
                Y2 = canvasHeight - (i * floorHeight),
                Stroke = System.Windows.Media.Brushes.Black,
                StrokeThickness = 1
            };
            ElevatorCanvas.Children.Add(floorLine);

            TextBlock floorLabel = new TextBlock
            {
                Text = $"Piętro {i}",
                FontSize = 12,
                Foreground = System.Windows.Media.Brushes.Black
            };
            Canvas.SetLeft(floorLabel, 5);
            Canvas.SetTop(floorLabel, canvasHeight - (i * floorHeight) - 20);
            ElevatorCanvas.Children.Add(floorLabel);
        }

        // Rysowanie wind
        var elevators = _elevatorSystem.GetElevators();
        int elevatorIndex = 0;
        foreach (var elevator in elevators)
        {
            double elevatorLeft = elevatorIndex * 60 + 50;
            double elevatorTop = canvasHeight - (elevator.CurrentFloor * floorHeight) - 50;

            Rectangle elevatorRect = new Rectangle
            {
                Width = 50,
                Height = 50,
                Fill = System.Windows.Media.Brushes.DarkRed
            };
            Canvas.SetLeft(elevatorRect, elevatorLeft);
            Canvas.SetTop(elevatorRect, elevatorTop);
            ElevatorCanvas.Children.Add(elevatorRect);

            // Wyświetlanie statusu
            TextBlock statusText = new TextBlock
            {
                Text = $"Winda {elevator.Id}\n{elevator.Status}",
                FontSize = 10,
                Foreground = System.Windows.Media.Brushes.White,
                TextAlignment = TextAlignment.Center
            };
            Canvas.SetLeft(statusText, elevatorLeft);
            Canvas.SetTop(statusText, elevatorTop + 10);
            ElevatorCanvas.Children.Add(statusText);

            elevatorIndex++;
        }
    }

    private void InitializeElevatorSelector()
    {
        ElevatorSelector.Items.Clear();
        foreach (var elevator in _elevatorSystem.GetElevators())
        {
            ElevatorSelector.Items.Add($"Winda {elevator.Id}");
        }
        ElevatorSelector.SelectedIndex = 0;
    }

    private void ElevatorSystem_ElevatorRequestCompleted(object sender, TimeSpan waitTime)
    {
        _totalWaitTime += waitTime.TotalSeconds;
        _totalRequests++;

        Console.WriteLine($"Żądanie obsłużone, czas oczekiwania: {waitTime.TotalSeconds} s");
        
        Dispatcher.Invoke(() =>
        {
            UpdateStatistics();
        });
    }
    
    private void UpdateStatistics()
    {
        if (_totalRequests > 0)
        {
            double averageWaitTime = _totalWaitTime / _totalRequests;
            AverageWaitTimeLabel.Text = $"Średni czas oczekiwania: {averageWaitTime:F2} s";
        }
    }

    private void ApplySettings_Click(object sender, RoutedEventArgs e)
    {
        int numberOfElevators = int.Parse(ElevatorCountInput.Text);
        int numberOfFloors = int.Parse(FloorCountInput.Text);

        // Usuń subskrypcje zdarzeń
        _elevatorSystem.ElevatorRequestCompleted -= ElevatorSystem_ElevatorRequestCompleted;
        foreach (var elevator in _elevatorSystem.GetElevators())
        {
            elevator.ElevatorLog -= Elevator_ElevatorLog;
        }
        _elevatorSystem.ShutdownSystem();

        // Zamknij stare okna logów
        foreach (var logWindow in _elevatorLogWindows.Values)
        {
            logWindow.Close();
        }
        _elevatorLogWindows.Clear();

        _elevatorSystem.Configuration.NumberOfElevators = numberOfElevators;
        _elevatorSystem.Configuration.NumberOfFloors = numberOfFloors;

        _elevatorSystem = new ElevatorSystem(_elevatorSystem.Configuration, _currentStrategy);
        _elevatorSystem.InitializeSystem();

        // Ponownie subskrybuj zdarzenia
        _elevatorSystem.ElevatorRequestCompleted += ElevatorSystem_ElevatorRequestCompleted;

        InitializeFloorButtons();
        InitializeElevatorSelector();
        InitializeElevatorLogs();
        UpdateElevators(null, null);
    }

    private void ChangeSpeed_Click(object sender, RoutedEventArgs e)
    {
        int selectedIndex = ElevatorSelector.SelectedIndex;
        if (selectedIndex >= 0)
        {
            int newSpeed = int.Parse(ElevatorSpeedInput.Text);
            _elevatorSystem.Configuration.SetSpeed(selectedIndex, newSpeed);
            var elevator = _elevatorSystem.GetElevators().FirstOrDefault(ev => ev.Id == selectedIndex);
            elevator?.UpdateSpeed(newSpeed);
        }
    }
    
    private void IncreaseElevatorCount_Click(object sender, RoutedEventArgs e)
    {
        int currentCount = int.Parse(ElevatorCountInput.Text);
        ElevatorCountInput.Text = (currentCount + 1).ToString();
    }

    private void DecreaseElevatorCount_Click(object sender, RoutedEventArgs e)
    {
        int currentCount = int.Parse(ElevatorCountInput.Text);
        if (currentCount > 1)
        {
            ElevatorCountInput.Text = (currentCount - 1).ToString();
        }
    }

    private void IncreaseFloorCount_Click(object sender, RoutedEventArgs e)
    {
        int currentCount = int.Parse(FloorCountInput.Text);
        FloorCountInput.Text = (currentCount + 1).ToString();
    }

    private void DecreaseFloorCount_Click(object sender, RoutedEventArgs e)
    {
        int currentCount = int.Parse(FloorCountInput.Text);
        if (currentCount > 1)
        {
            FloorCountInput.Text = (currentCount - 1).ToString();
        }
    }

}