using System;

namespace MbsCore.AdUnification.Infrastructure
{
    public interface IAdAdapter
    {
        event Action<AdType, bool> OnAdShown;
        
        AdType Type { get; }
        bool IsInitialized { get; }
        bool IsAdReady { get; }
        bool IsAdShowing { get; }
        
        void Initialize();
        void ShowAd(string placement, Action<AdType, bool> callback);
        void HideAd();
        void DeInitialize();
    }
}