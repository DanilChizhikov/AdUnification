namespace DTech.AdUnification.Editor.Tests
{
    internal sealed class FakeInterstitialAdAdapter : FakeAdAdapterBase<FakeInterstitialConfig, IInterstitialAd>
    {
        public FakeInterstitialAdAdapter(FakeInterstitialConfig config) : base(config)
        {
        }
    }
}