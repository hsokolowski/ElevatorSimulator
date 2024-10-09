using ElevatorSimulator.Model;

namespace ElevatorSimulator.Strategy;

public interface IElevatorAssignmentStrategy
{
    IElevator SelectElevator(List<IElevator> elevators, int requestedFloor);
}