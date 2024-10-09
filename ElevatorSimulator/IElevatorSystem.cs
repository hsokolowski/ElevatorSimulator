using ElevatorSimulator.Config;
using ElevatorSimulator.Model;

namespace ElevatorSimulator;

public interface IElevatorSystem
{
    void InitializeSystem();
    void ShutdownSystem();
    void RequestElevator(int floor);
    List<IElevator> GetElevators();
    IConfigurationService Configuration { get; }
}