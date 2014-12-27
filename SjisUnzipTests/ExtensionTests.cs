using System;
using System.Globalization;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SjisUnzip;

namespace SjisUnzipTests
{
	[TestClass]
	public class ExtensionTests
	{
		private readonly Encoding sjisEncoding = Encoding.GetEncoding(932);

		private readonly string textGarbled = "iƒeƒLƒXƒgƒtƒ@ƒCƒ‹j"; //  "（テキストファイル）"
		private readonly string textCorrect = "（テキストファイル）"; // "(TextFile)"
		private readonly string textAscii = "\"One bad programmer can easily create two new jobs a year.\" - David Parnas";

		[TestMethod]
		public void TestContainsNonAscii()
		{
			var res = textGarbled.ContainsNonAscii();
			Assert.IsTrue(res);

			res = textCorrect.ContainsNonAscii();
			Assert.IsTrue(res);

			res = textAscii.ContainsNonAscii();
			Assert.IsFalse(res);
		}

		[TestMethod]
		public void TestRawTranscode()
		{
			var degarbled = textGarbled.RawTranscode(Encoding.Default, Encoding.GetEncoding(932));
            Assert.AreEqual(textCorrect, degarbled, "Degarbled text should be equivalent to the uncorrupted original.");

			var engarbled = textCorrect.RawTranscode(Encoding.GetEncoding(932), Encoding.Default);
			Assert.AreEqual(textGarbled, engarbled, "Reversing the garbling process on correct text should match the example garbled version.");
		}

		[TestMethod]
		public void TestDecodeMojibake()
		{
			var degarbled = textGarbled.DecodeMojibake();
			Assert.AreEqual(textCorrect, degarbled, "Degarbled text should be equivalent to the uncorrupted original.");
		}

		[TestMethod]
		public void TestContainsJapanese()
		{
			var res = textAscii.ContainsJapanese();
			Assert.IsFalse(res, "Plain ascii strings should obviously not trigger true on this function.");

			res = textGarbled.ContainsJapanese();
			Assert.IsFalse(res);

			res = textCorrect.ContainsJapanese();
			Assert.IsTrue(res);
		}
	}
}
