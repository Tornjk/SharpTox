using System;
using System.Collections.Generic;
using System.Text;
using AutoFixture.NUnit3;
using NUnit.Framework;

namespace SharpTox.Core.UnitTests
{
    [TestFixture(TestOf = typeof(ToxId))]
    public class ToxId_Test
    {
        private byte[] invalidTestId;
        private byte[] validTestId;

        [SetUp]
        public void Setup()
        {
            this.invalidTestId = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38 };

            this.validTestId = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 2, 38 };
        }

        [Test]
        public void Constructor_NullIdBytes_ArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new ToxId((byte[])null));
        }

        [Test]
        public void Constructor_NullIdString_ArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new ToxId((string)null));
        }

        [Test, AutoData]
        public void Constructor_NullPublicKey_ArgumentNullException(uint nospam)
        {
            Assert.Throws<ArgumentNullException>(() => new ToxId(null, nospam));
        }

        [Test]
        public void Constructor_InvalidChecksum_ArgumentException()
        {
            Assert.Throws<ArgumentException>(() => new ToxId(this.invalidTestId));
        }

        [Test]
        public void Constructor_InvalidChecksumString_ArgumentException()
        {
            Assert.Throws<ArgumentException>(() => new ToxId(ToxTools.HexBinToString(this.invalidTestId)));
        }

        [Test]
        public void Constructor_IdNot38Bytes_ArgumentException([Range(0, 80, 4)] int length)
        {
            Assert.Throws<ArgumentException>(() => new ToxId(new byte[length]));
        }

        [Test]
        public void GetBytes_Result_NotSameAsInput()
        {
            var id = new ToxId(this.validTestId);
            var result = id.GetBytes();
            Assert.AreNotSame(this.validTestId, result);
        }

        [Test]
        public void GetBytes_Result_AreEqualAsInput()
        {
            var id = new ToxId(this.validTestId);
            var result = id.GetBytes();
            Assert.AreEqual(this.validTestId, result);
        }

        [Test]
        public void IsValid_ValidId_IsTrue()
        {
            var result = ToxId.IsValid(this.validTestId);
            Assert.IsTrue(result);
        }

        [Test]
        public void IsValid_InvalidId_IsFalse()
        {
            var result = ToxId.IsValid(this.invalidTestId);
            Assert.IsFalse(result);
        }

        [Test]
        public void IsValid_ValidIdString_IsTrue()
        {
            var result = ToxId.IsValid(ToxTools.HexBinToString(this.validTestId));
            Assert.IsTrue(result);
        }

        [Test]
        public void IsValid_InvalidIdString_IsFalse()
        {
            var result = ToxId.IsValid(ToxTools.HexBinToString(this.invalidTestId));
            Assert.IsFalse(result);
        }
    }
}
