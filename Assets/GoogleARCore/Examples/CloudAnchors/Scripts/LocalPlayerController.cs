//-----------------------------------------------------------------------
// <copyright file="LocalPlayerController.cs" company="Google">
//
// Copyright 2018 Google Inc. All Rights Reserved.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;

namespace GoogleARCore.Examples.CloudAnchors
{
    using UnityEngine;
    using UnityEngine.Networking;

    /// <summary>
    /// Local player controller. Handles the spawning of the networked Game Objects.
    /// </summary>
    public class LocalPlayerController : NetworkBehaviour
    {
        /// <summary>
        /// Brik-modellerne der vil repræsentere netværkede objekter i scenen.
        /// </summary>
        public GameObject BrickBrick;
        public GameObject WhiteBrick;

        /// <summary>      
        /// Bræt-model, der vil repræsentere det første anchor i scenen.
        /// </summary>
        public GameObject AnchorPrefab;

        public static int BrickID = 1;
        


        /// <summary>
        /// The Unity OnStartLocalPlayer() method.
        /// </summary>
        public override void OnStartLocalPlayer()
        {
            base.OnStartLocalPlayer();

            // A Name is provided to the Game Object so it can be found by other Scripts, since this is instantiated as
            // a prefab in the scene.
            gameObject.name = "LocalPlayer";
        }

        /// <summary>
        /// Will spawn the origin anchor and host the Cloud Anchor. Must be called by the host.
        /// </summary>
        /// <param name="position">Position of the object to be instantiated.</param>
        /// <param name="rotation">Rotation of the object to be instantiated.</param>
        /// <param name="anchor">The ARCore Anchor to be hosted.</param>
        public void SpawnAnchor(Vector3 position, Quaternion rotation, Component anchor)
        {
            
                // Instantiate Anchor model at the hit pose.
                var anchorObject = Instantiate(AnchorPrefab, position, rotation);

            anchorObject.transform.localScale = new Vector3(1.75f, 1.75f, 1.75f);

            // Anchor must be hosted in the device.
            anchorObject.GetComponent<AnchorController>().HostLastPlacedAnchor(anchor);

                // Host can spawn directly without using a Command because the server is running in this instance.
                NetworkServer.Spawn(anchorObject);

                //Brættets tællervariabel går op.  
        }


        /// <summary>
        /// A command run on the server that will spawn the Star prefab in all clients.
        /// </summary>
        /// <param name="position">Position of the object to be instantiated.</param>
        /// <param name="rotation">Rotation of the object to be instantiated.</param>
        [Command]
        public void CmdSpawnBlackBrick(Vector3 position, Quaternion rotation, string BrickID)
        {

            // Instantiate Star model at the hit pose.
            var brickBlack = Instantiate(BrickBrick, position, rotation);
            //brickBlack.transform.localScale = new Vector3(0.05f, 0.0075f, 0.05f);
            
            BrickBrick.name = "brickBlack1";

            brickBlack.gameObject.GetComponent<BrickID>().ID = LocalPlayerController.BrickID;
            

            //brickBlack.gameObject.GetComponent<BrickID>().BrickPos = position;
            LocalPlayerController.BrickID++;
            
            

            // Spawn the object in all clients.
           
            NetworkServer.Spawn(brickBlack);
           
           
        }

        [Command]
        public void CmdSpawnWhiteBrick(Vector3 position, Quaternion rotation)
        {
            var brickWhite = Instantiate(WhiteBrick, position, rotation);
            //brickWhite.transform.localScale = new Vector3(0.05f, 0.0075f, 0.05f);
           
                NetworkServer.Spawn(brickWhite);
           


        }

    }
}
