using GoogleARCore;
using System;
using System.Collections;
using System.Collections.Generic;
using Boo.Lang;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InstantiateObjectOnTouch : MonoBehaviour
{
    public Camera FirstPersonCamera;
    //public GameObject Board;
    
    // GameObject objekter bindes til prefabs for ønskede instantierede objekter/modeller.
    public GameObject BrickWhite1;
    public GameObject BrickBlack1;

    public Transform Board;

    // Tællende variabler. Styrer antal af gange metoder kan køres.
    public int BoardResultat { get; set; }
    public int BrickBlackResultat { get; set; }
    public int BrickWhiteResultat { get; set; }


    void Awake()
    {
        // Test af manuel fund af prefab via søgning på tag. Kan køre uden, hvis bundet i Unity editor.
        BrickWhite1 = GameObject.FindGameObjectWithTag("BrickWhiteTag");
        BrickBlack1 = GameObject.FindGameObjectWithTag("BrickBlackTag");
        
    }

    void Start()
    {
        
    }
    void Update()
    {
      
        Touch touch;
        if (Input.touchCount < 1 || (touch = Input.GetTouch(0)).phase != TouchPhase.Began)
        {
            return;
        }

        TrackableHit hit;
        var raycastFilter = TrackableHitFlags.PlaneWithinBounds | TrackableHitFlags.PlaneWithinPolygon;

        // Tjek om objekt med spillebrættets tag er blevet oprettet.
        if (GameObject.FindGameObjectsWithTag("BoardOthello").Length > 0 && BoardResultat != 1)
        {
            if (Frame.Raycast(touch.position.x, touch.position.y, raycastFilter, out hit) && Board != null)
            {
                //Laver ankeret som variabel
                var anchor = hit.Trackable.CreateAnchor(hit.Pose);

                //Instantierer objekt ud fra det ønskede prefab, position og rotation.
                var placedObject = Instantiate(Board, hit.Pose.position, hit.Pose.rotation);
                
                //Ændrer skala på det placerede objekt.
                placedObject.transform.localScale = new Vector3(1.75f, 1.75f, 1.75f);

                //Sætter det placerede objekts transforms parent til at være ankerets transform.
                placedObject.transform.parent = anchor.transform;

                //Brættets tællervariabel går op.
                BoardResultat += 1;
            }
        }

        /// <summary>
        /// Opretter sorte brikker.
        /// Tjekker om brættet er oprettet og om koden er kørt mindre end 64 gange via tællervariabel.
        /// </summary>
        if (GameObject.FindGameObjectsWithTag("BoardOthello").Length > 1 && BrickBlackResultat != 2)
        {
            if (Frame.Raycast(touch.position.x, touch.position.y, raycastFilter, out hit) && BrickBlack1 != null)
            {
                var anchor = hit.Trackable.CreateAnchor(hit.Pose);

                var placedObject = Instantiate(BrickBlack1, hit.Pose.position, Quaternion.identity);
                placedObject.transform.localScale = new Vector3(0.05f, 0.0075f, 0.05f);

                placedObject.transform.parent = anchor.transform;

                BrickBlackResultat += 1;
                OnGUI();
            }

        }

        /// <summary>
        /// Opretter hvide brikker.
        /// Tjekker om 64 sorte brikker findes og om koden er kørt mindre end 64 gange via tællervariabel.
        /// </summary>
        if (GameObject.FindGameObjectsWithTag("BrickBlackTag").Length >= 2 && BrickWhiteResultat != 2)
        {
            if (Frame.Raycast(touch.position.x, touch.position.y, raycastFilter, out hit) && BrickBlack1 != null)
            {
                var anchor = hit.Trackable.CreateAnchor(hit.Pose);

                var placedObject = Instantiate(BrickWhite1, hit.Pose.position, Quaternion.identity);
                placedObject.transform.localScale = new Vector3(0.05f, 0.0075f, 0.05f);

                placedObject.transform.parent = anchor.transform;

                BrickWhiteResultat += 1;
                OnGUI();
            }

        }

    }

 //Load et label til at tælle hvor mange brikker der bliver lavet.
    public void OnGUI()
    {
        GUI.Label(new Rect(10, 10, 100, 20), "Antal sort: " + BrickBlackResultat);
        GUI.Label(new Rect(20, 20, 400, 40), "Antal hvide: " + BrickWhiteResultat);
        GUI.Label(new Rect(40, 40, 800, 80), "Antal Board: " + BoardResultat);
    }

    // Forsøg med kollision, der skal destruere brikker og nedtælle på baggrund af deres farve, når de rammer en hitbox.
    void OnCollisionEnter(Collision collision)
    {
        var other = collision.gameObject;
        var brickCollider = collision.collider;
        if (other.tag == "BoundsBelowTag")
        {
            if (brickCollider.tag == "BrickBlackTag" || brickCollider.transform.name == "BrickBlack")
            {
                BrickBlackResultat -= 1;
                Destroy(gameObject);
            }

            if (brickCollider.tag == "BrickWhiteTag" || brickCollider.transform.name == "BrickWhite")
            {
                BrickWhiteResultat -= 1;
                Destroy(gameObject);
            }
        }
    }
}        