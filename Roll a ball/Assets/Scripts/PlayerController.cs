using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SocketIO;

public class PlayerController : MonoBehaviour {
    public float speed;
    public Text scoreText;
    public Text winText;

    private Rigidbody player;
    private int score = 0;

    private SocketIOComponent socket;

    private void Start()
    {
        player = GetComponent<Rigidbody>();
        SetScoreText();
        winText.text = "";

        GameObject go = GameObject.Find("SocketIO");
        socket = go.GetComponent<SocketIOComponent>();
    }

    private void FixedUpdate()
    {
        float movementHorizontal = Input.GetAxis("Horizontal");
        float movementVertical = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(movementHorizontal, 0.0f, movementVertical);

        player.AddForce(movement * speed);

        Dictionary<string, string> move = new Dictionary<string, string>();
        move["vector"] = movement.ToString();
        socket.Emit("movement", new JSONObject(move));
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Pick Up"))
        {
            other.gameObject.SetActive(false);
            score += 1;
            SetScoreText();
        }
    }

    void SetScoreText()
    {
        scoreText.text = "Score: " + score.ToString();
        if (score >= 12)
        {
            winText.text = "You win!";
        }
    }
}
