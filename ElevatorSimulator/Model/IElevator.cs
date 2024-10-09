namespace ElevatorSimulator.Model
{
    public interface IElevator
    {
        int Id { get; }
        int CurrentFloor { get; }
        ElevatorStatus Status { get; }
        int Speed { get; }

        void Start();
        void Stop();
        void AddFloorRequest(int floor);
        void UpdateSpeed(int newSpeed);

        event EventHandler<string> ElevatorLog;
    }
}