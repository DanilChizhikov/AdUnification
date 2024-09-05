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
        bool IsAdReady<T>() where T : IAd;
        bool IsAdShowing<T>() where T : IAd;
        void ShowAd(IAd request, Action<IAdResponse> callback);
        void HideAd<T>() where T : IAd;
        void DeInitialize();
    }
}