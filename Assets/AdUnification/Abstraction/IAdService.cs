namespace DTech.AdUnification
{
    public interface IAdService
    {
        bool IsInitialized { get; }

        void Initialize();
        bool IsReady<TAd>() where TAd : IAd;
        int GetAdNonAlloc<TAd>(TAd[] ads) where TAd : IAd;
        bool TryShowAd<TAd>(string placement = null) where TAd : IAd;
        void HideAd<TAd>() where TAd : IAd;
    }
}
