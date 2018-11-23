using NUnit.Framework;
using SharpTox.Core;
using SharpTox.Encryption;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SharpTox.Test
{
    [TestFixture]
    public class CoreSelfTests
    {
        [Test]
        public void TestToxPortBind()
        {
            using (var tox = new Tox(new ToxOptions { Ipv6Enabled = true, UdpEnabled = false }))
            {
                var error = ToxErrorGetPort.Ok;
                var port = tox.GetUdpPort(out error);
                Assert.AreEqual(ToxErrorGetPort.NotBound, error, "Tox bound to an udp port while it's not supposed to, port: {0}", port);
            }

            using (var tox = new Tox(new ToxOptions { Ipv6Enabled = true, UdpEnabled = true }))
            {
                var error = ToxErrorGetPort.Ok;
                var port = tox.GetUdpPort(out error);
                Assert.AreEqual(ToxErrorGetPort.Ok, error, "Failed to bind to an udp port");
            }
        }

        [Test]
        public void TestToxLoadData()
        {
            using (var tox1 = new Tox(ToxOptions.Default()))
            {
                tox1.Name = "Test";
                tox1.StatusMessage = "Hey";

                var data = tox1.GetData();
                using (var tox2 = new Tox(ToxOptions.Default(), ToxData.FromBytes(data.Bytes)))
                {
                    Assert.AreEqual(tox1.Id, tox2.Id, "Failed to load tox data correctly, tox id's don't match");
                    Assert.AreEqual(tox1.Name, tox2.Name, "Failed to load tox data correctly, names don't match");
                    Assert.AreEqual(tox1.StatusMessage, tox2.StatusMessage, "Failed to load tox data correctly, status messages don't match");
                }
            }
        }

        [Test]
        public void TestToxLoadSecretKey()
        {
            using (var tox1 = new Tox(ToxOptions.Default()))
            {
                var key1 = tox1.GetSecretKey();
                using (var tox2 = new Tox(ToxOptions.Default(), key1))
                {
                    Assert.AreEqual(key1, tox2.GetSecretKey(), "Private keys do not match");
                }
            }
        }

        [Test]
        public void TestToxSelfName()
        {
            using (var tox = new Tox(ToxOptions.Default()))
            {
                string name = "Test name";
                tox.Name = name;
                Assert.AreEqual(name, tox.Name, "Failed to set/retrieve name");
            }
        }

        [Test]
        public void TestToxSelfStatusMessage()
        {
            using (var tox = new Tox(ToxOptions.Default()))
            {
                string statusMessage = "Test status message";
                tox.StatusMessage = statusMessage;
                Assert.AreEqual(statusMessage, tox.StatusMessage, "Failed to set/retrieve status message");
            }
        }

        [Test]
        public void TestToxSelfStatus()
        {
            using (var tox = new Tox(ToxOptions.Default()))
            {
                var status = ToxUserStatus.Away;
                tox.Status = status;
                Assert.AreEqual(status, tox.Status, "Failed to set/retrieve status");
            }
        }

        [Test]
        public void TestToxNospam()
        {
            using (var tox = new Tox(ToxOptions.Default()))
            {
                byte[] randomBytes = new byte[ToxConstants.NospamSize];
                new Random().NextBytes(randomBytes);

                var nospam = BitConverter.ToUInt32(randomBytes, 0);
                tox.SetNospam(nospam);
                Assert.AreEqual(nospam, tox.GetNospam(), "Failed to set/get nospam correctly, values don't match");
            }
        }

        [Test]
        public void TestToxId()
        {
            using (var tox = new Tox(ToxOptions.Default()))
            {
                Assert.AreEqual(tox.Id, new ToxId(tox.Id.PublicKey.GetBytes(), tox.Id.Nospam), "Tox id's are not equal");
            }
        }

        [Test]
        public void TestToxEncryption()
        {
            string password = "heythisisatest";
            byte[] garbage = new byte[0xBEEF];
            new Random().NextBytes(garbage);

            byte[] encryptedData = ToxEncryption.Encrypt(garbage, password, out _);
            Assert.IsNotNull(encryptedData, "Failed to encrypt the data");

            byte[] decryptedData = ToxEncryption.Decrypt(encryptedData, password, out _);
            Assert.IsNotNull(decryptedData, "Failed to decrypt the data");

            if (!garbage.SequenceEqual(decryptedData))
            {
                Assert.Fail("Original data is not equal to the decrypted data");
            }
        }

        [Test]
        public void TestToxEncryptionLoad()
        {
            using (var options = ToxOptions.Default())
            using (var tox1 = new Tox(options))
            {
                tox1.Name = "Test";
                tox1.StatusMessage = "Hey";

                string password = "heythisisatest";
                var data = tox1.GetData(password, out _);

                Assert.IsNotNull(data, "Failed to encrypt the Tox data");
                Assert.IsTrue(data.IsEncrypted, "We encrypted the data, but toxencryptsave thinks we didn't");

                using (var tox2 = new Tox(options, ToxData.FromBytes(data.Bytes), password))
                {
                    Assert.AreEqual(tox1.Id, tox2.Id, "Failed to load tox data correctly, tox id's don't match");
                    Assert.AreEqual(tox1.Name, tox2.Name, "Failed to load tox data correctly, names don't match");
                    Assert.AreEqual(tox1.StatusMessage, tox2.StatusMessage, "Failed to load tox data correctly, status messages don't match");
                }
            }
        }

        [Test, Ignore("Requires a SOCKS5")]
        [MaxTime(10000)]
        public async Task TestToxProxySocks5()
        {
            var options = new ToxOptions
            {
                Ipv6Enabled = true,
                ProxyType = ToxProxyType.Socks5,
                ProxyHost = "127.0.0.1",
                ProxyPort = 9050
            };

            using (var tox = new Tox(options))
            {
                var error = ToxErrorBootstrap.Ok;

                foreach (var node in Globals.TcpRelays)
                {
                    bool result = tox.AddTcpRelay(node, out error);
                    if (!result || error != ToxErrorBootstrap.Ok)
                        Assert.Fail("Failed to bootstrap, error: {0}, result: {1}", error, result);
                }

                var connected = new TaskCompletionSource<bool>();
                tox.OnConnectionStatusChanged += (o, e) =>
                {
                    connected.SetResult(e.Status != ToxConnectionStatus.None);
                };

                tox.Start();

                await ToxTest.AssertTimeout(TimeSpan.FromSeconds(10), connected.Task);

                tox.Stop();
            }
        }

        [Test]
        public async Task TestToxFriendRequest()
        {
            using (var tox1 = new Tox(new ToxOptions { Ipv6Enabled = true, UdpEnabled = true }))
            using (var tox2 = new Tox(new ToxOptions { Ipv6Enabled = true, UdpEnabled = true }))
            {

                var error = ToxErrorFriendAdd.Ok;
                string message = "Hey, this is a test friend request.";
                //bool testFinished = false;

                Bootstrap(tox1);
                Bootstrap(tox2);

                var completed = new TaskCompletionSource<bool>();
                tox2.OnFriendRequestReceived += (sender, args) =>
                {
                    Assert.AreEqual(message, args.Message, "Message received in the friend request is not the same as the one that was sent");

                    var friend = tox2.AddFriendNoRequest(args.PublicKey, out error);
                    Assert.AreEqual(ToxErrorFriendAdd.Ok, error, "Failed to add friend (no request): {0}", error);

                    Assert.IsTrue(tox2.FriendExists(friend), "Friend doesn't exist according to core");

                    completed.TrySetResult(true);
                };

                tox1.AddFriend(tox2.Id, message, out error);
                Assert.AreEqual(ToxErrorFriendAdd.Ok, error, "Failed to add friend: {0}", error);

                tox1.Start();
                tox2.Start();

                await ToxTest.AssertTimeout(TimeSpan.FromMinutes(1), completed.Task);

                tox1.Stop();
                tox2.Stop();
            }
        }

        [Test]
        public void TestToxDataParsing()
        {
            using (var tox = new Tox(ToxOptions.Default()))
            {
                tox.Name = "Test";
                tox.StatusMessage = "Status";
                tox.Status = ToxUserStatus.Away;

                var data = tox.GetData();
                ToxDataInfo info = null;

                if (data == null || !data.TryParse(out info))
                    Assert.Fail("Parsing the data file failed");

                if (info.Id != tox.Id || info.Name != tox.Name || info.SecretKey != tox.GetSecretKey() || info.Status != tox.Status || info.StatusMessage != tox.StatusMessage)
                    Assert.Fail("Parsing the data file failed");
            }
        }

        [Test]
        public void TestToxIsDataEncrypted()
        {
            using (var tox = new Tox(ToxOptions.Default()))
            {
                Assert.IsFalse(tox.GetData().IsEncrypted);
            }
        }

        private static void Bootstrap(Tox tox)
        {
            foreach (var node in Globals.Nodes)
            {
                var success = tox.Bootstrap(node, out _);
                if (!success)
                {
                    throw new Exception("Bootstrap not successful");
                }
            }
        }
    }
}
