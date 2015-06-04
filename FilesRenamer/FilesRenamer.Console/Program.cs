using System;

namespace FilesRenamer.Console
{
    class Program
    {
        private static void Main(string[] args)
        {
            var directoryPath = GetFolderParh(args);
            var fileRenamer = new FileRenamer();
            fileRenamer.RenameFilesInDirectory(directoryPath);
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
