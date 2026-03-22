using System.Collections;
using System.Collections.Generic;
//using System.Numerics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR;

public class PlayerCpntroller : MonoBehaviour
{
    [Header("プレイヤーの動きの設定")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpPower = 10f;

    [Header("マウスでの首振りの設定")]
    [SerializeField] private float mouseSensitivity = 100f;
    [SerializeField] private float maxLookAngle = 80f;

    [Header("リスポーン設定")]
    [SerializeField] private Vector3 respawnPosition = new Vector3(5,10,5);
    [SerializeField] private float fallThreshold = -10f;

    [Header("ブロック破壊の設定")]
    [SerializeField] private float destroyDistance = 5f;
    [SerializeField] private GameObject destroyEffect;


    private CharacterController characterController;
    private Vector3 moveDirection;
    private float gravity = -20f;
    private bool isGrounded;

    private float xRotation;
    private Camera playerCamera;

    
    
    // Start is called before the first frame update
    void Start()
    {
       characterController = GetComponent<CharacterController>();

       playerCamera = GetComponentInChildren<Camera>();

       moveDirection = Vector3.zero;

       Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        HandleMouseLook();

        isGrounded = characterController.isGrounded;

        if (isGrounded &&  moveDirection.y < 0)
        {
            moveDirection.y = 0f;
        }

        float horizontal = 0f;
        float vertical = 0f;


        if (Input.GetKey(KeyCode.W))
        {
            vertical = 1f;
        }

        if (Input.GetKey(KeyCode.S))
        {
            vertical = -1f;
        }

        if (Input.GetKey(KeyCode.A))
        {
            horizontal = -1f;
        }

        if (Input.GetKey(KeyCode.D))
        {
            horizontal = 1f;
        }

        Vector3 move = transform.right * horizontal + transform.forward * vertical;

        characterController.Move(move * moveSpeed * Time.deltaTime);

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            moveDirection.y = jumpPower;
        }

        if (Input.GetMouseButtonDown(0))
        {
            DestroyBlockInSight();
        }

        if (Input.GetKeyDown(KeyCode.Return))
        {
            ResetGame();
        }

        moveDirection.y += gravity * Time.deltaTime;

        characterController.Move(moveDirection * Time.deltaTime);

        if (transform.position.y < fallThreshold)
        {
            transform.position = respawnPosition;

            moveDirection = Vector3.zero;
        }

        
    }


    void HandleMouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        transform.Rotate(Vector3.up * mouseX);

        xRotation -= mouseY;

        xRotation = Mathf.Clamp(xRotation, -maxLookAngle, maxLookAngle);

        playerCamera.transform.localRotation = Quaternion.Euler(xRotation, 0f,0f);
    }

    void DestroyBlockInSight()
    {
        Ray ray = playerCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));

        if (Physics.Raycast(ray, out RaycastHit hit, destroyDistance))
        {
            GameObject targetBlock = hit.collider.gameObject;

            GameObject effect = Instantiate(destroyEffect, targetBlock.transform.position,Quaternion.identity);

            Destroy(effect, 2f);

            Destroy(targetBlock);
        }
    }


    void ResetGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
