using System;
using System.Collections.Generic;

namespace DTech.AdUnification
{
    public sealed class AdService : IAdService, IDisposable
    {
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

        public bool IsReady<TAd>() where TAd : IAd
        {
            foreach (var provider in _providers)
            {
                if (provider.IsReady<TAd>())
                {
                    return true;
                }
            }
            
            return false;
        }

        public int GetAdNonAlloc<TAd>(TAd[] ads) where TAd : IAd
        {
            int index = 0;
            if (ads.ThrowIfNull().Length > 0)
            {
                foreach (var provider in _providers)
                {
                    if(!provider.TryGetAd(out TAd ad))
                    {
                        continue;
                    }

                    ads[index++] = ad;
                    if (index >= ads.Length)
                    {
                        break;
                    }
                }
            }

            return index;
        }

        public bool TryShowAd<TAd>(string placement = null) where TAd : IAd
        {
            if (TryGetProvider<TAd>(out IAdProvider provider))
            {
                return provider.TryShow<TAd>(placement);
            }
            
            return false;
        }

        public void HideAd<TAd>() where TAd : IAd
        {
            if (!IsInitialized)
            {
                return;
            }
            
            foreach (var provider in _providers)
            {
                provider.HideAd<TAd>();
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
        
        private bool TryGetProvider<TAd>(out IAdProvider provider) where TAd : IAd
        {
            provider = null;
            if (IsInitialized)
            {
                var availableProviders = new AvailableProviderCollection<TAd>(_providers);
                if (availableProviders.Count > 0)
                {
                    int randomWeight = _random.Next(availableProviders.MaxWeight);
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
            }

            return !ReferenceEquals(provider, null);
        }
    }
}