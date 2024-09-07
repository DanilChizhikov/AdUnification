namespace DTech.AdUnification
{
    internal sealed class AdResponse : IAdResponse
    {
        public AdType Type { get; set; }
        public bool IsSuccessful { get; set; }
    }
}