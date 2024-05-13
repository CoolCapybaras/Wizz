using UnityEngine;

public class ByteImage
{
	public byte[] data;
	private Texture2D texture;
	
	public ByteImage(byte[] data)
	{
		this.data = data;
	}

	public Texture2D GetTexture()
	{
		if (texture != null)
			return texture;

		texture = new Texture2D(2, 2);
		texture.LoadImage(data);
		return texture;
	}
}
