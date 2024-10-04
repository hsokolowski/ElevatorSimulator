using EvelatorSimulator.Model;

namespace EvelatorSimulator.Elevator;

public class ElevatorCreatorService : IElevatorCreatorService
{

    public Model.Elevator CreateDefaultElevator(int id)
    {
        return new Model.Elevator
        {
            Id = id,
            CurrentFloor = 0,
            TargetFloor = 0,
            Speed = 1, // Domyślna prędkość
            Capacity = 10, // Domyślna pojemność
            Status = ElevatorStatus.Idle,
            Passengers = new List<Model.Passenger>()
        };
    }
    public Model.Elevator CreateElevator(int id, int speed, int capacity)
    {
        return new Model.Elevator
        {
            Id = id,
            CurrentFloor = 0,
            TargetFloor = 0,
            Speed = speed, 
            Capacity = capacity,
            Status = ElevatorStatus.Idle,
            Passengers = new List<Model.Passenger>()
        };
    }
    public List<Model.Elevator> CreateMultipleElevators(int count, int speed, int capacity)
    {
        List<Model.Elevator> elevators = new List<Model.Elevator>();
        for (int i = 0; i < count; i++)
        {
            elevators.Add(CreateElevator(i, speed, capacity));
        }
        return elevators;
    }
}