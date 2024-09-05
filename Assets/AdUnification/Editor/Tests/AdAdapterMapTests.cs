using System.Collections.Generic;
using NUnit.Framework;

namespace DTech.AdUnification.Editor.Tests
{
    [TestFixture]
    internal sealed class AdAdapterMapTests
    {
        private AdAdapterMap<IAdAdapter> _map;

        [SetUp]
        public void Setup()
        {
            var adapters = new List<IAdAdapter>(2)
            {
                new FakeInterstitialAdAdapter(new FakeInterstitialConfig()),
                new FakeRewardedAdAdapter(new FakeRewardedConfig()),
            };
            
            for (int i = 0; i < adapters.Count; i++)
            {
                adapters[i].Initialize();
            }

            _map = new AdAdapterMap<IAdAdapter>(adapters);
        }

        [Test]
        public void AdAdapterMap_Get_InterstitialAdapter()
        {
            bool hasAdapter = _map.TryGetAdapter(typeof(IInterstitialAd), out IAdAdapter adapter);
            
            Assert.IsTrue(hasAdapter);
            Assert.IsTrue(adapter is FakeInterstitialAdAdapter);
        }
        
        [Test]
        public void AdAdapterMap_Get_CustomInterstitialAdapter()
        {
            bool hasAdapter = _map.TryGetAdapter(typeof(CustomInterstitialAd), out IAdAdapter adapter);
            
            Assert.IsTrue(hasAdapter);
            Assert.IsTrue(adapter is FakeInterstitialAdAdapter);
        }
        
        [Test]
        public void AdAdapterMap_Get_RewardedAdapter()
        {
            bool hasAdapter = _map.TryGetAdapter(typeof(IRewardedAd), out IAdAdapter adapter);
            
            Assert.IsTrue(hasAdapter);
            Assert.IsTrue(adapter is FakeRewardedAdAdapter);
        }
        
        [Test]
        public void AdAdapterMap_Get_CustomRewardedAdapter()
        {
            bool hasAdapter = _map.TryGetAdapter(typeof(CustomRewardedAd), out IAdAdapter adapter);
            
            Assert.IsTrue(hasAdapter);
            Assert.IsTrue(adapter is FakeRewardedAdAdapter);
        }

        [TearDown]
        public void TearDown()
        {
            for (int i = 0; i < _map.Count; i++)
            {
                IAdAdapter adapter = _map[i];
                adapter.DeInitialize();
            }
            
            _map.Clear();
        }
    }
}