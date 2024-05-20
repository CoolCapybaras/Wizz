using System.Collections.Generic;
using WizzServer.Models;

public class Quiz
{
	public int Id { get; set; }
	public string Name { get; set; }
	public ByteImage Image { get; set; }
	public string Description { get; set; }
	public int QuestionCount { get; set; }
	public int AuthorId { get; set; }
	public List<QuizQuestion> Questions { get; set; }
	
	public List<string> Hashtags { get; set; }

	public void Serialize(WizzStream stream, bool ignoreQuestions = true)
	{
		if (Id == 0)
		{
			stream.WriteByte(0);
			return;
		}

		stream.WriteVarInt(Id);
		stream.WriteString(Name);
		stream.WriteImage(Image);
		stream.WriteString(Description);
		stream.WriteVarInt(QuestionCount);
		stream.WriteVarInt(AuthorId);

		if (ignoreQuestions)
		{
			stream.WriteByte(0);
		}
		else
		{
			stream.WriteVarInt(Questions.Count);
			for (int i = 0; i < Questions.Count; i++)
				Questions[i].Serialize(stream);
		}
	}

	public static Quiz Deserialize(WizzStream stream)
	{
		int quizId = stream.ReadVarInt();
		if (quizId == 0)
			return null!;

		var quiz = new Quiz();
		quiz.Id = quizId;
		quiz.Name = stream.ReadString();
		quiz.Image = stream.ReadImage();
		quiz.Description = stream.ReadString();
		quiz.QuestionCount = stream.ReadVarInt();
		quiz.AuthorId = stream.ReadVarInt();

		int count = stream.ReadVarInt();
		quiz.Questions = new List<QuizQuestion>();
		for (int i = 0; i < count; i++)
			quiz.Questions.Add(QuizQuestion.Deserialize(stream));

		return quiz;
	}
}
