namespace DTech.AdUnification
{
    public interface IAdResponse
    {
        IAdRequest Request { get; }
        bool IsSuccessful { get; }
    }
}