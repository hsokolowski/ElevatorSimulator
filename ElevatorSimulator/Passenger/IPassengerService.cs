namespace EvelatorSimulator.Passenger;

public interface IPassengerService
{
    void AddPassenger(Model.Passenger passenger);
    List<Model.Passenger> GetPassengersAtFloor(int floor);
    void RemovePassengersFromFloor(int floor, List<Model.Passenger> passengers);
}