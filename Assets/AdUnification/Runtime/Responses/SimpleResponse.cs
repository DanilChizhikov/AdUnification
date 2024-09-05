namespace DTech.AdUnification
{
    internal sealed class SimpleResponse : IAdResponse
    {
        public IAd Ad { get; set; }
        public bool IsSuccessful { get; set; }
    }
}