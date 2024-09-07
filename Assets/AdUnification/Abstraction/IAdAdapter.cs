using System;

namespace DTech.AdUnification
{
    public interface IAdAdapter
    {
        event Action<IAdResponse> OnAdShown;
        
        AdType ServicedAdType { get; }
        
        bool IsInitialized { get; }
        bool IsReady { get; }
        bool IsShowing { get; }
        
        void Initialize();
        void ShowAd(IAdRequest request);
        void HideAd();
        void DeInitialize();
    }
}