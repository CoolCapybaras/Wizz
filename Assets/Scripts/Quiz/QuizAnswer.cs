public class QuizAnswer
{
	public QuizQuestionType Type { get; set; }
	public byte[] Ids { get; set; }
	public string Input { get; set; }

	public void Serialize(WizzStream stream)
	{
		stream.WriteVarInt(Type);
		if (Type == QuizQuestionType.Input)
			stream.WriteString(Input);
		else
			stream.WriteByteArray(Ids);
	}

	public static QuizAnswer Deserialize(WizzStream stream)
	{
		var answer = new QuizAnswer();
		answer.Type = (QuizQuestionType)stream.ReadVarInt();
		if (answer.Type == QuizQuestionType.Input)
			answer.Input = stream.ReadString();
		else
			answer.Ids = stream.ReadByteArray(4);
		return answer;
	}
}
