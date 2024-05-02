using UnityEngine;

public class Image
{
	private byte[] data;
	private Texture2D texture;
	
	public Image(byte[] data)
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
