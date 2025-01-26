using System;
using System.IO;

namespace DXT1Decompressor
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8; // Enable Unicode support

            // Program title with Unicode and color
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("╔════════════════════════════════════════╗");
            Console.WriteLine("║       🎨 DXT1 Texture Decompressor     ║");
            Console.WriteLine("╚════════════════════════════════════════╝");
            Console.ResetColor();

            // Check path
            if (args.Length < 1)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("⚠️  Usage: App.exe <path/to/file>");
                Console.ResetColor();
                return;
            }

            string filePath = args[0];

            // Check file exists
            if (!File.Exists(filePath))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"❌ File not found: {filePath}");
                Console.ResetColor();
                return;
            }

            try
            {
                using (var stream = File.OpenRead(filePath))
                using (var br = new BinaryReader(stream))
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"📂 Loading file: {filePath}");
                    Console.ResetColor();

                    if (LoadTexture(br, out var texture))
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("\n✅ Successfully loaded texture:\n");
                        Console.ResetColor();

                        // Send data
                        Console.ForegroundColor = ConsoleColor.Blue;
                        Console.WriteLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
                        Console.WriteLine($"🖼️  Width       : {texture.width}");
                        Console.WriteLine($"🖼️  Height      : {texture.height}");
                        Console.WriteLine($"🎨 Components  : {texture.components}");
                        Console.WriteLine($"⚙️  Flags       : {texture.flags}");
                        Console.WriteLine($"🔄 Level       : {texture.level}");
                        Console.WriteLine($"📦 Size (bytes): {texture.size}");
                        Console.WriteLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
                        Console.ResetColor();
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("❌ Failed to load texture. The file may be invalid.");
                        Console.ResetColor();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"❌ Error: {ex.Message}");
                Console.ResetColor();
            }
        }

        public static bool LoadTexture(BinaryReader br, out LRFTexture texture)
        {
            texture = new LRFTexture();
            try
            {
                texture.width = br.ReadUInt32();
                texture.height = br.ReadUInt32();
                texture.components = br.ReadUInt32();
                texture.flags = (LRFTexture.Flags)br.ReadUInt32();
                texture.level = br.ReadUInt32();
                texture.size = br.ReadUInt64();

                if (texture.size > int.MaxValue)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("❌ Texture size is too large.");
                    Console.ResetColor();
                    return false;
                }

                texture.data = br.ReadBytes((int)texture.size);

                if (texture.data.Length != (int)texture.size)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("❌ Unexpected end of file while reading texture data.");
                    Console.ResetColor();
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"❌ Error reading texture: {ex.Message}");
                Console.ResetColor();
                return false;
            }
        }
    }

    public class LRFTexture
    {
        public uint width { get; set; }
        public uint height { get; set; }
        public uint components { get; set; }
        public Flags flags { get; set; }
        public uint level { get; set; }
        public ulong size { get; set; }
        public byte[] data { get; set; }

        public enum Flags
        {
            None = 0
        }
    }
}
