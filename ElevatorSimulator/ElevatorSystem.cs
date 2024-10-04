using EvelatorSimulator.Config;
using EvelatorSimulator.Controller;
using EvelatorSimulator.Model;
using EvelatorSimulator.Passenger;

namespace EvelatorSimulator;

public class ElevatorSystem: IElevatorSystem
{
    public IConfigurationService Configuration { get; }
    public IPassengerService PassengerService { get; }

    private IElevatorController _elevatorController;
    private List<IElevator> _elevators;

    public ElevatorSystem(IConfigurationService config, IPassengerService passengerService)
    {
        Configuration = config;
        PassengerService = passengerService;
        _elevatorController = new ElevatorController();
        _elevators = new List<IElevator>();
        InitializeSystem();
    }

    public void InitializeSystem()
    {
        // Stop existing elevators if any
        foreach (var elevator in _elevators)
        {
            elevator.Stop();
        }

        // Clear existing elevators and controller state
        _elevators.Clear();
        _elevatorController = new ElevatorController();

        // Create new elevators based on updated configuration
        for (int i = 0; i < Configuration.NumberOfElevators; i++)
        {
            int speed = Configuration.GetSpeed(i);
            int capacity = Configuration.GetElevatorCapacity(i);
            var elevator = new Elevator(i, speed, capacity, PassengerService, Configuration);
            _elevators.Add(elevator);
            _elevatorController.RegisterElevator(elevator);
            elevator.Start();
        }
    }

    public void ShutdownSystem()
    {
        foreach (var elevator in _elevators)
        {
            elevator.Stop();
        }
        
        _elevators.Clear();
        _elevatorController = null;
    }

    public void RequestElevator(int floor)
    {
        _elevatorController.RequestElevator(floor);
    }

    public List<IElevator> GetElevators()
    {
        return _elevatorController.GetElevators();
    }
}