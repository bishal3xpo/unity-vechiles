using System.Collections;
using System.Collections.Generic;
using StarterAssets;
using UnityEngine;

public class AnimationHandler : MonoBehaviour
{
    Animator playerAnimator;
    bool isDriving;
    bool exitDriving;

    void Start()
    {
        playerAnimator = transform.GetComponent<Animator>();
    }

    public void EnablePlayerDrivingAnimation()
    {
        //exitDriving = false;
        //isDriving = true;
        //HandleDrivingAnimation();

        playerAnimator.Play("DrivingState");
        transform.GetComponent<ThirdPersonController>().enabled = false;
    }

    public void EnablePlayerExitDrivingAnimation()
    {
        //isDriving = false;
        //exitDriving = true ;
        //HandleDrivingAnimation();
        playerAnimator.Play("Idle Walk Run Blend");
        transform.GetComponent<ThirdPersonController>().enabled = true;
    }

    void HandleDrivingAnimation()
    {
        playerAnimator?.SetBool(nameof(isDriving), isDriving);
        playerAnimator?.SetBool(nameof(exitDriving), exitDriving);
    }


}
