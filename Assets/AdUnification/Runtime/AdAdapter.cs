using System;

namespace DTech.AdUnification
{
    public abstract class AdAdapter<TConfig> : IAdAdapter
            where TConfig : IAdConfig
    {
        public abstract event Action<AdType> OnAdLoaded;
        public event Action<AdType> OnAdBeganShow;
        public event Action<IAdResponse> OnAdShown;
        
        public abstract AdType ServicedAdType { get; }
        public bool IsInitialized { get; private set; }

        public abstract bool IsReady { get; }
        public abstract bool IsShowing { get; }
        
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

        public bool TryShowAd(IAdRequest request)
        {
            if (!IsInitialized)
            {
                return false;
            }
            
            OnAdBeganShow?.Invoke(request.Type);
            bool result = ShowAdProcessing(request);
            if (!result)
            {
                SendAdShown(new AdResponse
                {
                    Type = request.Type,
                    IsSuccessful = false,
                });
            }

            return result;
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
        protected abstract bool ShowAdProcessing(IAdRequest request);
        protected abstract void HideAdProcessing();
        protected void SendAdShown(IAdResponse response) => OnAdShown?.Invoke(response);
        protected virtual void DeInitializeProcessing() { }
    }
}