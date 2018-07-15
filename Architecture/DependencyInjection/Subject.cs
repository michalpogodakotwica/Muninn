using UnityEngine;
using UnityEngine.Events;

namespace Architecture.DependencyInjection
{
    [CreateAssetMenu(menuName = "Architecture/Subject")]
    public class Subject : ScriptableObject
    {
        public delegate void EventHandler();
        public event EventHandler OnRaiseEvent;

        public UnityEvent EarlyResponse;
        public UnityEvent LateResponse;

        public void Invoke()
        {
            EarlyResponse.Invoke();
            OnRaiseEvent?.Invoke();
            LateResponse.Invoke();
        }
    }
}