using System;
using System.Diagnostics;
using System.IO;

namespace Custis.TestCase.FileContentSort
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            #region Generate large file

            //var stopwatch = new Stopwatch();
            //var filePath = "C:\\fileSortTest\\testFile.txt";
            //long fileSize = 600 * 1024L * 1024L; // File size in bytes
            //long chunkSize = 100 * 1024L * 1024L;  // File size in bytes

            //if (!Directory.Exists("C:\\fileSortTest\\"))
            //{
            //    Directory.CreateDirectory("C:\\fileSortTest\\");
            //}

            //try
            //{
            //    stopwatch.Start();
            //    TextFileGenerator.CreateRandomTextFile(filePath, fileSize, minLineLength:8, maxLineLength:12, chunkSize);
            //}
            //catch (Exception ex)
            //{
            //    stopwatch.Stop();
            //    Console.WriteLine("[ERROR] " + ex.Message);
            //}
            //finally
            //{
            //    stopwatch.Stop();
            //    Console.WriteLine(string.Format("[Elapsed time] {0}", (object)stopwatch.Elapsed));
            //}

            #endregion


            #region Sort file

            //FileContentSorter.SortFileContent(filePath, chunkSize);

            #endregion
        }
    }
}
