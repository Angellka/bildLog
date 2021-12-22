using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NFileSystem
{
    public static class CFileSystem
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="directory"></param>
        /// <returns></returns>
        public static FileInfo CreateCurrentLogFile(DirectoryInfo directory)
        {
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="file"></param>
        public static void RenameLogFileToErrorLogFile(FileInfo file)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="directory"></param>
        /// <param name="period_days"></param>
        public static void DeleteOldFiles(DirectoryInfo directory, int period_days)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="f_out"></param>
        /// <param name="f_to"></param>
        public static void RemoveFile(string f_out, string f_to)
        {            
            if ((new FileInfo(f_to)).Exists) File.Delete(f_to);
            File.Move(f_out, f_to);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="f_out"></param>
        /// <param name="f_to"></param>
        public static void RemoveFile(FileInfo f_out, FileInfo f_to)
        {            
            CFileSystem.RemoveFile(f_out.FullName, f_to.FullName);
        }   
        
        public static void CreateDirectoryIfPossible(string directory)
        {
            if (Directory.Exists(directory) == false)
                Directory.CreateDirectory(directory);
        }           
    }
}
