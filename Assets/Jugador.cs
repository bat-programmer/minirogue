using System.Collections;
using System.Diagnostics;
using UnityEngine;


public class Jugador : MonoBehaviour
{
    int movimientoHorizontal = 0;
    int movimientoVertical = 0;
    Vector2 mov = new Vector2(0, 0);

    [SerializeField] private float speed = 100;
    float speedNormal;
    float speedShift;
    private Rigidbody2D rb;
    private Animator anim;
    private SpriteRenderer sr;    
    public Transform firePoint;
    private float fireballSpeed = 10;
    public int vida = 0;
    private Vector2 lastDirection = Vector2.right; // Default direction
    private PoolBolaDeFuego poolBolaDeFuego;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
        poolBolaDeFuego = FindObjectOfType<PoolBolaDeFuego>();

        speedNormal = speed;
        speedShift = speed * 10;
        cambiarVida(200);
    }

    void Update()
    {
        MovH();
        MovV();
        Sprint();
        ExecuteAttack();
        mov = new Vector2(movimientoHorizontal, movimientoVertical).normalized;
        // Update last direction if moving
        if (mov != Vector2.zero)
        {
            lastDirection = mov;
        }

        // Flip sprite based on movement direction
        if (mov.x < 0)
            sr.flipX = true;
        else if (mov.x > 0)
            sr.flipX = false;

        if(Input.GetKeyDown(KeyCode.Q))
        {
            UnityEngine.Debug.Log(string.Format("speed:{0}, vida:{1}",speed,vida));
        }

       
        transform.Translate(mov * speed * Time.deltaTime);
    }

    void MovH()
    {
        if (Input.GetKey(KeyCode.D))
            movimientoHorizontal = 1;
        else if (Input.GetKey(KeyCode.A))
            movimientoHorizontal = -1;
        else
            movimientoHorizontal = 0;
    }

    void MovV()
    {
        if (Input.GetKey(KeyCode.W))
            movimientoVertical = 1;
        else if (Input.GetKey(KeyCode.S))
            movimientoVertical = -1;
        else
            movimientoVertical = 0;
    }

    private void ExecuteAttack()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            anim.SetTrigger("Attack");
            Fire();
        }
    }

    private void Fire()
    {
        //GameObject fireball = Instantiate(fireballPrefab, firePoint.position, Quaternion.identity);
        //BolaDeFuego fireballScript = fireball.GetComponent<BolaDeFuego>();
        //fireballScript.SetDirection(lastDirection.normalized); // Fire in the last movement direction

        if (poolBolaDeFuego != null)
        {
            poolBolaDeFuego.GetFireball(firePoint.position, lastDirection.normalized);
        }
        
    }

    private void Sprint()
    {
        speed = Input.GetKey(KeyCode.LeftShift) ? speedShift : speedNormal;
    }

    public void cambiarVida(int a)
    {
        vida += a;
        print("vida actual: " + vida);
    }

    //Why Not Use Invoke() Instead?
    //Unity has Invoke("MethodName", delay), but it only works for void methods.
    //Invoke() doesn’t allow intermediate steps(like modifying speed before resetting).
    //IEnumerator lets us control the entire process over time, making it more powerful.

    public IEnumerator ApplySpeedBoost(float multiplier, float time)
    {
        speed *= multiplier;  // Increase speed
        UnityEngine.Debug.Log("Speed increased to " + speed);

        yield return new WaitForSeconds(time); // Wait for effect duration

        speed /= multiplier;  // Reset speed to normal
        UnityEngine.Debug.Log("Speed back to normal: "+ speed);
    }

    private void FixedUpdate()
    {
        //mov = new Vector2(movimientoHorizontal, movimientoVertical).normalized;
        ////rb.velocity = mov * speed * Time.fixedDeltaTime;
        //transform.Translate(mov * speed * Time.deltaTime);
    }
}
