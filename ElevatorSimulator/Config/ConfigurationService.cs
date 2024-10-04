namespace EvelatorSimulator.Config;

public class ConfigurationService : IConfigurationService
{
    public int NumberOfFloors { get; set; } = 5;
    public int NumberOfElevators { get; set; } = 2;
    public int LoadUnloadTime { get; set; } = 1;

    private Dictionary<int, int> _elevatorSpeeds = new Dictionary<int, int>();
    private Dictionary<int, int> _elevatorCapacities = new Dictionary<int, int>();

    public int GetSpeed(int elevatorId)
    {
        if (!_elevatorSpeeds.ContainsKey(elevatorId))
        {
            _elevatorSpeeds[elevatorId] = 1; // Default speed
        }
        return _elevatorSpeeds[elevatorId];
    }

    public void SetSpeed(int elevatorId, int speed)
    {
        _elevatorSpeeds[elevatorId] = speed;
    }

    public int GetElevatorCapacity(int elevatorId)
    {
        if (!_elevatorCapacities.ContainsKey(elevatorId))
        {
            _elevatorCapacities[elevatorId] = 10; // Default capacity
        }
        return _elevatorCapacities[elevatorId];
    }

    public void SetElevatorCapacity(int elevatorId, int capacity)
    {
        _elevatorCapacities[elevatorId] = capacity;
    }
}