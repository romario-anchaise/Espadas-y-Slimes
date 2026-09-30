using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Cinemachine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody2D))]
public class playermoviment : MonoBehaviour
{
    [SerializeField] private float m_speed = 4f;
    [SerializeField] private float m_jumpForce = 7.5f;
    [SerializeField] private Sensor_Bandit m_groundSensor;
    [SerializeField] private Transform m_groundCheck;
    [SerializeField] private float m_groundRadius = 0.12f;
    [Header("Reinicio por caída")]
    [SerializeField, Min(1f)] private float m_fallDistance = 12f;

    private Rigidbody2D m_body2d;
    private Collider2D m_collider;
    private Animator m_animator;
    private InputAction m_move;
    private InputAction m_jump;
    private bool m_jumpRequested;
    private bool m_hasAnimState;
    private bool m_hasGrounded;
    private bool m_hasAirSpeed;
    private bool m_hasJump;
    private float m_startY;
    private bool m_restarting;

    // Emisor de Cinemachine
    private CinemachineImpulseSource m_impulse;

    private void Awake()
    {
        m_startY = transform.position.y;
        m_body2d = GetComponent<Rigidbody2D>();
        m_collider = GetComponent<Collider2D>();
        m_animator = GetComponent<Animator>();
        
        m_impulse = GetComponent<CinemachineImpulseSource>();

        if (m_groundSensor == null)
            m_groundSensor = GetComponentInChildren<Sensor_Bandit>(true);
        if (m_groundSensor == null)
            m_groundSensor = transform.root.GetComponentInChildren<Sensor_Bandit>(true);

        if (m_groundCheck == null && m_groundSensor != null)
            m_groundCheck = m_groundSensor.transform;
        if (m_groundCheck == null)
        {
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

        if (m_animator != null && m_animator.runtimeAnimatorController != null)
        {
            foreach (AnimatorControllerParameter parameter in m_animator.parameters)
            {
                if (parameter.name == "AnimState" && parameter.type == AnimatorControllerParameterType.Int)
                    m_hasAnimState = true;
                if (parameter.name == "Grounded" && parameter.type == AnimatorControllerParameterType.Bool)
                    m_hasGrounded = true;
                if (parameter.name == "AirSpeed" && parameter.type == AnimatorControllerParameterType.Float)
                    m_hasAirSpeed = true;
                if (parameter.name == "Jump" && parameter.type == AnimatorControllerParameterType.Trigger)
                    m_hasJump = true;
            }
        }

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
        if (!m_restarting && transform.position.y < m_startY - m_fallDistance)
        {
            RestartLevel();
            return;
        }

        if (m_jump.WasPressedThisFrame())
            m_jumpRequested = true;

        float inputX = m_move.ReadValue<Vector2>().x;
        if (inputX > 0.01f)
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        else if (inputX < -0.01f)
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);

        if (m_animator != null)
        {
            if (m_hasAnimState)
                m_animator.SetInteger("AnimState", Mathf.Abs(inputX) > 0.01f ? 2 : 0);
            if (m_hasAirSpeed)
                m_animator.SetFloat("AirSpeed", m_body2d.linearVelocity.y);
        }
    }

    private void FixedUpdate()
    {
        float inputX = m_move.ReadValue<Vector2>().x;
        bool grounded = IsGrounded();

        if (m_animator != null && m_hasGrounded)
            m_animator.SetBool("Grounded", grounded);

        if (m_jumpRequested && grounded)
        {
            m_body2d.linearVelocity = new Vector2(inputX * m_speed, m_jumpForce);
            
            if (m_impulse != null)
                m_impulse.GenerateImpulse(0.5f);

            if (m_animator != null && m_hasJump)
                m_animator.SetTrigger("Jump");
            if (m_groundSensor != null)
                m_groundSensor.Disable(0.2f);
        }
        else
            m_body2d.linearVelocity = new Vector2(inputX * m_speed, m_body2d.linearVelocity.y);

        m_jumpRequested = false;
    }

    private bool IsGrounded()
    {
        if (m_groundSensor != null)
            return m_groundSensor.State();

        if (m_groundCheck != null)
        {
            Collider2D[] groundHits = Physics2D.OverlapCircleAll(m_groundCheck.position, m_groundRadius);
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

    private void RestartLevel()
    {
        m_restarting = true;
        m_move?.Disable();
        m_jump?.Disable();
        Time.timeScale = 1f;

        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.buildIndex);
    }
}
