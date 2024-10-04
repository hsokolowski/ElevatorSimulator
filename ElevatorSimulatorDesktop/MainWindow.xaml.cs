using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows.Threading;
using EvelatorSimulator;
using EvelatorSimulator.Config;
using EvelatorSimulator.Elevator;
using EvelatorSimulator.Movement;
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
        : this(new ElevatorSystem(
            new ConfigurationService(),
            new MovementService(),
            new PassengerService(),
            new ElevatorCreatorService())
        )
    {
    }

    public MainWindow(IElevatorSystem elevatorSystemFacade)
    {
        InitializeComponent();
        _elevatorSystemFacade = elevatorSystemFacade;
        
        InitializeFloorButtons();

        // Timer do aktualizacji UI
        _timer = new DispatcherTimer();
        _timer.Interval = TimeSpan.FromMilliseconds(500);
        _timer.Tick += UpdateElevators;
        _timer.Start();
        
        UpdateLabels(); // Aktualizacja labeli po starcie
    }

    private void InitializeFloorButtons()
    {
        FloorButtonPanel.Children.Clear();
        for (int i = _elevatorSystemFacade.GetNumberOfFloors() - 1; i >= 0; i--)
        {
            Button floorButton = new Button
            {
                Content = $"Piętro {i}",
                Tag = i,
                Height = 30,
                Width = 60,
                Margin = new Thickness(0, 5, 0, 0)
            };
            floorButton.Click += async (sender, e) =>
            {
                int floor = int.Parse((sender as Button).Tag.ToString());
                await _elevatorSystemFacade.MoveElevator(0, floor);  // Przemieszczenie windy
                await _elevatorSystemFacade.AssignPassengersToElevator(0); 
            };
            FloorButtonPanel.Children.Add(floorButton);
        }
    }

    private void UpdateElevators(object sender, EventArgs e)
    {
        ElevatorCanvas.Children.Clear();

        double canvasHeight = ElevatorCanvas.ActualHeight;
        double floorHeight = canvasHeight / _elevatorSystemFacade.GetNumberOfFloors();

        // Rysowanie linii pięter i numerów
        for (int i = 0; i < _elevatorSystemFacade.GetNumberOfFloors(); i++)
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
        var positions = _elevatorSystemFacade.GetElevatorPositions();
        for (int i = 0; i < positions.Count; i++)
        {
            double elevatorTop = canvasHeight - (positions[i] * floorHeight) - 50;

            Rectangle elevatorRect = new Rectangle
            {
                Margin = new Thickness(30, 0, 0, 0),
                Width = 50,
                Height = 50,
                Fill = System.Windows.Media.Brushes.DarkRed
            };
            Canvas.SetLeft(elevatorRect, i * 60); // Odstępy między windami
            Canvas.SetTop(elevatorRect, elevatorTop);
            ElevatorCanvas.Children.Add(elevatorRect);
        }
    }

    private void AddElevator_Click(object sender, RoutedEventArgs e)
    {
        _elevatorSystemFacade.AddElevator();
        UpdateLabels();
    }

    private void RemoveElevator_Click(object sender, RoutedEventArgs e)
    {
        _elevatorSystemFacade.RemoveElevator();
        UpdateLabels();
    }

    private void AddFloor_Click(object sender, RoutedEventArgs e)
    {
        _elevatorSystemFacade.SetNumberOfFloors(_elevatorSystemFacade.GetNumberOfFloors() + 1);
        InitializeFloorButtons();
        UpdateLabels();
    }

    private void RemoveFloor_Click(object sender, RoutedEventArgs e)
    {
        _elevatorSystemFacade.SetNumberOfFloors(_elevatorSystemFacade.GetNumberOfFloors() - 1);
        InitializeFloorButtons();
        UpdateLabels();
    }

    private void ApplyChangesToSelectedElevator_Click(object sender, RoutedEventArgs e)
    {
        if (int.TryParse(ElevatorSpeedInput.Text, out int speed))
        {
            int selectedIndex = ElevatorSelector.SelectedIndex;
            if (selectedIndex >= 0)
                _elevatorSystemFacade.SetSpeed(selectedIndex, speed);
        }
    }

    private void ApplyChangesToAllElevators_Click(object sender, RoutedEventArgs e)
    {
        if (int.TryParse(ElevatorSpeedInput.Text, out int speed))
        {
            for (int i = 0; i < _elevatorSystemFacade.GetNumberOfElevators(); i++)
            {
                _elevatorSystemFacade.SetSpeed(i, speed);
            }
        }
    }

    private void ElevatorSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (ElevatorSelector.SelectedIndex >= 0)
        {
            // Ustaw prędkość windy w polu tekstowym
            ElevatorSpeedInput.Text = $"Prędkość: {_elevatorSystemFacade.GetSpeed(ElevatorSelector.SelectedIndex)}";
        }
    }

    private void UpdateLabels()
    {
        ElevatorCountLabel.Text = $"Ilość wind: {_elevatorSystemFacade.GetNumberOfElevators()}";
        FloorCountLabel.Text = $"Ilość pięter: {_elevatorSystemFacade.GetNumberOfFloors()}";
    }
}