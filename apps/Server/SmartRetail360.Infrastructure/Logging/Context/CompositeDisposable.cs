namespace SmartRetail360.Infrastructure.Logging.Context;

public class CompositeDisposable : IDisposable
{
    private readonly IEnumerable<IDisposable> _disposables;

    public CompositeDisposable(IEnumerable<IDisposable> disposables)
    {
        _disposables = disposables;
    }

    public void Dispose()
    {
        foreach (var d in _disposables)
        {
            d.Dispose();
        }
    }
}