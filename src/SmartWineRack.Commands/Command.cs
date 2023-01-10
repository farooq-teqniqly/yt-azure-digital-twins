using SmartWineRack.Data.Repositories;

namespace SmartWineRack.Commands;

public abstract class Command<TResult>
{
    protected IRepository Repository { get; }

    protected Command(IRepository repository)
    {
        Repository = repository;
    }

    public abstract Task<TResult> Execute(IDictionary<string, object>? parameters = null);
}