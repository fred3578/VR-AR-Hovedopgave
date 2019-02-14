using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;


public class BrickController1 : NetworkBehaviour
{
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
    public Vector3 OldVector3 { get; set; }

    private Transform target;

    public NetworkInstanceId hitID { get; set; }

    [SyncVar] public NetworkInstanceId targetNetId;
    public string tekst { get; set; }

   [SyncVar] public Vector3 TargetVector3;
   [SyncVar] public GameObject GreyBrick;

    public List<GameObject> BlackBricks;


    void Start()
    {
        if (!isLocalPlayer)
        {
          return;
        }

        // variablen bruges til at indeholde de oprindelige informationer om et objekt.
        originalRotationValue = gameObject.transform;
        // Vi testede om vi havde brug for at finde objekter med tags før vi kunne bruge dem.
        TestTagBlackPiece = GameObject.FindWithTag("BrickBlackTag");
        TestTagWhitePiece = GameObject.FindWithTag("BrickWhiteTag");
        // Tællende variabler sat til 0.
        TestBrickBlackCount = 0;
        TestBrickWhiteCount = 0;
        //TempVector3 = Vector3.zero;
    }

    /*
    void Update()
    {
        //Tjek om skærmen røres. Hvis ikke går den ind og slutter Update for dette frame. Ellers bruger den raycast.
        if (Input.touchCount != 1)
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
        }
        
    }
    */

    public void GetBricksByID(int MoveID, Vector3 tempVector3)
    {
        GameObject PassingBrick;
        foreach (GameObject goBrickBlack in GameObject.FindGameObjectsWithTag("BrickBlackTag"))
        {
            BlackBricks.Add(goBrickBlack);
        }
        foreach (GameObject blackBrick in BlackBricks)
        {
            if (blackBrick.gameObject.GetComponent<BrickID>().ID == MoveID)
            {
                PassingBrick = blackBrick;
                if (PassingBrick.gameObject.transform.position != tempVector3)
                {
                    MoveBrickByID(PassingBrick, tempVector3);
                }
                
            }
            else
            {
                 break;
            }
        }  
    }

    public void MoveBrickByID(GameObject brick, Vector3 tempVector3)
    {
        brick.transform.position = new Vector3(tempVector3.x, tempVector3.y, 0);
        TargetVector3 = tempVector3;

    }



    //Metode for at bevæge brikker enkeltvis. Modtager og bruger hitInfo.
    public void MoveBrick(RaycastHit hitInfo)
    {
        GreyBrick = GameObject.Find("GreyBrick");
        //GreyPos = GreyBrick.transform.position;

        GameObject hitGameObject = hitInfo.collider.gameObject;

        
        //NetworkInstanceId id = hitGameObject.GetComponent<NetworkInstanceId>();
        //Variabel oprettes til at styre position for hitInfo's indeholdte transforms position.
        var newBrickPos = hitInfo.transform.position;
        newBrickPos.x = hitInfo.point.x;
        newBrickPos.z = hitInfo.point.z;

        hitInfo.transform.position = newBrickPos;

        //Nulstiller brikkens velocity inden den får ny position, så den ikke fortsætter efter placering.
        hitInfo.transform.GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);


        NetworkInstanceId hitID = hitGameObject.GetComponent<NetworkIdentity>().netId;

        int targetID = hitGameObject.GetComponent<BrickID>().ID;


        
        // Brikkens orientering nulstilles via oprettet rotationsvariabler.       
        var newBrickRot = hitInfo.transform;
        newBrickRot.rotation = Quaternion.Slerp(transform.rotation, originalRotationValue.rotation, Time.time * rotationResetSpeed);

        //Ændring af position på brikken.
        //hitInfo.transform.position = newBrickPos;

        //Variablen skal videregive positionen fra hitInfo til at bruge en OverlapSphere på


        TargetVector3 = newBrickPos;
        GetBricksByID(targetID, newBrickPos);
        
        //CmdSpawnOverlapSphere(tempVector3);

        //MoveTo(TestTagWhitePiece, newBrickPos);
        //MoveTo(hitGameObject, newBrickPos, id);
    }


    [Command]
    void CmdSpawnOverlapSphere(Vector3 tempVector3)
    {
        // GameObject targetObj = NetworkServer.FindLocalObject(goID);
        //targetObj.transform.position = new Vector3(tempVector3.x, tempVector3.y, 0);

        // RpcMoveToTarget(tempVector3, goID);



        RpcSpawnOverlapSphere(tempVector3); 
    }

    
    [ClientRpc]
    public void RpcSpawnOverlapSphere(Vector3 tempVector3)
    {
      Collider[] colliders;
        if ((colliders = Physics.OverlapSphere(tempVector3, 0.1f /* Radius */)).Length > 1)
    {
        foreach (var collider1 in colliders)
        {
            var go = collider1.gameObject;
            if (go == gameObject) continue;
            {   
                int goID = go.gameObject.GetComponent<BrickID>().ID;

                go.gameObject.transform.position = new Vector3(tempVector3.x, tempVector3.y, 0);

                GetBricksByID(goID, tempVector3);
                tekst = goID.ToString();
            }
        }
    }  
    }
    /*
    [Server]
    public void MoveTo(GameObject hitinfo, Vector3 newPosition)
    {
        hitinfo.transform.position = newPosition;
        RpcMoveTo(newPosition);
    }
    */
    

    //[ClientRpc]
    void MoveToTarget(Vector3 newPosition, NetworkInstanceId goID)
    {
        GameObject targetObj = ClientScene.FindLocalObject(goID);
        targetObj.transform.position = new Vector3(newPosition.x, newPosition.y, 0);

        /*
        GameObject hitinfo = GameObject.FindGameObjectWithTag("BrickBlackTag");

        if (hitinfo.gameObject.GetComponentInChildren<NetworkIdentity>().netId == goID)
        {
            hitinfo.transform.position = Vector3.MoveTowards(hitinfo.transform.position, newPosition, Speed);
        }
         */



        /*
        if (hitinfo.name == "brickBlack(clone)")
        {
            hitinfo.tag = "BrickBlackTag";
        }

        if (hitinfo.name == "brickWith(clone)")
        {
            hitinfo.tag = "BrickWhiteTag";
        }
        */

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
            //other.gameObject.GetComponent<Rigidbody>().useGravity = true;
            
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
        GUI.Label(new Rect(200, 300, 500, 200), "Tæller Sort: " + TestBrickBlackCount);
        GUI.Label(new Rect(400, 400, 500, 200), "NetID: " + tekst);
    }
}
