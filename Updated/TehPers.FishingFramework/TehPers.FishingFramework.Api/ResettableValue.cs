namespace TehPers.FishingFramework.Api
{
    public class ResettableValue<T>
    {
        public T OriginalValue { get; }
        public T Value { get; set; }

        public ResettableValue(T originalValue)
        {
            this.OriginalValue = originalValue;
            this.Value = originalValue;
        }

        public void Reset()
        {
            this.Value = this.OriginalValue;
        }
    }
}