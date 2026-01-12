using UnityEngine;
using UnityEngine.InputSystem;

public class CursorController : MonoBehaviour
{
    public float speed = 1000f;
    public InputActionAsset controlsAsset;
    public string actionPath = "Player/MoveCursor";

    private InputAction moveCursorAction;
    private RectTransform rectTransform;
    private RectTransform canvasRect;
    private Vector2 localPosition;

    private float minX, maxX, minY, maxY;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasRect = rectTransform.GetComponentInParent<Canvas>().GetComponent<RectTransform>();

        moveCursorAction = controlsAsset.FindAction(actionPath);
        if (moveCursorAction == null)
            Debug.LogError($"Action '{actionPath}' non trovata!");

        
        localPosition = Vector2.zero;
        rectTransform.anchoredPosition = localPosition;

        
        minX = -canvasRect.rect.width * canvasRect.pivot.x;
        maxX = canvasRect.rect.width * (1 - canvasRect.pivot.x);
        minY = -canvasRect.rect.height * canvasRect.pivot.y;
        maxY = canvasRect.rect.height * (1 - canvasRect.pivot.y);
    }

    private void OnEnable() => moveCursorAction?.Enable();
    private void OnDisable() => moveCursorAction?.Disable();

    private void Update()
    {
        if (moveCursorAction == null) return;

        Vector2 input = moveCursorAction.ReadValue<Vector2>();
        Vector2 delta = input * speed * Time.deltaTime;
        localPosition += delta;

        
        localPosition.x = Mathf.Clamp(localPosition.x, minX, maxX);
        localPosition.y = Mathf.Clamp(localPosition.y, minY, maxY);

        rectTransform.anchoredPosition = localPosition;
    }
}
