namespace ElevatorSimulator.Model
{
    public enum ElevatorStatus
    {
        Idle,
        MovingUp,
        MovingDown,
        Waiting
    }

    public class Elevator : IElevator
    {
        public int Id { get; }
        public int CurrentFloor { get; private set; }
        public ElevatorStatus Status { get; private set; }
        public int Speed { get; private set; } // Prędkość windy (piętra na sekundę)

        private SortedSet<int> _targetFloors = new SortedSet<int>();
        private CancellationTokenSource _cts;
        private Task _elevatorTask;

        // Zdarzenie do logowania
        public event EventHandler<string> ElevatorLog;

        public Elevator(int id, int speed)
        {
            Id = id;
            Speed = speed;
            CurrentFloor = 0;
            Status = ElevatorStatus.Idle;
        }

        public void Start()
        {
            _cts = new CancellationTokenSource();
            _elevatorTask = Task.Run(() => RunAsync(_cts.Token));
        }

        public void Stop()
        {
            if (_cts != null && !_cts.IsCancellationRequested)
            {
                _cts.Cancel();
            }

            try
            {
                _elevatorTask?.Wait();
            }
            catch (AggregateException ae)
            {
                ae.Handle(ex => ex is TaskCanceledException);
            }
        }

        public void AddFloorRequest(int floor)
        {
            lock (_targetFloors)
            {
                _targetFloors.Add(floor);
                Log($"Winda {Id}: Dodano żądanie piętra {floor}");
            }
        }

        private async Task RunAsync(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                int? nextFloor = null;
                lock (_targetFloors)
                {
                    if (_targetFloors.Count > 0)
                    {
                        nextFloor = GetNextFloor();
                        _targetFloors.Remove(nextFloor.Value);
                    }
                }

                if (nextFloor.HasValue)
                {
                    await MoveToFloorAsync(nextFloor.Value, token);
                }
                else
                {
                    Status = ElevatorStatus.Idle;
                    await Task.Delay(500, token);
                }
            }
        }

        private int GetNextFloor()
        {
            // Możesz zaimplementować własną logikę wyboru następnego piętra
            return _targetFloors.Min;
        }

        private async Task MoveToFloorAsync(int targetFloor, CancellationToken token)
        {
            Status = targetFloor > CurrentFloor ? ElevatorStatus.MovingUp : ElevatorStatus.MovingDown;
            Log($"Winda {Id}: Rozpoczyna ruch z piętra {CurrentFloor} na piętro {targetFloor}");

            while (CurrentFloor != targetFloor)
            {
                if (token.IsCancellationRequested) break;

                // Symulacja czasu potrzebnego na przemieszczenie się między piętrami
                await Task.Delay(1000 / Speed, token);

                CurrentFloor += Status == ElevatorStatus.MovingUp ? 1 : -1;
                Log($"Winda {Id}: Obecne piętro {CurrentFloor}");
            }

            Status = ElevatorStatus.Waiting;
            Log($"Winda {Id}: Dotarła na piętro {CurrentFloor}");

            // Symulacja czasu otwierania/zamykania drzwi
            await Task.Delay(1000, token);

            Status = ElevatorStatus.Idle;
        }

        private void Log(string message)
        {
            ElevatorLog?.Invoke(this, message);
        }

        public void UpdateSpeed(int newSpeed)
        {
            Speed = newSpeed;
            Log($"Winda {Id}: Zmieniono prędkość na {Speed}");
        }
    }
}