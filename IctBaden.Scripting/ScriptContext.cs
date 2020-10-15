using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace IctBaden.Scripting
{
    [ComVisible(true)]
    public class ScriptContext
    {
        private readonly Dictionary<string, object> _valueList;
        private QueryValue _queryValue;

        public delegate object QueryValue(string key);

        public ScriptContext()
        {
            _valueList = new Dictionary<string, object>();
            _queryValue = null;
        }

        public void Clear()
        {
            _valueList.Clear();
        }

        public void SetQuery(QueryValue query)
        {
            _queryValue = query;
        }

        public object this[string key]
        {
            set => _valueList[key] = value;
            get
            {
                var val = _queryValue?.Invoke(key);
                if (val != null)
                    return val;
                return _valueList.ContainsKey(key) ? _valueList[key] : null;
            }
        }


    }
}
