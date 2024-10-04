namespace EvelatorSimulator.Model;

public interface IElevator
{
    int Id { get; }
    int CurrentFloor { get; }
    ElevatorStatus Status { get; }
    int Speed { get; }
    void Start();
    void Stop();
    void AddFloorRequest(int floor);
    Task MoveToFloorAsync(int floor, CancellationToken token);
    List<Passenger> GetPassengers();
    void LoadPassengers(List<Passenger> passengers);
    void UnloadPassengers();
    void UpdateSpeed(int result);
}