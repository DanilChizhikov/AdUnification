using System;

namespace DTech.AdUnification
{
    public abstract class AdAdapter<TConfig> : IAdAdapter
            where TConfig : IAdConfig
    {
        private class AdCallbackHandler
        {
            public event Action<IAdResponse> OnCallback;

            private readonly IAd _request;

            public AdCallbackHandler(IAd request)
            {
                _request = request;
            }

            public void Callback(bool result)
            {
                var response = new SimpleResponse
                {
                    Ad = _request,
                    IsSuccessful = result,
                };
                
                OnCallback?.Invoke(response);
            }
        }

        public event Action<IAdResponse> OnAdShown;

        public abstract Type ServicedRequestType { get; }
        public bool IsInitialized { get; private set; }

        public bool IsAdReady => IsInitialized && IsReady();
        public bool IsAdShowing => IsInitialized && IsShowing();
        
        protected TConfig Config { get; }

        public AdAdapter(TConfig config)
        {
            Config = config.ThrowIfNull();
        }
        
        public void Initialize()
        {
            if (IsInitialized)
            {
                return;
            }
            
            InitializeProcessing();
            IsInitialized = true;
        }

        public void ShowAd(IAd request, Action<IAdResponse> callback)
        {
            var callbackHandler = new AdCallbackHandler(request);
            callbackHandler.OnCallback += callback;
            callbackHandler.OnCallback += OnAdShown;
            if (IsInitialized)
            {
                ShowAdProcessing(request, callbackHandler.Callback);
            }
            else
            {
                callbackHandler.Callback(false);
            }
        }
        
        public void HideAd()
        {
            if (IsInitialized)
            {
                HideAdProcessing();
            }
        }

        public void DeInitialize()
        {
            if (!IsInitialized)
            {
                return;
            }
            
            DeInitializeProcessing();
            IsInitialized = false;
        }

        protected virtual void InitializeProcessing() { }
        protected abstract bool IsReady();
        protected abstract bool IsShowing();
        protected abstract void ShowAdProcessing(IAd request, Action<bool> callback);
        protected abstract void HideAdProcessing();
        protected virtual void DeInitializeProcessing() { }
    }

    public abstract class AdAdapter<TConfig, TAd> : AdAdapter<TConfig>
        where TConfig : IAdConfig
        where TAd : IAd
    {
        public sealed override Type ServicedRequestType => typeof(TAd);

        public AdAdapter(TConfig config) : base(config)
        {
        }

        protected sealed override void ShowAdProcessing(IAd request, Action<bool> callback)
        {
            if (request is not TAd genericRequest)
            {
                throw new InvalidCastException($"Request is not of type {typeof(TAd).Name}");
            }
            
            ShowAdProcessing(genericRequest, callback);
        }
        
        protected abstract void ShowAdProcessing(TAd request, Action<bool> callback);
    }
}