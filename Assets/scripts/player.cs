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
    private Rigidbody rb;
    public Transform camera;
    private UDPClient client;
    private bool ready = false;
    private bool sent = false;
    Dictionary<string, Action<string[]>> opcodesPtr;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        Cursor.visible = false;
        client = SpeedTutorMainMenuSystem.MenuController.FindObjectOfType<UDPClient>().GetComponent<UDPClient>();
        initializeOpcodes();
    }

    // Update is called once per frame
    void Update()
    {
        if (!ready && !sent)
        {
            string query = "S_READY:" + client.nickName;
            client.SendData(query);
            sent = true;
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

        if (ready)
        {
            float v = Input.GetAxis("Vertical");

            rb.AddForce(transform.forward * v * speed);
            rb.MoveRotation(camera.rotation);
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

    private void initializeOpcodes()
    {
        opcodesPtr = new Dictionary<string, Action<string[]>>();
        opcodesPtr["C_COUNTDOWN_START"] = countDown;
        opcodesPtr["C_START"] = startRace;
    }
}
