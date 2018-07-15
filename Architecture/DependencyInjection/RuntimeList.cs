using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Architecture.DependencyInjection
{
    [CreateAssetMenu(menuName = "Architecture/RuntimeList")]
    public class RuntimeList : ScriptableObject, IEnumerable<GameObject>
    {
        private readonly List<GameObject> _list = new List<GameObject>();

        public IEnumerator<GameObject> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        public void Add(GameObject registration)
        {
            _list.Add(registration);
        }

        public void Remove(GameObject registration)
        {
            _list.Remove(registration);
        }
    }
}