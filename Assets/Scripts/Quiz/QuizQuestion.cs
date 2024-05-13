public enum QuizQuestionType
{
	Default,
	TrueOrFalse
}

public class QuizQuestion
{
	public QuizQuestionType Type { get; set; }
	public string Question { get; set; }
	public List<string> Answers { get; set; }
	public ByteImage Image { get; set; }
	public int Time { get; set; }

	public void Serialize(WizzStream stream)
	{
		stream.WriteVarInt(Type);
		stream.WriteString(Question);
		stream.WriteVarInt(Answers.Length);
		for (int i = 0; i < Answers.Length; i++)
			stream.WriteString(Answers[i]);
		stream.WriteImage(Image);
		stream.WriteVarInt(Time);
	}

	public static QuizQuestion Deserialize(WizzStream stream)
	{
		var question = new QuizQuestion();
		question.Type = (QuizQuestionType)stream.ReadVarInt();
		question.Question = stream.ReadString();

		int count = stream.ReadVarInt();
		question.Answers = new string[count];
		for (int i = 0; i < count; i++)
			question.Answers[i] = stream.ReadString();

		question.Image = stream.ReadImage();
		question.Time = stream.ReadVarInt();
		return question;
	}
}
