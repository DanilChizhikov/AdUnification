using System;

namespace MbsCore.AdUnification.Infrastructure
{
    public interface IAdService
    {
        event Action<AdvertisementStatus> OnStatusChanged;
        event Action<AdType, bool> OnAdShown;
        
        AdvertisementStatus Status { get; }
        bool IsAnyAdShowing { get; }
        
        void SetStatus(AdvertisementStatus value);
        bool IsAdReady(AdType type);
        bool IsAdShowing(AdType type);
        bool TryAdShow(AdType type, Action<AdType, bool> callback = null, string placement = null);
        void HideAd(AdType type);
    }
}
