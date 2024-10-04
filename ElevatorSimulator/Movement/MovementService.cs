using EvelatorSimulator.Config;
using EvelatorSimulator.Model;

namespace EvelatorSimulator.Movement;

public class MovementService : IMovementService
{
    private List<Model.Elevator> _elevators;

    public MovementService()
    {
        
    }

    public void InitializeElevators(List<Model.Elevator> elevators)
    {
        _elevators = elevators;
    }
    
    public async Task MoveElevator(int elevatorId, int targetFloor)
    {
        var elevator = _elevators.FirstOrDefault(e => e.Id == elevatorId);
        if (elevator == null) return;

        elevator.TargetFloor = targetFloor;
        elevator.Status = targetFloor > elevator.CurrentFloor ? ElevatorStatus.MovingUp : ElevatorStatus.MovingDown;

        while (elevator.CurrentFloor != targetFloor)
        {
            elevator.CurrentFloor += elevator.Status == ElevatorStatus.MovingUp ? 1 : -1;
            await Task.Delay(1000 / elevator.Speed);
        }

        elevator.Status = ElevatorStatus.Idle;
        // Czekaj 1.5 sekundy po osiągnięciu celu, zanim winda zacznie wracać na parter
        await Task.Delay(1500);

        // Winda wraca na parter
        await MoveElevatorToGround(elevatorId);
    }

    public async Task MoveElevatorToGround(int elevatorId)
    {
        var elevator = _elevators.FirstOrDefault(e => e.Id == elevatorId);
        if (elevator == null) return;

        if (elevator.CurrentFloor != 0)
        {
            elevator.Status = ElevatorStatus.MovingDown;
            while (elevator.CurrentFloor > 0)
            {
                elevator.CurrentFloor--;
                await Task.Delay(1000 / elevator.Speed);
            }
        }

        elevator.Status = ElevatorStatus.Idle;
    }
   
    public async Task LoadPassengers(int elevatorId, List<Model.Passenger> passengers, int loadTime)
    {
        var elevator = _elevators.FirstOrDefault(e => e.Id == elevatorId);
        if (elevator == null) return;

        elevator.Status = ElevatorStatus.LoadingPassengers;
        foreach (var passenger in passengers)
        {
            if (elevator.Passengers.Count < elevator.Capacity)
            {
                elevator.Passengers.Add(passenger);
                await Task.Delay(loadTime * 1000); // czas załadunku
            }
        }
        elevator.Status = ElevatorStatus.Idle;
    }

    public async Task UnloadPassengers(int elevatorId, List<Model.Passenger> passengers, int loadTime)
    {
        var elevator = _elevators.FirstOrDefault(e => e.Id == elevatorId);
        if (elevator == null) return;

        elevator.Status = ElevatorStatus.UnloadingPassengers;
        foreach (var passenger in passengers)
        {
            if (elevator.Passengers.Contains(passenger))
            {
                elevator.Passengers.Remove(passenger);
                await Task.Delay(loadTime * 1000); // czas rozładunku
            }
        }
        elevator.Status = ElevatorStatus.Idle;
    }

    public ElevatorStatus GetElevatorStatus(int elevatorId) => _elevators.FirstOrDefault(e => e.Id == elevatorId)?.Status ?? ElevatorStatus.Idle;

    public List<Model.Passenger> GetPassengersInElevator(int elevatorId) => _elevators.FirstOrDefault(e => e.Id == elevatorId)?.Passengers ?? new List<Model.Passenger>();
    public List<int> GetElevatorPositions()
    {
        return _elevators.Select(x => x.CurrentFloor).ToList();
    }

    public void QueueMovement(int elevatorIndex, int targetFloor)
    {
        // Logika kolejkowania może być dodana później
    }
}