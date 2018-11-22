using NUnit.Framework;
using SharpTox.Av;
using SharpTox.Core;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SharpTox.Test
{
    [TestFixture]
    public class AvFriendTests
    {
        private Tox tox1;
        private Tox tox2;
        private ToxAv _toxAv1;
        private ToxAv _toxAv2;

        [OneTimeSetUp]
        public async Task Init()
        {
            var options = new ToxOptions { Ipv6Enabled = true, UdpEnabled = true };
            tox1 = new Tox(options);
            tox2 = new Tox(options);

            _toxAv1 = new ToxAv(tox1);
            _toxAv2 = new ToxAv(tox2);

            var friend2 = tox1.AddFriend(tox2.Id, "hey", out _);
            tox2.AddFriend(tox1.Id, "hey", out _);

            var connected = new TaskCompletionSource<bool>();

            tox1.OnFriendConnectionStatusChanged += (o, e) =>
            {
                if (e.FriendNumber == friend2)
                {
                    connected.SetResult(e.Status != ToxConnectionStatus.None);
                }
            };

            await Task.WhenAny(Task.Delay(10000), connected.Task);

            Assert.IsTrue(connected.Task.IsCompleted);
            Assert.True(await connected.Task);

            bool answered = false;
            _toxAv1.Call(0, 48, 3000);

            _toxAv2.OnCallRequestReceived += (sender, e) =>
            {
                var error2 = ToxAvErrorAnswer.Ok;
                bool result2 = _toxAv2.Answer(e.FriendNumber, 48, 3000, out error2);
            };

            _toxAv1.OnCallStateChanged += (sender, e) =>
            {
                answered = true;
            };

            while (!answered) { DoIterate(); }
        }

        [OneTimeTearDown]
        public void Cleanup()
        {
            _toxAv1.Dispose();
            _toxAv2.Dispose();

            tox1.Dispose();
            tox2.Dispose();
        }

        private void DoIterate()
        {
            var time1 = Min(tox1.Iterate(), tox2.Iterate());
            var time2 = TimeSpan.FromMilliseconds(Math.Min(_toxAv1.Iterate(), _toxAv2.Iterate()));

            Thread.Sleep(Min(time1, time2));

            TimeSpan Min(TimeSpan a, TimeSpan b)
                => a < b ? a : b;
        }

        [Test]
        public void TestToxAvAudioBitrateChange()
        {
            int bitrate = 16;
            var error = ToxAvErrorSetBitrate.Ok;
            bool result = _toxAv1.SetAudioBitrate(0, bitrate, out error);

            if (!result || error != ToxAvErrorSetBitrate.Ok)
                Assert.Fail("Failed to set audio bitrate, error: {0}, result: {1}", error, result);
        }

        [Test]
        public void TestToxAvVideoBitrateChange()
        {
            int bitrate = 2000;
            var error = ToxAvErrorSetBitrate.Ok;
            bool result = _toxAv1.SetVideoBitrate(0, bitrate, out error);

            if (!result || error != ToxAvErrorSetBitrate.Ok)
                Assert.Fail("Failed to set video bitrate, error: {0}, result: {1}", error, result);
        }

        [Test]
        public void TestToxAvSendControl()
        {
            var control = ToxAvCallControl.Pause;
            var error = ToxAvErrorCallControl.Ok;
            bool testFinished = false;

            _toxAv2.OnCallStateChanged += (sender, e) =>
            {
                if (!e.State.HasFlag(ToxAvFriendCallState.Paused))
                    Assert.Fail("Tried to pause a call but the call state didn't change correctly, call state: {0}", e.State);

                testFinished = true;
            };

            bool result = _toxAv1.SendControl(0, control, out error);
            if (!result || error != ToxAvErrorCallControl.Ok)
                Assert.Fail("Could not send call control, error: {0}, result: {1}", error, result);

            while (!testFinished) { DoIterate(); }
        }

        [Test]
        public void TestToxAvSendAudio()
        {
            var stopWatch = new System.Diagnostics.Stopwatch();
            int count = 0;

            _toxAv2.OnAudioFrameReceived += (sender, e) =>
            {
                Console.WriteLine("Received frame, length: {0}, sampling rate: {1}", e.Frame.Data.Length, e.Frame.SamplingRate);
                count++;
            };

            for (int i = 0; i < 100; i++)
            {
                short[] frame = new short[1920];
                RandomShorts(frame);

                var error = ToxAvErrorSendFrame.Ok;
                bool result = _toxAv1.SendAudioFrame(0, new ToxAvAudioFrame(frame, 48000, 2), out error);

                if (!result || error != ToxAvErrorSendFrame.Ok)
                    Assert.Fail("Failed to send audio frame, error: {0}, result: {1}", error, result);

                DoIterate();
            }

            stopWatch.Start();

            while (stopWatch.Elapsed.TotalSeconds < 5)
            {
                //give the frames a bit less than 5 seconds to arrive
                DoIterate();
            }

            Console.WriteLine("Received a total of {0} audio frames", count);
        }

        [Test]
        public void TestToxAvSendVideo()
        {
            _toxAv2.OnVideoFrameReceived += (sender, e) =>
            {
                Console.WriteLine("Received frame, width: {0}, height: {1}", e.Frame.Width, e.Frame.Height);
            };

            for (int i = 0; i < 100; i++)
            {
                var random = new Random();
                int width = 800;
                int height = 600;

                byte[] y = new byte[width * height];
                byte[] u = new byte[(height / 2) * (width / 2)];
                byte[] v = new byte[(height / 2) * (width / 2)];

                var frame = new ToxAvVideoFrame(800, 600, y, u, v);

                var error = ToxAvErrorSendFrame.Ok;
                bool result = _toxAv1.SendVideoFrame(0, frame, out error);

                if (!result || error != ToxAvErrorSendFrame.Ok)
                    Assert.Fail("Failed to send video frame, error: {0}, result: {1}", error, result);

                DoIterate();
            }
        }

        private static void RandomShorts(short[] shorts)
        {
            var random = new Random();
            for (int i = 0; i < shorts.Length; i++)
            {
                byte[] s = new byte[sizeof(short)];
                random.NextBytes(s);

                shorts[i] = BitConverter.ToInt16(s, 0);
            }
        }
    }
}
