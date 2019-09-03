using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using System;

public class ARTapToPlaceObject : MonoBehaviour
{
    public GameObject objectToPlace;
    public GameObject placementIndicator;
    public ParticleSystem Confetti;
    public Camera ARCamera;

    private ARRaycastManager ARRaycast;
    private Pose placementPose;

    private bool placementPoseIsValid = false;
    private bool chickenPlaced;
    public List<GameObject> chickens;
    public List<GameObject> foods;

    private Vector3 chickenLoc;
    private Vector3 indicatorLoc;

    GameObject spawnedChicken;
    public GameObject spawnedFood;
    public GameObject food;

    void Start()
    {
        ARRaycast = FindObjectOfType<ARRaycastManager>();
    }


    void Update()
    {
        UpdatePlacementPose();
        UpdatePlacementIndicator();
        
        if (placementPoseIsValid && Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began && chickens.Count == 0)
        {
            PlaceObject();
        }

        if (placementPoseIsValid && Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began && chickens.Count == 1)
        {
            PlaceFood();
        }

        if (placementPoseIsValid && Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved)
        {
            PopConfetti();
        } 
        
        if(foods.Count > 0)
        {
            spawnedChicken.transform.position = spawnedFood.transform.position;
        }
    }

    private void PlaceObject()
    {
        spawnedChicken = Instantiate(objectToPlace, placementPose.position, placementPose.rotation);
        chickens.Add(spawnedChicken);
    }

    private void PlaceFood()
    {
        spawnedFood = Instantiate(food, placementPose.position, placementPose.rotation);
        foods.Add(spawnedFood);
    }

    private void PopConfetti()
    {
        Instantiate(Confetti, placementPose.position, placementPose.rotation);
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
