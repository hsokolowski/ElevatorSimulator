using EvelatorSimulator.Model;

namespace EvelatorSimulator.Movement;

public interface IMovementService
{
    void InitializeElevators(List<Model.Elevator> elevators);
    Task MoveElevator(int elevatorId, int targetFloor); // ruch windy na wybrane piętro
    Task LoadPassengers(int elevatorId, List<Model.Passenger> passengers, int loadTime); // załaduj pasażerów
    Task UnloadPassengers(int elevatorId, List<Model.Passenger> passengers, int loadTime); // rozładuj pasażerów
    ElevatorStatus GetElevatorStatus(int elevatorId);
    List<Model.Passenger> GetPassengersInElevator(int elevatorId);
    List<int> GetElevatorPositions();
    void QueueMovement(int elevatorIndex, int targetFloor);
}