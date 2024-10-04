using EvelatorSimulator.Model;

namespace EvelatorSimulator;

public interface IElevatorSystem
{
    // Konfiguracja systemu
    void SetNumberOfFloors(int floors);
    int GetNumberOfFloors();
    
    void SetNumberOfElevators(int count);
    int GetNumberOfElevators();

    void SetSpeed(int elevatorId, int speed);
    int GetSpeed(int elevatorId);

    void SetElevatorCapacity(int elevatorId, int capacity);
    int GetElevatorCapacity(int elevatorId);

    void SetLoadUnloadTime(int timeInSeconds);
    int GetLoadUnloadTime();

    // Sterowanie windami
    Task MoveElevator(int elevatorId, int targetFloor);
    ElevatorStatus GetElevatorStatus(int elevatorId);
    
    // Pasażerowie
    void AddPassenger(int currentFloor, int destinationFloor);
    Task AssignPassengersToElevator(int elevatorId);
    Task UnloadPassengersFromElevator(int elevatorId);

    // Informacje o windach i pasażerach
    List<int> GetElevatorPositions(); // Zwraca listę bieżących pięter, na których znajdują się windy
    List<Model.Passenger> GetPassengersInElevator(int elevatorId);
    
    // Inne
    void AddElevator();          // Dodawanie windy
    void RemoveElevator();       // Usuwanie windy
}