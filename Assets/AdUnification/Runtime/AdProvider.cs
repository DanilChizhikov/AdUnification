using System;
using System.Collections.Generic;

namespace DTech.AdUnification
{
    public abstract class AdProvider<TConfig, TAdapter> : IAdProvider
            where TConfig : IAdConfig where TAdapter : AdAdapter<TConfig>
    {
        private readonly AdAdapterMap<TConfig, TAdapter> _adAdapterMap;

        public event Action<IAdResponse> OnAdShown;
        public bool IsInitialized { get; private set; }

        public bool IsAnyAdShowing
        {
            get
            {
                for (int i = 0; i < _adAdapterMap.Count; i++)
                {
                    TAdapter adapter = _adAdapterMap[i];
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
            _adAdapterMap = new AdAdapterMap<TConfig, TAdapter>(adapters);
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

        public bool IsAdReady<T>() where T : IAd
        {
            if (!IsInitialized || !_adAdapterMap.TryGetAdapter(typeof(T), out TAdapter adapter))
            {
                return false;
            }

            return adapter.IsAdReady;
        }

        public bool IsAdShowing<T>() where T : IAd => IsAdShowing(typeof(T));

        public void ShowAd(IAd request, Action<IAdResponse> callback)
        {
            if (!IsInitialized || !_adAdapterMap.TryGetAdapter(request.GetType(), out TAdapter adapter))
            {
                var response = new SimpleResponse
                {
                    Ad = request,
                    IsSuccessful = false,
                };
                
                callback?.Invoke(response);
                return;
            }
            
            adapter.ShowAd(request, callback);
        }

        public void HideAd<T>() where T : IAd
        {
            if (!IsInitialized || !_adAdapterMap.TryGetAdapter(typeof(T), out TAdapter adapter))
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
            for (int i = 0; i < _adAdapterMap.Count; i++)
            {
                TAdapter adapter = _adAdapterMap[i];
                adapter.OnAdShown += AdShownCallback;
                adapter.Initialize();
            }
        }

        private bool IsAdShowing(Type requestType)
        {
            if (!IsInitialized || !_adAdapterMap.TryGetAdapter(requestType, out TAdapter adapter))
            {
                return false;
            }

            return adapter.IsAdShowing;
        }

        private void DeInitializeAdapters()
        {
            for (int i = 0; i < _adAdapterMap.Count; i++)
            {
                TAdapter adapter = _adAdapterMap[i];
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