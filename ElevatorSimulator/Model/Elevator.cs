namespace EvelatorSimulator.Model;

public class Elevator
{
    public int Id { get; set; }
    public int CurrentFloor { get; set; }
    public int TargetFloor { get; set; }
    public int Speed { get; set; } = 1; // prędkość windy w piętrach na sekundę
    public int Capacity { get; set; } // maksymalna liczba pasażerów
    public List<Passenger> Passengers { get; set; } = new List<Passenger>();

    public ElevatorStatus Status { get; set; } = ElevatorStatus.Idle;
}

public enum ElevatorStatus
{
    Idle,      // Winda stoi
    Moving,    // Winda w ruchu
    Waiting,    // Winda czeka na piętrze
    MovingUp,
    MovingDown,
    LoadingPassengers,
    UnloadingPassengers
}

public class Passenger
{
    public int Id { get; set; }
    public int CurrentFloor { get; set; }
    public int DestinationFloor { get; set; }
    public int TimeToLoad { get; set; } // czas wsiadania do windy
    public int TimeToUnload { get; set; } // czas wysiadania z windy
}