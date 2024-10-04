using System.Collections.Concurrent;
using EvelatorSimulator.Model;

namespace EvelatorSimulator.Controller;

public class ElevatorController : IElevatorController
{
    private List<IElevator> _elevators = new List<IElevator>();
    private BlockingCollection<int> _floorRequests = new BlockingCollection<int>();

    public ElevatorController()
    {
        Task.Run(() => ProcessFloorRequestsAsync());
    }

    public void RegisterElevator(IElevator elevator)
    {
        _elevators.Add(elevator);
    }

    public void RequestElevator(int floor)
    {
        _floorRequests.Add(floor);
    }

    private async Task ProcessFloorRequestsAsync()
    {
        foreach (var floor in _floorRequests.GetConsumingEnumerable())
        {
            var selectedElevator = SelectElevatorForRequest(floor);
            if (selectedElevator != null)
            {
                selectedElevator.AddFloorRequest(floor);
            }
            else
            {
                // All elevators are busy, re-enqueue the request
                _floorRequests.Add(floor);
                await Task.Delay(1000); // Wait before retrying
            }
        }
    }

    private IElevator SelectElevatorForRequest(int floor)
    {
        // Implement your assignment algorithm here
        return _elevators
            .Where(e => e.Status == ElevatorStatus.Idle)
            .OrderBy(e => Math.Abs(e.CurrentFloor - floor))
            .FirstOrDefault();
    }

    public List<IElevator> GetElevators()
    {
        return _elevators;
    }
}