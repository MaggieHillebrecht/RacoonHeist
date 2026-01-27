using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class StepClimber : MonoBehaviour
{
    public float maxStepHeight = 0.4f;
    public float stepCheckDistance = 0.3f;

    Rigidbody rb;
    GroundChecker groundChecker;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        if (!groundChecker)
            groundChecker = GetComponentInChildren<GroundChecker>();
    }

    void FixedUpdate()
    {
        if (!groundChecker.IsGrounded) return;

        Vector3 moveDir = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
        if (moveDir.magnitude < 0.1f) return;

        moveDir.Normalize();

        Vector3 bottom = transform.position + Vector3.up * 0.05f;
        Vector3 top = bottom + Vector3.up * 0.05f;

        if (!Physics.CapsuleCast(bottom, top, 0.3f, moveDir, stepCheckDistance))
            return;

        Vector3 stepBottom = bottom + Vector3.up * maxStepHeight;
        Vector3 stepTop = top + Vector3.up * maxStepHeight;

        if (!Physics.CapsuleCast(stepBottom, stepTop, 0.3f, moveDir, stepCheckDistance))
        {
            rb.position += Vector3.up * (maxStepHeight + 0.02f);
            rb.position += moveDir * 0.05f;
        }
    }
}