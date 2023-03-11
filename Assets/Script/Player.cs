using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/* 

- Courir
- S'accroupir
- une barre de vie constituer de 9 coeurs
- Une zone ou tu es ralenti 
- Respawn & Checkpoint
- Un objet immobile qui te ferait -1 de dégât si tu le touche 
- Collectibles
*/

/*
Il arrive que le dash en diagonal propulse Sunny loin dans la map.
C'est parce que, (pour l'exemple), le vector horizontal et vertical sont de 1, du coup en diagonale ça les additionnes donc ca fait 2.
Pour ça, il faut mettre une "normalize" et rajouter ses deux vecteurs ((donc x;1 et y;1)) pour qu'en diagonale ca fasse 0,5.
*/
public class Player : MonoBehaviour
{
    Rigidbody2D rb;
    SpriteRenderer sr;
    Animator animController;
    Vector2 ref_velocity = Vector2.zero;
    float walkingSpeed = 400f;
    float runningSpeed = 800f;
    float horizontalValue;
    float verticalValue;

    //SerializeField sert à afficher les paramètres dans le player
    //Le dash limit sert à mettre une Limite de Dash, je peux le changer directement dans le "Inspector" du player.
    [SerializeField] int dashLimit = 1;
    [SerializeField] int currentDash;
    [SerializeField] float jumpForce = 10f;
    private bool canDash = true;
    private bool canJump = true;
    private bool canRun = true;
    private bool isJumping = false;
    private bool isRunning = false;
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
            isJumping = false;
            rb.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
            canJump = false;
        }

        //si tu es pas en train de courir c'est que tu es en train de marcher
        float speedModifer;        
        if (isRunning){
            speedModifer = runningSpeed; 
        }
        else{
            speedModifer = walkingSpeed;
        }
        Vector2 target_velocity = new Vector2(horizontalValue * speedModifer * Time.fixedDeltaTime, rb.velocity.y);
        rb.velocity = Vector2.SmoothDamp(rb.velocity, target_velocity, ref ref_velocity, 0.05f);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        currentDash = dashLimit;
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
        if (!isRunning){
            rb.velocity = DashDirection;
        }
        else{
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