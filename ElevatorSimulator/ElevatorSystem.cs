using EvelatorSimulator.Config;
using EvelatorSimulator.Elevator;
using EvelatorSimulator.Model;
using EvelatorSimulator.Movement;
using EvelatorSimulator.Passenger;

namespace EvelatorSimulator;

public class ElevatorSystem: IElevatorSystem
{
    private readonly IConfigurationService _configurationService;
    private readonly IMovementService _movementService;
    private readonly IPassengerService _passengerService;
    private readonly IElevatorCreatorService _elevatorCreatorService;
    private List<Model.Elevator> _elevators;

    public ElevatorSystem(
        IConfigurationService configurationService,
        IMovementService movementService,
        IPassengerService passengerService, 
        IElevatorCreatorService elevatorCreatorService)
    {
        _configurationService = configurationService;
        _movementService = movementService;
        _passengerService = passengerService;
        _elevatorCreatorService = elevatorCreatorService;

        // Tworzenie wind na podstawie konfiguracji
        _elevators = _elevatorCreatorService.CreateMultipleElevators(
            _configurationService.GetNumberOfElevators(),
            _configurationService.GetSpeed(0), // Zakładam, że wszystkie windy mają tę samą prędkość na starcie
            _configurationService.GetElevatorCapacity(0) // Zakładam, że wszystkie windy mają tę samą pojemność na starcie
        );

        // Inicjalizacja wind w serwisie ruchu
        _movementService.InitializeElevators(_elevators);
    }

    public void AddElevator()
    {
        int newElevatorId = _elevators.Count; // Nowe ID dla windy
        var newElevator = _elevatorCreatorService.CreateDefaultElevator(newElevatorId);
        _elevators.Add(newElevator);
        _configurationService.SetNumberOfElevators(_elevators.Count); // Aktualizacja liczby wind
        _movementService.InitializeElevators(_elevators); // Zaktualizowanie listy wind w MovementService
    }

    public void RemoveElevator()
    {
        if (_elevators.Count > 0)
        {
            _elevators.RemoveAt(_elevators.Count - 1); // Usunięcie ostatniej windy
            _configurationService.SetNumberOfElevators(_elevators.Count); // Aktualizacja liczby wind
            _movementService.InitializeElevators(_elevators); // Zaktualizowanie listy wind w MovementService
        }
    }
    
    // Konfiguracja systemu
    public void SetNumberOfFloors(int floors) => _configurationService.SetNumberOfFloors(floors);
    public int GetNumberOfFloors() => _configurationService.GetNumberOfFloors();

    public void SetNumberOfElevators(int count) => _configurationService.SetNumberOfElevators(count);
    public int GetNumberOfElevators() => _configurationService.GetNumberOfElevators();

    public void SetSpeed(int elevatorId, int speed) => _configurationService.SetSpeed(elevatorId, speed);
    public int GetSpeed(int elevatorId) => _configurationService.GetSpeed(elevatorId);

    public void SetElevatorCapacity(int elevatorId, int capacity) => _configurationService.SetElevatorCapacity(elevatorId, capacity);
    public int GetElevatorCapacity(int elevatorId) => _configurationService.GetElevatorCapacity(elevatorId);

    public void SetLoadUnloadTime(int timeInSeconds) => _configurationService.SetLoadUnloadTime(timeInSeconds);
    public int GetLoadUnloadTime() => _configurationService.GetLoadUnloadTime();

    // Sterowanie windami
    public async Task MoveElevator(int elevatorId, int targetFloor) => await _movementService.MoveElevator(elevatorId, targetFloor);
    public ElevatorStatus GetElevatorStatus(int elevatorId) => _movementService.GetElevatorStatus(elevatorId);

    // Pasażerowie
    public void AddPassenger(int currentFloor, int destinationFloor) => _passengerService.AddPassenger(currentFloor, destinationFloor);

    public async Task AssignPassengersToElevator(int elevatorId)
    {
        var passengers = _passengerService.GetPassengersAtFloor(elevatorId);
        int loadUnloadTime = _configurationService.GetLoadUnloadTime();
        await _movementService.LoadPassengers(elevatorId, passengers, loadUnloadTime); // dynamiczny czas załadunku
    }
    
    public async Task UnloadPassengersFromElevator(int elevatorId)
    {
        var passengers = _movementService.GetPassengersInElevator(elevatorId);
        int loadUnloadTime = _configurationService.GetLoadUnloadTime();
        await _movementService.UnloadPassengers(elevatorId, passengers, loadUnloadTime); // dynamiczny czas rozładunku
    }

    // Informacje o windach i pasażerach
    public List<int> GetElevatorPositions()
    {
        return _movementService.GetElevatorPositions();
    }

    public List<Model.Passenger> GetPassengersInElevator(int elevatorId)
    {
        return _movementService.GetPassengersInElevator(elevatorId);
    }
    
}