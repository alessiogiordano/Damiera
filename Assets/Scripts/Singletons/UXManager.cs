using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UXManager : MonoBehaviour
{
    // Welcome Screen
    [SerializeField] private GameObject welcomeRoot;
        private int welcomeLocation = 0;
        [SerializeField] private GameObject welcomePageOne;
        [SerializeField] private GameObject welcomePageTwo;
        [SerializeField] private GameObject welcomePageThree;
        [SerializeField] private Button welcomePageNextButton;
    [SerializeField] private GameObject mainRoot;
        private int mainMode = 0;
        [SerializeField] private Text mainHeader;
        [SerializeField] private Button mainAboutButton;
        [SerializeField] private Button mainPreferencesButton;
        [SerializeField] private Button mainDeleteButton;
        [SerializeField] private Button mainCancelDeleteButton;
        [SerializeField] private Button mainNewButton;
        [SerializeField] private Button mainCancelNewButton;
        [SerializeField] private Button mainSlotOne;
        [SerializeField] private Button mainSlotTwo;
        [SerializeField] private Button mainSlotThree;
        [SerializeField] private Button mainSlotFour;
        [SerializeField] private Text mainHighscore;
        [SerializeField] private GameObject mainHighscoreTooltip;
        [SerializeField] private Text mainHighscoreTooltipText;
    [SerializeField] private GameObject aboutRoot;
        [SerializeField] private Button aboutTutorialButton;
        [SerializeField] private Button aboutCloseButton;
    [SerializeField] private GameObject preferencesRoot;
        [SerializeField] private Button preferencesCancelButton;
        [SerializeField] private Button preferencesOKButton;
        [SerializeField] private Dropdown preferencesRotationDropdown;
        [SerializeField] private Dropdown preferencesScalingDropdown;
        [SerializeField] private Slider preferencesDifficultySlider;
        [SerializeField] private Text preferencesDifficultyLabel;
        [SerializeField] private Toggle preferencesRotationToggle;
        [SerializeField] private Toggle preferencesVoiceoverToggle;
        [SerializeField] private Toggle preferencesSoundtrackToggle;
        [SerializeField] private Slider preferencesSoundSlider;
        [SerializeField] private Slider preferencesMusicSlider;
    [SerializeField] private GameObject newRoot;
        [SerializeField] private Button newCancelButton;
        [SerializeField] private Button newStartButton;
        [SerializeField] private Dropdown newModeDropdown;
        [SerializeField] private InputField newWhiteNameInput;
        [SerializeField] private InputField newBlackNameInput;
        [SerializeField] private Slider newDifficultySlider;
        [SerializeField] private Text newDifficultyLabel;
    [SerializeField] private GameObject pauseRoot;
        [SerializeField] private Button pauseSaveButton;
        [SerializeField] private Button pauseSaveAndQuitButton;
        [SerializeField] private Button pauseOKButton;
        [SerializeField] private Text pauseWhiteName;
        [SerializeField] private Text pauseWhiteStats;
        [SerializeField] private Text pauseBlackName;
        [SerializeField] private Text pauseBlackStats;
        [SerializeField] private Button pauseWhiteWithdrawalButton;
        [SerializeField] private Button pauseDrawButton;
        [SerializeField] private Button pauseBlackWithdrawalButton;
        [SerializeField] private Text pauseNowPlayingTitle;
        [SerializeField] private Text pauseNowPlayingStatus;
        [SerializeField] private Text pauseNowPlayingElapsed;
        [SerializeField] private Text pauseNowPlayingTotal;
        [SerializeField] private Slider pauseNowPlayingSlider;
    [SerializeField] private GameObject statusRoot;
        [SerializeField] private Button statusInfoButton;
        [SerializeField] private Text statusText;
    [SerializeField] private GameObject alertRoot;
        [SerializeField] private Text alertText;


    // Singleton
    private static UXManager _shared;
    public static UXManager Shared {
        get {
            if(_shared==null)
            {
                _shared = GameObject.FindObjectOfType<UXManager>();
                if(_shared==null)
                {
                    Debug.LogError("Attempted to use non instantiated singleton");
                }
            }
            return _shared;
        }
    }
    void Awake()
    {
        if(_shared==null)
        {
            _shared = this;
            GameObject.DontDestroyOnLoad(this.transform.parent.gameObject);
        }
        else
        {
            GameObject.Destroy(this.gameObject);
        }
    }

    void Start()
    {
        // Setup User Interface Buttons
        welcomePageNextButton.onClick.AddListener(WelcomePageNextButton);
        mainAboutButton.onClick.AddListener(MainAboutButton);
        mainPreferencesButton.onClick.AddListener(MainPreferencesButton);
        mainDeleteButton.onClick.AddListener(MainDeleteButton);
        mainCancelDeleteButton.onClick.AddListener(MainCancelDeleteButton);
        mainNewButton.onClick.AddListener(MainNewButton);
        mainCancelNewButton.onClick.AddListener(MainCancelNewButton);
        mainSlotOne.onClick.AddListener(MainSlotOne);
        mainSlotTwo.onClick.AddListener(MainSlotTwo);
        mainSlotThree.onClick.AddListener(MainSlotThree);
        mainSlotFour.onClick.AddListener(MainSlotFour);
        aboutTutorialButton.onClick.AddListener(AboutTutorialButton);
        aboutCloseButton.onClick.AddListener(AboutCloseButton);
        preferencesCancelButton.onClick.AddListener(PreferencesCancelButton);
        preferencesOKButton.onClick.AddListener(PreferencesOKButton);
        newCancelButton.onClick.AddListener(NewCancelButton);
        newStartButton.onClick.AddListener(NewStartButton);
        pauseSaveButton.onClick.AddListener(PauseSaveButton);
        pauseSaveAndQuitButton.onClick.AddListener(PauseSaveAndQuitButton);
        pauseOKButton.onClick.AddListener(PauseOKButton);
        pauseWhiteWithdrawalButton.onClick.AddListener(PauseWhiteWithdrawalButton);
        pauseDrawButton.onClick.AddListener(PauseDrawButton);
        pauseBlackWithdrawalButton.onClick.AddListener(PauseBlackWithdrawalButton);
        statusInfoButton.onClick.AddListener(StatusInfoButton);
        // Debug
        ShowWelcome();
    }
    void HideAll()
    {
        welcomeRoot.SetActive(false);
            welcomePageOne.SetActive(false);
            welcomePageTwo.SetActive(false);
            welcomePageThree.SetActive(false);
        mainRoot.SetActive(false);
        aboutRoot.SetActive(false);
        preferencesRoot.SetActive(false);
        newRoot.SetActive(false);
        pauseRoot.SetActive(false);
        statusRoot.SetActive(false);
        alertRoot.SetActive(false);
    }
    void ShowWelcome()
    {
        welcomeLocation = 1;
        welcomePageThree.SetActive(false);
        welcomePageTwo.SetActive(false);
        welcomePageOne.SetActive(true);
        welcomeRoot.SetActive(true);
    }
        void WelcomePageNextButton()
        {
            switch(welcomeLocation)
            {
                case 1: welcomePageTwo.SetActive(true); welcomePageOne.SetActive(false); welcomeLocation = 2; break;
                case 2: welcomePageThree.SetActive(true); welcomePageTwo.SetActive(false); welcomeLocation = 3; break;
                case 3: welcomePageThree.SetActive(false); ShowMain(); welcomeLocation = 0; break;
                default: ShowWelcome(); break;
            }
        }
    void ShowMain()
    {
        HideAll();
        mainMode = 1;
        mainHeader.text = "Seleziona uno slot per iniziare";
        mainCancelDeleteButton.gameObject.SetActive(false);
        mainCancelNewButton.gameObject.SetActive(false);
        mainRoot.SetActive(true);

    }
        void MainAboutButton() => ShowAboutPanel();

        void MainPreferencesButton() => ShowPreferencesPanel();
        void MainDeleteButton()
        {
            mainMode = 2;
            mainHeader.text = "Seleziona lo slot da cancellare";
            mainCancelDeleteButton.gameObject.SetActive(true);
        }
            void MainCancelDeleteButton() => ShowMain();
        void MainNewButton()
        {
            mainMode = 3;
            mainHeader.text = "Seleziona lo slot da sovrascrivere";
            mainCancelNewButton.gameObject.SetActive(true);
        }
            void MainCancelNewButton() => ShowMain();
        void MainSlot(int index)
        {
            if (mainMode == 1)
            {
                // TODO: Set slot in Persistance Manager
                // TODO: Check if slot is empty
                // Debug -- This line cause a null reference exception
                if (GameManager.Shared != null)
                    GameManager.Shared.NewGame(new HumanPlayer(PlayerColor.White, "Giocatore 1"), new ComputerPlayer(PlayerColor.Black, 2));
                HideAll();
                ShowStatus();
            }
            else if(mainMode == 2)
            {
                // TODO: Delete index
                ShowMain();
            }
            else if(mainMode == 3)
            {
                // Set slot in Persistance Manager
                ShowNew();
            }
        }
            void MainSlotOne() => MainSlot(1);
            void MainSlotTwo() => MainSlot(2);
            void MainSlotThree() => MainSlot(3);
            void MainSlotFour() => MainSlot(4);
    void ShowAboutPanel() => aboutRoot.SetActive(true);
        void AboutTutorialButton()
        {
            HideAll();
            ShowWelcome();
        }
        void AboutCloseButton() => aboutRoot.SetActive(false);
    void ShowPreferencesPanel() => preferencesRoot.SetActive(true);
        void PreferencesCancelButton() => preferencesRoot.SetActive(false);
        void PreferencesOKButton()
        {
            // TODO: Save Preferences
            preferencesRoot.SetActive(false);
        }
    void ShowNew() => newRoot.SetActive(true);
        void NewCancelButton() =>  newRoot.SetActive(false);
        void NewStartButton()
        {
            if (GameManager.Shared != null)
                    GameManager.Shared.NewGame(new HumanPlayer(PlayerColor.White, "Giocatore 1"), new ComputerPlayer(PlayerColor.Black, 2));
            HideAll();
            ShowStatus();
        }
    void ShowStatus() => statusRoot.SetActive(true);
        void StatusInfoButton() => ShowPause();
    void ShowPause()
    {
        if (GameManager.Shared != null)
            GameManager.Shared.PauseGame();
        pauseRoot.SetActive(true);
    }
        void PauseSaveButton() {}
        void PauseSaveAndQuitButton()
        {
            PauseSaveButton();
            // TODO: Close Game
            if (GameManager.Shared != null)
                GameManager.Shared.QuitGame();
            HideAll();
            ShowMain();
        }
        void PauseOKButton()
        {
            if (GameManager.Shared != null)
                GameManager.Shared.ResumeGame();
            pauseRoot.SetActive(false);
        }
        void PauseWhiteWithdrawalButton() => PauseSaveAndQuitButton();
        void PauseDrawButton() => PauseSaveAndQuitButton();
        void PauseBlackWithdrawalButton() => PauseSaveAndQuitButton();
}
