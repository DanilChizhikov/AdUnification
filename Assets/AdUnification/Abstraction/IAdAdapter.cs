using System;

namespace DTech.AdUnification
{
    public interface IAdAdapter
    {
        event Action<AdType> OnAdLoaded; 
        event Action<AdType> OnAdBeganShow; 
        event Action<IAdResponse> OnAdShown;
        
        AdType ServicedAdType { get; }
        
        bool IsInitialized { get; }
        bool IsReady { get; }
        bool IsShowing { get; }
        
        void Initialize();
        bool TryShowAd(IAdRequest request);
        void HideAd();
        void DeInitialize();
    }
}