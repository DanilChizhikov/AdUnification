namespace DTech.AdUnification.Editor.Tests
{
    internal sealed class FakeRewardedAdAdapter : FakeAdAdapterBase<FakeRewardedConfig, IRewardedAd>
    {
        public FakeRewardedAdAdapter(FakeRewardedConfig config) : base(config)
        {
        }
    }
}