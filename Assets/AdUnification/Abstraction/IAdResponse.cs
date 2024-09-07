namespace DTech.AdUnification
{
    public interface IAdResponse
    {
        AdType Type { get; }
        bool IsSuccessful { get; }
    }
}