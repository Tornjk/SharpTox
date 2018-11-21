using NUnit.Framework;
using SharpTox.Core;
using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace SharpTox.Test
{
    [TestFixture]
    public class MiscTests
    {
        [Test]
        public async Task TestToxBootstrapAndConnect()
        {
            using (var tox = new Tox(new ToxOptions { Ipv6Enabled = true, UdpEnabled = true }))
            {
                var error = ToxErrorBootstrap.Ok;

                foreach (var node in Globals.Nodes)
                {
                    bool result = tox.Bootstrap(node, out error);
                    if (!result || error != ToxErrorBootstrap.Ok)
                        Assert.Fail("Failed to bootstrap, error: {0}, result: {1}", error, result);
                }

                var connected = new TaskCompletionSource<bool>();
                tox.OnConnectionStatusChanged += (o, e) =>
                {
                    connected.SetResult(e.Status != ToxConnectionStatus.None);
                };

                tox.Start();
                await Task.WhenAny(Task.Delay(30000), connected.Task);
                Assert.True(connected.Task.IsCompleted, "Timeout");
                Assert.True(await connected.Task);
            }
        }

        [Test]
        public async Task TestToxBootstrapAndConnectTcp()
        {
            using (var tox = new Tox(new ToxOptions { Ipv6Enabled = true, UdpEnabled = false }))
            {
                var error = ToxErrorBootstrap.Ok;
                foreach (var node in Globals.TcpRelays)
                {
                    bool result = tox.AddTcpRelay(node, out error);
                    if (!result || error != ToxErrorBootstrap.Ok)
                        Assert.Fail("Failed to bootstrap error: {0}, result: {1}", error, result);
                }

                var connected = new TaskCompletionSource<bool>();
                tox.OnConnectionStatusChanged += (o, e) =>
                {
                    connected.SetResult(e.Status != ToxConnectionStatus.None);
                };

                tox.Start();
                await Task.WhenAny(Task.Delay(12000), connected.Task);
                Assert.True(connected.Task.IsCompleted, "Timeout");
                Assert.True(await connected.Task);
            }
        }

        [Test]
        public void TestToxHash()
        {
            byte[] data = new byte[0xBEEF];
            new Random().NextBytes(data);

            byte[] hash = ToxTools.Hash(data);
        }
    }
}
