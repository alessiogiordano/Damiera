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
        private int selectedSlot = 0;
        [SerializeField] private Text mainHeader;
        [SerializeField] private Button mainAboutButton;
        [SerializeField] private Button mainPreferencesButton;
        [SerializeField] private Button mainDeleteButton;
        [SerializeField] private Button mainCancelDeleteButton;
        [SerializeField] private Button mainNewButton;
        [SerializeField] private Button mainCancelNewButton;
        [SerializeField] private Texture emptySlotTexture;
        [SerializeField] private Texture filledSlotTexture;
        [SerializeField] private Button mainSlotOne;
        [SerializeField] private RawImage mainSlotOneIcon;
        [SerializeField] private Text mainSlotOneText;
        [SerializeField] private Button mainSlotTwo;
        [SerializeField] private RawImage mainSlotTwoIcon;
        [SerializeField] private Text mainSlotTwoText;
        [SerializeField] private Button mainSlotThree;
        [SerializeField] private RawImage mainSlotThreeIcon;
        [SerializeField] private Text mainSlotThreeText;
        [SerializeField] private Button mainSlotFour;
        [SerializeField] private RawImage mainSlotFourIcon;
        [SerializeField] private Text mainSlotFourText;
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
        [SerializeField] private Text pauseLastSaved;
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
        RenderAllSlots();
        RenderScores();
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
                if (PersistanceManager.SlotContainsData(index) && GameManager.Shared != null)
                {
                    RenderGameView();
                    GameManager.Shared.LoadGame(index);
                }
                else ShowNew(index);
            }
            else if(mainMode == 2)
            {
                PersistanceManager.DeleteSlot(index);
                ShowMain();
            }
            else if(mainMode == 3) ShowNew(index);
        }
        public void AbortGameLoad()
        {
            ShowMain();
        }
            void RenderSlot(int index, RawImage imageview, Text textview)
            {
                if(PersistanceManager.SlotContainsData(index))
                {
                    var slot = PersistanceManager.GetSlot(index);
                    string whitePlayer = (slot.Item4) ? slot.Item2 : "Computer";
                    string blackPlayer = (slot.Item5) ? slot.Item3 : "Computer";
                    string status = (slot.Item9) ? ((slot.Item10 == PlayerColor.White) ? "(Vince il bianco)" : (slot.Item10 == PlayerColor.Black) ? "(Vince il nero)" : "(Patta)") : ((slot.Item10 == PlayerColor.White) ? "(Muove il bianco)" : (slot.Item10 == PlayerColor.Black) ? "(Muove il nero)" : "");
                    textview.text = $"<color=white><b>{whitePlayer} - {blackPlayer} {status}</b></color>\nSlot {index} - {slot.Item1}";
                    imageview.texture = filledSlotTexture;
                }
                else
                {
                    textview.text = $"<color=white><b>Clicca per creare una partita nello Slot {index}</b></color>\nSlot {index} - Vuoto";
                    imageview.texture = emptySlotTexture;
                }
            }
            void RenderSlotOne() => RenderSlot(1, mainSlotOneIcon, mainSlotOneText);
            void RenderSlotTwo() => RenderSlot(2, mainSlotTwoIcon, mainSlotTwoText);
            void RenderSlotThree() => RenderSlot(3, mainSlotThreeIcon, mainSlotThreeText);
            void RenderSlotFour() => RenderSlot(4, mainSlotFourIcon, mainSlotFourText);
            void RenderAllSlots() { RenderSlotOne(); RenderSlotTwo(); RenderSlotThree(); RenderSlotFour(); }
            void MainSlotOne() => MainSlot(1);
            void MainSlotTwo() => MainSlot(2);
            void MainSlotThree() => MainSlot(3);
            void MainSlotFour() => MainSlot(4);
            void RenderTopScore() => mainHighscore.text = PersistanceManager.record;
            void RenderScoreboard() => mainHighscoreTooltipText.text = string.Join("\n", PersistanceManager.scoreboard);
            void RenderScores() { RenderTopScore(); RenderScoreboard(); }
    void ShowAboutPanel() => aboutRoot.SetActive(true);
        void AboutTutorialButton()
        {
            HideAll();
            ShowWelcome();
        }
        void AboutCloseButton() => aboutRoot.SetActive(false);
    void ShowPreferencesPanel()
    {
        preferencesRoot.SetActive(true);
        RenderPreferences();
    }
        void PreferencesCancelButton() => preferencesRoot.SetActive(false);
        void PreferencesOKButton()
        {
            SavePreferences();
            preferencesRoot.SetActive(false);
        }
        void RenderPreferences()
        {
            preferencesRotationDropdown.value = (int) PersistanceManager.viewportRotation;
            preferencesScalingDropdown.value = (int) PersistanceManager.hdpiScaling;
            int defaultComputerDifficulty = (int) PersistanceManager.computerDifficulty;
                preferencesDifficultySlider.value = defaultComputerDifficulty;
                preferencesDifficultyLabel.text = $"Il computer pensa {defaultComputerDifficulty} {(defaultComputerDifficulty != 1 ? "mosse" : "mossa")} in anticipo";
            preferencesRotationToggle.isOn = PersistanceManager.autoRotate;
            preferencesVoiceoverToggle.isOn = PersistanceManager.voiceover;
            preferencesSoundtrackToggle.isOn = PersistanceManager.soundtrack;
            preferencesSoundSlider.value = PersistanceManager.soundVolume;
            preferencesMusicSlider.value = PersistanceManager.musicVolume;
        }
        void SavePreferences()
        {
            PersistanceManager.viewportRotation = (ViewportRotation) preferencesRotationDropdown.value;
            PersistanceManager.hdpiScaling = (HdpiScaling) preferencesScalingDropdown.value;
            PersistanceManager.computerDifficulty = (ComputerDifficulty) preferencesDifficultySlider.value;
            PersistanceManager.autoRotate = preferencesRotationToggle.isOn;
            PersistanceManager.voiceover = preferencesVoiceoverToggle.isOn;
            PersistanceManager.soundtrack = preferencesSoundtrackToggle.isOn;
            PersistanceManager.soundVolume = preferencesSoundSlider.value;
            PersistanceManager.musicVolume = preferencesMusicSlider.value;
        }
    void ShowNew(int index)
    {
        selectedSlot = index;
        newRoot.SetActive(true);
    }
        void NewCancelButton() =>  newRoot.SetActive(false);
        void NewStartButton()
        {
            if (GameManager.Shared != null)
            {
                bool isWhiteHuman = newModeDropdown.value == 0 || newModeDropdown.value == 1;
                bool isBlackHuman = newModeDropdown.value == 0 || newModeDropdown.value == 2;
                string whiteName = newWhiteNameInput.text.Length > 0 ? newWhiteNameInput.text : "Giocatore";
                string blackName = newBlackNameInput.text.Length > 0 ? newBlackNameInput.text : "Giocatore";
                Player whitePlayer = (isWhiteHuman) ? (Player) new HumanPlayer(PlayerColor.White, whiteName) : (Player) new ComputerPlayer(PlayerColor.White, (int) newDifficultySlider.value);
                Player blackPlayer = (isBlackHuman) ? (Player) new HumanPlayer(PlayerColor.Black, blackName) : (Player) new ComputerPlayer(PlayerColor.Black, (int) newDifficultySlider.value);
                RenderGameView();
                GameManager.Shared.NewGame(selectedSlot, whitePlayer, blackPlayer);
            } 
        }
        void RenderGameView()
        {
            HideAll();
            ShowStatus();
        }
    void ShowStatus(string initialText = "")
    {
        statusText.text = initialText;
        statusRoot.SetActive(true);
    }
        public void UpdateStatus(string statusText) => ShowStatus(statusText);
        void StatusInfoButton() => ShowPause();
    void ShowPause()
    {
        if (GameManager.Shared != null)
            GameManager.Shared.PauseGame();
        RenderPause();
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
        void RenderPause()
        {
            var gameStats = (0, "", "", 0, 0, 0, 0, 0, 0, true);
            if (GameManager.Shared != null)
                gameStats = GameManager.Shared.detailedStats;
            string lastSave = (gameStats.Item1 != 0 && PersistanceManager.SlotContainsData(gameStats.Item1))
                ? PersistanceManager.GetSlotTimestamp(gameStats.Item1) : "mai";

            pauseLastSaved.text = $"Ultimo Salvataggio: {lastSave}";
            pauseWhiteName.text = gameStats.Item2;
            pauseWhiteStats.text = $"\n\n{gameStats.Item4}\n{gameStats.Item5}\n{gameStats.Item6}";
            pauseBlackName.text = gameStats.Item3;
            pauseBlackStats.text = $"\n\n{gameStats.Item7}\n{gameStats.Item8}\n{gameStats.Item9}";
            pauseWhiteWithdrawalButton.interactable = !gameStats.Item10;
            pauseDrawButton.interactable = !gameStats.Item10;
            pauseBlackWithdrawalButton.interactable = !gameStats.Item10;
            
            pauseNowPlayingTitle.text = "―";
            pauseNowPlayingStatus.text = "Nessun brano in riproduzione";
            pauseNowPlayingElapsed.text = "-:--";
            pauseNowPlayingTotal.text = "-:--";
            pauseNowPlayingSlider.value = 0f;
        }
}
