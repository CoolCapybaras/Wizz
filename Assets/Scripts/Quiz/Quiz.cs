using System;
using System.Collections.Generic;
using System.Linq;


public enum ModerationStatus
{
	NotModerated,
	InModeration,
	ModerationAccepted,
	ModerationRejected
}

[Serializable]
public enum HexColor
{
	Default = 2106434,
	Purple = 1972022,
	Green = 1387568,
	Red = 2823472,
}

public class Quiz
{
	public int Id { get; set; }
	public string Name { get; set; }
	public ByteImage Image { get; set; }
	public string Description { get; set; }
	public int QuestionCount { get; set; }
	public int AuthorId { get; set; }
	public ModerationStatus ModerationStatus { get; set; }
	public HexColor Color { get; set; }
	public float Score { get; set; }
	public List<QuizQuestion> Questions { get; set; }

	public List<string> Hashtags { get; set; }

	public void Serialize(WizzStream stream, bool includeQuestions = false)
	{
		stream.WriteVarInt(Id);
		stream.WriteString(Name);
		stream.WriteImage(Image);
		stream.WriteString(Description);
		stream.WriteVarInt(QuestionCount);
		stream.WriteVarInt(AuthorId);
		stream.WriteVarInt(ModerationStatus);
		stream.WriteVarInt(Color);
		stream.WriteVarInt((int)(Score * 10));

		if (includeQuestions)
		{
			stream.WriteVarInt(Questions.Count);
			for (int i = 0; i < Questions.Count; i++)
				Questions[i].Serialize(stream, true);
		}
		else
		{
			stream.WriteByte(0);
		}
	}



	public static Quiz Deserialize(WizzStream stream)
	{
		var quiz = new Quiz();
		quiz.Id = stream.ReadVarInt();
		quiz.Name = stream.ReadString();
		quiz.Image = stream.ReadImage();
		quiz.Description = stream.ReadString();
		quiz.QuestionCount = stream.ReadVarInt();
		quiz.AuthorId = stream.ReadVarInt();
		quiz.ModerationStatus = (ModerationStatus)stream.ReadVarInt();
		quiz.Color = (HexColor)stream.ReadVarInt();
		quiz.Score = stream.ReadVarInt() / 10f;

		int count = stream.ReadVarInt();
		quiz.Questions = new List<QuizQuestion>();
		for (int i = 0; i < count; i++)
			quiz.Questions.Add(QuizQuestion.Deserialize(stream));

		return quiz;
	}

	public Quiz Clone()
	{
		Quiz quiz = new();
		quiz.Id = Id;
		quiz.Name = Name;
		quiz.Description = Description;
		quiz.Image = new ByteImage(Image.data);
		quiz.Hashtags = Hashtags.ToList();
		quiz.Questions = new();
		foreach (var question in Questions)
			quiz.Questions.Add(question.Clone());
		return quiz;
	}
}
