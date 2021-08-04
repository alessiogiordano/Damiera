using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    // Fields
    [SerializeField] private Transform viewport;
    [SerializeField] private Camera settings;
    [SerializeField] private Direction direction = Direction.Neutral;
    [SerializeField] private float constantSpeed = 100.0f;
    [SerializeField] private float defaultAngle = -45.0f;
    [SerializeField] private float rotationAnimationDuration = 1.0f; // Se metti SerializeField i valori iniziali non vengono aggiornati se li cambi da codice...
    private (float, float, float, float) _settedRotationAnimation = (0f, 0f, 1.0f, 0f); // (X, Y, %, DELAY)

    // Coroutines
    IEnumerator ConstantRotation()
    {
        // True is useless, but clarifies that the code is run indefinetly
        while(true && (_settedRotationAnimation.Item3 >= 1.0f)) {
            if (direction != Direction.Neutral) {
                if (constantSpeed == 0.0) {
                    switch(direction) {
                        case Direction.Left: viewport.Rotate(0.0f, -90.0f, 0.0f, Space.World); break;
                        case Direction.Right: viewport.Rotate(0.0f, 90.0f, 0.0f, Space.World); break;
                        case Direction.Top: viewport.Rotate(-90.0f, 0.0f, 0.0f, Space.Self); break;
                        case Direction.Bottom: viewport.Rotate(90.0f, 0.0f, 0.0f, Space.Self); break;
                    }
                    yield return new WaitForSeconds(1.0f);
                } else {
                    switch(direction) {
                        case Direction.Left: viewport.Rotate(0.0f, -1.0f*constantSpeed*Time.deltaTime, 0.0f, Space.World); break;
                        case Direction.Right: viewport.Rotate(0.0f, 1.0f*constantSpeed*Time.deltaTime, 0.0f, Space.World); break;
                        case Direction.Top: viewport.Rotate(-1.0f*constantSpeed*Time.deltaTime, 0.0f, 0.0f, Space.Self); break;
                        case Direction.Bottom: viewport.Rotate(1.0f*constantSpeed*Time.deltaTime, 0.0f, 0.0f, Space.Self); break;
                    }
                    yield return null;
                }
            } else {
                yield return null;
            }
        }
    }
    IEnumerator AnimatedRotation()
    {
        if (_settedRotationAnimation.Item4 > 0.0f) yield return new WaitForSeconds(_settedRotationAnimation.Item4);
        float xVariation = _settedRotationAnimation.Item1 - viewport.localEulerAngles.x;
        float yVariation = _settedRotationAnimation.Item2 - viewport.localEulerAngles.y;
        while(_settedRotationAnimation.Item3 < 1.0f) {
            float progress = Time.deltaTime / rotationAnimationDuration;
            _settedRotationAnimation.Item3 = _settedRotationAnimation.Item3 + progress;
            if (_settedRotationAnimation.Item3 > 1.0f)
            {
                progress = progress - (_settedRotationAnimation.Item3 - 1.0f);
                _settedRotationAnimation.Item3 = 1.0f;
            }
            /*
            if (_settedRotationAnimation.Item3 < 0.5)
            {
                viewport.transform.localEulerAngles = new Vector3(viewport.transform.localEulerAngles.x + (xVariation * progress), viewport.transform.localEulerAngles.y, 0f);
                viewport.transform.localEulerAngles = new Vector3(viewport.transform.localEulerAngles.x, viewport.transform.localEulerAngles.y + (yVariation * progress), 0f);
            }
            else
            {
                viewport.Rotate(xVariation * progress, 0.0f, 0.0f, Space.Self);
                viewport.Rotate(0.0f, yVariation * progress, 0.0f, Space.World);
            }
            */
            viewport.Rotate(xVariation * progress, 0.0f, 0.0f, Space.Self);
            viewport.Rotate(0.0f, yVariation * progress, 0.0f, Space.World);
            yield return null;
        }
    }
    public void TurnCameraToDefault(float additionalHorizontalRotation = 0f, bool withAnimation = false, float delay = 0.0f)
    {
        TurnCamera(defaultAngle, additionalHorizontalRotation, false, withAnimation, delay);
    }
    // Control camera from other parts of the game
    public void TurnCamera(float x, float y, bool relativeMovement = true, bool withAnimation = false, float delay = 0.0f)
    {
        float offsetX = relativeMovement ? viewport.localEulerAngles.x : 0f;
        float offsetY = relativeMovement ? viewport.localEulerAngles.y : 0f;
        float absoluteX = x + offsetX;
        float absoluteY = y + offsetY;
        float normalizedX = absoluteX < 0 ? 360f + absoluteX : absoluteX;
        float normalizedY = absoluteY < 0 ? 360f + absoluteY : absoluteY;
        if (!withAnimation)
        {
            viewport.rotation = Quaternion.Euler(new Vector3(normalizedX, normalizedY, 0f));
            if (delay != 0f) Debug.Log("A delay in a non animated turn would result in the game freezing");
        }
        else
        {
            _settedRotationAnimation = (normalizedX, normalizedY, 0f, delay);
            StartCoroutine("AnimatedRotation");
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
            GameObject.DontDestroyOnLoad(this.transform.parent.gameObject);
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
            StartCoroutine("ConstantRotation");
        } else {
            direction = dir;
        }
    }
    public void OnInputUp(Direction dir) {
        if (direction == dir) {
            direction = Direction.Neutral;
            StopCoroutine("ConstantRotation");
        }
    }
}
