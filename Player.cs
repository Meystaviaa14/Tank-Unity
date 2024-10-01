using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private Rigidbody rb;

    [Header("Gun data")]
    [SerializeField] private Transform gunPoint;
    [SerializeField] private float bulletSpeed;
    [SerializeField] private GameObject bulletPrefab;

    [Header("Movement data")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float sprintSpeed; // Kecepatan sprint
    [SerializeField] private float rotationSpeed;

    [Header("Jumping data")]
    [SerializeField] private float jumpForce;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float groundCheckDistance = 0.1f;

    private float verticalInput;
    private float horizontalInput;
    private bool isGrounded;
    private bool isSprinting; // Status sprint

    [Header("Tower data")]
    [SerializeField] private Transform towerTransform;
    [SerializeField] private float towerRotationSpeed;

    [Header("Aim data")]
    [SerializeField] private Transform aimTransform;
    [SerializeField] private LayerMask whatIsAimMask;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        UpdateAim();
        CheckInputs();
        CheckGrounded();
    }

    private void CheckInputs()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
            Shoot();

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
            Jump();

        isSprinting = Input.GetKey(KeyCode.LeftShift); // Periksa input sprint

        verticalInput = Input.GetAxis("Vertical");
        horizontalInput = Input.GetAxis("Horizontal");
    }

    private void FixedUpdate()
    {
        ApplyMovement();
        ApplyBodyRotation();
        ApplyTowersRotation();
    }

    private void Shoot()
    {
        GameObject bullet = Instantiate(bulletPrefab, gunPoint.position, gunPoint.rotation);
        bullet.GetComponent<Rigidbody>().velocity = gunPoint.forward * bulletSpeed;
        Destroy(bullet, 7);
    }

    private void ApplyTowersRotation()
    {
        Vector3 direction = aimTransform.position - towerTransform.position;
        direction.y = 0;

        Quaternion targetRotation = Quaternion.LookRotation(direction);
        towerTransform.rotation = Quaternion.RotateTowards(towerTransform.rotation, targetRotation, towerRotationSpeed);
    }

    private void ApplyBodyRotation()
    {
        transform.Rotate(0, horizontalInput * rotationSpeed, 0);
    }

    private void ApplyMovement()
    {
        float currentMoveSpeed = isSprinting ? sprintSpeed : moveSpeed; // Gunakan kecepatan sprint jika aktif
        Vector3 movement = transform.forward * currentMoveSpeed * verticalInput;
        rb.velocity = new Vector3(movement.x, rb.velocity.y, movement.z);
    }

    private void UpdateAim()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, whatIsAimMask))
        {
            float fixedY = aimTransform.position.y;
            aimTransform.position = new Vector3(hit.point.x, fixedY, hit.point.z);
        }
    }

    private void Jump()
    {
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    private void CheckGrounded()
    {
        isGrounded = Physics.Raycast(transform.position, Vector3.down, groundCheckDistance, groundLayer);
    }
}