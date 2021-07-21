using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    // Fields
    [SerializeField] private Transform viewport;
    [SerializeField] private Camera settings;
    [SerializeField] private Direction direction = Direction.Neutral;
    [SerializeField] private float speed = 100.0f;

    // Coroutines
    IEnumerator rotate() {
        while(true) {
            if (direction != Direction.Neutral) {
                if (speed == 0.0) {
                    switch(direction) {
                        case Direction.Left: viewport.Rotate(0.0f, -90.0f, 0.0f, Space.World); break;
                        case Direction.Right: viewport.Rotate(0.0f, 90.0f, 0.0f, Space.World); break;
                        case Direction.Top: viewport.Rotate(-90.0f, 0.0f, 0.0f, Space.Self); break;
                        case Direction.Bottom: viewport.Rotate(90.0f, 0.0f, 0.0f, Space.Self); break;
                    }
                    //viewport.rotation = Quaternion.Euler(new Vector3(viewport.rotation.eulerAngles.x, viewport.rotation.eulerAngles.y, 0));
                    yield return new WaitForSeconds(1.0f);
                } else {
                    switch(direction) {
                        case Direction.Left: viewport.Rotate(0.0f, -1.0f*speed*Time.deltaTime, 0.0f, Space.World); break;
                        case Direction.Right: viewport.Rotate(0.0f, 1.0f*speed*Time.deltaTime, 0.0f, Space.World); break;
                        case Direction.Top: viewport.Rotate(-1.0f*speed*Time.deltaTime, 0.0f, 0.0f, Space.Self); break;
                        case Direction.Bottom: viewport.Rotate(1.0f*speed*Time.deltaTime, 0.0f, 0.0f, Space.Self); break;
                    }
                    //viewport.rotation = Quaternion.Euler(new Vector3(viewport.rotation.eulerAngles.x, viewport.rotation.eulerAngles.y, 0));
                    yield return null;
                }
            } else {
                yield return null;
            }
        }
    }
    // Singleton
    private static CameraManager _shared;
    public static CameraManager Shared {
        get {
            if(_shared==null)
            {
                _shared = GameObject.FindObjectOfType<CameraManager>();
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
            GameObject.DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            GameObject.Destroy(this.gameObject);
        }
    }
    // Handle Input
    public void OnInputDown(Direction dir) {
        if (direction == dir) return;
        if (direction == Direction.Neutral) {
            direction = dir;
            StartCoroutine("rotate");
        } else {
            direction = dir;
        }
    }
    public void OnInputUp(Direction dir) {
        if (direction == dir) {
            direction = Direction.Neutral;
            StopCoroutine("rotate");
        }
    }
    /*
    void Update()
    {
        // LeftArrow
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (direction == Direction.Neutral) {
                direction = Direction.Left;
                StartCoroutine("rotate");
            } else {
                direction = Direction.Left;
            }
        }
        if (Input.GetKeyUp(KeyCode.LeftArrow))
        {
            if (direction == Direction.Left) {
                direction = Direction.Neutral;
                StopCoroutine("rotate");
            }
        }
        // RightArrow
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (direction == Direction.Neutral) {
                direction = Direction.Right;
                StartCoroutine("rotate");
            } else {
                direction = Direction.Right;
            }
        }
        if (Input.GetKeyUp(KeyCode.RightArrow))
        {
            if (direction == Direction.Right) {
                direction = Direction.Neutral;
                StopCoroutine("rotate");
            }
        }
        // UpArrow
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (direction == Direction.Neutral) {
                direction = Direction.Top;
                StartCoroutine("rotate");
            } else {
                direction = Direction.Top;
            }
        }
        if (Input.GetKeyUp(KeyCode.UpArrow))
        {
            if (direction == Direction.Top) {
                direction = Direction.Neutral;
                StopCoroutine("rotate");
            }
        }
        // DownArrow
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (direction == Direction.Neutral) {
                direction = Direction.Bottom;
                StartCoroutine("rotate");
            } else {
                direction = Direction.Bottom;
            }
        }
        if (Input.GetKeyUp(KeyCode.DownArrow))
        {
            if (direction == Direction.Bottom) {
                direction = Direction.Neutral;
                StopCoroutine("rotate");
            }
        }
    }
    */
}
