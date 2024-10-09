namespace ElevatorSimulator.Config;

public class ConfigurationService : IConfigurationService
{
    public int NumberOfFloors { get; set; } = 10;
    public int NumberOfElevators { get; set; } = 2;
    private Dictionary<int, int> _elevatorSpeeds = new Dictionary<int, int>();

    public int GetSpeed(int elevatorId)
    {
        if (!_elevatorSpeeds.ContainsKey(elevatorId))
        {
            _elevatorSpeeds[elevatorId] = 1; // Domyślna prędkość
        }
        return _elevatorSpeeds[elevatorId];
    }

    public void SetSpeed(int elevatorId, int speed)
    {
        _elevatorSpeeds[elevatorId] = speed;
    }
}