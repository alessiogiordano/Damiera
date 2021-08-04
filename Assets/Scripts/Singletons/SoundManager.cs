using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [SerializeField] private AudioClip beep;
    [SerializeField] private AudioClip feedback;
    [SerializeField] private AudioClip[] soundtrack;
    [SerializeField] private string[] titles;
    private AudioSource _beepSource;
    private AudioSource _feedbackSource;
    private AudioSource _voiceoverSource;
    private bool _isSpeaking;
    public bool isSpeaking { get => _isSpeaking; }
    private AudioSource _soundtrackSource;
    private int _soundtrackIndex;
    private AudioClip nextClip
    {
        get
        {
            if (soundtrack.Length == 0) return null;
            _soundtrackIndex++;
            if (_soundtrackIndex == soundtrack.Length) _soundtrackIndex = 0;
            return soundtrack[_soundtrackIndex];
        }
    }
    private string currentTitle
    {
        get
        {
            if(_soundtrackIndex >= titles.Length) return "";
            return titles[_soundtrackIndex];
        }
    }
    private float currentLength
    {
        get
        {
            if (soundtrack.Length == 0) return 0;
            return soundtrack[_soundtrackIndex].length;
        }
    }
    public delegate void NowPlayingDelegate(string title, float elapsed, float total);
    private NowPlayingDelegate _nowPlayingHandlers;
    public void RegisterHandler(NowPlayingDelegate handler) => _nowPlayingHandlers += handler;
    public void UnregisterHandler(NowPlayingDelegate handler) => _nowPlayingHandlers -= handler;
    public bool enableSoundtrack
    {
        set
        {
            _soundtrackSource.Stop();
            StopCoroutine("Soundtrack");
            if (value) StartCoroutine("Soundtrack");
        }
    }
    public (string, float, float) nowPlaying
    {
        get
        {
            return _soundtrackSource.isPlaying ? (currentTitle, _soundtrackSource.time, currentLength) : ("", 0, 0);
        }
    }
    // Singleton
    private static SoundManager _shared;
    public static SoundManager Shared {
        get {
            if(_shared==null)
            {
                _shared = GameObject.FindObjectOfType<SoundManager>();
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
        // Beep
        _beepSource = gameObject.AddComponent<AudioSource>();
        _beepSource.volume = 1;
        _beepSource.clip = beep;
        // Volume Feedback
        _feedbackSource = gameObject.AddComponent<AudioSource>();
        _feedbackSource.volume = 1;
        _feedbackSource.clip = feedback;
        // Voiceover
        _voiceoverSource = gameObject.AddComponent<AudioSource>();
        _voiceoverSource.volume = 1;
        // Soundtrack
        _soundtrackSource = gameObject.AddComponent<AudioSource>();
        _soundtrackSource.volume = 1;
        _soundtrackIndex = new System.Random().Next(soundtrack.Length);
        //_soundtrackSource.clip = feedback;
    }
    public void Beep()
    {
        _beepSource.volume = PersistanceManager.soundVolume / 10f;
        _beepSource.Play();
    }
    public void Feedback(float volumeLevel)
    {
        _feedbackSource.volume = volumeLevel / 10f;
        _feedbackSource.Play();
    }
    IEnumerator Soundtrack()
    {
        bool showAlert = false;
        while (true)
        {
            _soundtrackSource.clip = nextClip;
            _soundtrackSource.volume = PersistanceManager.musicVolume / 10f;
            _soundtrackSource.Play();
            if (showAlert && (currentTitle != "")) UXManager.Shared.ShowAlert(currentTitle);
                else showAlert = true;
            while (_soundtrackSource.isPlaying)
            {
                _nowPlayingHandlers?.Invoke(currentTitle, _soundtrackSource.time, currentLength);
                yield return new WaitForSeconds(1f);
            }
            yield return null;
        }
    }
    IEnumerator Speak(string[] clips)
    {
        _isSpeaking = true;
        foreach (var clip in clips)
        {
            _voiceoverSource.clip = Resources.Load<AudioClip>(clip);
            _voiceoverSource.volume = PersistanceManager.soundVolume / 10f;
            _voiceoverSource.Play();
            while (_voiceoverSource.isPlaying) yield return null;
        }
        _isSpeaking = false;
    }
    public void SpeakMove(BoardCell source, BoardCell destination, bool isDama) => StartCoroutine("Speak", new string[] {$"Voiceover/{(isDama ? "dama" : "pedina")}-da", $"Voiceover/Board/{source.cell}", "Voiceover/a", $"Voiceover/Board/{destination.cell}"});
    public void SpeakCapture(BoardCell[] captures, bool isDama)
    {
        string[] arguments = new string[captures.Length + 1];
        arguments[0] = isDama ? "Voiceover/dama-mangia-in" : "Voiceover/pedina-mangia-in";
        for (int i = 0; i < captures.Length; i++) arguments[i + 1] = $"Voiceover/Board/{captures[i].cell}";
        StartCoroutine("Speak", arguments);
    }
    public void SpeakVictory(PlayerColor color) => StartCoroutine("Speak", new string[] {$"Voiceover/{(color == PlayerColor.White ? "vince-bianco" : "vince-nero")}"});
    public void SpeakDama(BoardCell cell) => StartCoroutine("Speak", new string[] {"Voiceover/pedina-dama", $"Voiceover/Board/{cell.cell}"});
    public void StopAllSpeaking()
    {
        StopCoroutine("Speak");
        _voiceoverSource.Stop();
        _isSpeaking = false;
    }

    [ContextMenu("Test Speech")]
    void TestSpeech() => StartCoroutine("Speak", new string[] {"Voiceover/pedina-da", "Voiceover/Board/C4", "Voiceover/ad", "Voiceover/Board/E7"});
}
