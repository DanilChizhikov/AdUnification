using System;

namespace DTech.AdUnification
{
    public interface IAdAdapter
    {
        event Action<IAdResponse> OnAdShown;
        
        Type ServicedRequestType { get; }
        
        bool IsInitialized { get; }
        bool IsAdReady { get; }
        bool IsAdShowing { get; }
        
        void Initialize();
        void ShowAd(IAd request, Action<IAdResponse> callback);
        void HideAd();
        void DeInitialize();
    }
}