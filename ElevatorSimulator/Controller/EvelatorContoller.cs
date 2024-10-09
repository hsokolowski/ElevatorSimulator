using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ElevatorSimulator.Model;
using ElevatorSimulator.Strategy;

namespace ElevatorSimulator.Controller
{
    public class ElevatorController : IElevatorController
    {
        private List<IElevator> _elevators = new List<IElevator>();
        private BlockingCollection<ElevatorRequest> _requests = new BlockingCollection<ElevatorRequest>();

        public event EventHandler<TimeSpan> ElevatorRequestCompleted;
        
        private IElevatorAssignmentStrategy _assignmentStrategy;
        
        public ElevatorController(IElevatorAssignmentStrategy assignmentStrategy)
        {
            _assignmentStrategy = assignmentStrategy;
            Task.Run(() => ProcessRequestsAsync());
        }
        
        public void RegisterElevator(IElevator elevator)
        {
            _elevators.Add(elevator);
        }

        public void AddElevatorRequest(int floor)
        {
            _requests.Add(new ElevatorRequest(floor));
        }
        
        private async Task ProcessRequestsAsync()
        {
            foreach (var request in _requests.GetConsumingEnumerable())
            {
                var selectedElevator = SelectElevatorForRequest(request.Floor);
                if (selectedElevator != null)
                {
                    selectedElevator.AddFloorRequest(request.Floor);

                    // Rejestrujemy czas oczekiwania
                    TimeSpan waitTime = DateTime.Now - request.RequestTime;
                    ElevatorRequestCompleted?.Invoke(this, waitTime);
                }
                else
                {
                    // Jeśli żadna winda nie jest dostępna, możemy odłożyć żądanie na później
                    await Task.Delay(500);
                    _requests.Add(request);
                }
            }
        }

        private IElevator SelectElevatorForRequest(int floor)
        {
            return _assignmentStrategy.SelectElevator(_elevators.ToList(), floor);
        }
        
        public List<IElevator> GetElevators()
        {
            return _elevators;
        }
        
        public IEnumerable<int> GetPendingRequests()
        {
            lock (_requests)
            {
                return _requests.Select(r => r.Floor).ToList();
            }
        }
    }
}