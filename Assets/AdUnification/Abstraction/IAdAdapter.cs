using System;

namespace DTech.AdUnification
{
    public interface IAdAdapter
    {
        Type ServicedAdType { get; }
        bool IsInitialized { get; }
        IAd BaseAD { get; }
        
        void Initialize();
        bool TryShowAd(string placement);
        void HideAd();
        void DeInitialize();
    }
}