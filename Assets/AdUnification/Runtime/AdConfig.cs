using MbsCore.AdUnification.Infrastructure;
using UnityEngine;

namespace MbsCore.AdUnification.Runtime
{
    public abstract class AdConfig : ScriptableObject, IAdConfig
    {
        [SerializeField, Min(0)] private int _weight = 0;

        public int Weight => _weight;
    }
}