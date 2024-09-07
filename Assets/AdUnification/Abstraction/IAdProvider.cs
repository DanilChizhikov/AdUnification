namespace DTech.AdUnification
{
    public interface IAdProvider
    {
        bool IsInitialized { get; }
        int Weight { get; }

        void Initialize();
        bool TryGetAd<TAd>(out TAd ad) where TAd : IAd;
        bool IsReady<TAd>() where TAd : IAd;
        bool TryShow<TAd>(string placement) where TAd : IAd;
        void HideAd<TAd>() where TAd : IAd;
        void DeInitialize();
    }
}