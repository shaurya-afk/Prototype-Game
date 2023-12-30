using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class PlayerMove : MonoBehaviour
{
    [SerializeField] CharacterController character;
    [SerializeField] float speed;
    [SerializeField] Transform cam;

    bool isGrounded;
    public LayerMask ground;
    public float groundDis = 0.4f;
    public Transform groundCheck;

    Vector3 velocity;
    float gravity = -9.81f;

    public float turnSmoothTime = 0.1f;
    float turnSmoothVelocity;

    [SerializeField] Animator anim;

    bool isCrouching;

    public float health = 10f;

    [SerializeField] TextMeshProUGUI countDownText, healthText;
    [SerializeField] GameObject deadScreen, hitSign;
    // Start is called before the first frame update
    void Start()
    {
        deadScreen.SetActive(false);
        hitSign.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        healthText.text = "Health: " + health.ToString();
        if(health <= 0)
        {
            health = 0;
            speed = 0;
            deadScreen.SetActive(true);
            StartCoroutine(DeadCountDown());
        }
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDis, ground);

        if(isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");

        Vector3 dir = new Vector3(x, 0f, z).normalized;
        if(dir.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            character.Move(moveDir.normalized * speed * Time.deltaTime);

            anim.SetBool("isRunning", true);
        }
        else
        {
            anim.SetBool("isRunning", false);
        }
        velocity.y += gravity * Time.deltaTime;
        character.Move(velocity * Time.deltaTime);

        if(Input.GetKeyDown(KeyCode.C))
        {
            if(!isCrouching)
            {
                anim.SetBool("isCrouching", true);
                character.height  = 1.78f;
                character.center = new Vector3(0f, 0.97f, 0f); // reduce height of the player
                isCrouching = true;
            }
            else
            {
                anim.SetBool("isCrouching", false);
                character.height = 2.78f;
                character.center = new Vector3(0f, 1.52f, 0f);
                isCrouching = false;
            }
        }
        if (dir.magnitude >= 0.1f && isCrouching)
        {
            anim.SetBool("isCrouchRun", true);
            anim.SetBool("isCrouching", false);
        }
        else if (dir.magnitude < 0.1f && isCrouching)
        {
            anim.SetBool("isCrouchRun", false);
            anim.SetBool("isCrouching", true);
        }
    }

    public IEnumerator takingDamage(float damage)
    {
        if(health > 0)
        {
            health -= damage;
            hitSign.SetActive(true);
            yield return new WaitForSeconds(1.5f);
            hitSign.SetActive(false);
        }
    }
    IEnumerator DeadCountDown()
    {
        int i = 3;
        while(i != 0)
        {
            countDownText.text = i.ToString();
            yield return new WaitForSeconds(1f);
            i--;
        }
        SceneManager.LoadScene(0);
    }
}
