using EvelatorSimulator.Model;

namespace EvelatorSimulator.Controller;

public interface IElevatorController
{
    void RequestElevator(int floor);
    void RegisterElevator(IElevator elevator);
    List<IElevator> GetElevators();
}