using System.Collections;
using System.Collections.Generic;
using StarterAssets;
using UnityEngine;

public class AnimationHandler : MonoBehaviour
{
    Animator playerAnimator;

    [SerializeField] string nameOfDrivingState;
    [SerializeField] string nameOfIdleState;

    void Start()
    {
        playerAnimator = transform.GetComponent<Animator>();
    }

    public void EnablePlayerDrivingAnimation()
    {
        playerAnimator.Play(nameOfDrivingState); 
        transform.GetComponent<ThirdPersonController>().enabled = false;
    }

    public void EnablePlayerExitDrivingAnimation()
    {
        playerAnimator.Play(nameOfIdleState); 
        transform.GetComponent<ThirdPersonController>().enabled = true;
    }

}
