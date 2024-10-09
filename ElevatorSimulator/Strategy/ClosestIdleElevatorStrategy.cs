using ElevatorSimulator.Model;

namespace ElevatorSimulator.Strategy;

public class ClosestIdleElevatorStrategy : IElevatorAssignmentStrategy

{
    public IElevator SelectElevator(List<IElevator> elevators, int requestedFloor)
    {
        return elevators
            .Where(e => e.Status == ElevatorStatus.Idle)
            .OrderBy(e => Math.Abs(e.CurrentFloor - requestedFloor))
            .FirstOrDefault();
    }
}