namespace Architecture.Variables
{
    public interface IValue<out T>
    {
        T Value { get; }
    }
}