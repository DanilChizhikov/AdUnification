using System;

namespace DTech.AdUnification
{
    public interface IAdService
    {
        event Action<AdType> OnAdLoaded;
        event Action<AdType> OnAdBeganShow; 
        event Action<IAdResponse> OnAdShown;
        
        bool IsInitialized { get; }

        void Initialize();
        bool IsReady(AdType type);
        bool TryShowAd(IAdRequest request);
        void HideAd(AdType type);
    }
}
