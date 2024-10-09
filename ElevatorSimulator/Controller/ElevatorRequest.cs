namespace ElevatorSimulator.Controller;

public class ElevatorRequest
{
    public int Floor { get; }
    public DateTime RequestTime { get; }

    public ElevatorRequest(int floor)
    {
        Floor = floor;
        RequestTime = DateTime.Now;
    }
}