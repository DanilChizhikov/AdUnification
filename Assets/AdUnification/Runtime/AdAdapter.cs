using System;

namespace DTech.AdUnification
{
    public abstract class AdAdapter<TConfig> : IAdAdapter
            where TConfig : IAdConfig
    {
        public abstract event Action<IAdResponse> OnAdShown;
        
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

        public void ShowAd(IAdRequest request)
        {
            if (IsInitialized)
            {
                ShowAdProcessing(request);
            }
            else
            {
                request.Callback?.Invoke(new AdResponse
                {
                    Type = request.Type,
                    IsSuccessful = false,
                });
            }
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
        protected abstract void ShowAdProcessing(IAdRequest request);
        protected abstract void HideAdProcessing();
        protected virtual void DeInitializeProcessing() { }
    }
}