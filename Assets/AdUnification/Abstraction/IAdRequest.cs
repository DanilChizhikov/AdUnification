using System;

namespace DTech.AdUnification
{
    public interface IAdRequest
    {
        AdType Type { get; }
        string Placement { get; }
        Action<IAdResponse> Callback { get; }
    }
}