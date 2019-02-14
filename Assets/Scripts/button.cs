using System.Collections;
using System.Collections.Generic;
using GoogleARCore;
using UnityEngine;
using UnityEngine.EventSystems;

public class button : MonoBehaviour
{
    public Camera FirstPersonCamera;
    public GameObject PlaceGameObject;

    void Update()
    {
        
        
        
        
        /*
         Touch touch;
         if (Input.touchCount < 1 || (touch = Input.GetTouch(0)).phase != TouchPhase.Began)
         {
             return;
         }

         TrackableHit hit;
         var raycastFilter = TrackableHitFlags.PlaneWithinBounds | TrackableHitFlags.PlaneWithinPolygon;

         if (Frame.Raycast(touch.position.x, touch.position.y, raycastFilter, out hit) && PlaceGameObject != null)
         {

             var anchor = hit.Trackable.CreateAnchor(hit.Pose);

             var placedObject = Instantiate(PlaceGameObject, hit.Pose.position, hit.Pose.rotation);

             placedObject.GetComponent<MeshRenderer>().material.color = Color.black;

             placedObject.transform.parent = anchor.transform;
         }
     }

     public void PlaceBlackBrick(Touch touch)
     { 
     TrackableHit hit;
     var raycastFilter = TrackableHitFlags.PlaneWithinBounds | TrackableHitFlags.PlaneWithinPolygon;

         if (Frame.Raycast(touch.position.x, touch.position.y, raycastFilter, out hit) && PlaceGameObject != null)
         {

             var anchor = hit.Trackable.CreateAnchor(hit.Pose);

             var placedObject = Instantiate(PlaceGameObject, hit.Pose.position, hit.Pose.rotation);

             placedObject.GetComponent<MeshRenderer>().material.color = Color.black;

             placedObject.transform.parent = anchor.transform;
         }
         */
    }

    public void BlackBrickClick()
    {
        Touch touch;
        if (Input.touchCount < 1 || (touch = Input.GetTouch(0)).phase != TouchPhase.Began)
        {
            return;
        }

        TrackableHit hit;
        var raycastFilter = TrackableHitFlags.PlaneWithinBounds | TrackableHitFlags.PlaneWithinPolygon;

        if (Frame.Raycast(touch.position.x, touch.position.y, raycastFilter, out hit) && PlaceGameObject != null)
        {

            var anchor = hit.Trackable.CreateAnchor(hit.Pose);

            var placedObject = Instantiate(PlaceGameObject, hit.Pose.position, hit.Pose.rotation);

            placedObject.GetComponent<MeshRenderer>().material.color = Color.black;

            placedObject.transform.parent = anchor.transform;
        }
    }

}