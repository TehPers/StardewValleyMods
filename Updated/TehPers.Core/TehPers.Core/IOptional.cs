namespace TehPers.Core {
    public interface IOptional<T>
    {
        bool HasValue { get; }

        T Value { get; }

        bool TryGetValue(out T value);
    }
}