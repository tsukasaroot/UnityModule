using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;
using TMPro;

public class player : MonoBehaviour
{
    public float speed;
    public Transform camera;
    public Rigidbody player_body;
    private Rigidbody player_body_rb;
    private Transform player_body_transform;
    private UDPClient client;
    private bool ready = false;
    private bool sent = false;
    Dictionary<string, Action<string[]>> opcodesPtr;

    private Vector3 m_vOriginalPosition;
    private Quaternion m_qOriginalRotation;

    private void Awake()
    {
        m_vOriginalPosition = transform.position;
        m_qOriginalRotation = transform.rotation;
    }

    // Start is called before the first frame update
    void Start()
    {
        player_body_rb = player_body.GetComponent<Rigidbody>();
        player_body_transform = player_body.GetComponent<Transform>();
        Cursor.visible = false;
        client = SpeedTutorMainMenuSystem.MenuController.FindObjectOfType<UDPClient>().GetComponent<UDPClient>();
        initializeOpcodes();
    }

    // Update is called once per frame
    void Update()
    {
        string query;
        if (!ready && !sent)
        {
            query = "S_READY:" + client.nickName;
            client.SendData(query);
            sent = true;
            query = null;
        }

        string toExecute = client.ReceiveData();
        if (toExecute != null)
        {
            string[] isValidCommand = toExecute.Split(':');

            Debug.Log(isValidCommand[0]);
            if (opcodesPtr.ContainsKey(isValidCommand[0]))
            {
                opcodesPtr[isValidCommand[0]](isValidCommand);
            }
            toExecute = null;
        }

        query = "S_MOVEMENT:" + client.nickName + ':';
        query += transform.position.x.ToString() + ':' + transform.position.y.ToString() + ':' + transform.position.z.ToString();
        client.SendData(query);
        query = null;

        if (ready)
        {
            float v = Input.GetAxis("Vertical");

            player_body.AddForce(transform.forward * v * speed);
            player_body.MoveRotation(camera.rotation);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag == "DeathZone")
        {
            transform.position = m_vOriginalPosition;
            transform.rotation = m_qOriginalRotation;
            player_body_rb.velocity = Vector3.zero;
        }
    }

    private void countDown(string[] chainList)
    {
        // display countDown on middle of screen
        Debug.Log("Countdown started...");
    }

    private void startRace(string[] chainList)
    {
        // When countDown is at 1, next packet server send is a C_START, so both player will have movements unlocked
    }

    private void manageSecondPlayerMovement(string[] chainList)
    {

    }

    private void initializeOpcodes()
    {
        opcodesPtr = new Dictionary<string, Action<string[]>>();
        opcodesPtr["C_COUNTDOWN_START"] = countDown;
        opcodesPtr["C_START"] = startRace;
        opcodesPtr["C_PLAYER_MOVEMENT"] = manageSecondPlayerMovement;
    }
}
