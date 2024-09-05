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
        bool AdIsReady<T>() where T : IAd;
        bool AdIsShowing<T>() where T : IAd;
        bool TryAdShow<T>(T request, Action<IAdResponse> callback = null) where T : IAd;
        void HideAd<T>() where T : IAd;
    }
}
