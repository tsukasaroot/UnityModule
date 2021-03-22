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
    public GameObject showCountdown;
    public Text countdownText;
    private UDPClient client;
    private bool ready = false;
    private bool sent = false;
    Dictionary<string, Action<string[]>> opcodesPtr;

    private Vector3 m_vOriginalPosition;
    private Quaternion m_qOriginalRotation;
    private Vector3 m_vLastCheckPointPosition;

    private void Awake()
    {
        m_vOriginalPosition = transform.position;
        m_qOriginalRotation = transform.rotation;
        m_vLastCheckPointPosition = m_vOriginalPosition;
    }

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

            query = "S_MOVEMENT:" + client.nickName + ':';
            query += transform.position.x.ToString() + ':' + transform.position.y.ToString() + ':' + transform.position.z.ToString();
            client.SendData(query);
            query = null;
            if (showCountdown.activeSelf)
                showCountdown.SetActive(false);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag == "DeathZone")
        {
            transform.position = m_vLastCheckPointPosition;
            transform.rotation = m_qOriginalRotation;
            rb.velocity = Vector3.zero;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "CheckPoint")
        {
            m_vLastCheckPointPosition = other.gameObject.transform.position;
        }
    }

    private void countDown(string[] chainList)
    {
        if (chainList[1] == "3")
            showCountdown.SetActive(true);
        countdownText.text = chainList[1];
    }

    private void startRace(string[] chainList)
    {
        // When countDown is at 1, next packet server send is a C_START, so both player will have movements unlocked
        ready = true;
        countdownText.text = "GO!";
    }

    private void manageSecondPlayerMovement(string[] chainList)
    {
        // Here we move the second player

    }

    private void initializeOpcodes()
    {
        opcodesPtr = new Dictionary<string, Action<string[]>>();
        opcodesPtr["C_COUNTDOWN_START"] = countDown;
        opcodesPtr["C_START"] = startRace;
        opcodesPtr["C_PLAYER_MOVEMENT"] = manageSecondPlayerMovement;
    }
}
