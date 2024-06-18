using System.Collections.Generic;
using System.IO;

namespace AllanDouglas.CactusInjector
{
    public static class GenerateInjectableTags
    {
        public const string TagsFilePath = "Assets/CactusInjector/Runtime/Src/Injector/_Generated_/GeneratedInjectableTags.g.cs";

        static public void GenerateFile(IEnumerable<string> strings, string filePath = TagsFilePath)
        {
            if (!Directory.Exists(Path.GetDirectoryName(filePath)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(filePath));
            }

            // Create or overwrite the file
            using StreamWriter writer = new(filePath);
            // Write the file header
            writer.WriteLine("namespace AllanDouglas.CactusInjector {");
            writer.WriteLine("  public static class InjectTag");
            writer.WriteLine("  {");

            // Write each string as a public const string
            foreach (string str in strings)
            {
                writer.WriteLine($"     public const string {GetValidIdentifier(str)} = \"{str}\";");
            }

            // Write the file footer
            writer.WriteLine("  }");
            writer.WriteLine("}");
        }

        // Helper method to generate a valid C# identifier from a string
        static string GetValidIdentifier(string input)
        {
            // Replace invalid characters with underscores and ensure it starts with a letter
            string validIdentifier = input.Replace(" ", "_");
            if (!char.IsLetter(validIdentifier[0]))
            {
                validIdentifier = "_" + validIdentifier;
            }

            return validIdentifier;
        }
    }
}
