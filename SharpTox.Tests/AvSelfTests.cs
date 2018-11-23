using NUnit.Framework;
using SharpTox.Av;
using SharpTox.Core;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SharpTox.Test
{
    [TestFixture]
    public class AvSelfTests
    {
        [Test]
        public async Task TestToxAvCallAndAnswer()
        {
            var options = new ToxOptions { Ipv6Enabled = true, UdpEnabled = true };
            using (var tox1 = new Tox(options))
            using (var tox2 = new Tox(options))
            using (var toxAv1 = new ToxAv(tox1))
            using (var toxAv2 = new ToxAv(tox2))
            {
                var tokenSource = new CancellationTokenSource();
                var it = Task.Run(async () =>
                {
                    while (!tokenSource.IsCancellationRequested)
                    {
                        var time1 = Min(tox1.Iterate(), tox2.Iterate());
                        var time2 = Min(toxAv1.Iterate(), toxAv2.Iterate());

                        await Task.Delay(Min(time1, time2));

                        TimeSpan Min(TimeSpan a, TimeSpan b)
                            => a < b ? a : b;
                    }
                });

                tox1.AddFriend(tox2.Id, "hey", out _);
                tox2.AddFriend(tox1.Id, "hey", out _);

                var tox1connected = new TaskCompletionSource<bool>();
                tox1.OnConnectionStatusChanged += (o, e) =>
                {
                    tox1connected.TrySetResult(e.Status != ToxConnectionStatus.None);
                };

                await ToxTest.AssertTimeout(TimeSpan.FromSeconds(20), tox1connected.Task);

                toxAv1.Call(0, 48, 30000, out _);

                var callrequest = new TaskCompletionSource<bool>();
                toxAv2.OnCallRequestReceived += (sender, e) =>
                {
                    var error2 = ToxAvErrorAnswer.Ok;
                    callrequest.TrySetResult(toxAv2.Answer(e.FriendNumber, 48, 30000, out error2));
                };

                var answered = new TaskCompletionSource<bool>();
                toxAv1.OnCallStateChanged += (sender, e) =>
                {
                    answered.TrySetResult(true);
                };

                await ToxTest.AssertTimeout(TimeSpan.FromSeconds(15), callrequest.Task);
                await ToxTest.AssertTimeout(TimeSpan.FromSeconds(10), answered.Task);
                tokenSource.Cancel();

                await it;
            }
        }
    }

    public static class ToxTest
    {

        public static async Task AssertTimeout(TimeSpan timeout, Task<bool> t)
        {
            await Task.WhenAny(Task.Delay(timeout), t);
            Assert.IsTrue(t.IsCompleted, $"Timeout after: {timeout}");
            Assert.IsTrue(await t);
        }
    }
}
