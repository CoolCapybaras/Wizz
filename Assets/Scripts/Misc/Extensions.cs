public static class Extensions
{
	public static int GetVarIntLength(this int value)
	{
		int amount = 0;
		do
		{
			value >>= 7;
			amount++;
		}
		while (value != 0);

		return amount;
	}
}
