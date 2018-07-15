using Sirenix.OdinInspector;
using UnityEngine;

namespace Architecture.DependencyInjection
{
    public class Registrator : SerializedMonoBehaviour
    {
        [SerializeField] private bool _activeOnly;
        [SerializeField] private IRegistrable _registrable;

        private void Start()
        {
            if (!_activeOnly)
                _registrable.Register(this);
        }

        private void OnEnable()
        {
            if (_activeOnly)
                _registrable.Register(this);
        }

        private void OnDisable()
        {
            if (_activeOnly)
                _registrable.Unregister(this);
        }

        private void OnDestroy()
        {
            if (!_activeOnly)
                _registrable.Unregister(this);
        }
    }
}