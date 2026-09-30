using UnityEngine;

[RequireComponent(typeof(Animator), typeof(PlayerMovement))]
public class PlayerAnimationController : MonoBehaviour
{
    private PlayerMovement m_movement;
    private Animator m_animator;
    private bool m_hasAnimState;
    private bool m_hasGrounded;
    private bool m_hasAirSpeed;
    private bool m_hasJump;

    private void Awake()
    {
        m_movement = GetComponent<PlayerMovement>();
        m_animator = GetComponent<Animator>();
        FindAnimatorParameters();
    }

    private void OnEnable()
    {
        if (m_movement != null)
            m_movement.Jumped += PlayJumpAnimation;
    }

    private void OnDisable()
    {
        if (m_movement != null)
            m_movement.Jumped -= PlayJumpAnimation;
    }

    private void Update()
    {
        if (m_animator == null || m_movement == null)
            return;

        if (m_hasAnimState)
        {
            int animationState = Mathf.Abs(m_movement.HorizontalInput) > 0.01f ? 2 : 0;
            m_animator.SetInteger("AnimState", animationState);
        }

        if (m_hasGrounded)
            m_animator.SetBool("Grounded", m_movement.IsOnGround);

        if (m_hasAirSpeed)
            m_animator.SetFloat("AirSpeed", m_movement.VerticalSpeed);
    }

    private void PlayJumpAnimation()
    {
        if (m_animator != null && m_hasJump)
            m_animator.SetTrigger("Jump");
    }

    private void FindAnimatorParameters()
    {
        if (m_animator == null || m_animator.runtimeAnimatorController == null)
            return;

        foreach (AnimatorControllerParameter parameter in m_animator.parameters)
        {
            if (parameter.name == "AnimState" && parameter.type == AnimatorControllerParameterType.Int)
                m_hasAnimState = true;
            else if (parameter.name == "Grounded" && parameter.type == AnimatorControllerParameterType.Bool)
                m_hasGrounded = true;
            else if (parameter.name == "AirSpeed" && parameter.type == AnimatorControllerParameterType.Float)
                m_hasAirSpeed = true;
            else if (parameter.name == "Jump" && parameter.type == AnimatorControllerParameterType.Trigger)
                m_hasJump = true;
        }
    }
}
