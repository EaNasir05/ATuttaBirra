using System.Collections;
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
    private float originalMaxWobble;

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
    private bool readyToGainBeer = true;
    private bool readyToLoseBeer = true;
    public bool shakingBeer = false;

    private void Awake()
    {
        inputMap = inputActions.FindActionMap("Player");
        holdT = inputMap.FindAction("Hold T");
        holdS = inputMap.FindAction("Hold S");
        rightHand = inputMap.FindAction("Speed");
        actionTest = inputMap.FindAction("Test");
        totalBeerConsumed = 0;
        startPos = transform.localPosition;
        startRot = transform.localRotation;
        randomHandMovement = Vector2.zero;
        iHateJews = false;
        readyToRandomlyMove = true;
        originalMaxWobble = beer.MaxWobble;
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
                else
                    UpdateLocalPosition();
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
        if (actionTest.WasPressedThisFrame())
        {
            StartCoroutine(GainBeer(0.05f));
        }
    }

    private void UpdateLocalPosition()
    {
        transform.localPosition = startPos;
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
            Debug.Log(extraMovement);
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

    private void UpdateWobble()
    {
        if (beer.fillAmount >= maxFill - 0.01)
        {
            beer.MaxWobble = 0;
        }
        else if (!shakingBeer)
        {
            beer.MaxWobble = state == DrinkState.Drinking ? 0.001f : originalMaxWobble;
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

    public IEnumerator LoseBeer(float fillGain)
    {
        if (!readyToLoseBeer)
            yield break;
        readyToLoseBeer = false;
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
        readyToLoseBeer = true;
    }

    public IEnumerator GainBeer(float fillLoss)
    {
        if (!readyToGainBeer)
            yield break;
        readyToGainBeer = false;
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
                decrement = beer.fillAmount - minFill;
            beer.fillAmount -= decrement;
            tot += decrement;
            previousFill = currentFill;
            yield return null;
        };
        readyToGainBeer = true;
    }

    private IEnumerator MoveRoutine()
    {
        int alcoolLevel = (int) totalBeerConsumed > 5 ? 5 : (int) totalBeerConsumed;
        float speed = rightHandSpeed * (1 - (alcoolLevel * 0.05f));
        float moveX = Mathf.Abs(rightHandMovement.x) > inputDeadZone ? rightHandMovement.x : 0f;
        float moveY = Mathf.Abs(rightHandMovement.y) > inputDeadZone ? rightHandMovement.y : 0f;
        transform.position += new Vector3(((moveX * speed) + randomHandMovement.x) * Time.deltaTime, 0, ((moveY * speed) + randomHandMovement.y) * Time.deltaTime);
        if (transform.localPosition.y >= 0.318)
            transform.localPosition = new Vector3(transform.localPosition.x, 0.317f, transform.localPosition.z);
        else if (transform.localPosition.y <= 0.14)
            transform.localPosition = new Vector3(transform.localPosition.x, 0.141f, transform.localPosition.z);
        if (transform.localPosition.x >= 0.106)
            transform.localPosition = new Vector3(0.105f, transform.localPosition.y, transform.localPosition.z);
        else if (transform.localPosition.x <= -0.1)
            transform.localPosition = new Vector3(-0.101f, transform.localPosition.y, transform.localPosition.z);
        yield return null;
    }

    private IEnumerator DrinkRoutine()
    {
        Vector3 currentPos = transform.localPosition;
        float tRot = Mathf.InverseLerp(minFill, maxFill, beer.fillAmount);
        float xRot = Mathf.Lerp(targetTransform.localRotation.eulerAngles.x, maxTilt, tRot);
        float yPos = Mathf.Lerp(targetTransform.localPosition.y, maxHeight, tRot);
        float zPos = Mathf.Lerp(targetTransform.localPosition.z, maxZ, tRot);
        Quaternion targetRotation = Quaternion.Euler(xRot, transform.localRotation.eulerAngles.y, transform.localRotation.eulerAngles.z);
        Vector3 targetPosition = new Vector3(targetTransform.localPosition.x, yPos, zPos);
        Quaternion maxRotation = Quaternion.Euler(maxTilt, transform.localRotation.eulerAngles.y, transform.localRotation.eulerAngles.z);
        Vector3 maxPosition = new Vector3(targetTransform.localPosition.x, maxHeight, maxZ);
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
                Vector3 deltaPos = absolutePos - transform.localPosition;
                transform.localPosition += deltaPos;
                Quaternion deltaRot = absoluteRot * Quaternion.Inverse(transform.localRotation);
                transform.localRotation = deltaRot * transform.localRotation;
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
                float y = transform.localRotation.eulerAngles.y;
                float z = transform.localRotation.eulerAngles.z;
                Quaternion absRot = Quaternion.Euler(x, y, z);
                transform.localRotation = absRot;

                Vector3 absPos = Vector3.Lerp(targetPosition, maxPosition, t);
                Vector3 deltaPos = absPos - transform.localPosition;
                transform.localPosition += deltaPos;

                float currentFill = Mathf.Lerp(startingFill, maxFill - shaderBugExtraFill, t);
                float deltaFill = previousFill - currentFill;
                beer.fillAmount -= deltaFill;
                GameManager.instance.UpdateTotalBeerConsumed(-(deltaFill * 2));
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
        if (beerConsumed * 100 >= 23.5)
        {
            GameManager.instance.UpdateTotalBeerConsumed(0.5f - (beerConsumed * 2));
            beerConsumed = 0.25f;
        }
        if (beer.fillAmount + extraFillWhileMoving >= maxFill - 0.01)
        {
            beer.fillAmount = maxFill + 2 + extraFillWhileMoving;
            iHateJews = true;
            needToGainExtra = false;
        }
        float startFill = beer.fillAmount;
        float endFill = startFill + extraFillWhileMoving;
        float maxDistance = Vector3.Distance(new Vector3(targetTransform.localPosition.x, maxHeight, targetTransform.localPosition.z), startPos);
        float distance = Vector3.Distance(transform.localPosition, startPos);
        float realReturnDuration = (distance / maxDistance) * returnDuration;
        float previousFillGain = startFill;

        Vector3 startPosAtReturn = transform.localPosition;
        Quaternion startRotAtReturn = transform.localRotation;

        while (elapsed < realReturnDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / returnDuration);
            Vector3 absPos = Vector3.Lerp(startPosAtReturn, startPos, t);
            Quaternion absRot = Quaternion.Lerp(startRotAtReturn, startRot, t);
            Vector3 deltaPos = absPos - transform.localPosition;
            transform.localPosition += deltaPos;
            Quaternion deltaRot = absRot * Quaternion.Inverse(transform.localRotation);
            transform.localRotation = deltaRot * transform.localRotation;

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
            if (beer.fillAmount < maxFill && maxFill - beer.fillAmount <= 0.01)
            {
                beer.fillAmount += (maxFill - beer.fillAmount);
            }
            iHateJews = false;
        }
        transform.localRotation = startRot;
        extraFillWhileMoving = 0f;
        totalBeerConsumed += beerConsumed * 2;
        GameManager.instance.UpdateAlcoolPower(beerConsumed * 2);
        beerConsumed = 0f;
        state = DrinkState.Idle;
        if (beer.fillAmount > maxFill)
            beer.fillAmount = maxFill;
    }

    public void ShakeBeer()
    {
        if (state == DrinkState.Drinking)
        {
            StartReturning();
            //StartCoroutine(ShakeJar());
        }
        else
        {
            if (beer.fillAmount < maxFill)
            {
                shakingBeer = true;
                beer.AddImpulse(new Vector2(1, 0), 5);
            }
        }
    }

    private IEnumerator ShakeJar()
    {
        float movementLength = 0;
        while (movementLength < 0.75f)
        {
            float y = 2 * Time.deltaTime;
            movementLength += y;
            transform.position += new Vector3(0, y, 0);
            yield return null;
        }
        while (movementLength > 0)
        {
            float y = -2 * Time.deltaTime;
            movementLength += y;
            transform.position += new Vector3(0, y, 0);
            yield return null;
        }
    }

    public bool IsIdling() => state == DrinkState.Idle;

    public bool IsDrinking() => state == DrinkState.Drinking;
    public bool IsMoving() => state == DrinkState.Moving;
}
