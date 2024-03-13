using System.IO;
using System.Text;

namespace B3dm.Tile
{
	public static class B3dmParser
	{
		public static B3dm ParseB3dm(Stream stream, bool AddGlbDetails = false)
		{
			using (BinaryReader binaryReader = new BinaryReader(stream))
			{
				string @string = Encoding.UTF8.GetString(binaryReader.ReadBytes(4));
				uint version = binaryReader.ReadUInt32();
				int num = 28;
				uint num2 = binaryReader.ReadUInt32();
				uint num3 = binaryReader.ReadUInt32();
				uint num4 = binaryReader.ReadUInt32();
				uint num5 = binaryReader.ReadUInt32();
				uint num6 = binaryReader.ReadUInt32();
				long num7 = num + num3 + num4 + num5;
				byte[] array = binaryReader.ReadBytes((int)num2 - num);
				MemoryStream stream2 = new MemoryStream(array);
				B3dm b3dm = new B3dm
				{
					Magic = @string,
					Version = (int)version,
					GlbData = array
				};
				if (AddGlbDetails)
				{
					Glb glb = GlbParser.ParseGlb(stream2);
					b3dm.Glb = glb;
				}
				return b3dm;
			}
		}
	}
}
