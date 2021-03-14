using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;
using TMPro;

namespace SpeedTutorMainMenuSystem
{
    public class MenuController : MonoBehaviour
    {
        public UDPClient UDPclient;
        private UDPClient client;
        Dictionary<string, Action<string[]>> opcodesPtr;
        private bool connected = false;
        private bool sent = false;
        private string secondPlayer = null;
        private int room;
        private bool isHost = false;

        #region Default Values
        [Header("Default Menu Values")]
        [SerializeField] private float defaultBrightness;
        [SerializeField] private float defaultVolume;
        [SerializeField] private int defaultSen;
        [SerializeField] private bool defaultInvertY;
        [SerializeField] public Text Top;
        [SerializeField] public Text NamePlayerResponse;

        [Header("Levels To Load")]
        public string _newGameButtonLevel;
        private string levelToLoad;

        private int menuNumber;
        #endregion

        #region Menu Dialogs
        [Header("Main Menu Components")]
        [SerializeField] private GameObject InputField;
        [SerializeField] private GameObject menuDefaultCanvas;
        [SerializeField] private GameObject GeneralSettingsCanvas;
        [SerializeField] private GameObject graphicsMenu;
        [SerializeField] private GameObject soundMenu;
        [SerializeField] private GameObject gameplayMenu;
        [SerializeField] private GameObject controlsMenu;
        [SerializeField] private GameObject confirmationMenu;
        [Space(10)]
        [Header("Menu Popout Dialogs")]
        [SerializeField] private GameObject noSaveDialog;
        [SerializeField] private GameObject newGameDialog;
        [SerializeField] private GameObject loadGameDialog;
        [SerializeField] private GameObject receivedInvitation;
        [SerializeField] private GameObject answerToInvitation;
        #endregion

        #region Slider Linking
        [Header("Menu Sliders")]
        [SerializeField] private Text controllerSenText;
        [SerializeField] private Slider controllerSenSlider;
        public float controlSenFloat = 2f;
        [Space(10)]
        [SerializeField] private Brightness brightnessEffect;
        [SerializeField] private Slider brightnessSlider;
        [SerializeField] private Text brightnessText;
        [Space(10)]
        [SerializeField] private Text volumeText;
        [SerializeField] private Slider volumeSlider;
        [Space(10)]
        [SerializeField] private Toggle invertYToggle;
        #endregion

        #region Initialisation - Button Selection & Menu Order
        private void Start()
        {
            menuNumber = 1;
            client = Instantiate(UDPclient);
            initializeOpcodes();
        }
        #endregion

        //MAIN SECTION
        public IEnumerator ConfirmationBox()
        {
            confirmationMenu.SetActive(true);
            yield return new WaitForSeconds(2);
            confirmationMenu.SetActive(false);
        }

        private void Update()
        {
            if (!connected && !sent)
            {
                string query = "S_LOGIN:";
                query += client.nickName + ':' + client.pass;
                client.SendData(query);
                sent = true;
            }

            string toExecute = this.client.ReceiveData();
            if (toExecute != null)
            {
                string[] isValidCommand = toExecute.Split(':');

                if (opcodesPtr.ContainsKey(isValidCommand[0]))
                {
                    opcodesPtr[isValidCommand[0]](isValidCommand);
                }
                toExecute = null;
            }

            if (this.connected)
            {
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    if (menuNumber == 2 || menuNumber == 7 || menuNumber == 8)
                    {
                        GoBackToMainMenu();
                        ClickSound();
                    }

                    else if (menuNumber == 3 || menuNumber == 4 || menuNumber == 5)
                    {
                        GoBackToOptionsMenu();
                        ClickSound();
                    }

                    else if (menuNumber == 6) //CONTROLS MENU
                    {
                        GoBackToGameplayMenu();
                        ClickSound();
                    }
                }

                if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
                {
                    SendInvite();
                    ClickSound();
                }
            }
        }

        private void SendInvite()
        {
            if (room > 0)
                Debug.Log("already in party");
            string guestToInvite = InputField.GetComponent<TMP_InputField>().text;
            string query = "S_SENDROOM_INVITATION:";
            query += client.nickName + ':' + guestToInvite;
            client.SendData(query);
        }

        private void login(string[] chainList)
        {
            this.connected = true;
        }

        private void responseInvitation(string[] chainList)
        {
            string guest = chainList[1];
            string answer = chainList[2];
            isHost = true;

            if (answer == "undefined")
            {
                Debug.Log("Player not online");
                return;
            }

            answer = (answer == "true") ? "Accepted" : "Declined";

            NamePlayerResponse.text = answer + " by " + guest;
            answerToInvitation.SetActive(true);
        }

        private void receiveInvitation(string[] chainList)
        {
            string host = chainList[1];
            string room = chainList[2];
            Top.text = chainList[1] + " for room " + chainList[2];

            if (chainList[1] != "undefined")
            {
                secondPlayer = chainList[1];
                room = chainList[2];
                receivedInvitation.SetActive(true);
            } 
            else
            {
                Debug.Log("Player not online");
            }
        }

        private void defineRoom(string[] chainList)
        {
            room = Int32.Parse(chainList[1]);
            Debug.Log(room);
        }

        private void initializeOpcodes()
        {
            opcodesPtr = new Dictionary<string, Action<string[]>>();
            opcodesPtr["C_LOGIN"] = login;
            opcodesPtr["C_ACCEPT_INVITATION"] = responseInvitation;
            opcodesPtr["C_SENDROOM_INVITATION"] = receiveInvitation;
            opcodesPtr["C_DEFINE_ROOM_HOST"] = defineRoom;
        }

        private void ClickSound()
        {
            GetComponent<AudioSource>().Play();
        }

        #region Menu Mouse Clicks
        public void MouseClick(string buttonType)
        {
            if (this.connected)
            {
                if (buttonType == "Controls")
                {
                    gameplayMenu.SetActive(false);
                    controlsMenu.SetActive(true);
                    menuNumber = 6;
                }

                if (buttonType == "Graphics")
                {
                    GeneralSettingsCanvas.SetActive(false);
                    graphicsMenu.SetActive(true);
                    menuNumber = 3;
                }

                if (buttonType == "Sound")
                {
                    GeneralSettingsCanvas.SetActive(false);
                    soundMenu.SetActive(true);
                    menuNumber = 4;
                }

                if (buttonType == "Gameplay")
                {
                    GeneralSettingsCanvas.SetActive(false);
                    gameplayMenu.SetActive(true);
                    menuNumber = 5;
                }

                if (buttonType == "Options")
                {
                    menuDefaultCanvas.SetActive(false);
                    GeneralSettingsCanvas.SetActive(true);
                    menuNumber = 2;
                }

                if (buttonType == "LoadGame")
                {
                    menuDefaultCanvas.SetActive(false);
                    loadGameDialog.SetActive(true);
                    menuNumber = 8;
                }

                if (buttonType == "NewGame")
                {
                    menuDefaultCanvas.SetActive(false);
                    newGameDialog.SetActive(true);
                    menuNumber = 7;
                }
            }

            if (buttonType == "Exit")
            {
                Debug.Log("YES QUIT!");
                Application.Quit();
            }
        }
        #endregion

        public void VolumeSlider(float volume)
        {
            AudioListener.volume = volume;
            volumeText.text = volume.ToString("0.0");
        }

        public void VolumeApply()
        {
            PlayerPrefs.SetFloat("masterVolume", AudioListener.volume);
            Debug.Log(PlayerPrefs.GetFloat("masterVolume"));
            StartCoroutine(ConfirmationBox());
        }

        public void BrightnessSlider(float brightness)
        {
            brightnessEffect.brightness = brightness;
            brightnessText.text = brightness.ToString("0.0");
        }

        public void BrightnessApply()
        {
            PlayerPrefs.SetFloat("masterBrightness", brightnessEffect.brightness);
            Debug.Log(PlayerPrefs.GetFloat("masterBrightness"));
            StartCoroutine(ConfirmationBox());
        }

        public void ControllerSen()
        {
            controllerSenText.text = controllerSenSlider.value.ToString("0");
            controlSenFloat = controllerSenSlider.value;
        }

        public void GameplayApply()
        {
            if (invertYToggle.isOn) //Invert Y ON
            {
                PlayerPrefs.SetInt("masterInvertY", 1);
                Debug.Log("Invert" + " " + PlayerPrefs.GetInt("masterInvertY"));
            }

            else if (!invertYToggle.isOn) //Invert Y OFF
            {
                PlayerPrefs.SetInt("masterInvertY", 0);
                Debug.Log(PlayerPrefs.GetInt("masterInvertY"));
            }

            PlayerPrefs.SetFloat("masterSen", controlSenFloat);
            Debug.Log("Sensitivity" + " " + PlayerPrefs.GetFloat("masterSen"));

            StartCoroutine(ConfirmationBox());
        }

        #region ResetButton
        public void ResetButton(string GraphicsMenu)
        {
            if (GraphicsMenu == "Brightness")
            {
                brightnessEffect.brightness = defaultBrightness;
                brightnessSlider.value = defaultBrightness;
                brightnessText.text = defaultBrightness.ToString("0.0");
                BrightnessApply();
            }

            if (GraphicsMenu == "Audio")
            {
                AudioListener.volume = defaultVolume;
                volumeSlider.value = defaultVolume;
                volumeText.text = defaultVolume.ToString("0.0");
                VolumeApply();
            }

            if (GraphicsMenu == "Graphics")
            {
                controllerSenText.text = defaultSen.ToString("0");
                controllerSenSlider.value = defaultSen;
                controlSenFloat = defaultSen;

                invertYToggle.isOn = false;

                GameplayApply();
            }
        }
        #endregion

        #region Dialog Options - This is where we load what has been saved in player prefs!
        public void ClickNewGameDialog(string ButtonType)
        {
            if (ButtonType == "Yes")
            {
                SceneManager.LoadScene(_newGameButtonLevel);
            }

            if (ButtonType == "No")
            {
                GoBackToMainMenu();
            }
        }

        public void ClickDismissAnswer(string ButtonType)
        {
            if (ButtonType == "Ok")
            {
                answerToInvitation.SetActive(false);
            }
        }

        public void ClickChoiceInvite(string ButtonType)
        {
            if (ButtonType == "Yes")
            {
                string query = "S_JOINROOM:" + secondPlayer + ':' + client.nickName + ":true";
                client.SendData(query);
                receivedInvitation.SetActive(false);
                // Hide PLAY button because not host of the room
            }

            if (ButtonType == "No")
            {
                receivedInvitation.SetActive(false);
                string query = "S_JOINROOM:" + secondPlayer + ':' + client.nickName + ":false";
                client.SendData(query);
                secondPlayer = null;
                room = 0;
            }
        }

        public void ClickLoadGameDialog(string ButtonType)
        {
            if (ButtonType == "Yes")
            {
                if (PlayerPrefs.HasKey("SavedLevel"))
                {
                    Debug.Log("I WANT TO LOAD THE SAVED GAME");
                    //LOAD LAST SAVED SCENE
                    levelToLoad = PlayerPrefs.GetString("SavedLevel");
                    SceneManager.LoadScene(levelToLoad);
                }

                else
                {
                    Debug.Log("Load Game Dialog");
                    menuDefaultCanvas.SetActive(false);
                    loadGameDialog.SetActive(false);
                    noSaveDialog.SetActive(true);
                }
            }

            if (ButtonType == "No")
            {
                GoBackToMainMenu();
            }
        }
        #endregion

        #region Back to Menus
        public void GoBackToOptionsMenu()
        {
            GeneralSettingsCanvas.SetActive(true);
            graphicsMenu.SetActive(false);
            soundMenu.SetActive(false);
            gameplayMenu.SetActive(false);

            GameplayApply();
            BrightnessApply();
            VolumeApply();

            menuNumber = 2;
        }

        public void GoBackToMainMenu()
        {
            menuDefaultCanvas.SetActive(true);
            newGameDialog.SetActive(false);
            loadGameDialog.SetActive(false);
            noSaveDialog.SetActive(false);
            GeneralSettingsCanvas.SetActive(false);
            graphicsMenu.SetActive(false);
            soundMenu.SetActive(false);
            gameplayMenu.SetActive(false);
            menuNumber = 1;
        }

        public void GoBackToGameplayMenu()
        {
            controlsMenu.SetActive(false);
            gameplayMenu.SetActive(true);
            menuNumber = 5;
        }

        public void ClickQuitOptions()
        {
            GoBackToMainMenu();
        }

        public void ClickNoSaveDialog()
        {
            GoBackToMainMenu();
        }
        #endregion
    }
}
