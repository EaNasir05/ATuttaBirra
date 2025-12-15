using System.Collections;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.InputSystem;

public enum DrinkState { Idle, Moving, Drinking, Returning }

public class DrinkSystem : MonoBehaviour
{
    [Header ("Input system")]
    [SerializeField] private InputActionAsset inputActions;
    [SerializeField] private float inputDeadZone;
    private InputActionMap inputMap;
    private InputAction holdT, holdS, rightHand, actionTest;
    private Vector2 rightHandMovement;
    private Vector2 randomHandMovement;
    private bool readyToRandomlyMove;

    [Header ("Game objects")]
    [SerializeField] private GameObject handOnSteering;
    [SerializeField] private GameObject handOnGlass;
    [SerializeField] private Transform targetTransform;
    [SerializeField] private GameObject[] beerParticles;
    private Rigidbody rb;

    [Header("Birra")]
    [SerializeField] private Liquid beer;
    [SerializeField] private float shaderBugExtraFill;
    [SerializeField] private float minFill;
    [SerializeField] private float maxFill;
    [SerializeField] private float maxTilt;
    [SerializeField] private float maxHeight;
    [SerializeField] private float maxZ;
    private float totalBeerConsumed;
    private float extraFillWhileMoving;
    private float startingFill;
    private float beerConsumed;

    [Header ("Durate e velocità")]
    [SerializeField] private float rightHandSpeed;
    [SerializeField] private float randomMovementMultiplier;
    [SerializeField] private float randomMovementDuration;
    [SerializeField] private float maxRandomMovement;
    [SerializeField] private float glassTiltDuration;
    [SerializeField] private float returnDuration;
    [SerializeField] private float drinkDuration;
    [SerializeField] private float beerLossDuration;
    private float realDrinkDuration;

    [Header ("Stati")]
    private DrinkState state = DrinkState.Idle;
    private Coroutine routine;
    private Vector3 startPos;
    private Quaternion startRot;
    private bool iHateJews;
    private bool stunned;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        inputMap = inputActions.FindActionMap("Player");
        holdT = inputMap.FindAction("Hold T");
        holdS = inputMap.FindAction("Hold S");
        rightHand = inputMap.FindAction("Speed");
        actionTest = inputMap.FindAction("Test");
        totalBeerConsumed = 0;
        startPos = transform.position;
        startRot = transform.rotation;
        randomHandMovement = Vector2.zero;
        iHateJews = false;
        stunned = false;
        readyToRandomlyMove = true;
    }

    private void OnEnable() => inputMap.Enable();
    private void OnDisable() => inputMap.Disable();

    private void Update()
    {
        bool holdingGlass = holdT.IsPressed() && holdS.IsPressed();
        rightHandMovement = rightHand.ReadValue<Vector2>();
        switch (state)
        {
            case DrinkState.Idle:
                if (holdingGlass)
                    StartMoving();
                break;
            case DrinkState.Moving:
                if (!holdingGlass)
                    StartReturning();
                else
                    StartMoving();
                break;
            case DrinkState.Drinking:
                if (!holdingGlass)
                    StartReturning();
                break;
            case DrinkState.Returning:
                break;
        }
        UpdateHands(holdingGlass);
        UpdateWobble();
        UpdateRotationFreezing();
        if (actionTest.WasPressedThisFrame())
        {
            StartCoroutine(GainBeer(0.2f));
        }
    }

    private IEnumerator CreateRandomMovement()
    {
        if (readyToRandomlyMove)
        {
            readyToRandomlyMove = false;
            int index = Random.Range(1, 7);
            float extraMovement = Mathf.Round(totalBeerConsumed) * randomMovementMultiplier;
            if (extraMovement > maxRandomMovement)
                extraMovement = maxRandomMovement;
            switch (index)
            {
                case 1:
                    randomHandMovement = new Vector2(-extraMovement, 0);
                    break;
                case 2:
                    randomHandMovement = new Vector2(-extraMovement, extraMovement);
                    break;
                case 3:
                    randomHandMovement = new Vector2(0, extraMovement);
                    break;
                case 4:
                    randomHandMovement = new Vector2(extraMovement, extraMovement);
                    break;
                case 5:
                    randomHandMovement = new Vector2(extraMovement, 0);
                    break;
                default:
                    randomHandMovement = new Vector2(extraMovement, -extraMovement);
                    break;
            }
            yield return new WaitForSeconds(randomMovementDuration);
            readyToRandomlyMove = true;
        }
    }

    private void UpdateHands(bool holdingGlass)
    {
        bool handShouldHold = state == DrinkState.Drinking || state == DrinkState.Returning || state == DrinkState.Moving;
        handOnGlass.SetActive(handShouldHold);
        handOnSteering.SetActive(!handShouldHold);
    }

    private void UpdateRotationFreezing()
    {
        if (state == DrinkState.Moving)
            rb.constraints |= RigidbodyConstraints.FreezeRotationX;
        else
            rb.constraints &= ~RigidbodyConstraints.FreezeRotationX;
    }

    private void UpdateWobble()
    {
        if (beer.fillAmount >= maxFill - 0.01)
        {
            beer.MaxWobble = 0;
        }
        else
        {
            bool stable = state == DrinkState.Drinking;
            beer.MaxWobble = stable ? 0.001f : 0.03f;
        }
    }

    private void StartMoving()
    {
        StartCoroutine(CreateRandomMovement());
        state = DrinkState.Moving;
        RestartRoutine(MoveRoutine());
    }

    public void StartDrinking()
    {
        if (beer.fillAmount < maxFill)
        {
            state = DrinkState.Drinking;
            RestartRoutine(DrinkRoutine());
        }
    }

    private void StartReturning()
    {
        if (state == DrinkState.Drinking || state == DrinkState.Idle || state == DrinkState.Moving)
        {
            state = DrinkState.Returning;
            RestartRoutine(ReturnRoutine());
        }
    }

    private void RestartRoutine(IEnumerator newRoutine)
    {
        if (routine != null)
            StopCoroutine(routine);
        routine = StartCoroutine(newRoutine);
    }

    private IEnumerator LoseBeer(float fillGain)
    {
        float elapsed = 0f;
        float previousFill = 0f;
        while (elapsed < beerLossDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / beerLossDuration);
            float currentFill = fillGain * t;
            float increment = currentFill - previousFill;
            if (beer.fillAmount + increment > maxFill)
                increment = maxFill - beer.fillAmount;
            beer.fillAmount += increment;
            previousFill = currentFill;
            yield return null;
        }
    }

    private IEnumerator GainBeer(float fillLoss)
    {
        float tot = 0;
        if (iHateJews)
        {
            iHateJews = false;
            beer.fillAmount -= 2;
        }
        float elapsed = 0f;
        float previousFill = 0f;
        while (elapsed < beerLossDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / beerLossDuration);
            float currentFill = fillLoss * t;
            float decrement = currentFill - previousFill;
            if (beer.fillAmount - decrement < minFill)
                decrement = minFill - beer.fillAmount;
            beer.fillAmount -= decrement;
            tot += decrement;
            previousFill = currentFill;
            yield return null;
        };
    }

    private IEnumerator MoveRoutine()
    {
        Vector3 currentVelocity = rb.linearVelocity;
        float moveX = Mathf.Abs(rightHandMovement.x) > inputDeadZone ? rightHandMovement.x : 0f;
        float moveY = Mathf.Abs(rightHandMovement.y) > inputDeadZone ? rightHandMovement.y : 0f;
        rb.linearVelocity = new Vector3((moveX * rightHandSpeed) + randomHandMovement.x, currentVelocity.y, (moveY * rightHandSpeed) + randomHandMovement.y);
        yield return null;
    }

    private IEnumerator DrinkRoutine()
    {
        Vector3 currentPos = transform.position;
        float tRot = Mathf.InverseLerp(minFill, maxFill, beer.fillAmount);
        float xRot = Mathf.Lerp(targetTransform.rotation.eulerAngles.x, maxTilt, tRot);
        float yPos = Mathf.Lerp(targetTransform.position.y, maxHeight, tRot);
        float zPos = Mathf.Lerp(targetTransform.position.z, maxZ, tRot);
        Quaternion targetRotation = Quaternion.Euler(xRot, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);
        Vector3 targetPosition = new Vector3(targetTransform.position.x, yPos, zPos);
        Quaternion maxRotation = Quaternion.Euler(maxTilt, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);
        Vector3 maxPosition = new Vector3(targetTransform.position.x, maxHeight, maxZ);
        extraFillWhileMoving = 0f;
        startingFill = beer.fillAmount;
        float startXDrinking = targetRotation.eulerAngles.x;
        float endXDrinking = maxRotation.eulerAngles.x;
        float elapsedMovement = 0f;
        float elapsedDrinking = 0f;
        realDrinkDuration = (beer.fillAmount - maxFill) * -1 * drinkDuration;
        float previousFill = 0f;
        bool firstTime = true;

        while (state == DrinkState.Drinking)
        {
            if (elapsedMovement < glassTiltDuration)
            {
                elapsedMovement += Time.deltaTime;
                float t = Mathf.Clamp01(elapsedMovement / glassTiltDuration);
                Vector3 absolutePos = Vector3.Lerp(currentPos, targetPosition, t);
                Quaternion absoluteRot = Quaternion.Lerp(startRot, targetRotation, t);
                Vector3 deltaPos = absolutePos - transform.position;
                transform.position += deltaPos;
                Quaternion deltaRot = absoluteRot * Quaternion.Inverse(transform.rotation);
                transform.rotation = deltaRot * transform.rotation;
                float targetFill = Mathf.Lerp(startingFill, startingFill - shaderBugExtraFill, t);
                float deltaFill = beer.fillAmount - targetFill;
                beer.fillAmount -= deltaFill;
                extraFillWhileMoving = startingFill - beer.fillAmount;
            }
            else
            {
                if (firstTime)
                {
                    previousFill = beer.fillAmount;
                    firstTime = false;
                }
                elapsedDrinking += Time.deltaTime;
                float t = Mathf.Clamp01(elapsedDrinking / realDrinkDuration);

                float x = Mathf.LerpAngle(startXDrinking, endXDrinking, t);
                float y = transform.rotation.eulerAngles.y;
                float z = transform.rotation.eulerAngles.z;
                Quaternion absRot = Quaternion.Euler(x, y, z);
                transform.rotation = absRot;

                Vector3 absPos = Vector3.Lerp(targetPosition, maxPosition, t);
                Vector3 deltaPos = absPos - transform.position;
                transform.position += deltaPos;

                float currentFill = Mathf.Lerp(startingFill, maxFill - shaderBugExtraFill, t);
                float deltaFill = previousFill - currentFill;
                beer.fillAmount -= deltaFill;
                GameManager.instance.UpdateTotalBeerConsumed(-deltaFill);
                beerConsumed -= deltaFill;
                previousFill = currentFill;

                if (beer.fillAmount + 0.01 >= maxFill - shaderBugExtraFill)
                {
                    StartReturning();
                    yield break;
                }
            }
            yield return null;
        }
    }

    private IEnumerator ReturnRoutine()
    {
        bool needToGainExtra = true;
        float elapsed = 0f;
        if (beerConsumed * 100 >= 95)
        {
            GameManager.instance.UpdateTotalBeerConsumed(1 - beerConsumed);
            beerConsumed = 1;
        }
        if (beer.fillAmount + extraFillWhileMoving >= maxFill - 0.01)
        {
            beer.fillAmount = maxFill + 2 + extraFillWhileMoving;
            iHateJews = true;
            needToGainExtra = false;
        }
        float startFill = beer.fillAmount;
        float endFill = startFill + extraFillWhileMoving;
        float maxDistance = Vector3.Distance(new Vector3(targetTransform.position.x, maxHeight, targetTransform.position.z), startPos);
        float distance = Vector3.Distance(transform.position, startPos);
        float realReturnDuration = (distance / maxDistance) * returnDuration;
        float previousFillGain = startFill;

        Vector3 startPosAtReturn = transform.position;
        Quaternion startRotAtReturn = transform.rotation;

        while (elapsed < realReturnDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / returnDuration);
            Vector3 absPos = Vector3.Lerp(startPosAtReturn, startPos, t);
            Quaternion absRot = Quaternion.Lerp(startRotAtReturn, startRot, t);
            Vector3 deltaPos = absPos - transform.position;
            transform.position += deltaPos;
            Quaternion deltaRot = absRot * Quaternion.Inverse(transform.rotation);
            transform.rotation = deltaRot * transform.rotation;

            if (needToGainExtra || extraFillWhileMoving == 0)
            {
                float tFill = Mathf.Clamp01(elapsed / (returnDuration / 2));
                float currentFillGain = Mathf.Lerp(startFill, endFill, tFill);
                float deltaFill = currentFillGain - previousFillGain;
                beer.fillAmount += deltaFill;
                previousFillGain = currentFillGain;
            }

            yield return null;
        }
        if (iHateJews)
        {
            beer.fillAmount -= 2f;
            if (beer.fillAmount < maxFill && maxFill - beer.fillAmount <= 0.02)
            {
                beer.fillAmount += (maxFill - beer.fillAmount);
            }
            iHateJews = false;
        }
        transform.rotation = startRot;
        extraFillWhileMoving = 0f;
        totalBeerConsumed += beerConsumed;
        GameManager.instance.UpdateAlcoolPower(beerConsumed);
        beerConsumed = 0f;
        state = DrinkState.Idle;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        if (beer.fillAmount > maxFill)
            beer.fillAmount = maxFill;
    }

    public bool IsDrinking() => state == DrinkState.Drinking;
    public bool IsMoving() => state == DrinkState.Moving;
}
