using System;
using System.Collections.Generic;
using MetadataExtractor;

namespace ExtractMetadataAndParse
{
    public static class ExtractMetadata
    {
        private static IReadOnlyList<Directory> _fileMetadata;

        public static IReadOnlyList<Directory> Extract(string file)
        {
            try
            {
                _fileMetadata = ImageMetadataReader.ReadMetadata(file);
            }
            catch
            {
                Console.CursorLeft = 0;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"File: {file} not support");
                Console.ResetColor();
                return null;
            }

            return _fileMetadata;
        }
    }
}