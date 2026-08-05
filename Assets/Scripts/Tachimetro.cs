using UnityEngine;

public class Tachimetro : MonoBehaviour
{
    [SerializeField] private float minAngle = 120f;
    [SerializeField] private float maxAngle = -120f;
    [SerializeField] private float smoothSpeed = 8f;
    private Transform _t;

    private float currentAngle;

    void Awake()
    {
        _t = transform;
    }

    void Update()
    {
        if (GameManager.instance == null) return;

        float alcool = GameManager.instance.GetAlcoolPower();
        float maxAlcool = GameManager.instance.GetMaxAlcoolPower();

        float t = Mathf.InverseLerp(0f, maxAlcool, alcool);
        float targetAngle = Mathf.Lerp(minAngle, maxAngle, t);

        currentAngle = Mathf.Lerp(currentAngle, targetAngle, Time.deltaTime * smoothSpeed);

        _t.localRotation = Quaternion.AngleAxis(currentAngle, Vector3.right);
    }
}
