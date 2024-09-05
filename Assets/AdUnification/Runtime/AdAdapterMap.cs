using System;
using System.Collections.Generic;

namespace DTech.AdUnification
{
    internal sealed class AdAdapterMap<TConfig, TAdapter>
        where TConfig : IAdConfig
        where TAdapter : AdAdapter<TConfig>
    {
        private readonly List<TAdapter> _adapters;
        private readonly Dictionary<Type, int> _adapterIndexMap;

        public int Count => _adapters.Count;
        public TAdapter this[int index] => _adapters[index];

        public AdAdapterMap(IEnumerable<TAdapter> collection)
        {
            _adapters = new List<TAdapter>(collection.ThrowIfNull());
            _adapterIndexMap = new Dictionary<Type, int>();
            for (int i = 0; i < _adapters.Count; i++)
            {
                IAdAdapter adapter = _adapters[i];
                _adapterIndexMap.Add(adapter.ServicedRequestType, i);
            }
        }

        public bool TryGetAdapter(Type adType, out TAdapter adapter)
        {
            bool hasAdapter = _adapterIndexMap.TryGetValue(adType, out int index);
            if (!hasAdapter)
            {
                index = GetAdapterIndex(adType);
                hasAdapter = index >= 0;
                if (hasAdapter)
                {
                    _adapterIndexMap.Add(adType, index);
                }
            }

            adapter = hasAdapter ? _adapters[index] : null;
            return hasAdapter;
        }
        
        private int GetAdapterIndex(Type adType)
        {
            int index = -1;
            int smallestWeight = int.MaxValue;
            for (int i = 0; i < _adapters.Count; i++)
            {
                TAdapter adapter = _adapters[i];
                if (!adapter.IsInitialized || !adapter.ServicedRequestType.IsAssignableFrom(adType))
                {
                    continue;
                }

                int weight = adapter.ServicedRequestType.Comparison(adType);
                if (weight <= smallestWeight)
                {
                    smallestWeight = weight;
                    index = i;
                }
            }
            
            return index;
        }
    }
}