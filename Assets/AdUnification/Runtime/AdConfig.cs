using UnityEngine;

namespace DTech.AdUnification
{
    public abstract class AdConfig : ScriptableObject, IAdConfig
    {
        [SerializeField, Min(0)] private int _weight = 0;

        public int Weight => _weight;
    }
}