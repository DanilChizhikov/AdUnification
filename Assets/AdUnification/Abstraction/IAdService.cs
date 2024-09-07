using System;

namespace DTech.AdUnification
{
    public interface IAdService
    {
        event Action<IAdResponse> OnAdShown;
        
        bool IsInitialized { get; }
        bool AnyAdIsShowing { get; }

        void Initialize();
        bool IsReady(AdType type);
        bool IsShowing(AdType type);
        bool TryShowAd(IAdRequest request);
        void HideAd(AdType type);
    }
}
