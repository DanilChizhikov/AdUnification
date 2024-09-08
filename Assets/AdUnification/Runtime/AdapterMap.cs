using System;
using System.Collections.Generic;

namespace DTech.AdUnification
{
    internal sealed class AdapterMap<TConfig>
        where TConfig : IAdConfig
    {
        private readonly List<AdAdapter<TConfig>> _adapters;
        private readonly Dictionary<Type, int> _adapterIndexMap;

        public int Count => _adapters.Count;
        public AdAdapter<TConfig> this[int index] => _adapters[index];

        public AdapterMap(IEnumerable<AdAdapter<TConfig>> adapters)
        {
            _adapters = new List<AdAdapter<TConfig>>(adapters.ThrowIfNull());
            _adapterIndexMap = new Dictionary<Type, int>(_adapters.Count);
            for (int i = 0; i < _adapters.Count; i++)
            {
                AdAdapter<TConfig> adapter = _adapters[i];
                _adapterIndexMap.Add(adapter.ServicedAdType, i);
            }
        }

        public bool TryGetAdapter(Type adType, out AdAdapter<TConfig> adapter)
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

        public void Clear()
        {
            _adapters.Clear();
            _adapterIndexMap.Clear();
        }
        
        private int GetAdapterIndex(Type adType)
        {
            int index = -1;
            int smallestWeight = int.MaxValue;
            for (int i = 0; i < _adapters.Count; i++)
            {
                AdAdapter<TConfig> adapter = _adapters[i];
                if (!adapter.ServicedAdType.IsAssignableFrom(adType))
                {
                    continue;
                }

                int weight = adapter.ServicedAdType.Comparison(adType);
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