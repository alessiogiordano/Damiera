using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    // Fields
    private MouseStatus mouseStatus = MouseStatus.Neutral;
    private Vector3 mouseStartPosition = new Vector3(0f, 0f, 0f);
    [SerializeField] private float dragSensibility = 2.0f;

    // Singleton
    private static InputManager _shared;
    public static InputManager Shared {
        get {
            if(_shared==null)
            {
                _shared = GameObject.FindObjectOfType<InputManager>();
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

    // Update handles input
    void Update()
    {
        // Check if game is running
        if(!GameManager.Shared.status.ShouldAcceptGameInput()) 
        {
            mouseStatus = MouseStatus.Neutral;
            return;
        }
        // Mouse
        {
            // Left Click
            {
                if (Input.GetMouseButtonDown(0))
                {
                    mouseStartPosition = Input.mousePosition;
                    mouseStatus = MouseStatus.Down;
                }
                if (Input.GetMouseButtonUp(0))
                {
                    if (mouseStatus == MouseStatus.Down)
                    {
                        // Handle Click
                        {
                            RaycastHit hit;
                            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                            if (Physics.Raycast(ray, out hit))
                            {
                                GameManager.Shared.Clicked(hit);
                            }
                        }
                    }
                    else if (mouseStatus == MouseStatus.Drag)
                    {
                        // Handle DragEnd
                        CameraManager.Shared.OnInputUp(Input.mousePosition.ToDirectionStartingFrom(mouseStartPosition));
                    }
                    mouseStatus = MouseStatus.Neutral;
                }
            }
            // Right Click
            {
                if (Input.GetMouseButtonDown(1))
                {

                }
                if (Input.GetMouseButtonUp(1))
                {

                }
            }
            // Drag
            if (PersistanceManager.canRotateWithMouse) {
                if (mouseStatus == MouseStatus.Down)
                {
                    if  (
                            (Mathf.Abs(Input.mousePosition.x - mouseStartPosition.x) > dragSensibility) ||
                            (Mathf.Abs(Input.mousePosition.y - mouseStartPosition.y) > dragSensibility)
                        ) 
                    {
                        mouseStatus = MouseStatus.Drag;
                        // Handle DragStart
                    }
                }
                else if (mouseStatus == MouseStatus.Drag)
                {
                    // Handle OnDrag
                    CameraManager.Shared.OnInputDown(Input.mousePosition.ToDirectionStartingFrom(mouseStartPosition));
                }
            }
        }
        // Keyboard
        if (PersistanceManager.canRotateWithKeyboard) {
            // LeftArrow
            {
                if (Input.GetKeyDown(KeyCode.LeftArrow))
                {
                    CameraManager.Shared.OnInputDown(KeyCode.LeftArrow.ToDirection());
                }
                if (Input.GetKeyUp(KeyCode.LeftArrow))
                {
                    CameraManager.Shared.OnInputUp(KeyCode.LeftArrow.ToDirection());
                }
            }
            // RightArrow
            {
                if (Input.GetKeyDown(KeyCode.RightArrow))
                {
                    CameraManager.Shared.OnInputDown(KeyCode.RightArrow.ToDirection());
                }
                if (Input.GetKeyUp(KeyCode.RightArrow))
                {
                    CameraManager.Shared.OnInputUp(KeyCode.RightArrow.ToDirection());
                }
            }
            // UpArrow
            {
                if (Input.GetKeyDown(KeyCode.UpArrow))
                {
                    CameraManager.Shared.OnInputDown(KeyCode.UpArrow.ToDirection());
                }
                if (Input.GetKeyUp(KeyCode.UpArrow))
                {
                    CameraManager.Shared.OnInputUp(KeyCode.UpArrow.ToDirection());
                }
            }
            // DownArrow
            {
                if (Input.GetKeyDown(KeyCode.DownArrow))
                {
                    CameraManager.Shared.OnInputDown(KeyCode.DownArrow.ToDirection());
                }
                if (Input.GetKeyUp(KeyCode.DownArrow))
                {
                    CameraManager.Shared.OnInputUp(KeyCode.DownArrow.ToDirection());
                }
            }
        }
    }
}
