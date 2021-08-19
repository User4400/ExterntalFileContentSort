using System;
using System.Collections.Generic;
using System.IO;

namespace Custis.TestCase.FileContentSort
{
    public static class FileContentSorter
    {
        /// <summary>
        /// Sorts file content using external memory
        /// </summary>
        /// <param name="filePath">Path to target file</param>
        /// <param name="chunkSize">File will be splitted to parts of this size</param>
        public static void SortFileContent(string filePath, long chunkSize = 104857600)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentException("Invalid file path");
            if (chunkSize < 1L)
                throw new ArgumentException("Chunk size must be at least 1.");
            FileContentSorter.SplitFile(filePath, chunkSize);
            FileContentSorter.SortChunks(Directory.GetFiles("C:\\fileSortTest\\", "split*.temp"));
            FileContentSorter.MergeChunks(Directory.GetFiles("C:\\fileSortTest\\", "sorted*.temp"), "C:\\fileSortTest\\BigFileSorted.txt");
        }

        /// <summary>
        /// Split file to chunks
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="chunkSize"></param>
        private static void SplitFile(string filePath, long chunkSize)
        {
            int chunkCount = 1;
            var dirPath = Path.GetDirectoryName(filePath);
            var chunkPath = Path.Combine(dirPath, $"split{chunkCount}.temp");

            var streamWriter = new StreamWriter(chunkPath);
            var streamReader = new StreamReader(filePath);
            try
            {
                while (streamReader.Peek() >= 0)
                {
                    streamWriter.WriteLine(streamReader.ReadLine());
                    if (streamWriter.BaseStream.Length > chunkSize && streamReader.Peek() >= 0)
                    {
                        streamWriter.Close();
                        ++chunkCount;
                        chunkPath = Path.Combine(dirPath, $"split{chunkCount}.temp");
                        streamWriter = new StreamWriter(chunkPath);
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                streamWriter.Close();
                streamReader.Close();
            }
        }

        /// <summary>
        /// Merge sorted chunks to single file
        /// </summary>
        /// <param name="chunkPaths"></param>
        /// <param name="mergedFilePath"></param>
        private static void MergeChunks(string[] chunkPaths, string mergedFilePath)
        {
            int length = chunkPaths.Length;
            int recordSize = 100;
            int maxUsage = 500000000 / length;
            double recordOverhead = 7.5;
            int bufferLen = (int)((maxUsage / recordSize) / recordOverhead);

            // Open files
            var readers = new StreamReader[length];
            for (int index = 0; index < length; ++index)
            {
                readers[index] = new StreamReader(chunkPaths[index]);
            }

            // Create queues
            var queues = new Queue<string>[length];
            for (int index = 0; index < length; ++index)
            {
                queues[index] = new Queue<string>(bufferLen);
            }

            for (int index = 0; index < length; ++index)
            {
                LoadQueue(queues[index], readers[index], bufferLen);
            }

            using (var streamWriter = new StreamWriter(mergedFilePath))
            {
                while (true)
                {

                    // Find the chunk with the lowest value
                    int lowestIndex = -1;
                    string lowestValue = "";
                    for (int index2 = 0; index2 < length; ++index2)
                    {
                        if (queues[index2] != null && (lowestIndex < 0 || 
                            string.Compare(queues[index2].Peek(), lowestValue, StringComparison.CurrentCulture) < 0))
                        {
                            lowestIndex = index2;
                            lowestValue = queues[index2].Peek();
                        }
                    }

                    // Was nothing found in any queue? We must be done then.
                    if (lowestIndex == -1)
                    {
                        break;
                    }

                    streamWriter.WriteLine(lowestValue);

                    queues[lowestIndex].Dequeue();
                    
                    if (queues[lowestIndex].Count == 0)
                    {
                        LoadQueue(queues[lowestIndex], readers[lowestIndex], bufferLen);
                        if (queues[lowestIndex].Count == 0)
                        {
                            queues[lowestIndex] = null;
                        }
                    }
                }
            }

            // Close and delete the files
            for (int index = 0; index < length; ++index)
            {
                readers[index].Close();
                File.Delete(chunkPaths[index]);
            }
        }

        /// <summary>
        /// Sort speciefied chunks
        /// </summary>
        /// <param name="chunkPaths"></param>
        private static void SortChunks(string[] chunkPaths)
        {
            foreach (string chunkPath in chunkPaths)
            {
                string[] content = File.ReadAllLines(chunkPath);
                Array.Sort(content);
                
                File.WriteAllLines(chunkPath.Replace("split", "sorted"), content);
                File.Delete(chunkPath);
         
                GC.Collect();
            }
        }

        private static void LoadQueue(Queue<string> queue, StreamReader file, int itemsCount)
        {
            for (int index = 0; index < itemsCount && file.Peek() >= 0; ++index)
            {
                queue.Enqueue(file.ReadLine());
            }
        }
    }
}
