using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage.Streams;

namespace KD.Navien.WaterBoilerMat.Universal.Extensions
{
	public static class DataExtensions
	{
		public static byte[] ToBytes(this IBuffer buffer)
		{
			var dataLength = buffer.Length;
			var data = new byte[dataLength];
			using (var reader = DataReader.FromBuffer(buffer))
			{
				reader.ReadBytes(data);
			}
			return data;
		}
		
		public static async Task<byte[]> ToBytesAsync(this IRandomAccessStream stream)
		{
			var data = new byte[stream.Size];
			using (var reader = new DataReader(stream.GetInputStreamAt(0)))
			{
				await reader.LoadAsync((uint)stream.Size);
				reader.ReadBytes(data);
			}

			return data;
		}

		public static IBuffer ToBuffer(this byte[] data)
		{
			using (var writer = new DataWriter())
			{
				writer.WriteBytes(data);
				return writer.DetachBuffer();
			}
		}
	}
}
