using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.UI;

public class PlayerDetector : MonoBehaviour
{
    [SerializeField] Button getInButton;
    [SerializeField] Button getOffButton;
    [SerializeField] Transform targetOfVechileForVirtualCamera; //this could be created through code as well.
    [SerializeField] GameObject vechileControls;

    CinemachineVirtualCamera virtualCamera;
    GameObject localPlayer;
    Transform localPlayerEntryPoint;
    CharacterController localPlayerCharacterController;
    ExamplePlayerController examplePlayerController;
    HandleAnimation handleAnimation;


    private void Start()
    {
        getInButton?.gameObject.SetActive(false);
        getOffButton?.gameObject.SetActive(false);
        virtualCamera = GameObject.FindGameObjectWithTag("PlayerFollowCamera").GetComponent<CinemachineVirtualCamera>();
        examplePlayerController = gameObject.GetComponentInParent<ExamplePlayerController>();
        DisableSteeringControls();
        AssignButton();
        StartCoroutine(TryGetLocalPlayer());
    }

    void AssignButton()
    {
        getInButton.onClick.AddListener(OnGetInButtonClicked);
        getOffButton.onClick.AddListener(OnGetOffButtonClicked);
    }

    IEnumerator TryGetLocalPlayer()
    {
        while (localPlayer == null)
        {
            localPlayer = GameObject.FindGameObjectWithTag("Player");
            yield return new WaitForSeconds(5);
        }
        localPlayerCharacterController = localPlayer.GetComponent<CharacterController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            localPlayerEntryPoint = other.transform;
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
            EnableSteeringControls();
            localPlayer.SetActive(false);
            Debug.Log("Test101: Virtual Camera is not Null");
            virtualCamera.Follow = targetOfVechileForVirtualCamera;
        }
       
    }

    public void OnGetOffButtonClicked()
    {
        getOffButton?.gameObject.SetActive(false);
        if (virtualCamera != null)
        {
            DisableSteeringControls();

            float direction = (localPlayer.transform.position.x > transform.position.x) ? 1f : -1f;
            Vector3 newPosition = localPlayer.transform.position;
            newPosition.x += 2f * direction;
            localPlayerCharacterController.enabled = false;
            localPlayer.transform.position = newPosition;
            localPlayerCharacterController.enabled = true;
            localPlayer.SetActive(true);

            virtualCamera.Follow = localPlayer.transform.Find("PlayerCameraRoot");
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
