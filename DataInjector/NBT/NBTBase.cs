using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TehPers.Stardew.DataInjector.NBT {
    public abstract class NBTBase {

        public NBTBase() {

        }

        protected abstract void ReadData(BinaryReader stream);

        protected abstract void WriteData(BinaryWriter stream);

        public static NBTBase CreateTag(byte id) {
            if (id >= TAG_IDS.Length) return null;
            return Activator.CreateInstance(TAG_IDS[id]) as NBTBase;
        }

        public static NBTBase ReadStream(Stream stream) {
            using (BinaryReader reader = new BinaryReader(stream)) {
                return ReadStream(reader);
            }
        }

        public static NBTBase ReadStream(BinaryReader stream) {
            NBTBase tag = CreateTag(stream.ReadByte());
            tag.ReadData(stream);
            return tag;
        }

        public static void WriteStream(Stream stream, NBTBase tag) {
            using (BinaryWriter writer = new BinaryWriter(stream)) {
                WriteStream(writer, tag);
            }
        }

        public static void WriteStream(BinaryWriter stream, NBTBase tag) {
            int id = Array.IndexOf(TAG_IDS, tag.GetType());
            if (id == -1)
                throw new ArgumentException("Unknown type " + tag.GetType().FullName, "tag");
            stream.Write((byte) id);
            tag.WriteData(stream);
        }

        private static Type[] TAG_IDS = new Type[] {
            typeof(NBTEnd),
            typeof(NBTByte),
            typeof(NBTShort),
            typeof(NBTInt),
            typeof(NBTLong),
            typeof(NBTFloat),
            typeof(NBTDouble),
            typeof(NBTByteArray),
            typeof(NBTString),
            typeof(NBTTagList),
            typeof(NBTTagCompound),
            typeof(NBTIntArray),
        };
    }
}
