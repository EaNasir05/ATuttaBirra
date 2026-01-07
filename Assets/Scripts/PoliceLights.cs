using UnityEngine;

public class PoliceSirenLerp : MonoBehaviour
{
    [Header("Luci")]
    [SerializeField] private Light leftLight;
    [SerializeField] private Light rightLight;

    [Header("Colori")]
    [SerializeField] private Color redColor = Color.red;
    [SerializeField] private Color blueColor = Color.blue;

    [Header("Velocità")]
    [Tooltip("Tempo (in secondi) per andare da rosso a blu")]
    [SerializeField] private float lerpDuration = 0.5f;

    private float t;
    private bool forward = true;

    private void Update()
    {
        if (lerpDuration <= 0f)
            return;

        float delta = Time.deltaTime / lerpDuration;

        if (forward)
            t += delta;
        else
            t -= delta;

        if (t >= 1f)
        {
            t = 1f;
            forward = false;
        }
        else if (t <= 0f)
        {
            t = 0f;
            forward = true;
        }

        Color currentColor = Color.Lerp(redColor, blueColor, t);

        leftLight.color = currentColor;
        rightLight.color = currentColor;
    }
}

