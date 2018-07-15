using System;
using UnityEngine;
using UnityEngine.Events;

namespace Architecture.DependencyInjection
{
    [Serializable]
    public class Observer : IRegistrable
    {
        [SerializeField] private Subject _subject;
        [SerializeField] private UnityEvent _response;

        public void Register(MonoBehaviour registrator)
        {
            _subject.OnRaiseEvent += _response.Invoke;
        }

        public void Unregister(MonoBehaviour registrator)
        {
            _subject.OnRaiseEvent -= _response.Invoke;
        }
    }
}