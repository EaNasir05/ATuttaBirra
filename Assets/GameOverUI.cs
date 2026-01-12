using TMPro;
using UnityEngine;

public class GameOverUI : MonoBehaviour
{
    public TMP_Text punteggioText; // renderlo pubblico per essere sicuro che compaia

    void Start()
    {
        if (punteggioText != null)
        {
            float score = GameManager.instance.finalScore;
            punteggioText.text = "Punteggio: " + Mathf.Round(score * 100) / 100 + " L";
        }
        else
        {
            Debug.LogWarning("PunteggioText non assegnato nell'Inspector!");
        }
    }
}
