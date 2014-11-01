using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Compression;

namespace SjisUnzip
{
	static class SjisUnzipApplication
	{
		static void printUsage()
		{
			Console.WriteLine("Usage: sjisunzip someFile.zip [toFolder]");
			Console.WriteLine("Usage: sjisunzip [-r] someFile.zip");
			Console.WriteLine("    -r: Recode file to {filename}_utf8.zip");
			Console.WriteLine("Examples:");
			Console.WriteLine("    sjisunzip aFile.zip");
			Console.WriteLine("    sjisunzip aFile.zip MyNewFolder");
		}

		static void Main(string[] args)
		{
			var recode = args.Any((s) => s.Equals("-r"));

			if (recode)
			{
				args = args.Where((arg) => arg != "-r").ToArray();
				if (args.Length > 0 && File.Exists(args[0]))
				{
					recodeFile(args[0]);
				}
			}
			else if (args.Length == 1 && File.Exists(args[0]))
			{
				extractSjisZip(args[0]);
			}
			else if (args.Length == 2 && File.Exists(args[0]))
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

		static void extractSjisZip(string fileName, string toFolder = "./")
		{
			Console.Write("Writing to folder {0}...", toFolder);
			using (var zipFile = new ZipArchive(new FileStream(fileName, FileMode.Open), 
				ZipArchiveMode.Read, false, Encoding.GetEncoding(932)))
			{
				zipFile.ExtractToDirectory(toFolder);
			}
			Console.WriteLine("Done.");
		}

		static void recodeFile(string srcFile)
		{
			var zipFile = new ZipArchive(new FileStream(srcFile, FileMode.Open), ZipArchiveMode.Read, false, Encoding.GetEncoding(932));
			var newFilePath = Path.Combine(Path.GetDirectoryName(srcFile), Path.GetFileNameWithoutExtension(srcFile) + "_utf8.zip");

			using (var newZip = new ZipArchive(new FileStream(newFilePath, FileMode.CreateNew), ZipArchiveMode.Create, false, Encoding.UTF8))
			{
				foreach (var zipEntry in zipFile.Entries)
				{
					var newFile = newZip.CreateEntry(zipEntry.FullName, CompressionLevel.Fastest);

					newFile.LastWriteTime = zipEntry.LastWriteTime;

					using (Stream newStream = newFile.Open(), oldStream = zipEntry.Open())
					{
						Console.WriteLine("Moved {0}", newFile.FullName);
						oldStream.CopyTo(newStream);
					}
				}
			}

			Console.WriteLine("Finished recoding {0} entries.", zipFile.Entries.Count);
		}
	}
}
