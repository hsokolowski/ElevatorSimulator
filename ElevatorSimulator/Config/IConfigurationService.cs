namespace EvelatorSimulator.Config;

public interface IConfigurationService
{
    void SetNumberOfFloors(int floors);
    int GetNumberOfFloors();

    void SetNumberOfElevators(int count);
    int GetNumberOfElevators();

    void SetSpeed(int elevatorId, int speed);
    int GetSpeed(int elevatorId);

    void SetElevatorCapacity(int elevatorId, int capacity);
    int GetElevatorCapacity(int elevatorId);

    void SetLoadUnloadTime(int timeInSeconds); // Czas załadunku/rozładunku dla wszystkich wind
    int GetLoadUnloadTime();
}