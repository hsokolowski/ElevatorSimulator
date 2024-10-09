using ElevatorSimulator.Model;

namespace ElevatorSimulator.Controller;

public interface IElevatorController
{
    void RegisterElevator(IElevator elevator);
    void AddElevatorRequest(int floor);
    List<IElevator> GetElevators();
}