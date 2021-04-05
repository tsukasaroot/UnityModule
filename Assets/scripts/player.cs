using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;
using TMPro;

public class player : MonoBehaviour
{
    /*
     *  Public variables for player management
     */
    public float speed;
    public Transform camera;
    public Rigidbody player_body;
    public float m_fAcceleratorSpeed;
    public GameObject otherPlayer;
    public GameObject showTimer;
    public Text writeTimer;
    public GameObject menu;

    /*
     *  Public variables for network timer
     */
    public GameObject showCountdown;
    public Text countdownText;
    public float interpolationPeriod;

    /*
     * Private variables for network management
     */
    private UDPClient client;
    private Rigidbody player_body_rb;
    private Transform player_body_transform;
    private bool ready = false;
    private bool sent = false;
    private bool end = false;
    Dictionary<string, Action<string[]>> opcodesPtr;
    private float time = 0.0f;
    private Scene scene;

    /*
     *  Respawn variables.
     */
    private Vector3 m_vOriginalPosition;
    private Vector3 m_vOriginalCameraPosition;
    private Quaternion m_qOriginalRotation;
    private Quaternion m_qOriginalCameraRotation;
    private Vector3 m_vLastCheckPointPosition;

    /*
     * Status variables
     */
    private bool bIsOnAccelerator = false;
    private short trophyNumber = 0;

    /*
     *  Music and sound effect variables
     */
    public AudioSource m_backgroundMusic;
    public AudioSource m_trophySoundEffect;

    private void Awake()
    {
        m_vOriginalPosition = transform.position;
        m_vOriginalCameraPosition = camera.position;
        m_qOriginalRotation = transform.rotation;
        m_qOriginalCameraRotation = camera.rotation;
        m_vLastCheckPointPosition = m_vOriginalPosition;
    }

    // Start is called before the first frame update
    void Start()
    {
        player_body_rb = player_body.GetComponent<Rigidbody>();
        player_body_transform = player_body.GetComponent<Transform>();
        Cursor.visible = false;
        client = SpeedTutorMainMenuSystem.MenuController.FindObjectOfType<UDPClient>().GetComponent<UDPClient>();
        initializeOpcodes();
        scene = SceneManager.GetActiveScene();
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

        if (ready && Input.GetKeyDown(KeyCode.Escape))
        {
            menu.SetActive(true);
            Cursor.visible = true;
        }

        if (ready && !end)
        {
            StartMusic();
            if (Physics.Raycast(player_body_transform.position, Vector3.down, 0.6f)) // isGrounded
            {
                float fVerticalForce = Input.GetAxis("Vertical") * (bIsOnAccelerator ? m_fAcceleratorSpeed : speed);
                Vector3 vMouvementVector = transform.rotation * new Vector3(0.0f, 0.0f, fVerticalForce);

                player_body.AddForce(vMouvementVector);
                player_body.MoveRotation(camera.rotation);

            }
            if (showCountdown.activeSelf)
                showCountdown.SetActive(false);
        }

        time += Time.deltaTime;

        if (time >= interpolationPeriod)
        {
            time = time - interpolationPeriod;

            query = "S_MOVEMENT:" + client.nickName + ':';
            query += transform.position.x.ToString() + ':' + transform.position.y.ToString() + ':' + transform.position.z.ToString();
            client.SendData(query);
            query = null;
        }
    }

    private void OnDestroy()
    {
        Cursor.visible = true;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag == "DeathZone" || collision.collider.name.Contains("TunnelEnding"))
        {
            transform.position = m_vLastCheckPointPosition;
            transform.rotation = m_qOriginalRotation;
            camera.position = m_vOriginalCameraPosition;
            camera.rotation = m_qOriginalCameraRotation;
            player_body_rb.velocity = Vector3.zero;
        }
        else if (collision.collider.tag == "END" && !end)
        {
            string query;
            query = "S_RACE_END:" + client.nickName + ':' + scene.name;
            client.SendData(query);
            end = true;
        }
        else if (collision.collider.tag == "Retarder")
        {
            player_body_rb.velocity /= 4.0f;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "CheckPoint")
        {
            m_vLastCheckPointPosition = other.gameObject.transform.position;
        }
        else if (other.tag == "Accelerator")
        {
            bIsOnAccelerator = true;
        }
        else if (other.tag == "Trophy")
        {
            if (!m_trophySoundEffect.isPlaying)
            {
                m_trophySoundEffect.Play();
            }
            other.gameObject.SetActive(false);
            trophyNumber++;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Accelerator")
        {
            bIsOnAccelerator = false;
        }
    }

    #region Network related functions
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
        sent = false;
        countdownText.text = "GO!";
    }

    private void manageSecondPlayerMovement(string[] chainList)
    {
        float x = float.Parse(chainList[1]);
        float y = float.Parse(chainList[2]);
        float z = float.Parse(chainList[3]);

        otherPlayer.transform.position = new Vector3(x, y, z);
    }

    private void getTimer(string[] chainList)
    {
        float time = float.Parse(chainList[1]);
        if (time > 60)
            time = time / 60;
        showTimer.SetActive(true);
        writeTimer.text += time.ToString();
    }

    private void leave(string[] chainList)
    {
        SceneManager.LoadScene(chainList[1]);
    }

    private void initializeOpcodes()
    {
        opcodesPtr = new Dictionary<string, Action<string[]>>();
        opcodesPtr["C_COUNTDOWN_START"] = countDown;
        opcodesPtr["C_START"] = startRace;
        opcodesPtr["C_PLAYER_MOVEMENT"] = manageSecondPlayerMovement;
        opcodesPtr["C_TIME"] = getTimer;
        opcodesPtr["C_LEAVE"] = leave;
    }
    #endregion

    private void StartMusic()
    {
        if (!m_backgroundMusic.isPlaying)
        {
            m_backgroundMusic.Play();
        }
    }

    #region Menu
    public void ClickLeaveRace(string ButtonType)
    {
        if (ButtonType == "No")
        {
            menu.SetActive(false);
            Cursor.visible = false;
        }
        if (ButtonType == "Yes")
        {
            string query;
            query = "S_LEAVE_RACE:" + client.nickName + ':' + "MainMenu";
            client.SendData(query);
        }
    }
    #endregion
}
