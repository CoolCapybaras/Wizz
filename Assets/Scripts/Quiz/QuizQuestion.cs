using System.Collections.Generic;
using System.Linq;

public enum QuizQuestionType
{
	Default,
	TrueOrFalse,
	Multiple,
	Input,
	Match
}

public class QuizQuestion
{
	public QuizQuestionType Type { get; set; }
	public string Question { get; set; }
	public List<string> Answers { get; set; }
	public ByteImage Image { get; set; }
	public int Time { get; set; }
	public int AnswerIndex { get; set; }

	public void Serialize(WizzStream stream)
	{
		stream.WriteVarInt(Type);
		stream.WriteString(Question);
		stream.WriteVarInt(Answers.Count);
		for (int i = 0; i < Answers.Count; i++)
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
		question.Answers = new List<string>();
		for (int i = 0; i < count; i++)
			question.Answers.Add(stream.ReadString());

		question.Image = stream.ReadImage();
		question.Time = stream.ReadVarInt();
		return question;
	}

	public QuizQuestion Clone()
	{
		var question = new QuizQuestion();
		question.Type = Type;
		question.Question = Question;
		question.Answers = Answers.ToList();
		question.Image = new ByteImage(Image.data);
		question.Time = Time;
		question.AnswerIndex = AnswerIndex;
		return question;
	}
}
