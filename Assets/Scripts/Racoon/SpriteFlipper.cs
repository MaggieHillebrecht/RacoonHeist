using UnityEngine;

public class SpriteFlipper : MonoBehaviour
{
    PlayerInputHandler input;

    void Awake()
    {
        input = GetComponent<PlayerInputHandler>();
    }

    void LateUpdate()
    {
        if (input.MoveDir.x == 0) return;

        Vector3 scale = transform.localScale;
        scale.x = input.MoveDir.x < 0 ? -Mathf.Abs(scale.x) : Mathf.Abs(scale.x);
        transform.localScale = scale;
    }
}