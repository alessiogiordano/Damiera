using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [SerializeField] private AudioClip beep;
    private AudioSource _beepSource;

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
        _beepSource = gameObject.AddComponent<AudioSource>();
        _beepSource.volume = 1;
        _beepSource.clip = beep;
    }
    public void Beep()
    {
        _beepSource.Play();
    }
}
