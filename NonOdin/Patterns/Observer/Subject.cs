using UnityEngine;
using UnityEngine.Events;

namespace Patterns.Observer
{
    [CreateAssetMenu(menuName = "Patterns/Subject")]
    public class Subject : ScriptableObject
    {
        public delegate void EventHandler();

        public UnityEvent EarlyResponse;
        public UnityEvent LateResponse;
        public event EventHandler OnRaiseEvent;

        public void Invoke()
        {
            EarlyResponse.Invoke();
            if (OnRaiseEvent != null)
                OnRaiseEvent.Invoke();
            LateResponse.Invoke();
        }
    }
}