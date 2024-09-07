using System;

namespace DTech.AdUnification
{
    public abstract class AdAdapter<TConfig> : IAdAdapter
            where TConfig : IAdConfig
    {
        public abstract Type ServicedAdType { get; }
        public bool IsInitialized { get; private set; }
        public abstract IAd BaseAD { get; }
        
        protected TConfig Config { get; }

        public AdAdapter(TConfig config) => Config = config.ThrowIfNull();
        
        public void Initialize()
        {
            if (IsInitialized)
            {
                return;
            }
            
            InitializeProcessing();
            IsInitialized = true;
        }

        public bool TryShowAd(string placement)
        {
            if (!IsInitialized || !BaseAD.IsReady)
            {
                return false;
            }
            
            ShowAdProcessing(placement);
            return true;
        }
        
        public void HideAd()
        {
            if (IsInitialized)
            {
                HideAdProcessing();
            }
        }

        public void DeInitialize()
        {
            if (!IsInitialized)
            {
                return;
            }
            
            DeInitializeProcessing();
            IsInitialized = false;
        }

        protected virtual void InitializeProcessing() { }
        protected abstract void ShowAdProcessing(string placement);
        protected abstract void HideAdProcessing();
        protected virtual void DeInitializeProcessing() { }
    }

    public abstract class AdAdapter<TConfig, TAd> : AdAdapter<TConfig>
        where TConfig : IAdConfig
        where TAd : IAd
    {
        public sealed override Type ServicedAdType => typeof(TAd);
        public sealed override IAd BaseAD => Ad;

        protected abstract TAd Ad { get; }
        
        public AdAdapter(TConfig config) : base(config)
        {
        }
    }
}