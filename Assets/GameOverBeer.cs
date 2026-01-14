using TMPro;
using UnityEngine;

public class GameOverBeer : MonoBehaviour
{
    public TMP_Text punteggioText;

    void Start()
    {
      

        float score = GameManager.instance.finalScore;
        punteggioText.text = "Ma hai bevuto " + Mathf.Round(score * 100f) / 100f + " pinte!";
    }
}

