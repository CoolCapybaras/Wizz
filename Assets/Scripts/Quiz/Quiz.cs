using System.Collections.Generic;
using UnityEngine;

public class Quiz
{
	public string Id { get; set; }
	public string Name { get; set; }
	public ByteImage Image { get; set; }
	public string ImagePath { get; set; }
	public string Description { get; set; }
	public int QuestionsCount { get; set; }
	public int AuthorId { get; set; }
	public List<QuizQuestion> Questions { get; set; }
}
