using System;
using System.Collections.Generic;

namespace DTech.AdUnification
{
    public abstract class AdProvider<TConfig, TAdapter> : IAdProvider
            where TConfig : IAdConfig where TAdapter : AdAdapter<TConfig>
    {
        private readonly HashSet<TAdapter> _adapters;
        private readonly Dictionary<AdType, TAdapter> _adapterMap;

        public event Action<AdType> OnAdLoaded
        {
            add
            {
                foreach (var adapter in _adapters)
                {
                    adapter.OnAdLoaded += value;
                }
            }

            remove
            {
                foreach (var adapter in _adapters)
                {
                    adapter.OnAdLoaded -= value;
                }
            }
        }

        public event Action<AdType> OnAdBeganShow
        {
            add
            {
                foreach (var adapter in _adapters)
                {
                    adapter.OnAdBeganShow += value;
                }
            }

            remove
            {
                foreach (var adapter in _adapters)
                {
                    adapter.OnAdBeganShow -= value;
                }
            }
        }

        public event Action<IAdResponse> OnAdShown
        {
            add
            {
                foreach (var adapter in _adapters)
                {
                    adapter.OnAdShown += value;
                }
            }

            remove
            {
                foreach (var adapter in _adapters)
                {
                    adapter.OnAdShown -= value;
                }
            }
        }
        
        public bool IsInitialized { get; private set; }

        public bool IsAnyAdShowing
        {
            get
            {
                foreach (var adapter in _adapters)
                {
                    if (adapter.IsShowing)
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
            _adapters = new HashSet<TAdapter>(adapters.ThrowIfNull());
            _adapterMap = new Dictionary<AdType, TAdapter>(_adapters.Count);
            foreach (var adapter in _adapters)
            {
                _adapterMap.Add(adapter.ServicedAdType, adapter);
            }
            
            IsInitialized = false;
        }
        
        public void Initialize()
        {
            if (IsInitialized)
            {
                return;
            }
            
            InitializeProcessing();
            InitializeAdapters();
            IsInitialized = true;
        }

        public bool IsReady(AdType type) => IsInitialized && _adapterMap.TryGetValue(type, out TAdapter adapter) && adapter.IsReady;

        public bool IsShowing(AdType type) => IsInitialized && _adapterMap.TryGetValue(type, out TAdapter adapter) && adapter.IsShowing;

        public bool TryShow(IAdRequest request)
        {
            if (!IsInitialized || !_adapterMap.TryGetValue(request.Type, out TAdapter adapter))
            {
                return false;
            }
            
            return adapter.TryShowAd(request);
        }

        public void HideAd(AdType type)
        {
            if (!IsInitialized)
            {
                return;
            }
            
            if (_adapterMap.TryGetValue(type, out TAdapter adapter))
            {
                adapter.HideAd();
            }
        }

        public void DeInitialize()
        {
            if (!IsInitialized)
            {
                return;
            }
            
            DeInitializeAdapters();
            DeInitializeProcessing();
            _adapters.Clear();
            _adapterMap.Clear();
            IsInitialized = false;
        }
        
        protected virtual void InitializeProcessing() { }
        protected virtual void DeInitializeProcessing() { }

        private void InitializeAdapters()
        {
            foreach (var adapter in _adapters)
            {
                adapter.Initialize();
            }
        }

        private void DeInitializeAdapters()
        {
            foreach (var adapter in _adapters)
            {
                adapter.DeInitialize();
            }
        }
    }
}