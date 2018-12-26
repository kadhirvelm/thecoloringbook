using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SocketIO;

public class PlayerController : MonoBehaviour {
    private SocketIOComponent socket;

    private void Start()
    {
        GameObject go = GameObject.Find("SocketIO");
        socket = go.GetComponent<SocketIOComponent>();
    }

    private void FixedUpdate()
    {
        float movementHorizontal = Input.GetAxis("Horizontal");
        float movementVertical = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(movementHorizontal, 0.0f, movementVertical);

        Dictionary<string, string> move = new Dictionary<string, string>();
        move["x"] = movement.x.ToString();
        move["z"] = movement.z.ToString();
        socket.Emit("movement", new JSONObject(move));
    }
}
