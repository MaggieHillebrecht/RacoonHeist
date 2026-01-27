using UnityEngine;

public class ItemInteract : MonoBehaviour
{
    [Header("References")]
    public Rigidbody playerRb;
    public Transform grabPoint;

    [Header("Settings")]
    public float interactRange = 4f;
    public float breakForce = 2000f;
    public float damping = 60f;
    public LayerMask interactLayers;

    GameObject grabbedObj;
    Rigidbody grabbedRb;
    ConfigurableJoint joint;

    PlayerInputHandler input;
    PlayerInteractionState interactionState;

    void Awake()
    {
        input = GetComponent<PlayerInputHandler>();
        interactionState = GetComponent<PlayerInteractionState>();
    }

    void OnEnable()
    {
        input.OnInteractPressed += TryGrab;
        input.OnInteractReleased += Drop;
    }

    void OnDisable()
    {
        input.OnInteractPressed -= TryGrab;
        input.OnInteractReleased -= Drop;
    }

    void TryGrab()
    {
        Debug.Log("TryGrab called");

        if (grabbedObj != null)
        {
            Debug.Log("Already grabbing an object: " + grabbedObj.name);
            return;
        }

        Collider[] hits = Physics.OverlapSphere(transform.position, interactRange, interactLayers);
        Debug.Log("Found " + hits.Length + " objects in range");

        foreach (Collider col in hits)
        {
            Debug.Log("Hit object: " + col.name);
            Rigidbody rb = col.attachedRigidbody;

            if (rb == null)
            {
                Debug.Log("No Rigidbody attached, skipping");
                continue;
            }

            if (rb.isKinematic)
            {
                Debug.Log("Rigidbody is kinematic, skipping");
                continue;
            }

            grabbedObj = rb.gameObject;
            grabbedRb = rb;

            Debug.Log("Attempting to grab: " + grabbedObj.name);
            CreateJoint(col);
            interactionState?.StartPulling();
            Debug.Log("Grab successful");

            return;
        }

        Debug.Log("No valid object found to grab");
    }

    void CreateJoint(Collider col)
    {
        Vector3 grabPointWorld = col.ClosestPoint(grabPoint.position);
        Debug.Log("Grab point world position: " + grabPointWorld);

        joint = grabbedObj.AddComponent<ConfigurableJoint>();
        joint.connectedBody = playerRb;
        joint.autoConfigureConnectedAnchor = false;

        joint.anchor = grabbedObj.transform.InverseTransformPoint(grabPointWorld);
        joint.connectedAnchor = playerRb.transform.InverseTransformPoint(grabPoint.position);

        Debug.Log("Joint anchor: " + joint.anchor + ", connectedAnchor: " + joint.connectedAnchor);

        SoftJointLimit limit = new SoftJointLimit { limit = 0.1f };
        joint.linearLimit = limit;

        joint.xMotion = joint.yMotion = joint.zMotion = ConfigurableJointMotion.Limited;
        joint.angularXMotion = joint.angularYMotion = joint.angularZMotion = ConfigurableJointMotion.Free;

        JointDrive drive = new JointDrive
        {
            positionSpring = 0f,
            positionDamper = damping,
            maximumForce = Mathf.Infinity
        };

        joint.xDrive = joint.yDrive = joint.zDrive = drive;

        joint.breakForce = breakForce;
        joint.breakTorque = breakForce;
        joint.enableCollision = true;

        Debug.Log("Joint created on: " + grabbedObj.name);
    }

    void Drop()
    {
        if (grabbedObj == null)
        {
            Debug.Log("Drop called, but no object is grabbed");
            return;
        }

        if (joint != null)
        {
            Destroy(joint);
            Debug.Log("Joint destroyed");
        }

        Debug.Log("Dropped object: " + grabbedObj.name);

        grabbedObj = null;
        grabbedRb = null;
        joint = null;

        interactionState?.StopPulling();
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactRange);
    }
}