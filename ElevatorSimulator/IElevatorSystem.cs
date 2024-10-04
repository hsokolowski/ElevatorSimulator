using EvelatorSimulator.Config;
using EvelatorSimulator.Model;
using EvelatorSimulator.Passenger;

namespace EvelatorSimulator;

public interface IElevatorSystem
{
    void InitializeSystem();
    void ShutdownSystem();

    void RequestElevator(int floor);
    List<IElevator> GetElevators();
    IConfigurationService Configuration { get; }
    IPassengerService PassengerService { get; }
}