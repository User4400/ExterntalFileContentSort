using System;
using System.Collections.Generic;
using System.IO;
using System.Management;
using System.Text;

namespace Custis.TestCase.FileContentSort
{
    internal static class TextFileGenerator
    {
        private static Random _random = new Random();

        /// <summary>
        /// Generate text file with random content
        /// </summary>
        public static void CreateRandomTextFile(string filePath, long fileSize, int minLineLength, int maxLineLength, long chunkSize)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentException("Invalid file path");
            if (fileSize < 1L)
                throw new ArgumentException("File size must be at least 1.");
            if (minLineLength < 1)
                throw new ArgumentException("Min line length must be at least 1.");
            if (maxLineLength < 1)
                throw new ArgumentException("Max line length must be at least 1.");

            char[] allowedCharsSequence = GetAllowedCharsSequence();
            long unwrittenBytes = fileSize;
            string fileContent;

            using (var file = File.CreateText(filePath))
            {
                Console.WriteLine("Write with splitting");
                
                do
                {
                    Console.WriteLine($@"Progress {100L - unwrittenBytes / (fileSize / 100L)}%  |  Processed {fileSize - unwrittenBytes}/{fileSize} bytes");

                    fileContent = CreateFileContent(chunkSize, minLineLength, maxLineLength, allowedCharsSequence);
                    file.Write(fileContent);
                    unwrittenBytes -= chunkSize;
                }
                while (unwrittenBytes > 0L && unwrittenBytes < fileSize);

                Console.WriteLine("File generating finished");
            }
        }

        /// <summary>
        /// Generate file content
        /// </summary>
        private static string CreateFileContent(long length, int minLineLength, int maxLineLength, char[] allowedCharacters)
        {
            var sb = new StringBuilder();
            char[] randomCharSequence;

            for (int i = 0; i < length; i += randomCharSequence.Length)
            {
                long writtenData = length - i;
                if (maxLineLength <= writtenData)
                {
                    randomCharSequence = GenerateRandomCharSequence(_random.Next(minLineLength, maxLineLength), allowedCharacters);
                    sb.Append(randomCharSequence);
                    sb.AppendLine();
                }
                else
                {
                    break;
                }
            }

            GC.Collect();

            return sb.ToString();
        }

        /// <summary>
        /// Returns allowed characters. Low case letters and digits.
        /// </summary>
        private static char[] GetAllowedCharsSequence()
        {
            var charList = new List<char>();
        
            // Add low case letters
            for (int i = 97; i < 123; ++i)
            {
                char ch = (char)i;
                charList.Add(ch);
            }
            
            // Add digits
            for (int i = 48; i < 58; ++i)
            {
                char ch = (char)i;
                charList.Add(ch);
            }
            
            return charList.ToArray();
        }

        /// <summary>
        /// Generates random char sequence from allowed characters
        /// </summary>
        /// <param name="length"></param>
        /// <param name="allowedChars"></param>
        /// <returns></returns>
        private static char[] GenerateRandomCharSequence(int length, params char[] allowedChars)
        {
            char[] chars = new char[length];
            int randomIndex;
            char allowedChar;

            for (int i = 0; i < length; ++i)
            {
                randomIndex = _random.Next(0, allowedChars.Length);
                allowedChar = allowedChars[randomIndex];
                chars[i] = allowedChar;
            }
            
            return chars;
        }
    }
}
