using System;
using NUnit.Framework;
using SharpTox.Core;

namespace SharpTox.Tests
{
    [TestFixture]
    public class ToxKey_Test
    {
        [Test]
        public void Constructor_ByteArrayNotOfLengthPublicToxKeyLength_ArgumentException()
        {
            Assert.Throws<ArgumentException>(() => new ToxKey(ToxKeyType.Public, new byte[ToxConstants.PublicKeySize + 1]));
        }

        [Test]
        public void Constructor_ByteArrayNotOfLengthSecretToxKeyLength_ArgumentException()
        {
            Assert.Throws<ArgumentException>(() => new ToxKey(ToxKeyType.Secret, new byte[ToxConstants.SecretKeySize + 1]));
        }

        [Test]
        public void Constructor_NullBytes_ArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new ToxKey(ToxKeyType.Public, (byte[])null));
        }
        [Test]
        public void Constructor_NullString_ArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new ToxKey(ToxKeyType.Public, (string)null));
        }

        [TestCase(ToxKeyType.Public, ToxConstants.PublicKeySize)]
        [TestCase(ToxKeyType.Secret, ToxConstants.SecretKeySize)]
        public void GetBytes_InputByteArray_AreNotSame(ToxKeyType keyType, int keySize)
        {
            var input = new byte[keySize];
            var key = new ToxKey(keyType, input);
            var result = key.GetBytes();

            Assert.AreNotSame(input, result);
        }

        [TestCase(ToxKeyType.Public, ToxConstants.PublicKeySize)]
        [TestCase(ToxKeyType.Secret, ToxConstants.SecretKeySize)]
        public void Keys_SameByteArray_AreEqual(ToxKeyType keyType, int keySize)
        {
            var key = new byte[keySize];
            var key1 = new ToxKey(keyType, key);
            var key2 = new ToxKey(keyType, key);
            Assert.AreEqual(key1, key2);
        }

        [Test]
        public void Equals_DifferentByteArrays_IsFalse()
        {
            var bytes = new byte[ToxConstants.PublicKeySize];
            var key1 = new ToxKey(ToxKeyType.Public, bytes);

            bytes[0] = 100;
            var key2 = new ToxKey(ToxKeyType.Public, bytes);
            var equal = key1 == key2;
            Assert.IsFalse(equal);
        }
    }
}
