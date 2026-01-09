using System;
using System.Collections.Generic;
using System.IO;

namespace BioluminescentGames.Utils.Utilities
{
    public static class IOUtils
    {
        public static List<FileInfo> GetFilesInDirectory(DirectoryInfo directory,
            Func<DirectoryInfo, bool> ignorePredicate)
        {
            List<FileInfo> files = new List<FileInfo>();

            foreach (DirectoryInfo dir in directory.GetDirectories())
            {
                if (ignorePredicate(dir))
                    continue;

                files.AddRange(GetFilesInDirectory(dir, ignorePredicate));
            }

            files.AddRange(directory.GetFiles());

            return files;
        }

        public static List<FileInfo> GetFilesInDirectory(DirectoryInfo directory)
        {
            return GetFilesInDirectory(directory, _ => true);
        }

        public static void EnsureDirectory(string path)
        {
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
        }

        public static void CreateEmptyFile(string filename)
        {
            File.Create(filename).Dispose();
        }
    }
}
