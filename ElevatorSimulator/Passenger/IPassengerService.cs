namespace EvelatorSimulator.Passenger;

public interface IPassengerService
{
    void AddPassenger(int currentFloor, int destinationFloor);
    List<Model.Passenger> GetPassengersAtFloor(int floor);
    Task AssignPassengersToElevator(int elevatorId, List<Model.Passenger> passengers);
}