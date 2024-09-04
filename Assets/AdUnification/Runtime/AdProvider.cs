using System;
using System.Collections.Generic;

namespace DTech.AdUnification
{
    public abstract class AdProvider<TConfig, TAdapter> : IAdProvider
            where TConfig : IAdConfig where TAdapter : AdAdapter<TConfig>
    {
        private readonly Dictionary<Type, int> _adapterIndexMap;
        private readonly List<TAdapter> _adapters;

        public event Action<IAdResponse> OnAdShown;
        public bool IsInitialized { get; private set; }

        public bool IsAnyAdShowing
        {
            get
            {
                for (int i = 0; i < _adapters.Count; i++)
                {
                    TAdapter adapter = _adapters[i];
                    if (adapter.IsAdShowing)
                    {
                        return true;
                    }
                }

                return false;
            }
        }
        
        public int Weight => Config.Weight;
        
        protected TConfig Config { get; }

        public AdProvider(TConfig config, IEnumerable<TAdapter> adapters)
        {
            Config = config.ThrowIfNull();
            _adapterIndexMap = new Dictionary<Type, int>();
            _adapters = new List<TAdapter>(adapters.ThrowIfNull());
            for (int i = 0; i < _adapters.Count; i++)
            {
                TAdapter adapter = _adapters[i];
                _adapterIndexMap.Add(adapter.ServicedRequestType, i);
            }
            
            IsInitialized = false;
        }
        
        public void Initialize()
        {
            if (IsInitialized)
            {
                return;
            }
            
            InitializeAdapters();
            InitializeProcessing();
            IsInitialized = true;
        }

        public bool IsAdReady<T>() where T : IAdRequest
        {
            if (!IsInitialized || !TryGetAdapter(typeof(T), out TAdapter adapter))
            {
                return false;
            }

            return adapter.IsAdReady;
        }

        public bool IsAdShowing<T>() where T : IAdRequest => IsAdShowing(typeof(T));

        public void ShowAd(IAdRequest request, Action<IAdResponse> callback)
        {
            if (!IsInitialized || !TryGetAdapter(request.GetType(), out TAdapter adapter))
            {
                var response = new SimpleResponse
                {
                    Request = request,
                    IsSuccessful = false,
                };
                
                callback?.Invoke(response);
                return;
            }
            
            adapter.ShowAd(request, callback);
        }

        public void HideAd<T>() where T : IAdRequest
        {
            if (!IsInitialized || !TryGetAdapter(typeof(T), out TAdapter adapter))
            {
                return;
            }
            
            adapter.HideAd();
        }

        public void DeInitialize()
        {
            if (!IsInitialized)
            {
                return;
            }
            
            DeInitializeAdapters();
            DeInitializeProcessing();
            IsInitialized = false;
        }
        
        protected virtual void InitializeProcessing() { }
        protected virtual void DeInitializeProcessing() { }

        private void InitializeAdapters()
        {
            for (int i = 0; i < _adapters.Count; i++)
            {
                TAdapter adapter = _adapters[i];
                adapter.OnAdShown += AdShownCallback;
                adapter.Initialize();
            }
        }

        private bool IsAdShowing(Type requestType)
        {
            if (!IsInitialized || !TryGetAdapter(requestType, out TAdapter adapter))
            {
                return false;
            }

            return adapter.IsAdShowing;
        }

        private bool TryGetAdapter(Type requestType, out TAdapter adapter)
        {
            bool hasAdapter = _adapterIndexMap.TryGetValue(requestType, out int index);
            if (!hasAdapter)
            {
                index = GetAdapterIndex(requestType);
                hasAdapter = index >= 0;
                if (hasAdapter)
                {
                    _adapterIndexMap.Add(requestType, index);
                }
            }

            adapter = hasAdapter ? _adapters[index] : null;
            return hasAdapter;
        }

        private int GetAdapterIndex(Type requestType)
        {
            int index = -1;
            int smallestWeight = int.MaxValue;
            for (int i = 0; i < _adapters.Count; i++)
            {
                TAdapter adapter = _adapters[i];
                if (!adapter.ServicedRequestType.IsAssignableFrom(requestType))
                {
                    continue;
                }

                int weight = adapter.ServicedRequestType.Comparison(requestType);
                if (weight <= smallestWeight)
                {
                    smallestWeight = weight;
                    index = i;
                }
            }
            
            return index;
        }

        private void DeInitializeAdapters()
        {
            for (int i = 0; i < _adapters.Count; i++)
            {
                TAdapter adapter = _adapters[i];
                adapter.OnAdShown -= AdShownCallback;
                adapter.DeInitialize();
            }
        }
        
        private void AdShownCallback(IAdResponse response)
        {
            OnAdShown?.Invoke(response);
        }
    }
}