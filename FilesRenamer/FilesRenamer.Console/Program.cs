using System.Threading.Tasks;
using NLog;

namespace FilesRenamer.Console
{
    class Program
    {
        private static void Main(string[] args)
        {
            string rootDirectory;
            if (!TryGetFolderPath(args, out rootDirectory))
            {
                return;
            }
            var fileRenamer = new FileRenamer();
            var task = Task.Factory.StartNew(()=>fileRenamer.RenameFilesInDirectory(rootDirectory));
            task.Wait();
        }
        
        /// <summary>
        /// Пытается разобрать аргументы и вытащить от туда папку назначения
        /// </summary>
        /// <param name="args">аргументы запуска приложения</param>
        /// <param name="rootDirectory">путь к папке, в которой необходимо переименовать файлы</param>
        /// <returns>тру, если удалось вытащить путь к папке назначения</returns>
        private static bool TryGetFolderPath(string[] args, out string rootDirectory)
        {
            rootDirectory = null;
            if (args.Length < 1 || args[0] == "/?")
            {
                System.Console.WriteLine(@"Задайте первым аргументом путь к папке. Например, "">FileRenamer.exe c:\MyPhotos""");
                LogManager.GetCurrentClassLogger().Warn("Заданы неверные аргументы или задан аргумент справки");
                return false;
            }
            rootDirectory = args[0];
            return true;
        }
    }
}
