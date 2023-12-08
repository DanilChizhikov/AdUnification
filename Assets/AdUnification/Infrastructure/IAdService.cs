using System;

namespace MbsCore.AdUnification.Infrastructure
{
    public interface IAdService
    {
        event Action<AdStatus> OnStatusChanged;
        event Action<AdType, bool> OnAdShown;
        
        bool IsInitialized { get; }
        AdStatus Status { get; }
        bool IsAnyAdShowing { get; }

        void Initialize();
        void SetStatus(AdStatus value);
        bool IsAdReady(AdType type);
        bool IsAdShowing(AdType type);
        bool TryAdShow(AdType type, Action<AdType, bool> callback = null, string placement = null);
        void HideAd(AdType type);
    }
}
