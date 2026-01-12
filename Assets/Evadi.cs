using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class UIButtonTriggerUI : MonoBehaviour
{
    public string sceneToLoad = "Road"; 
    public InputActionAsset controlsAsset; 
    public string actionPath = "Player/Submit"; 
    public RectTransform cursorRect; 

    private InputAction submitAction;

    private void Awake()
    {
        submitAction = controlsAsset.FindAction(actionPath);
        if (submitAction == null)
            Debug.LogError($"Action '{actionPath}' non trovata nell'asset {controlsAsset.name}!");
    }

    private void OnEnable() => submitAction?.Enable();
    private void OnDisable() => submitAction?.Disable();

    private void Update()
    {
        if (submitAction != null && submitAction.triggered)
        {
            
            if (RectTransformUtility.RectangleContainsScreenPoint(
                    GetComponent<RectTransform>(), cursorRect.position))
            {
                SceneManager.LoadScene(sceneToLoad);
            }
        }
    }
}

