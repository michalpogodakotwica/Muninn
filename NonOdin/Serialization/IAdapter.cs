using Patterns.Variables;

namespace Serialization
{
    public interface IAdapter<TDataFormat>
    {
        void FromData(Variable[] template, TDataFormat data);
        TDataFormat ToData(Variable[] template);
    }
}