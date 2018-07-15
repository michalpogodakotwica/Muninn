using UnityEngine;

namespace Architecture.DependencyInjection
{
    public interface IRegistrable
    {
        void Register(MonoBehaviour registrator);
        void Unregister(MonoBehaviour registrator);
    }
}