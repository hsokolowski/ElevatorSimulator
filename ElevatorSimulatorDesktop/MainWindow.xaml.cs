using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows.Threading;
using EvelatorSimulator;
using EvelatorSimulator.Config;
using EvelatorSimulator.Passenger;

namespace ElevatorSimulatorDesktop;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private readonly IElevatorSystem _elevatorSystemFacade;
    private DispatcherTimer _timer;
    
    public MainWindow()
    {
        InitializeComponent();
        _elevatorSystemFacade = new ElevatorSystem(new ConfigurationService(), new PassengerService());
        _elevatorSystemFacade.InitializeSystem();

        InitializeFloorButtons();
        InitializeElevatorSelector();

        _timer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(500)
        };
        _timer.Tick += UpdateElevators;
        _timer.Start();

        UpdateLabels();
    }
    
    private void InitializeElevatorSelector()
    {
        ElevatorSelector.Items.Clear();
        var elevators = _elevatorSystemFacade.GetElevators();
        foreach (var elevator in elevators)
        {
            ElevatorSelector.Items.Add($"Elevator {elevator.Id}");
        }
    }
    
    protected override void OnClosing(CancelEventArgs e)
    {
        _elevatorSystemFacade.ShutdownSystem();
        base.OnClosing(e);
    }

    private void InitializeFloorButtons()
    {
        FloorButtonPanel.Children.Clear();
        for (int i = _elevatorSystemFacade.Configuration.NumberOfFloors - 1; i >= 0; i--)
        {
            Button floorButton = new Button
            {
                Content = $"Piętro {i}",
                Tag = i,
                Height = 30,
                Width = 60,
                Margin = new Thickness(0, 5, 0, 0)
            };
            floorButton.Click += FloorButton_Click;
            FloorButtonPanel.Children.Add(floorButton);
        }
    }
    
    private void FloorButton_Click(object sender, RoutedEventArgs e)
    {
        int floor = int.Parse((sender as Button).Tag.ToString());
        _elevatorSystemFacade.RequestElevator(floor);
    }

    private void UpdateElevators(object sender, EventArgs e)
    {
        ElevatorCanvas.Children.Clear();

        double canvasHeight = ElevatorCanvas.ActualHeight;
        double floorHeight = canvasHeight / _elevatorSystemFacade.Configuration.NumberOfFloors;

        // Rysowanie linii pięter i numerów
        for (int i = 0; i < _elevatorSystemFacade.Configuration.NumberOfFloors; i++)
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
                Text = $"P{i}",
                FontSize = 12,
                Foreground = System.Windows.Media.Brushes.Black
            };
            Canvas.SetLeft(floorLabel, 5); // Ustawienie labelki z lewej strony
            Canvas.SetTop(floorLabel, canvasHeight - (i * floorHeight) - 20); // Ustawienie nad kreską
            ElevatorCanvas.Children.Add(floorLabel);
        }

        // Rysowanie wind
        var elevators = _elevatorSystemFacade.GetElevators();
        foreach (var elevator in elevators)
        {
            double elevatorTop = canvasHeight - (elevator.CurrentFloor * floorHeight) - 50;

            Rectangle elevatorRect = new Rectangle
            {
                Width = 50,
                Height = 50,
                Margin = new Thickness(30,0,0,0),
                Fill = System.Windows.Media.Brushes.DarkRed,
                
            };
            Canvas.SetLeft(elevatorRect, elevator.Id * 60);
            Canvas.SetTop(elevatorRect, elevatorTop);
            ElevatorCanvas.Children.Add(elevatorRect);

            // Optionally, display elevator status
            TextBlock statusText = new TextBlock
            {
                Margin = new Thickness(30,0,0,0),
                Text = elevator.Status.ToString(),
                FontSize = 10,
                Foreground = System.Windows.Media.Brushes.Black
            };
            Canvas.SetLeft(statusText, elevator.Id * 60);
            Canvas.SetTop(statusText, elevatorTop - 20);
            ElevatorCanvas.Children.Add(statusText);
        }
    }

    private void AddElevator_Click(object sender, RoutedEventArgs e)
    {
        _elevatorSystemFacade.Configuration.NumberOfElevators++;
        _elevatorSystemFacade.ShutdownSystem();
        _elevatorSystemFacade.InitializeSystem();
        InitializeElevatorSelector();
        UpdateLabels();
    }

    private void RemoveElevator_Click(object sender, RoutedEventArgs e)
    {
        if (_elevatorSystemFacade.Configuration.NumberOfElevators > 1)
        {
            _elevatorSystemFacade.Configuration.NumberOfElevators--;
            _elevatorSystemFacade.ShutdownSystem();
            _elevatorSystemFacade.InitializeSystem();
            UpdateLabels();
        }
    }

    private void AddFloor_Click(object sender, RoutedEventArgs e)
    {
        _elevatorSystemFacade.Configuration.NumberOfFloors++;
        InitializeFloorButtons();
        UpdateLabels();
    }

    private void RemoveFloor_Click(object sender, RoutedEventArgs e)
    {
        if (_elevatorSystemFacade.Configuration.NumberOfFloors > 1)
        {
            _elevatorSystemFacade.Configuration.NumberOfFloors--;
            InitializeFloorButtons();
            UpdateLabels();
        }
    }

    private void ApplyChangesToSelectedElevator_Click(object sender, RoutedEventArgs e)
    {
        if (int.TryParse(ElevatorSpeedInput.Text, out int speed))
        {
            int selectedIndex = ElevatorSelector.SelectedIndex;
            if (selectedIndex >= 0)
            {
                _elevatorSystemFacade.Configuration.SetSpeed(selectedIndex, speed);
                var elevator = _elevatorSystemFacade.GetElevators().FirstOrDefault(e => e.Id == selectedIndex);
                if (elevator != null)
                {
                    elevator.UpdateSpeed(speed);
                }
            }
        }
    }

    private void ApplyChangesToAllElevators_Click(object sender, RoutedEventArgs e)
    {
        if (int.TryParse(ElevatorSpeedInput.Text, out int speed))
        {
            var elevators = _elevatorSystemFacade.GetElevators();
            foreach (var elevator in elevators)
            {
                _elevatorSystemFacade.Configuration.SetSpeed(elevator.Id, speed);
                elevator.UpdateSpeed(speed);
            }
        }
    }

    private void ElevatorSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (ElevatorSelector.SelectedIndex >= 0)
        {
            var elevator = _elevatorSystemFacade.GetElevators().FirstOrDefault(ev => ev.Id == ElevatorSelector.SelectedIndex);
            if (elevator != null)
            {
                ElevatorSpeedInput.Text = elevator.Speed.ToString();
            }
        }
    }

    private void UpdateLabels()
    {
        ElevatorCountLabel.Text = $"Ilość wind: {_elevatorSystemFacade.Configuration.NumberOfElevators}";
        FloorCountLabel.Text = $"Ilość pięter: {_elevatorSystemFacade.Configuration.NumberOfFloors}";
    }
}