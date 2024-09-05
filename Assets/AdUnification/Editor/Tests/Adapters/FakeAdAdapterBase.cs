using System;

namespace DTech.AdUnification.Editor.Tests
{
    internal abstract class FakeAdAdapterBase<TConfig, TAd> : AdAdapter<TConfig, TAd>
        where TConfig : FakeAdConfig
        where TAd : IAd
    {
        private bool _isShown;
        
        public FakeAdAdapterBase(TConfig config) : base(config)
        {
            _isShown = false;
        }

        protected override bool IsReady()
        {
            return true;
        }

        protected override bool IsShowing()
        {
            return _isShown;
        }

        protected override void ShowAdProcessing(TAd request, Action<bool> callback)
        {
            _isShown = true;
            callback?.Invoke(true);
        }

        protected override void HideAdProcessing()
        {
            _isShown = false;
        }
    }
}