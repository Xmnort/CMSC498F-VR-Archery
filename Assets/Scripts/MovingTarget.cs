using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingTarget : MonoBehaviour, IHittable
{
    private Rigidbody rb;
    private bool stopped = false;

    private Vector3 nextposition;
    private Vector3 originPosition;

    [SerializeField] private AudioSource hitAudio;

    [SerializeField] private float arriveThreshold, movementRadius = 2, speed = 1;

    private void Awake() {
        rb = GetComponent<Rigidbody>();
        originPosition = transform.position;
        nextposition = GetNewMovementPosition();
    }

    private Vector3 GetNewMovementPosition() {
        return originPosition + (Vector3)Random.insideUnitCircle * movementRadius;
    }
    
    // moves target around it's original position
    private void FixedUpdate() {
        if (!stopped) {
            if(Vector3.Distance(transform.position,nextposition) < arriveThreshold) {
                nextposition = GetNewMovementPosition();
            }

            Vector3 direction = nextposition - transform.position;
            rb.MovePosition(transform.position + direction.normalized * Time.fixedDeltaTime * speed);
        }
    }

    // plays target hit audio once shot by arrow
    private void OnCollisionEnter(Collision collision) {
        if ((rb.isKinematic || collision.gameObject.CompareTag("Arrow")) == false) {
            hitAudio.Play();
        }
    }

    // stops the target once hit and reduces number of targets left on GUI
    public void GetHit() {
        
        // updates the UI to reflect number of targets left
        if (!stopped) 
            GUI.targetsLeft--;

        rb.isKinematic = false;
        stopped = true;
    }
}

public interface IHittable {
    void GetHit();
}