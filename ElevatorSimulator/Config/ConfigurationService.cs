namespace EvelatorSimulator.Config;

public class ConfigurationService : IConfigurationService
{
    private int _numberOfFloors;
    private int _numberOfElevators;
    private Dictionary<int, int> _elevatorSpeeds = new Dictionary<int, int>();
    private Dictionary<int, int> _elevatorCapacities = new Dictionary<int, int>();
    private int _loadUnloadTime;

    public ConfigurationService()
    {
        _numberOfElevators = 2;
        _numberOfFloors = 5;
    }
    public void SetNumberOfFloors(int floors) => _numberOfFloors = floors;
    public int GetNumberOfFloors() => _numberOfFloors;

    public void SetNumberOfElevators(int count)
    {
        _numberOfElevators = count;
        // Dodaj domyślne prędkości i pojemności dla nowych wind
        for (int i = 0; i < _numberOfElevators; i++)
        {
            if (!_elevatorSpeeds.ContainsKey(i))
            {
                _elevatorSpeeds[i] = 1; // Domyślna prędkość
            }

            if (!_elevatorCapacities.ContainsKey(i))
            {
                _elevatorCapacities[i] = 10; // Domyślna pojemność
            }
        }
    }
    public int GetNumberOfElevators() => _numberOfElevators;

    public void SetSpeed(int elevatorId, int speed) => _elevatorSpeeds[elevatorId] = speed;
    public int GetSpeed(int elevatorId)
    {
        if (!_elevatorSpeeds.ContainsKey(elevatorId))
        {
            _elevatorSpeeds[elevatorId] = 1; // Domyślna prędkość
        }

        return _elevatorSpeeds[elevatorId];
    }

    public void SetElevatorCapacity(int elevatorId, int capacity) => _elevatorCapacities[elevatorId] = capacity;
    public int GetElevatorCapacity(int elevatorId)
    {
        if (!_elevatorCapacities.ContainsKey(elevatorId))
        {
            _elevatorCapacities[elevatorId] = 10; // Domyślna pojemność, jeśli nie ustawiono
        }
        
        return _elevatorCapacities[elevatorId];
    }

    public void SetLoadUnloadTime(int timeInSeconds) => _loadUnloadTime = timeInSeconds;
    public int GetLoadUnloadTime() => _loadUnloadTime;
}