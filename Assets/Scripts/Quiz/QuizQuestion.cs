using System.Collections.Generic;
using UnityEngine;

public class QuizQuestion
{
	public string Question { get; set; }
	public List<string> Answers { get; set; }
	public ByteImage Image { get; set; }
	public string ImagePath { get; set; }
	public int Time { get; set; }
	public int Countdown { get; set; }
	
	public int AnswerIndex { get; set; }
}
