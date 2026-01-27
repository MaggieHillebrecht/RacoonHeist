using UnityEngine;

public class PlayerJump : MonoBehaviour
{
    public float jumpForce = 7f;
    RBMovementMotor motor;
    GroundChecker groundChecker;

    void Awake()
    {
        motor = GetComponent<RBMovementMotor>();
        groundChecker = GetComponentInChildren<GroundChecker>();
    }

    public void OnJumpPressed()
    {
        Debug.Log($"[JUMP] OnJumpPressed | grounded={groundChecker.IsGrounded}");

        if (groundChecker.IsGrounded)
        {
            motor.SetVerticalVelocity(jumpForce);
        }
    }
}