using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct PedinaAnimation
{
    private Vector3 _origin;
    private Vector3 _destination;
    private float _progress;
    private float _duration;

    public PedinaAnimation(BoardCell origin, BoardCell destination, float duration = 1.0f)
    {
        _origin = origin.localPosition;
        _destination = destination.localPosition;
        _progress = 0f;
        _duration = duration;
    }

    public bool running
    {
        get
        {
            return (_progress < 1.0f);
        }
    }

    public Vector3 frame {
        get
        {
            if (running)
            {
                // Progress since last frame = Time elapsed since last frame / expected duration of the animation;
                _progress = _progress + (Time.deltaTime / _duration);
                Vector3 output = Vector3.Lerp(_origin, _destination, _progress);
                return new Vector3(output.x, 0f, output.z);
            }
            else
            {
                return _destination;
            }
        }
    }
}
