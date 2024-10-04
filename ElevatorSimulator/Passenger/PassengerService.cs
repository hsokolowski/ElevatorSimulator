namespace EvelatorSimulator.Passenger;

public class PassengerService : IPassengerService
{
    private Dictionary<int, List<Model.Passenger>> _passengersAtFloors = new Dictionary<int, List<Model.Passenger>>();

    public void AddPassenger(Model.Passenger passenger)
    {
        if (!_passengersAtFloors.ContainsKey(passenger.CurrentFloor))
        {
            _passengersAtFloors[passenger.CurrentFloor] = new List<Model.Passenger>();
        }
        _passengersAtFloors[passenger.CurrentFloor].Add(passenger);
    }

    public List<Model.Passenger> GetPassengersAtFloor(int floor)
    {
        return _passengersAtFloors.ContainsKey(floor) ? _passengersAtFloors[floor] : new List<Model.Passenger>();
    }

    public void RemovePassengersFromFloor(int floor, List<Model.Passenger> passengers)
    {
        if (_passengersAtFloors.ContainsKey(floor))
        {
            foreach (var passenger in passengers)
            {
                _passengersAtFloors[floor].Remove(passenger);
            }
        }
    }
}