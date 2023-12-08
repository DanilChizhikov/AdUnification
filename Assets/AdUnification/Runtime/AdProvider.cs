using System;
using System.Collections.Generic;
using MbsCore.AdUnification.Infrastructure;

namespace MbsCore.AdUnification.Runtime
{
    public abstract class AdProvider<TConfig, TAdapter> : IAdProvider
            where TConfig : IAdConfig where TAdapter : AdAdapter<TConfig>
    {
        private readonly Dictionary<AdType, TAdapter> _adaptersMap;
        
        public event Action<AdType, bool> OnAdShown;
        
        public bool IsInitialized { get; private set; }

        public bool IsAnyAdShowing
        {
            get
            {
                foreach (var adapter in _adaptersMap)
                {
                    if (IsAdShowing(adapter.Key))
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
            _adaptersMap = new Dictionary<AdType, TAdapter>();
            foreach (TAdapter adapter in adapters)
            {
                _adaptersMap.Add(adapter.Type, adapter);
            }

            Config = config;
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

        public bool IsAdReady(AdType type)
        {
            if (!IsInitialized || !_adaptersMap.TryGetValue(type, out TAdapter adapter))
            {
                return false;
            }

            return adapter.IsAdReady;
        }

        public bool IsAdShowing(AdType type)
        {
            if (!IsInitialized || !_adaptersMap.TryGetValue(type, out TAdapter adapter))
            {
                return false;
            }

            return adapter.IsAdShowing;
        }

        public void ShowAd(AdType type, Action<AdType, bool> callback, string placement)
        {
            if (!IsInitialized || !_adaptersMap.TryGetValue(type, out TAdapter adapter))
            {
                callback?.Invoke(type, false);
                return;
            }
            
            adapter.ShowAd(placement, callback);
        }

        public void HideAd(AdType type)
        {
            if (!IsInitialized || !_adaptersMap.TryGetValue(type, out TAdapter adapter))
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
            foreach (var adapter in _adaptersMap)
            {
                adapter.Value.OnAdShown += AdShownCallback;
                adapter.Value.Initialize();
            }
        }

        private void DeInitializeAdapters()
        {
            foreach (var adapter in _adaptersMap)
            {
                adapter.Value.OnAdShown -= AdShownCallback;
                adapter.Value.DeInitialize();
            }
        }
        
        private void AdShownCallback(AdType type, bool result)
        {
            OnAdShown?.Invoke(type, result);
        }
    }
}