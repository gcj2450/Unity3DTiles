using System.IO;
using System.Text;

namespace B3dm.Tile
{
	public class GlbParser
	{
		public static Glb ParseGlb(Stream stream)
		{
			BinaryReader binaryReader = new BinaryReader(stream);
			uint magic = binaryReader.ReadUInt32();
			uint version = binaryReader.ReadUInt32();
			uint length = binaryReader.ReadUInt32();
			uint count = binaryReader.ReadUInt32();
			binaryReader.ReadUInt32();
			byte[] bytes = binaryReader.ReadBytes((int)count);
			string @string = Encoding.UTF8.GetString(bytes);
			uint count2 = binaryReader.ReadUInt32();
			binaryReader.ReadUInt32();
			byte[] gltfModelBin = binaryReader.ReadBytes((int)count2);
			return new Glb
			{
				Magic = magic,
				Version = version,
				GltfModelJson = @string,
				GltfModelBin = gltfModelBin,
				Length = length
			};
		}
	}
}
