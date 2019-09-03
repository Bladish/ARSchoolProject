using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR.ARCore;
using GoogleARCore;
using System;

public class JonathansTapToPlaceChicken : MonoBehaviour
{
    public GameObject objectToPlace;
    public GameObject placementIndicator;
    public Camera ARCamera;

    private ARRaycastManager ARRaycast;
    private Pose placementPose;

    private bool placementPoseIsValid = false;
    
    public Anchor anchor;
    public GameObject anchorPrefab;
    public GameObject unAnchoredPrefab;

    Vector3 lastAnchoredPosition;
    Quaternion lastAnchorRotation;
    Pose anchorPose;

    void Start() {
        ARRaycast = FindObjectOfType<ARRaycastManager>();

    }
    
    void Update() {
        if (Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Began)
        {
           anchor = Session.CreateAnchor(anchorPose);
            GameObject.Instantiate(unAnchoredPrefab, 
                anchor.transform.position,
                anchor.transform.rotation,
                anchor.transform);
            GameObject.Instantiate(unAnchoredPrefab, 
                anchor.transform.position,
                anchor.transform.rotation);
            lastAnchoredPosition = anchor.transform.position;
            lastAnchorRotation = anchor.transform.rotation;
        }

        if (anchor == null)
        {

            if (anchor.transform.position != lastAnchoredPosition)
            {
                Debug.Log(Vector3.Distance(anchor.transform.position, lastAnchoredPosition));
                lastAnchoredPosition = anchor.transform.position;
            }
            if (anchor.transform.rotation != lastAnchorRotation)
            {
                Debug.Log(Quaternion.Angle(anchor.transform.rotation, lastAnchorRotation));
                lastAnchorRotation = anchor.transform.rotation;
            }
        }
        UpdatePlacementPose();
        UpdatePlacementIndicator();

        if (placementPoseIsValid && Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended)
        {
            PlacePose();
        }

    }

    private void PlacePose() {
        anchorPose = new Pose(placementPose.position, placementPose.rotation);
    }


    private void UpdatePlacementIndicator() {
        if (placementPoseIsValid)
        {
            placementIndicator.SetActive(true);
            placementIndicator.transform.SetPositionAndRotation(placementPose.position, placementPose.rotation);
        }
        else
        {
            placementIndicator.SetActive(false);
        }
    }

    private void UpdatePlacementPose() {
        var screenCenter = Camera.main.ViewportToScreenPoint(new Vector3(0.5f, 0.5f));
        List<ARRaycastHit> hits = new List<ARRaycastHit>();
        // Fick ändra från UnityEngine.experimental.XR.trackableType till UnityEngine.XR.ARSubsystems.TrackableType

        ARRaycast.Raycast(screenCenter, hits, TrackableType.Planes);
        
        placementPoseIsValid = hits.Count > 0;
        if (placementPoseIsValid)
        {
            placementPose = hits[0].pose;

            var cameraForward = Camera.current.transform.forward;
            var cameraBearing = new Vector3(cameraForward.x, 0, cameraForward.z).normalized;
            placementPose.rotation = Quaternion.LookRotation(cameraBearing);
        }
    }

}
