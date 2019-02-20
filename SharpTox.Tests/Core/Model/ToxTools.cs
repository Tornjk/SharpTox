using System;
using System.Collections;
using NUnit.Framework;
using SharpTox.Core;

namespace SharpTox.Core.UnitTests
{
    [TestFixture(TestOf =typeof(ToxTools))]
    public class ToxTools_Test
    {
        [TestCaseSource(nameof(StringToBinaryCases))]
        public void StringToHexBin_Result_AreEqual(string input, byte[] expected)
        {
            var result = ToxTools.StringToHexBin(input);
            Assert.AreEqual(expected, result);
        }


        [TestCaseSource(nameof(BinaryToStringCases))]
        public void HexBinToString_Result_AreEqual(byte[] input, string expected)
        {
            var result = ToxTools.HexBinToString(input);
            Assert.AreEqual(expected, result);
        }

        public static IEnumerable BinaryToStringCases
        {
            get
            {
                yield return new TestCaseData(new byte[0], "");
                yield return new TestCaseData(new byte[] { 1, 2, 3 }, "010203");
                yield return new TestCaseData(new byte[] { 0xFF, 0x32, 0x51 }, "FF3251");
            }
        }

        public static IEnumerable StringToBinaryCases
        {
            get
            {
                yield return new TestCaseData("", new byte[0]);
                yield return new TestCaseData("010203", new byte[] { 1, 2, 3 });
                yield return new TestCaseData("FC8422", new byte[] { 0xFC, 0x84, 0x22 });
            }
        }

        [TestCase("FFFFFF")]
        [TestCase("FACB12")]
        [TestCase("123456")]
        [TestCase("0102")]
        [TestCase("00")]
        public void ValidHexString_Input_IsTrue(string hexString)
        {
            var result = ToxTools.ValidHexString(hexString);
            Assert.IsTrue(result);
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("F")]
        [TestCase("GFF8")]
        [TestCase("     ")]
        [TestCase("FFFFFFFFFFFFFFFFFFFFF")]
        public void ValidHexString_Input_IsFalse(string hexString)
        {
            var result = ToxTools.ValidHexString(hexString);
            Assert.IsFalse(result);
        }
    }
}
