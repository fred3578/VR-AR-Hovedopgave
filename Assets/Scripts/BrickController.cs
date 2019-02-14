using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrickController : MonoBehaviour
{
    public Camera PlayerCamera;
    public GameObject TestTagBlackPiece;
    public GameObject TestTagWhitePiece;
    //Tællende variabler brugt til at se om kode i if-sætning blev brugt.
    public int TestBrickBlackCount { get; set; }
    public int TestBrickWhiteCount { get; set; }
    
    // Rotationsvariabler.
    public Transform originalRotationValue;
    float rotationResetSpeed = 1.0f;

    // Frederik prøver kollisioner af 1/2
    public float Speed;          
    public float Acceleration;
    public Vector3 Velocity = Vector3.zero;
    
    void Start()
    {  
        // variablen bruges til at indeholde de oprindelige informationer om et objekt.
        originalRotationValue = gameObject.transform;
        // Vi testede om vi havde brug for at finde objekter med tags før vi kunne bruge dem.
        TestTagBlackPiece = GameObject.FindWithTag("BrickBlackTag");
        TestTagWhitePiece = GameObject.FindWithTag("BrickWhiteTag");
        // Tællende variabler sat til 0.
        TestBrickBlackCount = 0;
        TestBrickWhiteCount = 0;
    }


    void Update()
    {
        //Tjek om skærmen røres. Hvis ikke går den ind og slutter Update for dette frame. Ellers bruger den raycast.
        if(Input.touchCount != 1)
            return;

        /*for (var i = 0; i < Input.touchCount; i++)
        {
            if (Input.GetTouch(i).phase == TouchPhase.Began)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(i).position);
                var hitInfo = new RaycastHit();
                if (Physics.Raycast(ray, out hitInfo))
                {
                    if (hitInfo.transform.tag == "BrickBlackTag" || hitInfo.transform.tag == "BrickWhiteTag")
                    {
                        MoveBrick(hitInfo);
                    }
                       
                }
            }
        }*/
        var ray = Camera.main.ScreenPointToRay(Input.touches[0].position);
        var hitInfo = new RaycastHit();
        if (Physics.Raycast(ray, out hitInfo))
        {
            // Tjek om raycast rammer en brik.
           if (hitInfo.transform.tag == "BrickBlackTag" || hitInfo.transform.tag == "BrickWhiteTag")
            {
                MoveBrick(hitInfo);
            }
        }
    }

    //Metode for at bevæge brikker enkeltvis. Modtager og bruger hitInfo.
    public void MoveBrick(RaycastHit hitInfo)
    {       
        //Variabel oprettes til at styre position for hitInfo's indeholdte transforms position.
            var newBrickPos = hitInfo.transform.position;
            newBrickPos.x = hitInfo.point.x;
            newBrickPos.z = hitInfo.point.z;
          
        //Nulstiller brikkens velocity inden den får ny position, så den ikke fortsætter efter placering.
            hitInfo.transform.GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);

        // Brikkens orientering nulstilles via oprettet rotationsvariabler.       
            var newBrickRot = hitInfo.transform;
            newBrickRot.rotation = Quaternion.Slerp(transform.rotation, originalRotationValue.rotation, Time.time * rotationResetSpeed);

        //Ændring af position på brikken.
            hitInfo.transform.position = newBrickPos; 
    }

    //Kollisionsmetode 
    void OnCollisionEnter(Collision collision)
    {
        var other = collision.gameObject;

        // Stopper brikkerne efter tid, når de har kollideret med hinanden.
        if (other.tag == "BrickBlackTag" || other.tag == "BrickWhiteTag")
        {
            transform.position = Vector3.SmoothDamp(transform.position, originalRotationValue.position, ref Velocity, Time.time * rotationResetSpeed);
            transform.rotation = Quaternion.Slerp(transform.rotation, originalRotationValue.rotation, Time.time * rotationResetSpeed);
        }
        /*
        if (other.tag == "BoundsBelow")
        {
            if (gameObject.tag == "BrickBlackTag" || transform.name == "BrickBlack")
            {
                Destroy(gameObject);
            }
            if (gameObject.tag == "BrickWhiteTag" || transform.name == "BrickWhite")
            {
                Destroy(gameObject);
            }
        }
        */
    }
   
    //Tæller-funktion, der viser antal af frames, hvor metoden er kørt med ændring på de to tællende variabler.
    public void OnGUI()
    {
        GUI.Label(new Rect(200,300,500,200), "Tæller Sort: " + TestBrickBlackCount);
        GUI.Label(new Rect(400, 400, 500, 200), "Tæller Hvid: " + TestBrickWhiteCount);
    }

    
}
