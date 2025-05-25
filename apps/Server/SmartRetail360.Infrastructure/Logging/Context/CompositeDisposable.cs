namespace SmartRetail360.Infrastructure.Logging.Context;

public sealed class CompositeDisposable : IDisposable
{
    private readonly IEnumerable<IDisposable> _disposables;
    private bool _disposed;

    public CompositeDisposable(IEnumerable<IDisposable> disposables)
    {
        _disposables = disposables;
    }

    public void Dispose()
    {
        if (_disposed) return;

        foreach (var d in _disposables)
        {
            d.Dispose();
        }

        _disposed = true;
    }
}