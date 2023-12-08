using System;

namespace MbsCore.AdUnification.Infrastructure
{
    public interface IAdProvider
    {
        event Action<AdType, bool> OnAdShown; 
        
        bool IsInitialized { get; }
        bool IsAnyAdShowing { get; }
        int Weight { get; }

        void Initialize();
        bool IsAdReady(AdType type);
        bool IsAdShowing(AdType type);
        void ShowAd(AdType type, Action<AdType, bool> callback, string placement);
        void HideAd(AdType type);
        void DeInitialize();
    }
}