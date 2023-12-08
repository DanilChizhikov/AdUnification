using System;
using System.Collections.Generic;
using MbsCore.AdUnification.Infrastructure;
using UnityEngine;
using Random = UnityEngine.Random;

namespace MbsCore.AdUnification.Runtime
{
    public sealed class AdService : IAdService, IDisposable
    {
        public event Action<AdStatus> OnStatusChanged;
        public event Action<AdType, bool> OnAdShown;

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
            _providers = new HashSet<IAdProvider>(providers);
            ResetToDefault();
        }

        public AdService(params IAdProvider[] providers)
        {
            _providers = new HashSet<IAdProvider>(providers);
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

        public bool IsAdReady(AdType type)
        {
            if (Application.isEditor)
            {
                return true;
            }

            foreach (IAdProvider provider in _providers)
            {
                if (provider.IsAdReady(type))
                {
                    return true;
                }
            }

            return false;
        }

        public bool IsAdShowing(AdType type)
        {
            foreach (IAdProvider provider in _providers)
            {
                if (provider.IsAdShowing(type))
                {
                    return true;
                }
            }

            return false;
        }

        public bool TryAdShow(AdType type, Action<AdType, bool> callback = null, string placement = null)
        {
            if (!IsAdReady(type))
            {
                return false;
            }

            switch (Status)
            {
                case AdStatus.Default:
                {
                    if (!TryGetProvider(type, out IAdProvider provider))
                    {
                        return false;
                    }

                    provider.ShowAd(type, callback, placement);
                    return true;
                }

                case AdStatus.Blocked:
                {
                    callback?.Invoke(type, true);
                } return true;

                default:
                    return false;
            }
        }

        public void HideAd(AdType type)
        {
            if (!IsAdShowing(type))
            {
                return;
            }

            foreach (IAdProvider provider in _providers)
            {
                if (provider.IsAdShowing(type))
                {
                    provider.HideAd(type);
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
        
        private bool TryGetProvider(AdType type, out IAdProvider provider)
        {
            provider = null;
            var availableProviders = new HashSet<IAdProvider>();
            int maxWeight = int.MinValue;
            foreach (IAdProvider advertisementProvider in _providers)
            {
                if (!advertisementProvider.IsAdReady(type))
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
        
        private void AdShownCallback(AdType type, bool result)
        {
            OnAdShown?.Invoke(type, result);
        }
    }
}