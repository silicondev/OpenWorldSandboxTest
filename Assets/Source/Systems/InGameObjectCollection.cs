using Assets.Source.Systems.Abstracts;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Source.Systems
{
    public class InGameObjectCollection : IEnumerable<InGameObject>
    {
        private List<InGameObject> _list = new();

        public InGameObject this[string key]
        {
            get => GetValue(key);
            set
            {
                _list.RemoveAll(x => x.Name == key);
                Add(value);
            }
        }

        public ICollection<string> Keys => _list.Select(x => x.Name).ToList();

        public ICollection<InGameObject> Values => new List<InGameObject>(_list);

        public int Count => _list.Count;

        public bool IsReadOnly => false;

        public bool ReplaceMode { get; set; }

        public InGameObjectCollection(List<InGameObject>? list = null)
        {
            if (list != null)
                _list = list;
        }

        public void Add(InGameObject value)
        {
            if (ContainsKey(value.Name))
            {
                if (ReplaceMode)
                    Remove(value.Name);
                else
                    return;
            }
            value.OnDispose += (object sender, InGameObjectEventArgs e) =>
            {
                Remove(value.Name);
            };
            _list.Add(value);
        }

        public void Clear()
        {
            _list.ForEach((x) => x.Dispose(false));
            _list.Clear();
        }

        public bool ContainsKey(string key) =>
            _list.Any(x => x.Name == key);

        public void CopyTo(InGameObject[] array, int arrayIndex) =>
            _list.CopyTo(array, arrayIndex);

        public void CopyTo(InGameObjectCollection collection) =>
            collection._list = new List<InGameObject>(this._list);

        public IEnumerator<InGameObject> GetEnumerator() =>
            _list.GetEnumerator();

        public bool Remove(string key)
        {
            if (ContainsKey(key))
            {
                var val = GetValue(key);
                val.Dispose(false);
                _list.Remove(val);
                return true;
            }
            return false;
        }

        public bool TryGetValue(string key, out InGameObject value)
        {
            value = GetValue(key);
            return value != null;
        }

        public InGameObject GetValue(string key) =>
            _list.FirstOrDefault(x => x.Name == key);

        IEnumerator IEnumerable.GetEnumerator() =>
            _list.GetEnumerator();
    }
}
