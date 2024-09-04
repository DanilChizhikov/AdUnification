namespace DTech.AdUnification
{
    public interface IBannerAdRequest : IAdRequest
    {
        AdPosition Position { get; }
    }
}