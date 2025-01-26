using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DXT1Decompressor
{
    class LRFReader
    {
        public enum Type
        {
            Unknown, Model, Shader, Texture
        }

        const string lrf_magic = "Lrf ";
        const UInt32 lrf_magic_i = 0x4C726620;
        const string lrf_chunk_header = "LrfH";
        const string lrf_chunk_3d_palette = "LrP3";
        const string lrf_chunk_4d_palette = "LrP4";
        const string lrf_chunk_3d_mesh = "LrM3";
        const string lrf_chunk_shader = "LrS ";
        const string lrf_chunk_shaderprogram = "LrSP";
        const string lrf_chunk_texture = "LrTx";

        private static bool ReadChunkHeader(BinaryReader br, out string id, out UInt64 siz)
        {
            int pk = -1;
            try
            {
                pk = br.PeekChar();
            }
            catch (Exception)
            {
                id = null;
                siz = 0;
                return false;
            }
            if (pk == -1)
            {
                id = null;
                siz = 0;
                return false;
            }
            var bid = br.ReadChars(4);
            Array.Reverse(bid);
            siz = br.ReadUInt64();
            id = new string(bid);

            return true;
        }

        private static Type HeaderTypeToType(UInt32 type)
        {
            switch (type)
            {
                case 1: return Type.Model;
                case 2: return Type.Shader;
                case 3: return Type.Texture;
            }
            return Type.Unknown;
        }

        public static Type ArchiveType(string path)
        {
            FileStream fs = File.OpenRead(path);
            using (BinaryReader br = new BinaryReader(fs))
            {
                var magic = br.ReadUInt32(); // magic

                if (magic != lrf_magic_i)
                {
                    return Type.Unknown;
                }

                string id;
                UInt64 siz;

                if (!ReadChunkHeader(br, out id, out siz))
                {
                    return Type.Unknown;
                }
                var type = br.ReadUInt32();
                return HeaderTypeToType(type);
            }
            return Type.Unknown;
        }

        public class LRFHeader
        {
            public UInt32 type;
        }

        public class LRF3DMesh
        {
            public string name;
            public UInt64 faces;
            public string material;
            public float[] bb_min = new float[3];
            public float[] bb_max = new float[3];
            public UInt64 bones;
            public UInt64 anims;
        }

        public class LRFTexture
        {
            [Flags]
            public enum Flags
            {
                DXT1 = 1
            }
            public UInt32 width, height, components;
            public Flags flags;
            public UInt32 level;
            public UInt64 size;
            public byte[] data;
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
                    Console.WriteLine("Texture size is too large.");
                    return false;
                }

                texture.data = br.ReadBytes((int)texture.size);

                if (texture.data.Length != (int)texture.size)
                {
                    Console.WriteLine("Unexpected end of file while reading texture data.");
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading texture: {ex.Message}");
                return false;
            }
        }


        public static bool ReadHeader(BinaryReader br, out LRFHeader hdr)
        {
            hdr = new LRFHeader();
            hdr.type = br.ReadUInt32();

            return true;
        }

        public static bool IsLRFFile(BinaryReader br)
        {
            var magic = br.ReadUInt32(); // magic

            return magic == 0x4C726620;
        }

        public static Dictionary<string, string> ExtractDetails(string path)
        {
            var ret = new Dictionary<string, string>();

            FileStream fs = File.OpenRead(path);
            using (BinaryReader br = new BinaryReader(fs))
            {
                if (!IsLRFFile(br))
                {
                    ret["Warning"] = "Not a valid LRF file!";
                    return ret;
                }

                string id;
                UInt64 siz;

                int mesh_count = 0;

                while (true)
                {
                    if (!ReadChunkHeader(br, out id, out siz))
                    {
                        return ret;
                    }
                    switch (id)
                    {
                        case lrf_chunk_header:
                            LRFHeader hdr;
                            ReadHeader(br, out hdr);
                            ret["Archive type"] = HeaderTypeToType(hdr.type).ToString();
                            break;
                        case lrf_chunk_texture:
                            LRFTexture texture;
                            if (LoadTexture(br, out texture))
                            {
                                ret["Resolution"] = string.Format("{0}x{1}x{2}", texture.width, texture.height, texture.components);
                                var flags = texture.flags;
                                if (texture.flags.HasFlag(LRFTexture.Flags.DXT1))
                                {
                                    ret["Format"] = "DXT1";
                                }
                                else
                                {
                                    ret["Format"] = "RGB";
                                }
                                ret["Mipmap level"] = texture.level.ToString();
                                ret["Size"] = texture.size.ToString();
                            }
                            break;
                    }
                }
            }
        }

        public static LRFTexture LoadTexture(string path)
        {
            LRFTexture ret = null;
            FileStream fs = File.OpenRead(path);
            using (BinaryReader br = new BinaryReader(fs))
            {
                if (!IsLRFFile(br))
                {
                    return ret;
                }

                string id;
                UInt64 siz;

                while (true)
                {
                    if (!ReadChunkHeader(br, out id, out siz))
                    {
                        break;
                    }
                    switch (id)
                    {
                        case lrf_chunk_header:
                            LRFHeader hdr;
                            ReadHeader(br, out hdr);
                            if (HeaderTypeToType(hdr.type) != Type.Texture)
                            {
                                ret = null;
                                break;
                            }
                            break;
                        case lrf_chunk_texture:
                            LRFTexture texture;
                            if (LoadTexture(br, out texture))
                            {
                                ret = texture;
                                break;
                            }
                            break;
                    }
                }
            }
            return ret;
        }

    }
}