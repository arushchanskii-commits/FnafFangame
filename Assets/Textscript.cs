using UnityEngine;
using UnityEngine.UI;

public class Textscript : MonoBehaviour
{
    public Text scoreText;
    public circleclicking scoreSource;

    void Update()
    {
        if (scoreText == null)
        {
            return;
        }

        if (scoreSource == null)
        {
            scoreSource = FindObjectOfType<circleclicking>();
        }

        if (scoreSource == null)
        {
            scoreText.text = "0";
            return;
        }

        scoreText.text = scoreSource.score.ToString();
    }
}
