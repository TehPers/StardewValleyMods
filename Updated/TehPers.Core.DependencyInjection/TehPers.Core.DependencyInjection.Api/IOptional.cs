namespace TehPers.Core.DependencyInjection.Api {
    public interface IOptional<T>
    {
        T Value { get; }
        bool HasValue { get; }
        bool TryGetValue(out T value);
    }
}