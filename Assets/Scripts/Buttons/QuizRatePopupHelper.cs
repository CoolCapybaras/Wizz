using System.Collections.Generic;
using Net.Packets.Serverbound;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;

namespace Buttons
{
    
    public class QuizRatePopupHelper: MonoBehaviour
    {
        public int score;
        public TextMeshProUGUI scoreText;
        public GameObject stars;
        
        public void OnReadyPressed()
        {
            score = int.Parse(scoreText.text);
            LocalClient.instance.SendPacket(new UpdateQuizRating() {Score = score});
            gameObject.SetActive(false);
        }

        public void OnCancelPressed()
        {
            gameObject.SetActive(false);
        }

        public void ChoseRating1()
        {
            stars.transform.GetChild(0).GetChild(4).GetChild(0).GetComponent<Image>().fillAmount = 0.2f;
            scoreText.text = "1";
        }
        
        public void ChoseRating2()
        {
            stars.transform.GetChild(0).GetChild(4).GetChild(0).GetComponent<Image>().fillAmount = 0.4f;
            scoreText.text = "2";
        }
        
        public void ChoseRating3()
        {
            stars.transform.GetChild(0).GetChild(4).GetChild(0).GetComponent<Image>().fillAmount = 0.6f;
            scoreText.text = "3";
        }
        
        public void ChoseRating4()
        {
            stars.transform.GetChild(0).GetChild(4).GetChild(0).GetComponent<Image>().fillAmount = 0.8f;
            scoreText.text = "4";
        }
        
        public void ChoseRating5()
        {
            stars.transform.GetChild(0).GetChild(4).GetChild(0).GetComponent<Image>().fillAmount = 1f;
            scoreText.text = "5";
        }
    }
}