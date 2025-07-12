using System.Collections;
using UnityEngine;

public class MovementController : MonoBehaviour
{
    private int movimientoHorizontal = 0;
    private int movimientoVertical = 0;
    private Vector2 mov = new Vector2(0, 0);
    private float speed;
    public float baseSpeed;
    private Rigidbody2D rb;
    private Vector2 lastDirection = Vector2.right; // Default direction
    private float currentSpeedModifier = 1f;
    private Coroutine speedModifierCoroutine;
    private bool isConfused = false;
    private Coroutine confusionCoroutine;
    private bool invertMovement = false;
    private bool randomizeMovement = false;

    public float Speed
    {
        get { return speed; }
        set { speed = value; baseSpeed = value; }
    }

    public void Initialize(Rigidbody2D rigidbody)
    {
        rb = rigidbody;
    }

    public void UpdateMovement()
    {
        MovH();
        MovV();
        mov = new Vector2(movimientoHorizontal, movimientoVertical).normalized;
        if (mov != Vector2.zero)
        {
            lastDirection = mov;
        }

        transform.Translate(mov * speed * currentSpeedModifier * Time.deltaTime);
    }

    private void MovH()
    {
        int input = 0;
        KeyCode rightKey = GetKeyBinding("MoveRight", KeyCode.D);
        KeyCode leftKey = GetKeyBinding("MoveLeft", KeyCode.A);

        if (Input.GetKey(rightKey))
            input = 1;
        else if (Input.GetKey(leftKey))
            input = -1;

        movimientoHorizontal = GetConfusedMovement(input);
    }

    private void MovV()
    {
        int input = 0;
        KeyCode upKey = GetKeyBinding("MoveUp", KeyCode.W);
        KeyCode downKey = GetKeyBinding("MoveDown", KeyCode.S);

        if (Input.GetKey(upKey))
            input = 1;
        else if (Input.GetKey(downKey))
            input = -1;

        movimientoVertical = GetConfusedMovement(input);
    }

    private KeyCode GetKeyBinding(string prefKey, KeyCode defaultKey)
    {
        if (PlayerPrefs.HasKey(prefKey))
            return (KeyCode)PlayerPrefs.GetInt(prefKey);
        return defaultKey;
    }

    private int GetConfusedMovement(int input)
    {
        if (!isConfused) return input;

        if (randomizeMovement)
        {
            return Random.Range(-1, 2); // Returns -1, 0, or 1 randomly
        }
        else if (invertMovement)
        {
            return -input;
        }

        return input;
    }

    public void ApplySpeedModifier(float modifier, float duration)
    {
        if (speedModifierCoroutine != null)
        {
            StopCoroutine(speedModifierCoroutine);
        }
        speedModifierCoroutine = StartCoroutine(SpeedModifierCoroutine(modifier, duration));
    }

    public void ApplyPermanentSpeedModifier(float modifier)
    {
        currentSpeedModifier = modifier;
    }

    private IEnumerator SpeedModifierCoroutine(float modifier, float duration)
    {
        currentSpeedModifier = modifier;
        yield return new WaitForSeconds(duration);
        currentSpeedModifier = 1f;
    }

    public void ApplyConfusionEffect(float duration, bool invert, bool randomize)
    {
        if (confusionCoroutine != null)
        {
            StopCoroutine(confusionCoroutine);
        }

        invertMovement = invert;
        randomizeMovement = randomize;
        isConfused = true;
        confusionCoroutine = StartCoroutine(ConfusionEffectCoroutine(duration));
    }

    private IEnumerator ConfusionEffectCoroutine(float duration)
    {
        yield return new WaitForSeconds(duration);
        isConfused = false;
        invertMovement = false;
        randomizeMovement = false;
    }
}
