using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class VehicleManger : MonoBehaviour
{
    public enum ViewState
    {
        TPV,
        FPV
    }
    [SerializeField] Button getInButton;
    [SerializeField] Button getOffButton;
    [SerializeField] Transform targetOfVechileForVirtualCamera; //this could be created through code as well.
    [SerializeField] GameObject vechileControls;
    [SerializeField] GameObject firstSit;
    [SerializeField] Transform fpvView;
    [SerializeField] Button swithViewButton;
 
    CinemachineVirtualCamera virtualCamera;
    GameObject localPlayer;
    CharacterController localPlayerCharacterController;
    VehicleController examplePlayerController;
    AnimationHandler handleAnimation;
    bool isPlayerDriving;
    ViewState currentViewState;

    private void Start()
    {
        getInButton?.gameObject.SetActive(false);
        getOffButton?.gameObject.SetActive(false);
        swithViewButton?.gameObject.SetActive(false);
        virtualCamera = GameObject.FindGameObjectWithTag("PlayerFollowCamera").GetComponent<CinemachineVirtualCamera>();
        examplePlayerController = gameObject.GetComponentInParent<VehicleController>();
        currentViewState = ViewState.TPV;

        DisableSteeringControls();
        AssignButton();
        StartCoroutine(TryGetLocalPlayer());
    }

    void AssignButton()
    {
        getInButton.onClick.AddListener(OnGetInButtonClicked);
        getOffButton.onClick.AddListener(OnGetOffButtonClicked);
        swithViewButton.onClick.AddListener(OnSwitchViewButtonClicked);
    }

    IEnumerator TryGetLocalPlayer()
    {
        while (localPlayer == null)
        {
            localPlayer = GameObject.FindGameObjectWithTag("Player");
            yield return new WaitForSeconds(5);
        }
        localPlayerCharacterController = localPlayer.GetComponent<CharacterController>();
        handleAnimation = localPlayer.GetComponentInParent<AnimationHandler>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            getInButton?.gameObject.SetActive(true);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            getInButton?.gameObject.SetActive(false);
        }
    }

    public void OnGetInButtonClicked()
    {
        Debug.Log("Test101: GetInButton Clicked");
        getInButton?.gameObject.SetActive(false);
        getOffButton?.gameObject.SetActive(true);
        if (virtualCamera != null)
        {
            isPlayerDriving = true;
            swithViewButton?.gameObject.SetActive(true);
            EnableSteeringControls();
            StartCoroutine(MovePlayerAlongWithVechile());
            handleAnimation.EnablePlayerDrivingAnimation();
            virtualCamera.Follow = targetOfVechileForVirtualCamera;
        }
       
    }

    public void OnGetOffButtonClicked()
    {
        getOffButton?.gameObject.SetActive(false);
        if (virtualCamera != null)
        {
            isPlayerDriving = false;
            swithViewButton?.gameObject.SetActive(false);
            DisableSteeringControls();
            handleAnimation.EnablePlayerExitDrivingAnimation();
            
            Vector3 newPosition = transform.position;
            newPosition.x += 2f;
            localPlayerCharacterController.enabled = false;
            localPlayer.transform.position = newPosition;
            localPlayerCharacterController.enabled = true;
            localPlayer.SetActive(true);

            virtualCamera.Follow = localPlayer.transform.Find("PlayerCameraRoot");
        }
    }

    IEnumerator MovePlayerAlongWithVechile()
    {
        localPlayerCharacterController.enabled = false;
        while (isPlayerDriving)
        {
            localPlayer.transform.position = firstSit.transform.position;
            localPlayer.transform.rotation = firstSit.transform.rotation;
            yield return null;
        }
        localPlayerCharacterController.enabled = true;
    }

    public void OnSwitchViewButtonClicked()
    {
        currentViewState = (currentViewState == ViewState.TPV) ? ViewState.FPV : ViewState.TPV;
        if (currentViewState == ViewState.TPV)
        {
            swithViewButton.GetComponentInChildren<TextMeshProUGUI>().text = nameof(ViewState.FPV);
            virtualCamera.Follow = targetOfVechileForVirtualCamera;
        }
        else
        {
            swithViewButton.GetComponentInChildren<TextMeshProUGUI>().text = nameof(ViewState.TPV);
            virtualCamera.Follow = fpvView;
        }

    }

    void EnableSteeringControls()
    {
        vechileControls?.gameObject.SetActive(true);
        examplePlayerController.enabled = true;
        
    }

    void DisableSteeringControls()
    {
        vechileControls?.gameObject.SetActive(false);
        examplePlayerController.enabled = false;
    }
}
