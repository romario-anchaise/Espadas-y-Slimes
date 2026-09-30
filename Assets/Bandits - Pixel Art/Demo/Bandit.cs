using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D), typeof(Animator))]
public class Bandit : MonoBehaviour
{
    [SerializeField] private float m_speed = 4f;
    [SerializeField] private float m_jumpForce = 7.5f;

    private Animator m_animator;
    private Rigidbody2D m_body2d;
    private Sensor_Bandit m_groundSensor;
    private InputAction m_move;
    private InputAction m_jump;
    private InputAction m_attack;
    private bool m_grounded;
    private bool m_combatIdle;
    private bool m_isDead;

    private void Awake()
    {
        m_animator = GetComponent<Animator>();
        m_body2d = GetComponent<Rigidbody2D>();
        m_groundSensor = transform.Find("GroundSensor").GetComponent<Sensor_Bandit>();

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

        m_attack = new InputAction("Attack", InputActionType.Button);
        m_attack.AddBinding("<Mouse>/leftButton");
        m_attack.AddBinding("<Gamepad>/buttonWest");
    }

    private void OnEnable()
    {
        m_move.Enable();
        m_jump.Enable();
        m_attack.Enable();
    }

    private void OnDisable()
    {
        m_move.Disable();
        m_jump.Disable();
        m_attack.Disable();
    }

    private void OnDestroy()
    {
        m_move.Dispose();
        m_jump.Dispose();
        m_attack.Dispose();
    }

    private void Update()
    {
        bool touchingGround = m_groundSensor.State();
        if (m_grounded != touchingGround)
        {
            m_grounded = touchingGround;
            m_animator.SetBool("Grounded", m_grounded);
        }

        float inputX = m_move.ReadValue<Vector2>().x;
        if (inputX > 0.01f)
            transform.localScale = new Vector3(-1f, 1f, 1f);
        else if (inputX < -0.01f)
            transform.localScale = new Vector3(1f, 1f, 1f);

        m_animator.SetFloat("AirSpeed", m_body2d.linearVelocity.y);

        Keyboard keyboard = Keyboard.current;
        if (keyboard != null && keyboard.eKey.wasPressedThisFrame)
        {
            m_animator.SetTrigger(m_isDead ? "Recover" : "Death");
            m_isDead = !m_isDead;
        }
        else if (keyboard != null && keyboard.qKey.wasPressedThisFrame)
            m_animator.SetTrigger("Hurt");
        else if (m_attack.WasPressedThisFrame())
            m_animator.SetTrigger("Attack");
        else if (keyboard != null && keyboard.fKey.wasPressedThisFrame)
            m_combatIdle = !m_combatIdle;
        else if (m_jump.WasPressedThisFrame() && m_grounded)
        {
            m_animator.SetTrigger("Jump");
            m_grounded = false;
            m_animator.SetBool("Grounded", false);
            m_body2d.linearVelocity = new Vector2(m_body2d.linearVelocity.x, m_jumpForce);
            m_groundSensor.Disable(0.2f);
        }
        else if (Mathf.Abs(inputX) > 0.01f)
            m_animator.SetInteger("AnimState", 2);
        else
            m_animator.SetInteger("AnimState", m_combatIdle ? 1 : 0);
    }

    private void FixedUpdate()
    {
        float inputX = m_move.ReadValue<Vector2>().x;
        m_body2d.linearVelocity = new Vector2(inputX * m_speed, m_body2d.linearVelocity.y);
    }
}
