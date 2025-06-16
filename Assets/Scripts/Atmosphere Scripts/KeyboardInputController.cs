using UnityEngine;

public class KeyboardInputController : MonoBehaviour
{
    [SerializeField] private GeneralController generalController;
    private string _mood;
    private bool keyDown = false;

    //getters & setters
    public bool KeyDown {  get { return keyDown; } }

    void Start()
    {
        if (generalController == null)
            Debug.Log("General controller not found");
        _mood = generalController.Mood;
    }

    // Update is called once per frame
    void Update()
    {
        HandleInput();
    }

    private void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.Keypad1))
        {
            _mood = "sad";
            keyDown = true;
        }

        if (Input.GetKeyDown(KeyCode.Keypad2))
        {
            _mood = "stressed";

            keyDown = true;
        }

        if (Input.GetKeyDown(KeyCode.Keypad3))
        {
            _mood = "neutral";
            keyDown = true;
        }

        if (Input.GetKeyDown(KeyCode.Keypad4))
        {
            _mood = "calm";
            keyDown = true;
        }
        if (Input.GetKeyDown(KeyCode.Keypad5))
        {
            _mood = "anxious";
            keyDown = true;
        }
    }
}
