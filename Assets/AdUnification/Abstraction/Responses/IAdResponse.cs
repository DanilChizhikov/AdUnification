namespace DTech.AdUnification
{
    public interface IAdResponse
    {
        IAd Ad { get; }
        bool IsSuccessful { get; }
    }
}