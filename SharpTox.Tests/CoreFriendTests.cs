using NUnit.Framework;
using SharpTox.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SharpTox.Test
{
    [TestFixture]
    public class CoreFriendTests
    {
        private Tox tox1;
        private Tox tox2;

        [OneTimeSetUp]
        public async Task Init()
        {
            var options = new ToxOptions { Ipv6Enabled = true, UdpEnabled = true };// new ToxOptions_DEPRECATED(true, true);
            this.tox1 = new Tox(options);
            this.tox2 = new Tox(options);

            this.tox1.AddFriend(tox2.Id, "hey", out _);
            this.tox2.AddFriend(tox1.Id, "hey", out _);

            using (var tokenSource = new CancellationTokenSource())
            {
                _ = Task.Run(() =>
                {
                    while (!tokenSource.IsCancellationRequested)
                    {
                        DoIterate();
                    }
                });

                var completed = new TaskCompletionSource<bool>();
                this.tox1.OnFriendConnectionStatusChanged += (o, e) =>
                {
                    completed.SetResult(e.Status != ToxConnectionStatus.None);
                };

                await Task.WhenAny(Task.Delay(10000), completed.Task);

                if (!completed.Task.IsCompleted)
                {
                    Assert.Fail();
                }

                Assert.True(await completed.Task);
            }
        }

        [OneTimeTearDown]
        public void Cleanup()
        {
            tox1.Dispose();
            tox2.Dispose();
        }

        private void DoIterate()
        {
            int time1 = tox1.Iterate();
            int time2 = tox2.Iterate();

            Thread.Sleep(Math.Min(time1, time2));
        }

        [Test]
        public void TestToxMessage()
        {
            string messageFormat = "Hey! This is test message number ";
            int messageCount = 100;
            int receivedMessageCount = 0;

            EventHandler<ToxEventArgs.FriendMessageEventArgs> callback = (sender, args) =>
            {
                if (args.MessageType != ToxMessageType.Message || args.Message != messageFormat + receivedMessageCount)
                    Assert.Fail("Message arrived got garbled");

                receivedMessageCount++;
            };

            tox2.OnFriendMessageReceived += callback;

            for (int i = 0; i < messageCount; i++)
            {
                var sendError = ToxErrorSendMessage.Ok;
                tox1.SendMessage(0, messageFormat + i.ToString(), ToxMessageType.Message, out sendError);
                if (sendError != ToxErrorSendMessage.Ok)
                    Assert.Fail("Failed to send message to friend: {0}", sendError);
            }

            while (receivedMessageCount != messageCount) { DoIterate(); }
            tox2.OnFriendMessageReceived -= callback;
            Console.WriteLine("Received all messages without errors");
        }

        [Test]
        public void TestToxAction()
        {
            string actionFormat = "Hey! This is test action number ";
            int actionCount = 100;
            int receivedActionCount = 0;

            EventHandler<ToxEventArgs.FriendMessageEventArgs> callback = (sender, args) =>
            {
                if (args.MessageType != ToxMessageType.Action || args.Message != actionFormat + receivedActionCount)
                    Assert.Fail("Action arrived got garbled");

                receivedActionCount++;
            };

            tox2.OnFriendMessageReceived += callback;

            for (int i = 0; i < actionCount; i++)
            {
                var sendError = ToxErrorSendMessage.Ok;
                tox1.SendMessage(0, actionFormat + i.ToString(), ToxMessageType.Action, out sendError);
                if (sendError != ToxErrorSendMessage.Ok)
                    Assert.Fail("Failed to send action to friend: {0}", sendError);
            }

            while (receivedActionCount != actionCount) { DoIterate(); }
            tox2.OnFriendMessageReceived -= callback;
            Console.WriteLine("Received all actions without errors");
        }

        [Test]
        public void TestToxName()
        {
            string name = "Test, test and test";
            bool testFinished = false;

            tox2.OnFriendNameChanged += (sender, args) =>
            {
                if (args.Name != name)
                    Assert.Fail("Name received is not equal to the name we set");

                testFinished = true;
            };
            tox1.Name = name;

            while (!testFinished) { DoIterate(); }
        }

        [Test]
        public void TestToxStatus()
        {
            var status = ToxUserStatus.Busy;
            bool testFinished = false;

            tox2.OnFriendStatusChanged += (sender, args) =>
            {
                if (args.Status != status)
                    Assert.Fail("Status received is not equal to the status we set");

                testFinished = true;
            };
            tox1.Status = status;

            while (!testFinished) { DoIterate(); }
        }

        [Test]
        public void TestToxStatusMessage()
        {
            string message = "Test, test and test";
            bool testFinished = false;

            tox2.OnFriendStatusMessageChanged += (sender, args) =>
            {
                if (args.StatusMessage != message)
                    Assert.Fail("Status message received is not equal to the status message we set");

                testFinished = true;
            };
            tox1.StatusMessage = message;

            while (testFinished) { DoIterate(); }
        }

        [Test]
        public void TestToxTyping()
        {
            bool isTyping = true;
            bool testFinished = false;

            tox2.OnFriendTypingChanged += (sender, args) =>
            {
                if (args.IsTyping != isTyping)
                    Assert.Fail("IsTyping value received does not equal the one we set");

                testFinished = true;
            };

            {
                var error = ToxErrorSetTyping.Ok;
                bool result = tox1.SetTypingStatus(0, isTyping, out error);
                if (!result || error != ToxErrorSetTyping.Ok)
                    Assert.Fail("Failed to set typing status, error: {0}, result: {1}", error, result);

                while (!testFinished) { DoIterate(); }
            }
        }

        [Test]
        public void TestToxFriendPublicKey()
        {
            var error = ToxErrorFriendGetPublicKey.Ok;
            var publicKey = tox2.GetFriendPublicKey(0, out error);
            if (error != ToxErrorFriendGetPublicKey.Ok)
                Assert.Fail("Could not get friend public key, error: {0}", error);

            var error2 = ToxErrorFriendByPublicKey.Ok;
            var friend = tox2.GetFriendByPublicKey(publicKey, out error2);
            if (friend != 0 || error2 != ToxErrorFriendByPublicKey.Ok)
                Assert.Fail("Could not get friend by public key, error: {0}, friend: {1}", error2, friend);
        }

        [Test]
        public void TestToxLossyPacket()
        {
            int receivedPackets = 0;
            byte[] data = new byte[ToxConstants.MaxCustomPacketSize];
            new Random().NextBytes(data);
            data[0] = 210;

            tox2.OnFriendLossyPacketReceived += (sender, args) =>
            {
                if (args.Data.Length != data.Length || data[0] != args.Data[0])
                    Assert.Fail("Packet doesn't have the same length/identifier");
                else if (!data.SequenceEqual(args.Data))
                    Assert.Fail("Packet contents don't match");

                receivedPackets++;
            };

            var error = ToxErrorFriendCustomPacket.Ok;
            bool result = tox1.FriendSendLossyPacket(0, data, out error);
            if (!result || error != ToxErrorFriendCustomPacket.Ok)
                Assert.Fail("Failed to send lossy packet to friend, error: {0}, result: {1}", error, result);

            while (receivedPackets != 1) { DoIterate(); }
        }

        [Test]
        public void TestToxLosslessPacket()
        {
            int receivedPackets = 0;
            byte[] data = new byte[ToxConstants.MaxCustomPacketSize];
            new Random().NextBytes(data);
            data[0] = 170;

            tox2.OnFriendLosslessPacketReceived += (sender, args) =>
            {
                if (args.Data.Length != data.Length || data[0] != args.Data[0])
                    Assert.Fail("Packet doesn't have the same length/identifier");
                else if (!data.SequenceEqual(args.Data))
                    Assert.Fail("Packet contents don't match");

                receivedPackets++;
            };

            var error = ToxErrorFriendCustomPacket.Ok;
            bool result = tox1.FriendSendLosslessPacket(0, data, out error);
            if (!result || error != ToxErrorFriendCustomPacket.Ok)
                Assert.Fail("Failed to send lossless packet to friend, error: {0}, result: {1}", error, result);

            while (receivedPackets != 1) { DoIterate(); }
        }

        [Test]
        public void TestToxFileTransfer()
        {
            byte[] fileData = new byte[0xBEEEF];
            byte[] receivedData = new byte[0xBEEEF];
            new Random().NextBytes(fileData);

            string fileName = "testing.dat";
            bool fileReceived = false;

            tox2.OnFileSendRequestReceived += (sender, args) =>
            {
                if (fileName != args.FileName)
                    Assert.Fail("Filenames do not match");

                if (args.FileSize != fileData.Length)
                    Assert.Fail("File lengths do not match");

                var error2 = ToxErrorFileControl.Ok;
                bool result = tox2.FileControl(args.FriendNumber, args.FileNumber, ToxFileControl.Resume, out error2);
                if (!result || error2 != ToxErrorFileControl.Ok)
                    Assert.Fail("Failed to send file control, error: {0}, result: {1}", error2, result);
            };

            var error = ToxErrorFileSend.Ok;
            var fileInfo = tox1.FileSend(0, ToxFileKind.Data, fileData.Length, fileName, out error);
            if (error != ToxErrorFileSend.Ok)
                Assert.Fail("Failed to send a file send request, error: {0}", error);

            tox1.OnFileChunkRequested += (sender, args) =>
            {
                byte[] data = new byte[args.Length];
                Array.Copy(fileData, args.Position, data, 0, args.Length);

                var error2 = ToxErrorFileSendChunk.Ok;
                bool result = tox1.FileSendChunk(args.FriendNumber, args.FileNumber, args.Position, data, out error2);
                if (!result || error2 != ToxErrorFileSendChunk.Ok)
                    Assert.Fail("Failed to send chunk, error: {0}, result: {1}", error2, result);
            };

            tox2.OnFileChunkReceived += (sender, args) =>
            {
                if (args.Position == fileData.Length)
                    fileReceived = true;
                else
                    Array.Copy(args.Data, 0, receivedData, args.Position, args.Data.Length);
            };

            while (!fileReceived) { DoIterate(); }

            if (!fileData.SequenceEqual(receivedData))
                Assert.Fail("Original data is not equal to the data we received");
        }
    }
}
