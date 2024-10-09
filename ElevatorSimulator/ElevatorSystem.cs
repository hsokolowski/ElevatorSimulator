using ElevatorSimulator.Config;
using ElevatorSimulator.Controller;
using ElevatorSimulator.Model;
using ElevatorSimulator.Strategy;

namespace ElevatorSimulator
{
    public class ElevatorSystem : IElevatorSystem
    {
        private readonly IElevatorAssignmentStrategy _assignmentStrategy;
        public IConfigurationService Configuration { get; }

        private ElevatorController _elevatorController;
        private List<IElevator> _elevators;

        public ElevatorSystem(IConfigurationService config, IElevatorAssignmentStrategy assignmentStrategy)
        {
            _assignmentStrategy = assignmentStrategy;
            Configuration = config;
            _elevatorController = new ElevatorController(_assignmentStrategy);
            _elevators = new List<IElevator>();
        }

        public void InitializeSystem()
        {
            // Zatrzymaj istniejące windy, jeśli są
            foreach (var elevator in _elevators)
            {
                elevator.Stop();
            }

            // Wyczyść listę wind i kontroler
            _elevators.Clear();
            _elevatorController = new ElevatorController(_assignmentStrategy);

            // Tworzenie nowych wind na podstawie konfiguracji
            for (int i = 0; i < Configuration.NumberOfElevators; i++)
            {
                int speed = Configuration.GetSpeed(i);
                var elevator = new Elevator(i, speed);
                _elevators.Add(elevator);
                _elevatorController.RegisterElevator(elevator);
                elevator.Start();
            }
        }

        public void ShutdownSystem()
        {
            foreach (var elevator in _elevators)
            {
                elevator.Stop();
            }
            _elevators.Clear();
            _elevatorController = null;
        }

        public void RequestElevator(int floor)
        {
            _elevatorController.AddElevatorRequest(floor);
        }

        public List<IElevator> GetElevators()
        {
            return _elevatorController.GetElevators();
        }
        
        public event EventHandler<TimeSpan> ElevatorRequestCompleted
        {
            add { _elevatorController.ElevatorRequestCompleted += value; }
            remove { _elevatorController.ElevatorRequestCompleted -= value; }
        }
        
        public IEnumerable<int> GetPendingRequests()
        {
            return _elevatorController.GetPendingRequests();
        }
    }
}