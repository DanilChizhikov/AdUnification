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
        bool IsAdReady<T>() where T : IAdRequest;
        bool IsAdShowing<T>() where T : IAdRequest;
        void ShowAd(IAdRequest request, Action<IAdResponse> callback);
        void HideAd<T>() where T : IAdRequest;
        void DeInitialize();
    }
}