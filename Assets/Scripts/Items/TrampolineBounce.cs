using UnityEngine;

public class TrampolineBounce : MonoBehaviour
{
    [Header("Player Bounce")]
    public float playerBounceForce = 12f;

    [Header("Object Bounce")]
    public float objectBounceForce = 10f;

    private void OnCollisionEnter(Collision collision)
    {
        // ---------- PLAYER ----------
        PlayerJump playerJump = collision.gameObject.GetComponent<PlayerJump>();
        if (playerJump != null)
        {
            playerJump.Bounce(playerBounceForce);
            return;
        }

        // ---------- OTHER OBJECTS ----------
        Rigidbody rb = collision.rigidbody;
        if (rb == null) return;

        Vector3 vel = rb.linearVelocity;
        vel.y = 0f;
        rb.linearVelocity = vel;

        rb.AddForce(Vector3.up * objectBounceForce, ForceMode.VelocityChange);
    }
}