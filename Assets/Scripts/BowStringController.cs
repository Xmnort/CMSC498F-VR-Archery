using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;

public class BowStringController : MonoBehaviour
{
    [SerializeField] private BowString bowStringRenderer;

    private XRGrabInteractable interactable;

    [SerializeField] private Transform midPointGrabObject, midPointVisualObject, midPointParent;

    [SerializeField] private float bowStringStretchLimit = 0.3f;

    private Transform interactor;
    private float strength, previousStrength;

    [SerializeField] private float stringSoundThreshold = 0.001f;
    [SerializeField] private AudioSource stringPullAudio;

    public UnityEvent OnBowPulled;
    public UnityEvent<float> OnBowReleased;

    // sets the interactor that grabs the bow string
    private void Awake() {
        interactable = midPointGrabObject.GetComponent<XRGrabInteractable>();
    }

    private void Start() {
        // listens for when the hand grips the string
        interactable.selectEntered.AddListener(PrepareBowString);

        // listens when the hands releases the string
        interactable.selectExited.AddListener(ResetBowString);
    }

    // resets the string back to neutral position
    private void ResetBowString(SelectExitEventArgs trigger) {
        OnBowReleased?.Invoke(strength);
        strength = 0;
        previousStrength = 0;
        stringPullAudio.pitch = 1;
        stringPullAudio.Stop();

        interactor = null;
        midPointGrabObject.localPosition = Vector3.zero;
        midPointVisualObject.localPosition = Vector3.zero;
        bowStringRenderer.CreateString(null);
    }

    // references the hand grabbing the bow string and checks when string is being pulled
    private void PrepareBowString(SelectEnterEventArgs trigger) {
        interactor = trigger.interactorObject.transform;
        OnBowPulled?.Invoke();
    }

    private void Update() {
        if (interactor != null) {
            // activate the timer when the user first pulls the string
            GUI.timerOn = true;
            
            // convert bow string mid point position to the local space of the MidPoint
            Vector3 midPointLocalSpace = midPointParent.InverseTransformPoint(midPointGrabObject.position); 

            // get the offset between the string pulled back position and neutral
            float midPointLocalZAbs = Mathf.Abs(midPointLocalSpace.z);
            previousStrength = strength;

            StringPushedToStart(midPointLocalSpace);
            StringPulledToLimit(midPointLocalZAbs, midPointLocalSpace);
            PullingString(midPointLocalZAbs, midPointLocalSpace);

            bowStringRenderer.CreateString(midPointVisualObject.position);
        }
    }

    // stops string when pulled to the bow's limit
    private void StringPulledToLimit(float midPointLocalZAbs, Vector3 midPointLocalSpace) {
        stringPullAudio.Pause();

        // Specify max pulling limit for the string and keep the string to go any farther than "bowStringStretchLimit"
        if (midPointLocalSpace.z < 0 && midPointLocalZAbs >= bowStringStretchLimit) {
            strength = 1;

            //Vector3 direction = midPointParent.TransformDirection(new Vector3(0, 0, midPointLocalSpace.z));
            midPointVisualObject.localPosition = new Vector3(0, 0, -bowStringStretchLimit);
        }
    }

    // stops string from being pushed into the bow, keeps string in neutral position
    private void StringPushedToStart(Vector3 midPointLocalSpace) {
        if (midPointLocalSpace.z >= 0) {
            // reset the string pull audio to the beginning
            stringPullAudio.pitch = 1;
            stringPullAudio.Stop();

            strength = 0;
            midPointVisualObject.localPosition = Vector3.zero;
        }
    }

    // calculates the strength to apply to the arrow depending on how far pulled bak the string is inbetween 0 and the string limit
    private void PullingString(float midPointLocalZAbs, Vector3 midPointLocalSpace) {
        // what happens when string is between between point 0 and the string pull limit
        if (midPointLocalSpace.z < 0 && midPointLocalZAbs < bowStringStretchLimit) {

            // starts playing the string pull sfx once the bow is being pulled
            if (stringPullAudio.isPlaying == false && strength <= 0.01f) {
                stringPullAudio.Play();
            }
            
            strength = calculateStrength(midPointLocalZAbs, 0, bowStringStretchLimit, 0, 1);
            midPointVisualObject.localPosition = new Vector3(0, 0, midPointLocalSpace.z);
            
            PlayStringPullAudio();
        }
    }

    // details how to play the string pull audio when pulling back the string
    private void PlayStringPullAudio() {
        // Check if string moved enough to play audio
        if (Mathf.Abs(strength - previousStrength) > stringSoundThreshold) {
            
            // play string sound in reverse if string is push into the bow
            if (strength < previousStrength) {
                stringPullAudio.pitch = -1;

            // play audio normally otherwise
            } else {
                stringPullAudio.pitch = 1;
            }

            stringPullAudio.UnPause();

        // if string not moving pause the audio
        } else {
            stringPullAudio.Pause();
        }
    }
    
    // calculates the speed the arrow should fly out on release based on string pull back distance
    private float calculateStrength(float bowMidPt, int fromMin, float fromMax, int toMin, int toMax) {
        return (bowMidPt - fromMin) / (fromMax - fromMin) * (toMax - toMin) + toMin;
    }
}
