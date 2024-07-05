﻿public class QuizAnswer
{
	public QuizQuestionType Type { get; set; }
	public int Id { get; set; }
	public byte[] Ids { get; set; }
	public string Input { get; set; }

	public void Serialize(WizzStream stream)
	{
		stream.WriteVarInt(Type);
		if (Type == QuizQuestionType.Default
			|| Type == QuizQuestionType.TrueOrFalse)
			stream.WriteVarInt(Id);
		else if (Type == QuizQuestionType.Multiple
			|| Type == QuizQuestionType.Match)
			stream.WriteByteArray(Ids);
		else
			stream.WriteString(Input);
	}

	public static QuizAnswer Deserialize(WizzStream stream)
	{
		var answer = new QuizAnswer();
		answer.Type = (QuizQuestionType)stream.ReadVarInt();
		if (answer.Type == QuizQuestionType.Default
			|| answer.Type == QuizQuestionType.TrueOrFalse)
			answer.Id = stream.ReadVarInt();
		else if (answer.Type == QuizQuestionType.Multiple
			|| answer.Type == QuizQuestionType.Match)
			answer.Ids = stream.ReadByteArray(4);
		else
			answer.Input = stream.ReadString();
		return answer;
	}
}