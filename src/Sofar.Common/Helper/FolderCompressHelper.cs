using System.IO.Compression;

namespace Sofar.Common.Helper
{
    public class FolderCompressHelper
    {
        /// <summary>
        /// 压缩一个文件夹到同名的ZIP文件
        /// </summary>
        public static async Task<string> ZipDirectory(string rootPath, DirectoryInfo folderDir)
        {
            var result = await Task.Run(() =>
            {
                string zipPath = Path.Combine(rootPath, $"{folderDir.Name}.zip");
                ZipFile.CreateFromDirectory(folderDir.FullName, zipPath);
                return zipPath;
            });

            return result;
        }
    }
}
