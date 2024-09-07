using System;

namespace DTech.AdUnification
{
    public interface IAdProvider
    {
        event Action<IAdResponse> OnAdShown; 
        
        bool IsInitialized { get; }
        bool IsAnyAdShowing { get; }
        int Weight { get; }

        void Initialize();
        bool IsReady(AdType type);
        bool IsShowing(AdType type);
        void Show(IAdRequest request);
        void HideAd(AdType type);
        void DeInitialize();
    }
}