using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DXT1Decompressor
{
    class Program
    {
        static void Main(string[] args)
        {
            // Set console to use UTF-8 encoding to support emojis
            Console.OutputEncoding = Encoding.UTF8;

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("╔════════════════════════════════════════╗");
            Console.WriteLine("║       🎨 DXT1 Texture Decompressor     ║");
            Console.WriteLine("╚════════════════════════════════════════╝");
            Console.ResetColor();

            string filePath;

            // Check if a file path is provided as a command-line argument
            if (args.Length > 0)
            {
                filePath = args[0];
                PrintMessage($"📂 Using file from arguments: {filePath}", ConsoleColor.Cyan);
            }
            else
            {
                // Prompt the user to enter the file path
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write("📥 Please enter the path to the LRF file: ");
                Console.ResetColor();
                filePath = Console.ReadLine();
            }

            // Validate the provided file path
            if (string.IsNullOrWhiteSpace(filePath))
            {
                PrintMessage("❌ No file path provided. Exiting.", ConsoleColor.Red);
                return;
            }

            if (!System.IO.File.Exists(filePath))
            {
                PrintMessage($"❌ File not found: {filePath}", ConsoleColor.Red);
                return;
            }

            try
            {
                PrintMessage($"🔍 Loading texture from: {filePath}\n", ConsoleColor.Yellow);
                var tex = LRFReader.LoadTexture(filePath);

                if (tex == null)
                {
                    PrintMessage("❌ Failed to load texture from the specified file.", ConsoleColor.Red);
                    return;
                }

                // Display texture details
                PrintMessage("✅ Texture loaded successfully.", ConsoleColor.Green);
                PrintMessage("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━", ConsoleColor.Blue);
                PrintMessage($"📏 Dimensions: {tex.width}x{tex.height}, Components: {tex.components}", ConsoleColor.Cyan);
                PrintMessage($"🖌  Format: {(tex.flags.HasFlag(LRFReader.LRFTexture.Flags.DXT1) ? "DXT1" : "RGB")}", ConsoleColor.Cyan);
                PrintMessage($"🔢 Mipmap level: {tex.level}", ConsoleColor.Cyan);
                PrintMessage($"📦 Size: {tex.size} bytes", ConsoleColor.Cyan);
                PrintMessage("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━\n", ConsoleColor.Blue);

                // Decompress the texture
                PrintMessage("🖥  Decompressing DXT1 texture...", ConsoleColor.Yellow);
                var decompressor = new DXT1Decompressor((int)tex.width, (int)tex.height, tex.data);
                var bitmap = decompressor.ToBitmap();

                // Generate output file name based on input file
                string outputFileName = System.IO.Path.GetFileNameWithoutExtension(filePath) + ".bmp";
                bitmap.Save(outputFileName);
                PrintMessage($"💾 Bitmap saved as: {outputFileName}", ConsoleColor.Green);
            }
            catch (Exception ex)
            {
                PrintMessage($"❌ An error occurred: {ex.Message}", ConsoleColor.Red);
            }
            finally
            {
                // Prompt to keep the terminal open
                PromptExit();
            }
        }

        /// <summary>
        /// Prints a colored message to the console with optional emoji support.
        /// </summary>
        /// <param name="message">The message to display.</param>
        /// <param name="color">The color of the message text.</param>
        static void PrintMessage(string message, ConsoleColor color)
        {
            var previousColor = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.WriteLine(message);
            Console.ForegroundColor = previousColor;
        }

        /// <summary>
        /// Prompts the user to press any key before exiting.
        /// </summary>
        static void PromptExit()
        {
            Console.WriteLine("\n🔚 Press any key to exit...");
            Console.ReadKey();
        }
    }
}
