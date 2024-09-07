using System;

namespace DTech.AdUnification
{
    public interface IAdProvider
    {
        event Action<AdType> OnAdLoaded;
        event Action<AdType> OnAdBeganShow;
        event Action<IAdResponse> OnAdShown; 
        
        bool IsInitialized { get; }
        int Weight { get; }

        void Initialize();
        bool IsReady(AdType type);
        bool TryShow(IAdRequest request);
        void HideAd(AdType type);
        void DeInitialize();
    }
}