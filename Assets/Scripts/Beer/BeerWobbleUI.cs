using UnityEngine;

public class BeerDirectionalTiltUI : MonoBehaviour
{
    public RectTransform cursorRect;

    [Header("Pivots (0-1 UI space)")]
    public Vector2 rightPivot = new Vector2(1f, 0f);
    public Vector2 leftPivot = new Vector2(0f, 0f);

    [Header("Tilt")]
    public float tiltAngle = 12f;
    public float returnSpeed = 10f;

    private RectTransform rect;
    private Vector2 startPos;
    private Vector2 startPivot;

    private bool wasHovering;
    private float currentTilt;
    private float targetTilt;

    private Vector2 lastCursorPos;

    void Awake()
    {
        rect = GetComponent<RectTransform>();
        startPos = rect.anchoredPosition;
        startPivot = rect.pivot;
        lastCursorPos = cursorRect.position;
    }

    void Update()
    {
        Vector2 currentCursorPos = cursorRect.position;
        Vector2 cursorDelta = currentCursorPos - lastCursorPos;

        bool hovering = RectTransformUtility.RectangleContainsScreenPoint(
            rect, currentCursorPos);

        
        if (hovering && !wasHovering)
        {
            
            if (cursorDelta.x > 0)
            {
                SetPivot(leftPivot);
                targetTilt = -tiltAngle;
            }
            else
            {
                SetPivot(rightPivot);
                targetTilt = tiltAngle;
            }
        }

        
        if (!hovering && wasHovering)
        {
            targetTilt = 0f;
        }

        currentTilt = Mathf.Lerp(currentTilt, targetTilt, Time.deltaTime * returnSpeed);
        rect.localRotation = Quaternion.Euler(0, 0, currentTilt);

        wasHovering = hovering;
        lastCursorPos = currentCursorPos;
    }

    void SetPivot(Vector2 newPivot)
    {
        Vector2 size = rect.rect.size;
        Vector2 deltaPivot = rect.pivot - newPivot;

        Vector3 deltaPos = new Vector3(
            deltaPivot.x * size.x * rect.localScale.x,
            deltaPivot.y * size.y * rect.localScale.y
        );

        rect.pivot = newPivot;
        rect.anchoredPosition -= new Vector2(deltaPos.x, deltaPos.y);
    }
}
