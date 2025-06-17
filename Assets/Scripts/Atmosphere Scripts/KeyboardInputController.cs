using UnityEngine;

public class KeyboardInputController : MonoBehaviour
{
    [SerializeField] private GeneralController generalController;
    private string _mood;
    private bool keyDown = false;

    //getters & setters
    public bool KeyDown {  get { return keyDown; } set { keyDown = value; } }

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
            generalController.Mood = "sad";
            keyDown = true;
        }

        if (Input.GetKeyDown(KeyCode.Keypad2))
        {
            generalController.Mood = "stressed";

            keyDown = true;
        }

        if (Input.GetKeyDown(KeyCode.Keypad3))
        {
            generalController.Mood = "neutral";
            keyDown = true;
        }

        if (Input.GetKeyDown(KeyCode.Keypad4))
        {
            generalController.Mood = "calm";
            keyDown = true;
        }
        if (Input.GetKeyDown(KeyCode.Keypad5))
        {
            generalController.Mood = "anxious";
            keyDown = true;
        }

        if (keyDown)
        {
            generalController.Cont = 0;
        }
           
    }
}
