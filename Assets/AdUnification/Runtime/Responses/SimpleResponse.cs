namespace DTech.AdUnification
{
    internal sealed class SimpleResponse : IAdResponse
    {
        public IAdRequest Request { get; set; }
        public bool IsSuccessful { get; set; }
    }
}