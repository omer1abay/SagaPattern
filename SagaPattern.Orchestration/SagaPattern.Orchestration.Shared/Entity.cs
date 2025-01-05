namespace SagaPattern.Orchestration.Shared;

public abstract class Entity
{
    private readonly List<IDomainEvent> _domainEvents = [];

    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    public void Publish(IDomainEvent domainEvent) => _domainEvents.Add(domainEvent);

    public void ClearEvents() => _domainEvents.Clear(); 

}
