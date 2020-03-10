using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace IctBaden.Scripting.Library
{
    [ComVisible(true)]
    public class ScriptContext
    {
        private readonly Dictionary<string, object> valueList;
        private QueryValue queryValue;

        public delegate object QueryValue(string key);

        public ScriptContext()
        {
            valueList = new Dictionary<string, object>();
            queryValue = null;
        }

        public void Clear()
        {
            valueList.Clear();
        }

        public void SetQuery(QueryValue query)
        {
            queryValue = query;
        }

        public object this[string key]
        {
            set => valueList[key] = value;
            get
            {
                var val = queryValue?.Invoke(key);
                if (val != null)
                    return val;
                return valueList.ContainsKey(key) ? valueList[key] : null;
            }
        }


    }
}
