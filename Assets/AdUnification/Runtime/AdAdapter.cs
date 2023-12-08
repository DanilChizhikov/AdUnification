using System;
using MbsCore.AdUnification.Infrastructure;

namespace MbsCore.AdUnification.Runtime
{
    public abstract class AdAdapter<TConfig> : IAdAdapter
            where TConfig : IAdConfig
    {
        public event Action<AdType, bool> OnAdShown;
        
        public abstract AdType Type { get; }
        public bool IsInitialized { get; private set; }
        public abstract bool IsAdReady { get; }
        public abstract bool IsAdShowing { get; }
        
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
            if (IsInitialized)
            {
                ShowAdProcessing(placement, ShowAdCallback);
            }
            else
            {
                ShowAdCallback(false);
            }

            void ShowAdCallback(bool result)
            {
                callback?.Invoke(Type, result);
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
        protected abstract void ShowAdProcessing(string placement, Action<bool> callback);
        protected abstract void HideAdProcessing();
        protected virtual void DeInitializeProcessing() { }
    }
}