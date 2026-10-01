using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCombat : MonoBehaviour
{
    [SerializeField, Min(0.1f)] private float attackCooldown = 0.45f;

    private InputAction attackAction;
    private float nextAttackTime;

    public event Action Attacked;

    private void Awake()
    {
        attackAction = new InputAction("Attack", InputActionType.Button);
        attackAction.AddBinding("<Keyboard>/j");
        attackAction.AddBinding("<Mouse>/leftButton");
        attackAction.AddBinding("<Gamepad>/buttonWest");
    }

    private void OnEnable()
    {
        attackAction.Enable();
    }

    private void OnDisable()
    {
        attackAction.Disable();
    }

    private void OnDestroy()
    {
        attackAction.Dispose();
    }

    private void Update()
    {
        if (!attackAction.WasPressedThisFrame() || Time.time < nextAttackTime)
            return;

        nextAttackTime = Time.time + attackCooldown;
        Attacked?.Invoke();
    }
}
