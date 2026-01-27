using UnityEngine;

[RequireComponent(typeof(Collider))]
public class SlipperyPuddle : MonoBehaviour
{
    [Header("Slippery Settings")]
    public Vector3 slideDirection = new Vector3(0, 0, 1);
    public float slideForce = 20f;
    public float maxSpeed = 12f;
    public PhysicsMaterial slipperyMaterial;

    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.CompareTag("Player")) return;

        Rigidbody rb = collision.rigidbody;
        if (rb == null) return;

        // Disable player control
        var interaction = collision.gameObject.GetComponent<PlayerInteractionState>();
        interaction?.StartPulling(); // reuse "restricted movement" state

        if (slipperyMaterial != null)
        {
            Collider col = GetComponent<Collider>();
            if (col != null)
                col.material = slipperyMaterial;
        }

        Vector3 worldDir = transform.TransformDirection(slideDirection.normalized);
        rb.AddForce(worldDir * slideForce, ForceMode.VelocityChange);
    }

    private void OnCollisionStay(Collision collision)
    {
        if (!collision.gameObject.CompareTag("Player")) return;

        Rigidbody rb = collision.rigidbody;
        if (rb == null) return;

        bool grounded = false;
        Vector3 avgNormal = Vector3.zero;

        foreach (var contact in collision.contacts)
        {
            avgNormal += contact.normal;
            if (contact.normal.y > 0.3f)
                grounded = true;
        }

        avgNormal.Normalize();

        if (!grounded) return;

        Vector3 worldDir = transform.TransformDirection(slideDirection.normalized);
        worldDir = Vector3.ProjectOnPlane(worldDir, avgNormal).normalized;

        rb.AddForce(worldDir * slideForce * Time.fixedDeltaTime, ForceMode.VelocityChange);

        if (rb.linearVelocity.magnitude > maxSpeed)
            rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;
    }

    private void OnCollisionExit(Collision collision)
    {
        if (!collision.gameObject.CompareTag("Player")) return;

        var interaction = collision.gameObject.GetComponent<PlayerInteractionState>();
        interaction?.StopPulling();

        Collider col = GetComponent<Collider>();
        if (col != null)
            col.material = null;
    }
}