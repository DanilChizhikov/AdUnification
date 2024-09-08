using System.Collections.Generic;

namespace DTech.AdUnification
{
    public abstract class AdProvider<TConfig> : IAdProvider
            where TConfig : IAdConfig
    {
        private readonly AdapterMap<TConfig> _adapterMap;
        
        public abstract bool IsInitialized { get; }
        
        public int Weight => Config.Weight;
        
        protected TConfig Config { get; }

        public AdProvider(TConfig config, IEnumerable<AdAdapter<TConfig>> adapters)
        {
            Config = config.ThrowIfNull();
            _adapterMap = new AdapterMap<TConfig>(adapters.ThrowIfNull());
        }

        public abstract void Initialize();

        public bool TryGetAd<TAd>(out TAd ad) where TAd : IAd
        {
            ad = default;
            bool result = false;
            if (_adapterMap.TryGetAdapter(typeof(TAd), out AdAdapter<TConfig> adapter) && adapter.BaseAD is TAd genericAd)
            {
                ad = genericAd;
                result = true;
            }

            return result;
        }

        public bool IsReady<TAd>() where TAd : IAd => IsInitialized && TryGetAd(out TAd ad) && ad.IsReady;

        public bool TryShow<TAd>(string placement) where TAd : IAd
        {
            if (!IsInitialized || !_adapterMap.TryGetAdapter(typeof(TAd), out AdAdapter<TConfig> adapter))
            {
                return false;
            }

            return adapter.TryShowAd(placement);
        }

        public void HideAd<TAd>() where TAd : IAd
        {
            if (IsInitialized && _adapterMap.TryGetAdapter(typeof(TAd), out AdAdapter<TConfig> adapter))
            {
                adapter.HideAd();
            }
        }

        public virtual void DeInitialize()
        {
            if (!IsInitialized)
            {
                return;
            }
            
            DeInitializeAdapters();
            _adapterMap.Clear();
        }

        protected void InitializeAdapters()
        {
            for (int i = 0; i < _adapterMap.Count; i++)
            {
                _adapterMap[i].Initialize();
            }
        }

        protected void DeInitializeAdapters()
        {
            for (int i = 0; i < _adapterMap.Count; i++)
            {
                _adapterMap[i].DeInitialize();
            }
        }
    }
}