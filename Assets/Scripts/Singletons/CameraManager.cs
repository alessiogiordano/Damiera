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

    [SerializeField] private float defaultAngle = -45.0f;

    // Coroutines
    IEnumerator Rotate() {
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
        viewport.transform.rotation = Quaternion.Euler(new Vector3(defaultAngle, 0.0f, 0.0f));
    }
    // Handle Input
    public void OnInputDown(Direction dir) {
        if (direction == dir) return;
        if (direction == Direction.Neutral) {
            direction = dir;
            StartCoroutine("Rotate");
        } else {
            direction = dir;
        }
    }
    public void OnInputUp(Direction dir) {
        if (direction == dir) {
            direction = Direction.Neutral;
            StopCoroutine("Rotate");
        }
    }
}
