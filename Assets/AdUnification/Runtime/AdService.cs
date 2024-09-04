using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace DTech.AdUnification
{
    public sealed class AdService : IAdService, IDisposable
    {
        public event Action<AdStatus> OnStatusChanged;
        public event Action<IAdResponse> OnAdShown;

        private readonly HashSet<IAdProvider> _providers;
        
        public bool IsInitialized { get; private set; }
        public AdStatus Status { get; private set; }

        public bool IsAnyAdShowing
        {
            get
            {
                foreach (IAdProvider provider in _providers)
                {
                    if (provider.IsAnyAdShowing)
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        public AdService(IEnumerable<IAdProvider> providers)
        {
            _providers = new HashSet<IAdProvider>(providers.ThrowIfNull());
            ResetToDefault();
        }

        public AdService(params IAdProvider[] providers)
        {
            _providers = new HashSet<IAdProvider>(providers.ThrowIfNull());
            ResetToDefault();
        }

        public void Initialize()
        {
            if (IsInitialized)
            {
                return;
            }
            
            foreach (IAdProvider provider in _providers)
            {
                provider.OnAdShown += AdShownCallback;
                provider.Initialize();
            }

            IsInitialized = true;
        }

        public void SetStatus(AdStatus value)
        {
            if (Status == value)
            {
                return;
            }

            Status = value;
            OnStatusChanged?.Invoke(Status);
        }

        public bool AdIsReady<T>() where T : IAdRequest
        {
            if (Application.isEditor)
            {
                return true;
            }

            foreach (IAdProvider provider in _providers)
            {
                if (provider.IsAdReady<T>())
                {
                    return true;
                }
            }

            return false;
        }

        public bool AdIsShowing<T>() where T : IAdRequest
        {
            foreach (IAdProvider provider in _providers)
            {
                if (provider.IsAdShowing<T>())
                {
                    return true;
                }
            }

            return false;
        }

        public bool TryAdShow<T>(T request, Action<IAdResponse> callback = null) where T : IAdRequest
        {
            if (!AdIsReady<T>())
            {
                return false;
            }
            
            switch (Status)
            {
                case AdStatus.Default:
                {
                    if (!TryGetProvider<T>(out IAdProvider provider))
                    {
                        return false;
                    }

                    provider.ShowAd(request.ThrowIfNull(), callback);
                    return true;
                }

                case AdStatus.Blocked:
                {
                    var response = new SimpleResponse
                    {
                        Request = request.ThrowIfNull(),
                        IsSuccessful = true,
                    };
                    
                    callback?.Invoke(response);
                } return true;

                default:
                    return false;
            }
        }

        public void HideAd<T>() where T : IAdRequest
        {
            if (!AdIsShowing<T>())
            {
                return;
            }

            foreach (IAdProvider provider in _providers)
            {
                if (provider.IsAdShowing<T>())
                {
                    provider.HideAd<T>();
                }
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
                provider.OnAdShown -= AdShownCallback;
                provider.DeInitialize();
            }
            
            IsInitialized = false;
        }
        
        private bool TryGetProvider<T>(out IAdProvider provider) where T : IAdRequest
        {
            provider = null;
            var availableProviders = new HashSet<IAdProvider>();
            int maxWeight = int.MinValue;
            foreach (IAdProvider advertisementProvider in _providers)
            {
                if (!advertisementProvider.IsAdReady<T>())
                {
                    continue;
                }

                maxWeight += advertisementProvider.Weight;
                availableProviders.Add(advertisementProvider);
            }

            int randomWeight = Random.Range(0, maxWeight);
            foreach (IAdProvider availableProvider in availableProviders)
            {
                randomWeight -= availableProvider.Weight;
                if (randomWeight <= 0)
                {
                    provider = availableProvider;
                    return true;
                }
            }

            return false;
        }

        private void ResetToDefault()
        {
            IsInitialized = false;
            Status = AdStatus.Default;
        }
        
        private void AdShownCallback(IAdResponse response)
        {
            OnAdShown?.Invoke(response);
        }
    }
}