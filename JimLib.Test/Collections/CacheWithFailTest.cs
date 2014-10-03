using System;
using System.Threading.Tasks;
using FluentAssertions;
using JimBobBennett.JimLib.Collections;
using NUnit.Framework;

namespace JimBobBennett.JimLib.Test.Collections
{
    public class Data
    {
        public string String { get; set; }
        public int Int { get; set; }    
    }

    [TestFixture]
    public class CacheWithFailTest
    {
        [Test]
        public void DumpedDataCanBeReloaded()
        {
            var cache = new CacheWithFail<string, Data>();
            cache.Add("Foo", new Data { String = "Foo", Int = 1 });
            cache.Add("Bar", new Data { String = "Bar", Int = 2 });

            var json = cache.DumpCacheAsJson();

            var newCache = new CacheWithFail<string, Data>();
            newCache.LoadCacheFromJson(json);

            Data data;

            newCache.TryGetValue("Foo", out data).Should().Be(CacheState.Found);
            data.String.Should().Be("Foo");
            data.Int.Should().Be(1);

            newCache.TryGetValue("Bar", out data).Should().Be(CacheState.Found);
            data.String.Should().Be("Bar");
            data.Int.Should().Be(2);
        }

        [Test]
        public async Task IfItemIsNotInCacheFuncIsCalled()
        {
            var cache = new CacheWithFail<string, Data>();
            cache.Add("Foo", new Data { String = "Foo", Int = 1 });
            var called = false;

            await cache.GetOrAddAsync("Bar", async s => await Task.Run(() =>
            {
                called = true;
                return new Data { String = "Bar", Int = 2 };
            }));

            called.Should().BeTrue();
        }

        [Test]
        public async Task IfItemIsInCacheFuncIsNotCalled()
        {
            var cache = new CacheWithFail<string, Data>();
            cache.Add("Foo", new Data { String = "Foo", Int = 1 });
            var called = false;

            await cache.GetOrAddAsync("Foo", async s => await Task.Run(() =>
            {
                called = true;
                return new Data { String = "Foo", Int = 1 };
            }));

            called.Should().BeFalse();
        }

        [Test]
        public async Task IfAddFailsThenFuncIsNotCalledAgain()
        {
            var cache = new CacheWithFail<string, Data>();
            cache.Add("Foo", new Data { String = "Foo", Int = 1 });

            await cache.GetOrAddAsync("Bar", async s => await Task.Run(() => (Data)null));

            var called = false;
            await cache.GetOrAddAsync("Bar", async s => await Task.Run(() =>
            {
                called = true;
                return (Data)null;
            }));

            called.Should().BeFalse();
        }

        [Test]
        public async Task IfAddFailsThenFuncIsCalledAgainAfterTimeout()
        {
            var cache = new CacheWithFail<string, Data>(TimeSpan.FromMilliseconds(100));
            cache.Add("Foo", new Data { String = "Foo", Int = 1 });

            await cache.GetOrAddAsync("Bar", async s => await Task.Run(() => (Data)null));

            await Task.Delay(1000);

            var called = false;
            await cache.GetOrAddAsync("Bar", async s => await Task.Run(() =>
            {
                called = true;
                return (Data)null;
            }));

            called.Should().BeTrue();
        }
    }
}
