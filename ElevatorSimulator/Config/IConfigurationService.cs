namespace EvelatorSimulator.Config;

public interface IConfigurationService
{
    int NumberOfFloors { get; set; }
    int NumberOfElevators { get; set; }
    int LoadUnloadTime { get; set; }

    int GetSpeed(int elevatorId);
    void SetSpeed(int elevatorId, int speed);
    int GetElevatorCapacity(int elevatorId);
    void SetElevatorCapacity(int elevatorId, int capacity);
}