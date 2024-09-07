namespace DTech.AdUnification
{
    public interface IAdRequest
    {
        AdType Type { get; }
        string Placement { get; }
    }
}