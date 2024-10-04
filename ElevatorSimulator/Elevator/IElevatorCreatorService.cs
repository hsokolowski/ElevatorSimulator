namespace EvelatorSimulator.Elevator;

public interface IElevatorCreatorService
{
    Model.Elevator CreateDefaultElevator(int id); // Tworzy windę z domyślnymi parametrami
    Model.Elevator CreateElevator(int id, int speed, int capacity); // Tworzy windę na podstawie parametrów
    List<Model.Elevator> CreateMultipleElevators(int count, int speed, int capacity); // Tworzy wiele wind
}