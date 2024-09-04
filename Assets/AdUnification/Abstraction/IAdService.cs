using System;

namespace DTech.AdUnification
{
    public interface IAdService
    {
        event Action<AdStatus> OnStatusChanged;
        event Action<IAdResponse> OnAdShown;
        
        bool IsInitialized { get; }
        AdStatus Status { get; }
        bool IsAnyAdShowing { get; }

        void Initialize();
        void SetStatus(AdStatus value);
        bool AdIsReady<T>() where T : IAdRequest;
        bool AdIsShowing<T>() where T : IAdRequest;
        bool TryAdShow<T>(T request, Action<IAdResponse> callback = null) where T : IAdRequest;
        void HideAd<T>() where T : IAdRequest;
    }
}
