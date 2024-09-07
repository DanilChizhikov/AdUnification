using System;
using System.Collections.Generic;

namespace DTech.AdUnification
{
    public sealed class AdService : IAdService, IDisposable
    {
        public event Action<AdType> OnAdLoaded
        {
            add
            {
                foreach (var provider in _providers)
                {
                    provider.OnAdLoaded += value;
                }
            }

            remove
            {
                foreach (var provider in _providers)
                {
                    provider.OnAdLoaded -= value;
                }
            }
        }

        public event Action<AdType> OnAdBeganShow
        {
            add
            {
                foreach (var provider in _providers)
                {
                    provider.OnAdBeganShow += value;
                }
            }

            remove
            {
                foreach (var provider in _providers)
                {
                    provider.OnAdBeganShow -= value;
                }
            }
        }

        public event Action<IAdResponse> OnAdShown
        {
            add
            {
                foreach (var provider in _providers)
                {
                    provider.OnAdShown += value;
                }
            }

            remove
            {
                foreach (var provider in _providers)
                {
                    provider.OnAdShown -= value;
                }
            }
        }

        private readonly HashSet<IAdProvider> _providers;
        private readonly Random _random;
        
        public bool IsInitialized { get; private set; }

        public AdService(IEnumerable<IAdProvider> providers)
        {
            _providers = new HashSet<IAdProvider>(providers.ThrowIfNull());
            _random = new Random();
            IsInitialized = false;
        }

        public AdService(params IAdProvider[] providers) : this(new List<IAdProvider>(providers))
        {
        }

        public void Initialize()
        {
            if (IsInitialized)
            {
                return;
            }
            
            foreach (IAdProvider provider in _providers)
            {
                provider.Initialize();
            }

            IsInitialized = true;
        }

        public bool IsReady(AdType type)
        {
            if (IsInitialized)
            {
                foreach (var provider in _providers)
                {
                    if (provider.IsReady(type))
                    {
                        return true;
                    }
                }
            }
            
            return false;
        }

        public bool TryShowAd(IAdRequest request)
        {
            bool result = TryGetProvider(request.ThrowIfNull().Type, out IAdProvider provider);
            return result && provider.TryShow(request);
        }
        public void HideAd(AdType type)
        {
            foreach (var provider in _providers)
            {
                provider.HideAd(type);
            }
        }

        public void Dispose()
        {
            if (!IsInitialized)
            {
                return;
            }

            foreach (IAdProvider provider in _providers)
            {
                provider.DeInitialize();
            }
            
            IsInitialized = false;
        }
        
        private bool TryGetProvider(AdType type, out IAdProvider provider)
        {
            provider = null;
            var availableProviders = new List<IAdProvider>(_providers.Count);
            int maxWeight = 0;
            foreach (IAdProvider advertisementProvider in _providers)
            {
                if (!advertisementProvider.IsInitialized || !advertisementProvider.IsReady(type))
                {
                    continue;
                }

                maxWeight += advertisementProvider.Weight;
                availableProviders.Add(advertisementProvider);
            }

            if (availableProviders.Count > 0)
            {
                int randomWeight = _random.Next(maxWeight);
                for (int i = 0; i < availableProviders.Count; i++)
                {
                    IAdProvider availableProvider = availableProviders[i];
                    randomWeight -= availableProvider.Weight;
                    if (randomWeight <= 0)
                    {
                        provider = availableProvider;
                        break;
                    }
                } 
            }

            return !ReferenceEquals(provider, null);
        }
    }
}