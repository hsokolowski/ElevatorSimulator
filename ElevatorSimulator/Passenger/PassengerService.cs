namespace EvelatorSimulator.Passenger;

public class PassengerService : IPassengerService
{
    private readonly Dictionary<int, List<Model.Passenger>> _passengersAtFloors = new();

    public void AddPassenger(int currentFloor, int destinationFloor)
    {
        var passenger = new Model.Passenger
        {
            CurrentFloor = currentFloor,
            DestinationFloor = destinationFloor,
            TimeToLoad = 1, // domyślny czas wsiadania
            TimeToUnload = 1 // domyślny czas wysiadania
        };

        if (!_passengersAtFloors.ContainsKey(currentFloor))
        {
            _passengersAtFloors[currentFloor] = new List<Model.Passenger>();
        }

        _passengersAtFloors[currentFloor].Add(passenger);
    }

    public List<Model.Passenger> GetPassengersAtFloor(int floor)
    {
        return _passengersAtFloors.ContainsKey(floor) ? _passengersAtFloors[floor] : new List<Model.Passenger>();
    }

    public async Task AssignPassengersToElevator(int elevatorId, List<Model.Passenger> passengers)
    {
        // Implementacja przydzielania pasażerów do windy
        // Można to połączyć z serwisem zarządzającym windami (ElevatorControlService)
    }
}