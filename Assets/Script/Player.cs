using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/* 

- Courir done
- S'accroupir done
- Climb done
- une barre de vie constituer de 9 coeurs
- Une zone ou tu es ralenti 
- Respawn & Checkpoint
- Un objet immobile qui te ferait -1 de dégât si tu le touche 
- Collectibles
*/


public class Player : MonoBehaviour
{
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private Animator animController;
    private Vector2 ref_velocity = Vector2.zero;
    private float walkingSpeed = 400f;
    private float runningSpeed = 800f;
    //Lui mettre son animation, idem pour la course, le dash & le climb
    private float crouchingSpeed = 200f;
    private float horizontalValue;
    private float verticalValue;

    //SerializeField sert à afficher les paramètres dans le player
    //Le dash limit sert à mettre une Limite de Dash, je peux le changer directement dans le "Inspector" du player.
    [SerializeField] private int dashLimit = 1;
    [SerializeField] private int currentDash;
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private bool isCrouching = false;
    private bool grounded = false;
    private bool canDash = true;
    private bool canJump = true;
    private bool canRun = true;
    private bool isJumping = false;
    [SerializeField] private bool isRunning = false;
    private bool isDashing = false;
    //C'est la force du dash en vertical et horizontal. Sunny se tourne automatiquement vers la droite, à modifier. 
    [SerializeField] private float horizontalDashingPower = 24f;
    [SerializeField] private float verticalDashingPower = 14f;
    [SerializeField] private float dashingTime = 0.001f;
    //Le chiffre inscrit correspond au temps avant que Sunny puisse re-sauter. 
    [SerializeField] private float dashingCooldown = 1f;


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        animController = GetComponent<Animator>();
        currentDash = dashLimit;
    }
    // Update is called once per frame
    void Update()
    {
        if (isDashing)
        {
            return;
        }
        //Gère l'orientation du sprite en horizontal
        horizontalValue = Input.GetAxis("Horizontal");
        if (horizontalValue < 0) sr.flipX = true;
        else if (horizontalValue > 0) sr.flipX = false;
        //animController.SetBool("Running", horizontalValue != 0);
        animController.SetFloat("speed", Mathf.Abs(horizontalValue));
        verticalValue = Input.GetAxis("Vertical");

        if (Input.GetKeyDown(KeyCode.Space) && canJump)
        {
            isJumping = true;
            animController.SetBool("Jumping", true);
        }

        if (Input.GetKeyDown(KeyCode.LeftControl) && canRun)
        {
            isRunning = true;
            animController.SetBool("Running", true);
        }
        if (Input.GetKeyUp(KeyCode.LeftControl) && canRun)
        {
            isRunning = false;
            animController.SetBool("Running", false);
        }

        if (Input.GetKeyDown(KeyCode.LeftAlt))
        {
            isCrouching = true;
            animController.SetBool("Crouching", true);
        }
        if (Input.GetKeyUp(KeyCode.LeftAlt))
        {
            isCrouching = false;
            animController.SetBool("Crouching", false);
        }

        animController.SetFloat("speed", Mathf.Abs(horizontalValue));

        //Rémi a dit qu'il était pas fan du Coroutine, donc si vous arrivez à modifier c'est tant mieux
        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash && currentDash > 0)
        {
            StartCoroutine(Dash());
        }
    }
    void FixedUpdate()
    {
        if (isDashing)
        {
            return;
        }
        if (isJumping)
        {
            grounded = false;
            isJumping = false;
            rb.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
            canJump = false;
        }
        //si tu es pas en train de courir c'est que tu es en train de marcher
        float speedModifer;
        if (isRunning)
        {
            Debug.Log("je crous");
            speedModifer = runningSpeed;
        }
        else if (isCrouching)
        {
            speedModifer = crouchingSpeed;
        }
        else
        {
            speedModifer = walkingSpeed;
        }
        Vector2 target_velocity = new Vector2(horizontalValue * speedModifer * Time.fixedDeltaTime, rb.velocity.y);
        rb.velocity = Vector2.SmoothDamp(rb.velocity, target_velocity, ref ref_velocity, 0.05f);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        currentDash = dashLimit;
        grounded = true;
        canJump = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        animController.SetBool("Jumping", false);
    }
    private IEnumerator Dash()
    {
        currentDash--;
        //Pour dash vers le haut, clic, flèche haut puis left 
        //Ces lignes permettent de dash dans toute les directions
        Vector2 DashDirection = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        DashDirection = DashDirection.normalized;
        DashDirection = new Vector2(DashDirection.x * horizontalDashingPower, DashDirection.y * verticalDashingPower);
        if (!isRunning)
        {
            rb.velocity = DashDirection;
        }
        else
        {
            rb.velocity += DashDirection;
        }
        Debug.Log(rb.velocity);


        //Je crois que "return" est utilisé pour relancer le code dès que Sunny a toucher le sol
        yield return new WaitForSeconds(dashingTime);
        isDashing = false;
        yield return new WaitForSeconds(dashingCooldown);
        canDash = true;
    }
}