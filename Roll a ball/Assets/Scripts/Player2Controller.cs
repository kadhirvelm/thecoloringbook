using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SocketIO;

public class Player2Controller : MonoBehaviour
{
    public float speed;

    private Rigidbody player2;
    private SocketIOComponent socket;

    void Start()
    {
        player2 = GetComponent<Rigidbody>();

        GameObject go = GameObject.Find("SocketIO");
        socket = go.GetComponent<SocketIOComponent>();

        socket.On("movement_2", MovePlayer2);
    }

    // Update is called once per frame
    void MovePlayer2(SocketIOEvent e)
    {
        var data = e.data;
        Vector3 vector = new Vector3(float.Parse(data["x"].str), 0.0f, float.Parse(data["z"].str));
        player2.AddForce(vector * speed);
    }
}
