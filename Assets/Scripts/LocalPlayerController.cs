using System.Collections;
using System.Collections.Generic;
using GoogleARCore;
using UnityEngine;
using UnityEngine.Networking;

namespace Assets.Scripts
{
    public class LocalPlayerController : NetworkBehaviour
    {
       
        public Transform Board;
        
        // GameObject objekter bindes til prefabs for ønskede instantierede objekter/modeller.
        public GameObject BrickWhite1;
        public GameObject BrickBlack1;

        public string Testcheck { get; set; }


        // Tællende variabler. Styrer antal af gange metoder kan køres.
        public int BoardResultat { get; set; }
        public int BrickBlackResultat { get; set; }
        public int BrickWhiteResultat { get; set; }

        void Start()
        {
            Testcheck = "Localplayer er statet";
            OnGUI();

            Instantiate(BrickBlack1, transform.position, transform.rotation);
           

        }

        public override void OnStartLocalPlayer()
        {
            base.OnStartLocalPlayer();

            // A Name is provided to the Game Object so it can be found by other Scripts, since this is instantiated as
            // a prefab in the scene.
            gameObject.name = "LocalPlayer";
        }



        public void Hostboard()
        {
           
        }

        public void SpawnAnchor(Vector3 position, Quaternion rotation, Component anchor)
        {
            // Instantiate Anchor model at the hit pose.
            var anchorObject = Instantiate(Board, position, rotation);

            //Ændrer skala på det placerede objekt.
            anchorObject.transform.localScale = new Vector3(1.75f, 1.75f, 1.75f);

            //Sætter det placerede objekts transforms parent til at være ankerets transform.
            anchorObject.transform.parent = anchor.transform;

            //Brættets tællervariabel går op.
            BoardResultat += 1;

            // Anchor must be hosted in the device.
            anchorObject.GetComponent<AnchorController>().HostLastPlacedAnchor(anchor);

            // Host can spawn directly without using a Command because the server is running in this instance.
            NetworkServer.Spawn(anchorObject.gameObject);
        }

        [Command]
        public void CmdSpawnBrick(Vector3 position, Quaternion rotation)
        {
            if (GameObject.FindGameObjectsWithTag("BoardOthello").Length > 1 && BrickBlackResultat != 64)
            {
                var brickBlackObject = Instantiate(BrickBlack1, position, rotation);
                brickBlackObject.transform.localScale = new Vector3(0.05f, 0.0075f, 0.05f);

                NetworkServer.Spawn(brickBlackObject);

                BrickBlackResultat += 1;
            }
            if (GameObject.FindGameObjectsWithTag("BrickBlackTag").Length >= 64 && BrickBlackResultat >= 64 && BrickWhiteResultat != 64)
            {

                var brickWhiteObject = Instantiate(BrickWhite1, position, rotation);
                brickWhiteObject.transform.localScale = new Vector3(0.05f, 0.0075f, 0.05f);

                NetworkServer.Spawn(brickWhiteObject);

                BrickWhiteResultat += 1;
            }
            
        }

        /*
        [Command]
        public void CmdSpawnBrickWhite(Vector3 position, Quaternion rotation)
        {
            var brickWhiteObject = Instantiate(BrickWhite1, position, rotation);
            brickWhiteObject.transform.localScale = new Vector3(0.05f, 0.0075f, 0.05f);

            NetworkServer.Spawn(brickWhiteObject);

            BrickWhiteResultat += 1;
        }
        */

        public void OnGUI()
        {
            GUI.Label(new Rect(10, 10, 100, 20), " " + Testcheck);
        }

        // Update is called once per frame
            void Update()
        {
            Touch touch;
            if (Input.touchCount < 1 || (touch = Input.GetTouch(0)).phase != TouchPhase.Began)
            {
                return;
            }

            Hostboard();

            
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
                    NetworkServer.Spawn(placedObject.gameObject);
                }
            }
        }
    }
}
