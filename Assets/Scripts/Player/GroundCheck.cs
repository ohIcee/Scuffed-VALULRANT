using UnityEngine;

public class GroundCheck : MonoBehaviour
{
    [SerializeField] LayerMask groundMask;
    [SerializeField] Vector3 offset;
    [SerializeField] float size;

    [HideInInspector] public bool isGrounded;

    public event System.Action Grounded;

    void LateUpdate()
    {
        bool isGroundedNow = Physics.CheckSphere(transform.position + offset, size, groundMask);
        if (isGroundedNow && !isGrounded)
            Grounded?.Invoke();
        isGrounded = isGroundedNow;
    }

    void OnDrawGizmosSelected()
    {
        // Draw a yellow sphere at the transform's position
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(transform.position + offset, size);
    }
}