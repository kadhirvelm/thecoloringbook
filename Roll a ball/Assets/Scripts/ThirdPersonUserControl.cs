using System;
using UnityEngine;
using System.Collections.Generic;
using UnityStandardAssets.CrossPlatformInput;
using SocketIO;

namespace UnityStandardAssets.Characters.ThirdPerson
{
    [RequireComponent(typeof (ThirdPersonCharacter))]
    public class ThirdPersonUserControl : MonoBehaviour
    {
        public string playerNumber;
        public bool playerControlled;

        private ThirdPersonCharacter m_Character; // A reference to the ThirdPersonCharacter on the object
        private Transform m_Cam;                  // A reference to the main camera in the scenes transform
        private Vector3 m_CamForward;             // The current forward direction of the camera
        private Vector3 m_Move;
        private bool m_Jump;                      // the world-relative desired move direction, calculated from the camForward and user input.

        private SocketIOComponent socket;

        
        private void Start()
        {
            // get the transform of the main camera
            if (Camera.main != null)
            {
                m_Cam = Camera.main.transform;
            }
            else
            {
                Debug.LogWarning(
                    "Warning: no main camera found. Third person character needs a Camera tagged \"MainCamera\", for camera-relative controls.", gameObject);
                // we use self-relative controls in this case, which probably isn't what the user wants, but hey, we warned them!
            }

            // get the third person character ( this should never be null due to require component )
            m_Character = GetComponent<ThirdPersonCharacter>();

            GameObject go = GameObject.Find("SocketIO");
            socket = go.GetComponent<SocketIOComponent>();

            socket.On("movement", HandleMovement);
        }

        private void HandleMovement(SocketIOEvent e)
        {
            if (e.data["player_number"].str == playerNumber && !playerControlled)
            {
                Debug.Log(e.data + " MOVE ME");
                Debug.Log(new Vector3(float.Parse(e.data["m_Move_x"].str), float.Parse(e.data["m_Move_y"].str), float.Parse(e.data["m_Move_z"].str)) + " " + bool.Parse(e.data["crouch"].str) + " " + bool.Parse(e.data["m_Jump"].str));
                m_Character.Move(new Vector3(float.Parse(e.data["m_Move_x"].str), float.Parse(e.data["m_Move_y"].str), float.Parse(e.data["m_Move_z"].str)), bool.Parse(e.data["crouch"].str), bool.Parse(e.data["m_Jump"].str));
            }
        }


        private void Update()
        {
            if (!m_Jump)
            {
                m_Jump = CrossPlatformInputManager.GetButtonDown("Jump");
            }
        }


        // Fixed update is called in sync with physics
        private void FixedUpdate()
        {
            // read inputs
            if (playerControlled)
            {
                float h = CrossPlatformInputManager.GetAxis("Horizontal");
                float v = CrossPlatformInputManager.GetAxis("Vertical");
                bool crouch = Input.GetKey(KeyCode.C);

                // calculate move direction to pass to character
                if (m_Cam != null)
                {
                    // calculate camera relative direction to move:
                    m_CamForward = Vector3.Scale(m_Cam.forward, new Vector3(1, 0, 1)).normalized;
                    m_Move = v * m_CamForward + h * m_Cam.right;
                }
                else
                {
                    // we use world-relative directions in the case of no main camera
                    m_Move = v * Vector3.forward + h * Vector3.right;
                }
    #if !MOBILE_INPUT
                // walk speed multiplier
                if (Input.GetKey(KeyCode.LeftShift)) m_Move *= 0.5f;
    #endif

                // pass all parameters to the character control script
                m_Character.Move(m_Move, crouch, m_Jump);
                m_Jump = false;

                Dictionary<string, string> move = new Dictionary<string, string>();
                move["m_Move_x"] = m_Move.x.ToString();
                move["m_Move_y"] = m_Move.y.ToString();
                move["m_Move_z"] = m_Move.z.ToString();
                move["crouch"] = crouch.ToString();
                move["m_Jump"] = m_Jump.ToString();
                move["player_number"] = playerNumber;
                socket.Emit("movement", new JSONObject(move));
            }
        }
    }
}
