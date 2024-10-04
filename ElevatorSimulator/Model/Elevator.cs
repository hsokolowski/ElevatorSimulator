using System.Collections.Concurrent;
using EvelatorSimulator.Config;
using EvelatorSimulator.Passenger;

namespace EvelatorSimulator.Model;

public class Elevator: IElevator
{
    public int Id { get; }
    public int CurrentFloor { get; private set; }
    public ElevatorStatus Status { get; private set; }
    public int Speed { get; set; }
    private int Capacity { get; set; }
    private List<Passenger> Passengers { get; } = new List<Passenger>();

    private BlockingCollection<int> _floorRequests = new BlockingCollection<int>();
    private CancellationTokenSource _cts;
    private Task _elevatorTask;
    
    private readonly IPassengerService _passengerService;
    private readonly IConfigurationService _configurationService;

    public Elevator(int id, int speed, int capacity, IPassengerService passengerService, IConfigurationService configurationService)
    {
        Id = id;
        Speed = speed;
        Capacity = capacity;
        CurrentFloor = 0;
        Status = ElevatorStatus.Idle;

        _passengerService = passengerService;
        _configurationService = configurationService;
    }

    public void Start()
    {
        _cts = new CancellationTokenSource();
        _elevatorTask = Task.Run(() => ProcessRequestsAsync(_cts.Token));
    }

    public void Stop()
    {
        if (_cts != null && !_cts.IsCancellationRequested)
        {
            _cts.Cancel();
        }

        try
        {
            _elevatorTask?.Wait();
        }
        catch (AggregateException ae)
        {
            ae.Handle(ex => ex is TaskCanceledException);
        }
    }

    public void AddFloorRequest(int floor)
    {
        _floorRequests.Add(floor);
    }
    
    private async Task ProcessRequestsAsync(CancellationToken token)
    {
        try
        {
            foreach (var targetFloor in _floorRequests.GetConsumingEnumerable(token))
            {
                if (token.IsCancellationRequested)
                {
                    break;
                }
                await MoveToFloorAsync(targetFloor, token);
            }
        }
        catch (OperationCanceledException)
        {
            // Handle task cancellation
        }
        finally
        {
            // Clean up resources if needed
        }
    }

    public async Task MoveToFloorAsync(int targetFloor, CancellationToken token)
    {
        Status = targetFloor > CurrentFloor ? ElevatorStatus.MovingUp : ElevatorStatus.MovingDown;

        while (CurrentFloor != targetFloor)
        {
            if (token.IsCancellationRequested)
            {
                break;
            }
            CurrentFloor += Status == ElevatorStatus.MovingUp ? 1 : -1;
            await Task.Delay(1000 / Speed, token);
        }

        Status = ElevatorStatus.Waiting;
        await Task.Delay(1500, token); // Wait at the floor
        UnloadPassengersAtCurrentFloor();
        LoadPassengersAtCurrentFloor();

        Status = ElevatorStatus.Idle;
    }

    private void UnloadPassengersAtCurrentFloor()
    {
        var passengersToUnload = Passengers.Where(p => p.DestinationFloor == CurrentFloor).ToList();
        foreach (var passenger in passengersToUnload)
        {
            Passengers.Remove(passenger);
        }
    }

    private void LoadPassengersAtCurrentFloor()
    {
        var passengersAtFloor = _passengerService.GetPassengersAtFloor(CurrentFloor);
        var passengersToLoad = passengersAtFloor.Take(Capacity - Passengers.Count).ToList();
        foreach (var passenger in passengersToLoad)
        {
            Passengers.Add(passenger);
        }
        _passengerService.RemovePassengersFromFloor(CurrentFloor, passengersToLoad);
    }

    public List<Passenger> GetPassengers()
    {
        return Passengers;
    }

    public void LoadPassengers(List<Passenger> passengers)
    {
        Passengers.AddRange(passengers.Take(Capacity - Passengers.Count));
    }

    public void UnloadPassengers()
    {
        Passengers.Clear();
    }
    
    public void UpdateSpeed(int newSpeed)
    {
        Speed = newSpeed;
    }
}

public enum ElevatorStatus
{
    Idle,      // Winda stoi
    Moving,    // Winda w ruchu
    Waiting,    // Winda czeka na piętrze
    MovingUp,
    MovingDown,
    LoadingPassengers,
    UnloadingPassengers
}

public class Passenger
{
    public int Id { get; set; }
    public int CurrentFloor { get; set; }
    public int DestinationFloor { get; set; }
    public int TimeToLoad { get; set; } // czas wsiadania do windy
    public int TimeToUnload { get; set; } // czas wysiadania z windy
}