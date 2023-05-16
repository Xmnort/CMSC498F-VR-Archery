using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowController : MonoBehaviour
{
    [SerializeField] private GameObject midPointVisual, arrowPrefab, arrowSpawnPoint;

    [SerializeField] private float arrowMaxSpeed = 10;
    [SerializeField] private AudioSource bowReleaseAudio;

    // shows the arrow when pulling back the string
    public void PrepareArrow() {
        midPointVisual.SetActive(true);
    }

    public void ReleaseArrow(float strength) {
        bowReleaseAudio.Play();
        midPointVisual.SetActive(false);

        // apply force to arrow when released from bow
        GameObject arrow = Instantiate(arrowPrefab);
        arrow.transform.position = arrowSpawnPoint.transform.position;
        arrow.transform.rotation = midPointVisual.transform.rotation;
        
        Rigidbody rb = arrow.GetComponent<Rigidbody>();
        rb.AddForce(midPointVisual.transform.forward * strength * arrowMaxSpeed, ForceMode.Impulse);
    }
}
