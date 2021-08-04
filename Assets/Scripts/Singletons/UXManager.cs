using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UXManager : MonoBehaviour
{
    // Welcome Screen
    [SerializeField] private CanvasScaler scaler;
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
        [SerializeField] private Button preferencesResetButton;
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
        preferencesResetButton.onClick.AddListener(PreferencesResetButton);
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

        // Setup User Interface Sliders
        preferencesDifficultySlider.onValueChanged.AddListener(data => PreferencesDifficultySlider());
        newDifficultySlider.onValueChanged.AddListener(data => NewDifficultySlider());
        preferencesSoundSlider.onValueChanged.AddListener(data => TestSoundVolume());
        preferencesMusicSlider.onValueChanged.AddListener(data => TestMusicVolume());

        // Setup Hover Events
        EventTrigger tooltipTriggers = mainHighscore.gameObject.AddComponent<EventTrigger>();;
        EventTrigger.Entry enterEntry = new EventTrigger.Entry();
            enterEntry.eventID = EventTriggerType.PointerEnter;
            enterEntry.callback.AddListener((data) => { ShowScoreboard(); });
        EventTrigger.Entry leaveEntry = new EventTrigger.Entry();
            leaveEntry.eventID = EventTriggerType.PointerExit;
            leaveEntry.callback.AddListener((data) => { HideScoreboard(); });
        tooltipTriggers.triggers.Add(enterEntry);
        tooltipTriggers.triggers.Add(leaveEntry);
        
        // Main Views
        StartCoroutine("UpdateDPI");
        if(PersistanceManager.firstLoad) ShowWelcome();
        else ShowMain();
    }

    IEnumerator UpdateDPI()
    {
        HdpiScaling preference = PersistanceManager.hdpiScaling;
        bool autoUpdate = preference == HdpiScaling.Auto;
        do
        {
            switch (preference)
            {
                case HdpiScaling.Single: scaler.scaleFactor = 1f; break;
                case HdpiScaling.Half: scaler.scaleFactor = 1.5f; break;
                case HdpiScaling.Double: scaler.scaleFactor = 2f; break;
                case HdpiScaling.Auto: 
                    float dpi = scaler.scaleFactor = Screen.dpi;
                    if (dpi == 0) scaler.scaleFactor = 1f;
                    else
                    {
                        float displayScale = (dpi / 100);
                        scaler.scaleFactor = (displayScale < 1.5f) ? 1f : (displayScale < 2f) ? 1.5f : 2f;
                    } 
                    break;
            }
            yield return new WaitForSeconds(1f);
        } while (autoUpdate);
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
                case 3: welcomePageThree.SetActive(false); PersistanceManager.firstLoad = false; ShowMain(); welcomeLocation = 0; break;
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
            void ShowScoreboard()
            {
                if (!PersistanceManager.hasRecord) return;
                mainHighscoreTooltip.SetActive(true);
                Color color = mainHighscore.color;
                color.a = 0;
                mainHighscore.color = color;
            }
            void HideScoreboard()
            {
                mainHighscoreTooltip.SetActive(false);
                Color color = mainHighscore.color;
                color.a = 1;
                mainHighscore.color = color;
            }
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
        void PreferencesResetButton()
        {
            PersistanceManager.DeleteScoreboard();
            RenderScores();
        }
        void PreferencesCancelButton() => preferencesRoot.SetActive(false);
        void PreferencesOKButton()
        {
            SavePreferences();
            preferencesRoot.SetActive(false);
        }
        void DifficultySlider(float value, Text label)
        {
            label.text = $"Il computer pensa {value} {(value != 1 ? "mosse" : "mossa")} in anticipo";
        }
        void PreferencesDifficultySlider() => DifficultySlider(preferencesDifficultySlider.value, preferencesDifficultyLabel);
        void TestVolume(float value) => SoundManager.Shared.Feedback(value);
        void TestSoundVolume() => TestVolume(preferencesSoundSlider.value);
        void TestMusicVolume() => TestVolume(preferencesMusicSlider.value);
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
            PersistanceManager.soundVolume = (int) preferencesSoundSlider.value;
            PersistanceManager.musicVolume = (int) preferencesMusicSlider.value;
            StopCoroutine("UpdateDPI");
            StartCoroutine("UpdateDPI");
        }
    void ShowNew(int index)
    {
        selectedSlot = index;
        newModeDropdown.value = 0;
        int defaultComputerDifficulty = (int) PersistanceManager.computerDifficulty;
            newDifficultySlider.value = defaultComputerDifficulty;
            newDifficultyLabel.text = $"Il computer pensa {defaultComputerDifficulty} {(defaultComputerDifficulty != 1 ? "mosse" : "mossa")} in anticipo";
        newWhiteNameInput.text = "";
        newBlackNameInput.text = "";
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
        void NewDifficultySlider() => DifficultySlider(newDifficultySlider.value, newDifficultyLabel);
        void RenderGameView()
        {
            HideAll();
            ShowStatus();
            if (SoundManager.Shared != null && PersistanceManager.soundtrack) SoundManager.Shared.enableSoundtrack = true;
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
        if (SoundManager.Shared != null)
            SoundManager.Shared.RegisterHandler(RenderNowPlaying);
        RenderPause();
        pauseRoot.SetActive(true);
    }
        void PauseSaveButton() {
            if (GameManager.Shared != null)
                GameManager.Shared.SaveGame();
            RenderPause();
        }
        void PauseSaveAndQuitButton()
        {
            PauseSaveButton();
            if (GameManager.Shared != null)
                GameManager.Shared.QuitGame();
            if (SoundManager.Shared != null)
                SoundManager.Shared.UnregisterHandler(RenderNowPlaying);
            HideAll();
            if (SoundManager.Shared != null)
            {
                SoundManager.Shared.enableSoundtrack = false;
                SoundManager.Shared.StopAllSpeaking();
            }
            ShowMain();
        }
        void PauseOKButton()
        {
            if (GameManager.Shared != null)
                GameManager.Shared.ResumeGame();
            if (SoundManager.Shared != null)
                SoundManager.Shared.UnregisterHandler(RenderNowPlaying);
            pauseRoot.SetActive(false);
        }
        void PauseWhiteWithdrawalButton()
        {
            GameManager.Shared.WithdrawGame(PlayerColor.White);
            PauseSaveAndQuitButton();
        }
        void PauseDrawButton()
        {
            GameManager.Shared.WithdrawGame();
            PauseSaveAndQuitButton();
        }
        void PauseBlackWithdrawalButton()
        {
            GameManager.Shared.WithdrawGame(PlayerColor.Black);
            PauseSaveAndQuitButton();
        }
        void RenderPause()
        {
            var nowPlaying = (SoundManager.Shared != null) ? SoundManager.Shared.nowPlaying : ("", 0, 0);
            RenderNowPlaying(nowPlaying.Item1, nowPlaying.Item2, nowPlaying.Item3);
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
        }
        public void RenderNowPlaying(string title, float elapsed, float total)
        {
            pauseNowPlayingTitle.text = (title != "") ? title : "―";
            pauseNowPlayingStatus.text = (total > 0) ? "In riproduzione" : "Nessun brano in riproduzione";
            pauseNowPlayingElapsed.text = (total > 0) ? FormatMinutesSeconds(elapsed) : "-:--";
            pauseNowPlayingTotal.text = (total > 0) ? FormatMinutesSeconds(total) : "-:--";
            pauseNowPlayingSlider.value = (total > 0) ? elapsed / total : 0;
        }
        string FormatMinutesSeconds(float value)
        {
            int integerValue = (int) value;
            int minutes = (integerValue - (integerValue % 60)) / 60;
            int seconds = integerValue % 60;
            return (seconds < 10) ? $"{minutes}:0{seconds}" : $"{minutes}:{seconds}";
        }
    [ContextMenu("Test Alert")]
    public void TestAlert() => ShowAlert();
    public void ShowAlert(string message = "Messaggio della notifica")
    {
        if (pauseRoot.activeSelf) return;
        StopCoroutine("HideAlert");
        alertText.text = message;
        alertRoot.SetActive(true);
        StartCoroutine("HideAlert");
    }
    IEnumerator HideAlert()
    {
        yield return new WaitForSeconds(5f);
        alertRoot.SetActive(false);
    }
}
