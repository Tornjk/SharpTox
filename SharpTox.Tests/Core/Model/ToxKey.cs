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
        public void Equal_SameByteArray_IsTrue(ToxKeyType keyType, int keySize)
        {
            var key = new byte[keySize];
            var key1 = new ToxKey(keyType, key);
            var key2 = new ToxKey(keyType, key);
            var equal = key1 == key2;
            Assert.IsTrue(equal);
        }
    }
}
