namespace ElevatorSimulator.Config;

public interface IConfigurationService
{
    int NumberOfFloors { get; set; }
    int NumberOfElevators { get; set; }
    int GetSpeed(int elevatorId);
    void SetSpeed(int elevatorId, int speed);
}