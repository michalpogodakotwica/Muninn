using System;
using UnityEngine;

namespace Architecture.DependencyInjection
{
    [Serializable]
    public class ListElement : IRegistrable
    {
        [SerializeField] private RuntimeList _list;

        public void Register(MonoBehaviour registrator)
        {
            _list.Add(registrator.gameObject);
        }

        public void Unregister(MonoBehaviour registrator)
        {
            _list.Remove(registrator.gameObject);
        }
    }
}