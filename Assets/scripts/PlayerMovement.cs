using System;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movimiento")]
    [SerializeField] private float m_speed = 4f;
    [SerializeField] private float m_jumpForce = 7.5f;

    [Header("Detección del suelo")]
    [SerializeField] private Sensor_Bandit m_groundSensor;
    [SerializeField] private Transform m_groundCheck;
    [SerializeField] private float m_groundRadius = 0.12f;

    private Rigidbody2D m_body2d;
    private Collider2D m_collider;
    private CinemachineImpulseSource m_impulse;
    private InputAction m_move;
    private InputAction m_jump;
    private bool m_jumpRequested;

    public float HorizontalInput { get; private set; }
    public bool IsOnGround { get; private set; }
    public float VerticalSpeed => m_body2d != null ? m_body2d.linearVelocity.y : 0f;

    public event Action Jumped;

    private void Awake()
    {
        m_body2d = GetComponent<Rigidbody2D>();
        m_collider = GetComponent<Collider2D>();
        m_impulse = GetComponent<CinemachineImpulseSource>();

        FindGroundCheck();
        CreateInputActions();
    }

    private void OnEnable()
    {
        m_move?.Enable();
        m_jump?.Enable();
    }

    private void OnDisable()
    {
        m_move?.Disable();
        m_jump?.Disable();
    }

    private void OnDestroy()
    {
        m_move?.Dispose();
        m_jump?.Dispose();
    }

    private void Update()
    {
        HorizontalInput = m_move.ReadValue<Vector2>().x;

        if (m_jump.WasPressedThisFrame())
            m_jumpRequested = true;

        UpdateFacingDirection();
    }

    private void FixedUpdate()
    {
        IsOnGround = CheckGrounded();

        if (m_jumpRequested && IsOnGround)
        {
            m_body2d.linearVelocity = new Vector2(HorizontalInput * m_speed, m_jumpForce);
            IsOnGround = false;

            m_impulse?.GenerateImpulse(0.5f);
            Jumped?.Invoke();
            m_groundSensor?.Disable(0.2f);
        }
        else
        {
            m_body2d.linearVelocity = new Vector2(
                HorizontalInput * m_speed,
                m_body2d.linearVelocity.y);
        }

        m_jumpRequested = false;
    }

    private void CreateInputActions()
    {
        m_move = new InputAction("Move", InputActionType.Value);
        m_move.AddCompositeBinding("2DVector")
            .With("Up", "<Keyboard>/w")
            .With("Down", "<Keyboard>/s")
            .With("Left", "<Keyboard>/a")
            .With("Right", "<Keyboard>/d");
        m_move.AddCompositeBinding("2DVector")
            .With("Up", "<Keyboard>/upArrow")
            .With("Down", "<Keyboard>/downArrow")
            .With("Left", "<Keyboard>/leftArrow")
            .With("Right", "<Keyboard>/rightArrow");
        m_move.AddBinding("<Gamepad>/leftStick");

        m_jump = new InputAction("Jump", InputActionType.Button);
        m_jump.AddBinding("<Keyboard>/space");
        m_jump.AddBinding("<Gamepad>/buttonSouth");
    }

    private void FindGroundCheck()
    {
        if (m_groundSensor == null)
            m_groundSensor = GetComponentInChildren<Sensor_Bandit>(true);
        if (m_groundSensor == null)
            m_groundSensor = transform.root.GetComponentInChildren<Sensor_Bandit>(true);

        if (m_groundCheck == null && m_groundSensor != null)
            m_groundCheck = m_groundSensor.transform;

        if (m_groundCheck != null)
            return;

        foreach (Transform child in GetComponentsInChildren<Transform>(true))
        {
            if (child != transform &&
                (child.name == "GroundSensor" || child.name == "groundsenor" ||
                 child.name == "ground" || child.name == "Ground"))
            {
                m_groundCheck = child;
                break;
            }
        }
    }

    private void UpdateFacingDirection()
    {
        if (HorizontalInput > 0.01f)
        {
            transform.localScale = new Vector3(
                -Mathf.Abs(transform.localScale.x),
                transform.localScale.y,
                transform.localScale.z);
        }
        else if (HorizontalInput < -0.01f)
        {
            transform.localScale = new Vector3(
                Mathf.Abs(transform.localScale.x),
                transform.localScale.y,
                transform.localScale.z);
        }
    }

    private bool CheckGrounded()
    {
        if (m_groundSensor != null)
            return m_groundSensor.State();

        if (m_groundCheck != null)
        {
            Collider2D[] groundHits = Physics2D.OverlapCircleAll(
                m_groundCheck.position,
                m_groundRadius);

            foreach (Collider2D hit in groundHits)
            {
                if (!hit.isTrigger && hit.attachedRigidbody != m_body2d)
                    return true;
            }

            return false;
        }

        if (m_collider == null)
            return false;

        Bounds feet = m_collider.bounds;
        Vector2 center = new Vector2(feet.center.x, feet.min.y - 0.04f);
        Vector2 size = new Vector2(feet.size.x * 0.8f, 0.08f);
        Collider2D[] hits = Physics2D.OverlapBoxAll(center, size, 0f);

        foreach (Collider2D hit in hits)
        {
            if (hit != m_collider && !hit.isTrigger)
                return true;
        }

        return false;
    }
}
