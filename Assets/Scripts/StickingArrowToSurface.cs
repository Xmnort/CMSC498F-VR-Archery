using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StickingArrowToSurface : MonoBehaviour
{
    [SerializeField]
    private Rigidbody rb;
    [SerializeField]
    private SphereCollider myCollider;

    [SerializeField]
    private GameObject stickingArrow;

    private void OnCollisionEnter(Collision other) {
        // needed for correct orientation of arrow after sticking
        rb.isKinematic = true;
        myCollider.isTrigger = true;

        GameObject arrow = Instantiate(stickingArrow);
        arrow.transform.position = transform.position;
        arrow.transform.forward = transform.forward;

        // attaches the arrow to the object that it hit
        if (other.collider.attachedRigidbody != null) {
            arrow.transform.parent = other.collider.attachedRigidbody.transform;
        }

        // causes the moving targets to stop moving after hit
        other.collider.GetComponent<IHittable>()?.GetHit();

        // destroys the flying arrow after adding the sticking arrow
        Destroy(gameObject);
    }
}
