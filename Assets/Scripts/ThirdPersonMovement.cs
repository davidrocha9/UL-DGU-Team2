/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonMovement : MonoBehaviour
{
    public CharacterController controller;
    public float speed = 6f;
    public float turnSmoothTime = 0.1f;
    float turnSmoothVelocity;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;
        Debug.Log(direction);

        if (direction.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);  
            transform.rotation = Quaternion.Euler(0f, targetAngle, 0f);
            controller.Move(direction*speed*Time.deltaTime);
        }
    }
}*/

using System.Collections;


using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Security.Cryptography;
using System.Threading;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ThirdPersonMovement : MonoBehaviour
{
    public CharacterController controller;
    public Transform cam;

    public float speed = 6;
    public float gravity = -9.81f;
    public float jumpHeight = 1.5f;
    Vector3 velocity;
    Vector2 movementRcvd;

    float turnSmoothVelocity;
    public float turnSmoothTime = 0.1f;

    // Player run & stamina stuff
    private float runSpeed = 12;
    private float normalSpeed = 6;
    [SerializeField] private Slider staminaSlider;

    // Update is called once per frame
    void Update()
    {
        if (transform.position.y <= 1.6 && velocity.y < 0)
        {
            velocity.y = -2f;
        }
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
        
        // run control
        if (Input.GetKey(KeyCode.LeftShift) && GameManager.gameManager._playerStamina.Stamina > 0 && transform.position.y <= 1.6f)
        {
            speed = runSpeed;
           
            GameManager.gameManager._playerStamina.Stamina -= GameManager.gameManager._playerStamina.UseAmount * Time.deltaTime;
            staminaSlider.value = GameManager.gameManager._playerStamina.Stamina;
        }
        else
        {

            speed = normalSpeed;
 
            if (GameManager.gameManager._playerStamina.Stamina < GameManager.gameManager._playerStamina.MaxStamina)
            {
                GameManager.gameManager._playerStamina.Stamina += GameManager.gameManager._playerStamina.ReloadAmount * Time.deltaTime;
                staminaSlider.value = GameManager.gameManager._playerStamina.Stamina;
            }

        }
    }

    void FixedUpdate(){
        MovePlayer();
    }

    void MovePlayer(){
        Vector3 direction = new Vector3(movementRcvd.x, 0f, movementRcvd.y).normalized;

        if(direction.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            controller.Move(moveDir.normalized * speed * Time.deltaTime);
        }
    }

    void OnMove(InputValue input){
        movementRcvd = input.Get<Vector2>();
    }

    void OnJump(InputValue input){
        if (transform.position.y <= 1.6f)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2 * gravity);
        }
    }
}