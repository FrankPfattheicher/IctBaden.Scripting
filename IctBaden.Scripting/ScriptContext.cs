using System.Collections.Generic;
using IctBaden.Framework.Types;
// ReSharper disable UnusedMember.Global

namespace IctBaden.Scripting
{
    public class ScriptContext
    {
        private readonly Dictionary<string, object> _valueList;

        public ScriptContext()
        {
            _valueList = new Dictionary<string, object>();
        }

        public void Clear()
        {
            _valueList.Clear();
        }

        public object this[string key]
        {
            set => _valueList[key] = value;
            get => _valueList.ContainsKey(key) ? _valueList[key] : null;
        }

        public T GetValue<T>(string key)
        {
            return (T)UniversalConverter.ConvertToType(this[key], typeof(T));
        }

        public void SetValue(string key, object value) => this[key] = value;

    }
}
