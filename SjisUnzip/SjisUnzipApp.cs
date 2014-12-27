using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Compression;

namespace SjisUnzip
{
	public class SjisUnzipApp
	{
		private readonly Encoding sjisEncoding = Encoding.GetEncoding(932);

		public void Main(string[] args)
		{
			var recode = args.Any((s) => s.Equals("-r"));

			if (recode && args.Length == 2)
			{
				args = args.Where((arg) => arg != "-r").ToArray();
				if (args.Length > 0 && File.Exists(args[0]))
				{
					recodeFile(args[0]);
				}
			}
			else if (args.Length == 1 && Directory.Exists(args[0]))
			{
				recodeCorruptFilenames(args[0], true);
			}
			else if (args.Length == 1 && File.Exists(args[0]) && args[0].EndsWith(".zip", true, CultureInfo.CurrentCulture))
			{
				extractSjisZip(args[0]);
			}
			else if (args.Length == 2 && File.Exists(args[0]) && args[0].EndsWith(".zip", true, CultureInfo.CurrentCulture))
			{
				var folderPath = Path.GetDirectoryName(args[0]);
				var newFolderPath = Path.Combine(folderPath, args[1]);
				Directory.CreateDirectory(newFolderPath);
				extractSjisZip(args[0], newFolderPath);
			}
			else
			{
				printUsage();
			}
		}

		static void printUsage()
		{
			"Usage: sjisunzip someFile.zip [toFolder]".wl();
			"Usage: sjisunzip [-r] someFile.zip".wl();
			"    -r: Recode file to {filename}_utf8.zip".wl();
			"Usage: sjisunzip ./some_folder_with_corrupt_filenames".wl();
			"Examples:".wl();
			"    sjisunzip aFile.zip".wl();
			"    sjisunzip aFile.zip MyNewFolder".wl();
		}

		private void extractSjisZip(string fileName, string toFolder = "./")
		{
			"Writing to folder {0}...".wl(toFolder);

			using (var zipFile = new ZipArchive(new FileStream(fileName, FileMode.Open), 
				ZipArchiveMode.Read, false, Encoding.GetEncoding(932)))
			{
				zipFile.ExtractToDirectory(toFolder);
			}

			"Done.".wl();
		}

		private void recodeFile(string srcFile)
		{
			var zipFile = new ZipArchive(new FileStream(srcFile, FileMode.Open), ZipArchiveMode.Read, false, sjisEncoding);
			var newFilePath = Path.Combine(Path.GetDirectoryName(srcFile), Path.GetFileNameWithoutExtension(srcFile) + "_utf8.zip");

			using (var newZip = new ZipArchive(new FileStream(newFilePath, FileMode.CreateNew), ZipArchiveMode.Create, false, Encoding.UTF8))
			{
				foreach (var zipEntry in zipFile.Entries)
				{
					var newFile = newZip.CreateEntry(zipEntry.FullName, CompressionLevel.Fastest);

					newFile.LastWriteTime = zipEntry.LastWriteTime;

					using (Stream newStream = newFile.Open(), oldStream = zipEntry.Open())
					{
						"Moved {0}".wl(newFile.FullName);
						oldStream.CopyTo(newStream);
					}
				}
			}

			"Finished recoding {0} entries.".wl(zipFile.Entries.Count);
        }

		readonly Func<char, bool> dirSeparatorComparator = c => c == Path.DirectorySeparatorChar;

		private void recodeCorruptFilenames(string directoryPath, bool recurse)
		{
			var rootDir = new DirectoryInfo(directoryPath);
			var dirs = rootDir.GetDirectories("*", recurse ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly).ToList();
			var files = rootDir.GetFiles("*", recurse ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);

			files.Where(fi => fi.Name.ContainsNonAscii() && !fi.Name.ContainsJapanese())
				.ToList()
				.ForEach(
					fi => fi.Rename(fi.Name.DecodeMojibake())
				);
			
			// Sort reversed based on directory depth, rename deepest first and this won't rename a root before a leaf.
			dirs.Sort((d2, d1) => d1.FullName.Count(dirSeparatorComparator).CompareTo(d2.FullName.Count(dirSeparatorComparator)));

			dirs.Where(di => di.Name.ContainsNonAscii() && !di.Name.ContainsJapanese())
				.ToList()
				.ForEach(
					di => di.Rename(di.Name.DecodeMojibake())
				);

			if (rootDir.Name.ContainsNonAscii() && !rootDir.Name.ContainsJapanese())
			{
				rootDir.Rename(rootDir.Name.DecodeMojibake());
			}
		}
	}
}
