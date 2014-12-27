using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SjisUnzip
{
	public static class ExtensionMethods
	{
		//
		// System.String
		//

		/// <summary>
		/// Checks whether a string contains any characters outside the typical ascii range. 127 or higher.
		/// </summary>
		/// <param name="str">A string dum dum.</param>
		/// <returns>Boolean flag</returns>
		public static bool ContainsNonAscii(this string str)
		{
			// http://www.asciitable.com/
			return str.Any(t => t > 0x7E);
		}

		/// <summary>
		/// A function to do a raw reinterpretation of character encoding without using the .Convert() function.
		/// This allows accidentally transposed strings to have their encoding fixed by reversing the process.
		/// </summary>
		/// <param name="str">The string being operated on.</param>
		/// <param name="from">The source encoding the bytes will be extracted as.</param>
		/// <param name="to">The destination encoding the bytes will be 'interpreted' as.</param>
		/// <returns>The reinterpreted string.</returns>
		public static string RawTranscode(this string str, Encoding from, Encoding to)
		{
			var rawBytes = from.GetBytes(str);
			return to.GetString(rawBytes);
		}

		/// <summary>
		/// A wrapper for the RawTranscode() function which passes the correct parameters for fixing Sjis mishaps.
		/// </summary>
		/// <see cref="RawTranscode"/>
		/// <param name="str">The garbled string.</param>
		/// <returns>A fixed unicode string.</returns>
		public static string DecodeMojibake(this string str)
		{
			return str.RawTranscode(Encoding.Default, Encoding.GetEncoding(932));
		}

		/// <summary>
		/// Checks the contents of a string to see if any are within CJK and a few other unicode ranges.
		/// </summary>
		/// <remarks>
		/// See <a href="http://www.rikai.com/library/kanjitables/kanji_codes.unicode.shtml">Reference Document</a> for ranges.
		/// </remarks>
		/// <param name="str">Any string.</param>
		/// <returns>Boolean flag if any characters found.</returns>
		public static bool ContainsJapanese(this string str)
		{
			return str.Any(c =>
				// Punctuation, Hiragana, Katakana
				(c > 0x3000 && c < 0x30ff) ||
				// Romaji and half-width kana
				(c > 0xff00 && c < 0xffef) ||
				// CJK Extension
				(c > 0x3400 && c < 0x4dbf) ||
				// CJK
				(c > 0x4e00 && c < 0x9fff)
			);
		}

		/// <summary>
		/// Convenience method calling Console.WriteLine. Looks cool too.
		/// </summary>
		/// <example>
		/// "Hello World".wl();
		/// </example>
		public static void wl(this string str)
		{
			Console.WriteLine(str);
		}

		/// <summary>
		/// Convenience method for calling Console.WriteLine with params.
		/// </summary>
		/// <param name="str">The input format string.</param>
		/// <param name="o">Args for populating the format string.</param>
		/// <example>
		/// "Elapsed time was {0} seconds.".wl(timer.getSeconds());
		/// </example>
		public static void wl(this string str, params object[] o)
		{
			Console.WriteLine(str, o);
		}

		//
		// System.IO.FileInfo
		//

		/// <summary>
		/// Renames a file while keeping it in the same directory. The typical MoveTo is considered absolute,
		/// whereas this is considered relative.
		/// </summary>
		/// <param name="fi">The operating fileInfo.</param>
		/// <param name="newName">A new filename.</param>
		public static void Rename(this FileInfo fi, string newName)
		{
			fi.MoveTo(Path.Combine(fi.Directory.FullName, newName));
		}

		/// <summary>
		/// Renames a file in place and keeps the original extension regardless of what the new name is.
		/// </summary>
		/// <param name="fi">The operating fileInfo.</param>
		/// <param name="newName">A new filename which will have the original extension appended onto.</param>
		public static void RenameKeepExt(this FileInfo fi, string newName)
		{
			fi.MoveTo(Path.Combine(fi.Directory.FullName, newName) + Path.GetExtension(fi.FullName));
		}

		//
		// System.IO.DirectoryInfo
		//

		/// <summary>
		/// Does an in-place rename of a directory. Path modifications will not work properly (e.g. ../myname).
		/// </summary>
		/// <param name="di">The operating DirectoryInfo</param>
		/// <param name="newName">A new name for the directory.</param>
		public static void Rename(this DirectoryInfo di, string newName)
		{
			di.MoveTo(Path.Combine(di.Parent.FullName, newName));
		}
	}
}
