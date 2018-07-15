using UnityEngine;
using UnityEngine.Events;

namespace Patterns.Observer
{
    public class Observer : MonoBehaviour
    {
        [SerializeField] private bool _raiseEvenetOnEnable;
        [SerializeField] private Subject _subject;
        public UnityEvent Response;

        private void OnEnable()
        {
            if (_raiseEvenetOnEnable)
                OnEventRaise();
            if (_subject != null)
                Subscribe();
        }

        private void OnDisable()
        {
            if (_subject != null)
                Unsubscribe();
        }

        public void SetSubject(Subject subject)
        {
            if (isActiveAndEnabled)
                OnDisable();
            _subject = subject;
            if (isActiveAndEnabled)
                OnEnable();
        }

        private void Subscribe()
        {
            _subject.OnRaiseEvent += OnEventRaise;
        }

        private void Unsubscribe()
        {
            _subject.OnRaiseEvent -= OnEventRaise;
        }

        public void OnEventRaise()
        {
            Response.Invoke();
        }
    }
}