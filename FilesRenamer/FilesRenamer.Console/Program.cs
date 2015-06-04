using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FilesRenamer.Console
{
    class Program
    {
        private const int TotalWidth = 3;

        private static void Main(string[] args)
        {
            var directoryPath = GetFolderParh(args);

            RenameFilesInDirectory(directoryPath);
        }

        private static void RenameFilesInDirectory(string directoryPath)
        {
            if (!Directory.Exists(directoryPath))
            {
                throw new DirectoryNotFoundException();
            }
            var filePathList = Directory.GetFiles(directoryPath);
            foreach (var file in filePathList)
            {
                System.Console.WriteLine(file);
            }
            var files = filePathList.Select(x => new FileInfo(x)).OrderBy(x => x.Name).ToArray();
            for (int i = 1; i < files.Length+1; i++)
            {
                var file = files[i];
                var extension = file.Extension;
                var newFileName = i.ToString().PadLeft(TotalWidth, '0');
                var newFilePath = Path.Combine(directoryPath, newFileName + extension);
                File.Move(file.FullName, newFilePath);
            }
        }

        private static string GetFolderParh(string[] args)
        {
            string folderPath = null;
            if (args.Length < 1)
            {
                System.Console.WriteLine("Не заданы аргументы");
                throw new ArgumentException("Не заданы аргументы");
            }
            if (args.Length == 1)
            {
                folderPath = args[0];
            }
            return folderPath;
        }
    }
}
