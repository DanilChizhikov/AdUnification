using System.Collections.Generic;

namespace DTech.AdUnification
{
    internal sealed class AvailableProviderCollection<TAd>
        where TAd : IAd
    {
        private readonly List<IAdProvider> _providers;

        public int Count => _providers.Count;
        public IAdProvider this[int index] => _providers[index];
        public int MaxWeight { get; }
        
        public AvailableProviderCollection(IEnumerable<IAdProvider> providers)
        {
            providers.ThrowIfNull();
            _providers = new List<IAdProvider>();
            MaxWeight = 0;
            foreach (var provider in providers)
            {
                if (!provider.IsInitialized || provider.IsReady<TAd>())
                {
                    continue;
                }

                MaxWeight += provider.Weight;
                _providers.Add(provider);
            }
        }
    }
}