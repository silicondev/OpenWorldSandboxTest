using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Source.Systems
{
    public static class KeyboardHelper
    {
        private static Dictionary<KeyCode, bool> _keys = new Dictionary<KeyCode, bool>();
        private static Dictionary<KeyCode, int> _keyQueue = new Dictionary<KeyCode, int>();

        public static bool CheckKey(KeyCode key)
        {
            if (Input.GetKey(key))
            {
                if (!_keys.ContainsKey(key) || !_keys[key])
                {
                    _keys[key] = true;
                    return true;
                }
                else
                    return false;
            }
            else
            {
                _keys[key] = false;
                return false;
            }
        }

        public static KeyCode[] ProcessKeyQueue()
        {
            var newDict = new Dictionary<KeyCode, int>();
            var outList = new List<KeyCode>();
            foreach (var kvp in _keyQueue)
            {
                int newVal = kvp.Value - 1;
                if (newVal <= 0)
                    outList.Add(kvp.Key);
                else
                    newDict.Add(kvp.Key, newVal);
            }
            _keyQueue = newDict;
            return outList.ToArray();
        }

        public static void AddToQueue(KeyCode code, int wait)
        {
            if (!_keyQueue.ContainsKey(code))
                _keyQueue.Add(code, wait);
        }

        public static KeyCode[] GetKeys(params KeyCode[] checkKeys)
        {
            var list = new List<KeyCode>();
            foreach (var checkKey in checkKeys)
            {
                if (Input.GetKey(checkKey))
                    list.Add(checkKey);
            }
            return list.ToArray();
        }

        public static KeyCode[] GetPressKeys(params KeyCode[] checkKeys)
        {
            var list = new List<KeyCode>();
            foreach (var key in checkKeys)
            {
                if (CheckKey(key))
                    list.Add(key);
            }
            return list.ToArray();
        }
    }
}
