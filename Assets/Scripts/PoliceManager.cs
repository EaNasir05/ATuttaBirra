
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement; 

public class PoliceChaseSystem : MonoBehaviour
{
    public enum PoliceState
    {
        None,
        Far,
        Near,
        VeryNear
    }

    [Header("Tempi")]
    [SerializeField] private float timeToFar = 5f;
    [SerializeField] private float timeToNear = 5f;
    [SerializeField] private float timeToVeryNear = 5f;
    [SerializeField] private float timeToGameOver = 5f;

    [Header("Specchietto (URP)")]
    [SerializeField] private MeshRenderer mirrorRenderer;
    [SerializeField] private Texture noPoliceTexture;
    [SerializeField] private Texture policeFarTexture;
    [SerializeField] private Texture policeNearTexture;
    [SerializeField] private Texture policeVeryNearTexture;

    private Coroutine chaseRoutine;
    private PoliceState currentState = PoliceState.None;
    private Material mirrorMaterial;

    private void Awake()
    {
       
        mirrorMaterial = mirrorRenderer.material;
        ResetPolice();
    }

   
    public void OnPlayerDrink()
    {
        if (chaseRoutine != null)
            StopCoroutine(chaseRoutine);

        chaseRoutine = StartCoroutine(PoliceChaseRoutine());
    }

    private IEnumerator PoliceChaseRoutine()
    {
        ResetPolice();

        yield return new WaitForSeconds(timeToFar);
        SetState(PoliceState.Far);

        yield return new WaitForSeconds(timeToNear);
        SetState(PoliceState.Near);

        yield return new WaitForSeconds(timeToVeryNear);
        SetState(PoliceState.VeryNear);

        yield return new WaitForSeconds(timeToGameOver);
        TriggerGameOver();
    }

    private void SetState(PoliceState newState)
    {
        currentState = newState;

        switch (newState)
        {
            case PoliceState.None:
                mirrorMaterial.SetTexture("_BaseMap", noPoliceTexture);
                break;

            case PoliceState.Far:
                mirrorMaterial.SetTexture("_BaseMap", policeFarTexture);
                break;

            case PoliceState.Near:
                mirrorMaterial.SetTexture("_BaseMap", policeNearTexture);
                break;

            case PoliceState.VeryNear:
                mirrorMaterial.SetTexture("_BaseMap", policeVeryNearTexture);
                break;
        }
    }

    private void ResetPolice()
    {
        SetState(PoliceState.None);
    }

    private void TriggerGameOver()
    {
        
        SceneManager.LoadScene("gameover");
    }
}
