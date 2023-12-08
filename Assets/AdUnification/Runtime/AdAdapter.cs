using System;
using MbsCore.AdUnification.Infrastructure;

namespace MbsCore.AdUnification.Runtime
{
    public abstract class AdAdapter<TConfig> : IAdAdapter
            where TConfig : IAdConfig
    {
        private class AdCallbackHandler
        {
            public event Action<AdType, bool> OnCallback; 
            
            private readonly AdType _type;

            public AdCallbackHandler(AdType type)
            {
                _type = type;
            }

            public void Callback(bool result)
            {
                OnCallback?.Invoke(_type, result);
            }
        }
        
        public event Action<AdType, bool> OnAdShown;
        
        public abstract AdType Type { get; }
        public bool IsInitialized { get; private set; }

        public bool IsAdReady => IsInitialized && IsReady();
        public bool IsAdShowing => IsInitialized && IsShowing();
        
        protected TConfig Config { get; }

        public AdAdapter(TConfig config)
        {
            Config = config;
        }
        
        public void Initialize()
        {
            if (IsInitialized)
            {
                return;
            }
            
            InitializeProcessing();
            IsInitialized = true;
        }

        public void ShowAd(string placement, Action<AdType, bool> callback)
        {
            var callbackHandler = new AdCallbackHandler(Type);
            callbackHandler.OnCallback += callback;
            callbackHandler.OnCallback += OnAdShown;
            if (IsInitialized)
            {
                ShowAdProcessing(placement, callbackHandler.Callback);
            }
            else
            {
                callbackHandler.Callback(false);
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
        protected abstract bool IsReady();
        protected abstract bool IsShowing();
        protected abstract void ShowAdProcessing(string placement, Action<bool> callback);
        protected abstract void HideAdProcessing();
        protected virtual void DeInitializeProcessing() { }
    }
}