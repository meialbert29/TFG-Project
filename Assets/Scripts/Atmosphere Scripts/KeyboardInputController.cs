using UnityEngine;

public class KeyboardInputController : MonoBehaviour
{
    [SerializeField] private GeneralController generalController;
    [SerializeField] private LeavesVFXController _vfxController;
    private string _mood;
    private bool keyDown = false;

    //getters & setters
    public bool KeyDown { get { return keyDown; } set { keyDown = value; } }

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
            generalController.SetMood("sad");
            EnableFallingLeaves();
            keyDown = true;
        }

        if (Input.GetKeyDown(KeyCode.Keypad2))
        {
            generalController.SetMood("stressed");
            EnableFallingLeaves();
            keyDown = true;
        }

        if (Input.GetKeyDown(KeyCode.Keypad3))
        {
            generalController.SetMood("neutral");
            keyDown = true;
        }

        if (Input.GetKeyDown(KeyCode.Keypad4))
        {
            generalController.SetMood("calm");
            keyDown = true;
        }
        if (Input.GetKeyDown(KeyCode.Keypad5))
        {
            generalController.SetMood("anxious");
            EnableFallingLeaves();
            keyDown = true;
        }

        if (keyDown)
        {
            generalController.contTrees = 0;
            generalController.MoodChanging = true;
        }

    }

    private void EnableFallingLeaves()
    {
        foreach (var tree in generalController.TreesList)
        {
            tree.GetComponentInChildren<LeavesVFXController>().Fall = true;
        }
        foreach (var bush in generalController.BushesList)
        {
            bush.GetComponentInChildren<LeavesVFXController>().Fall = true;
        }
    }
}
