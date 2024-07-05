﻿using System;
using System.Buffers.Binary;
using System.Text;
using System.Threading.Tasks;

public partial class WizzStream
{
	public byte ReadUByte()
	{
		Span<byte> buffer = stackalloc byte[1];
		BaseStream.Read(buffer);
		return buffer[0];
	}

	public async Task<byte> ReadUByteAsync()
	{
		var buffer = new byte[1];
		await this.ReadAsync(buffer);
		return buffer[0];
	}

	public bool ReadBoolean()
	{
		return ReadUByte() == 0x01;
	}

	public async Task<bool> ReadBooleanAsync()
	{
		var value = (int)await this.ReadUByteAsync();
		if (value == 0x00)
			return false;
		else if (value == 0x01)
			return true;
		else
			throw new ArgumentOutOfRangeException("Byte returned by stream is out of range (0x00 or 0x01)", nameof(BaseStream));
	}

	public int ReadInt()
	{
		Span<byte> buffer = stackalloc byte[4];
		this.Read(buffer);
		return BinaryPrimitives.ReadInt32LittleEndian(buffer);
	}

	public async Task<int> ReadIntAsync()
	{
		using var buffer = new RentedArray<byte>(sizeof(int));
		await this.ReadAsync(buffer);
		return BinaryPrimitives.ReadInt32LittleEndian(buffer);
	}

	public long ReadLong()
	{
		Span<byte> buffer = stackalloc byte[8];
		this.Read(buffer);
		return BinaryPrimitives.ReadInt64LittleEndian(buffer);
	}

	public async Task<long> ReadLongAsync()
	{
		using var buffer = new RentedArray<byte>(sizeof(long));
		await this.ReadAsync(buffer);
		return BinaryPrimitives.ReadInt64LittleEndian(buffer);
	}

	public string ReadString(int maxLength = 32767)
	{
		var length = ReadVarInt();
		if (length == 0)
			return string.Empty;

		var buffer = new byte[length];
		this.Read(buffer);

		var value = Encoding.UTF8.GetString(buffer);
		if (maxLength > 0 && value.Length > maxLength)
		{
			throw new ArgumentException($"string ({value.Length}) exceeded maximum length ({maxLength})", nameof(value));
		}
		return value;
	}

	public async Task<string> ReadStringAsync(int maxLength = 32767)
	{
		var length = await this.ReadVarIntAsync();
		using var buffer = new RentedArray<byte>(length);
		if (BitConverter.IsLittleEndian)
		{
			buffer.Span.Reverse();
		}
		await this.ReadAsync(buffer);

		var value = Encoding.UTF8.GetString(buffer);
		if (maxLength > 0 && value.Length > maxLength)
		{
			throw new ArgumentException($"string ({value.Length}) exceeded maximum length ({maxLength})", nameof(maxLength));
		}
		return value;
	}

	public int ReadVarInt()
	{
		int numRead = 0;
		int result = 0;
		byte read;
		do
		{
			read = this.ReadUByte();
			int value = read & 0b01111111;
			result |= value << (7 * numRead);

			numRead++;
			if (numRead > 5)
			{
				throw new InvalidOperationException("VarInt is too big");
			}
		} while ((read & 0b10000000) != 0);

		return result;
	}

	public virtual async Task<int> ReadVarIntAsync()
	{
		int numRead = 0;
		int result = 0;
		byte read;
		do
		{
			read = await this.ReadUByteAsync();
			int value = read & 0b01111111;
			result |= value << (7 * numRead);

			numRead++;
			if (numRead > 5)
			{
				throw new InvalidOperationException("VarInt is too big");
			}
		} while ((read & 0b10000000) != 0);

		return result;
	}

	public byte[] ReadByteArray(int length = 0)
	{
		if (length == 0)
			length = ReadVarInt();

		var buffer = new byte[length];
		Read(buffer);
		return buffer;
	}

	public async Task<byte[]> ReadByteArrayAsync(int length = 0)
	{
		if (length == 0)
			length = await this.ReadVarIntAsync();

		var result = new byte[length];
		if (length == 0)
			return result;

		int n = length;
		while (true)
		{
			n -= await this.ReadAsync(result, length - n, n);
			if (n == 0)
				break;
		}
		return result;
	}

	public long ReadVarLong()
	{
		int numRead = 0;
		long result = 0;
		byte read;
		do
		{
			read = this.ReadUByte();
			int value = (read & 0b01111111);
			result |= (long)value << (7 * numRead);

			numRead++;
			if (numRead > 10)
			{
				throw new InvalidOperationException("VarLong is too big");
			}
		} while ((read & 0b10000000) != 0);

		return result;
	}

	public async Task<long> ReadVarLongAsync()
	{
		int numRead = 0;
		long result = 0;
		byte read;
		do
		{
			read = await this.ReadUByteAsync();
			int value = (read & 0b01111111);
			result |= (long)value << (7 * numRead);

			numRead++;
			if (numRead > 10)
			{
				throw new InvalidOperationException("VarLong is too big");
			}
		} while ((read & 0b10000000) != 0);

		return result;
	}

	public ByteImage ReadImage()
	{
		var length = ReadVarInt();
		if (length == 0)
			return new ByteImage(null);

		var buffer = new byte[length];
		this.Read(buffer);
		return new ByteImage(buffer);
	}
}
