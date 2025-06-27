using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(Animator))]
public class TransitionManager : MonoBehaviour
{
    private static TransitionManager instance;
    [SerializeField] private GameObject transitionCanvas;
    private bool transitionFinished = false;

    private Animator m_Anim;
    private int HashShowAnim = Animator.StringToHash("Show");

    private const string sceneName_MainMenu = "StartScene";
    private const string sceneName_GameScene = "GameScene";

    [SerializeField] private Slider progressSlider;
    [SerializeField] private TextMeshProUGUI progressLabel;
    [SerializeField] private TextMeshProUGUI transitionInformationLabel;
    [Multiline]
    private readonly string[] quotesStart =
    {
        "Hello, it's nice to see you again!",
        "'The fears we don't face become our limits.' -Robin Sharma.",
        "Talk a little nice to yourself today.",
        "Find the courage to keep trying, even when it seems like no progress is being made.",
        "Progress, not perfection.",
        "The calmer you are, the clearer you think.",
        "1% better everyday."
    };

    private readonly string[] quotesAfterGame =
    {
        "Do what scares you. Until it doesn't.",
        "Action cures anxiety",
        "Satify your soul, not the society.",
        "Give your best in everything.",
        "If it comes; let it. If it goes; let it.",
        "Invest in yourself.",
        "A negative mind will never give you a positive life."
    };


    public static TransitionManager Instance
    {
        get
        {
            // search prefab in resources to load it
            if(instance == null)
            {
                instance = Instantiate(Resources.Load<TransitionManager>("Prefabs/TransitionManager"));
                instance.Initialise();
            }
            return instance;
        }
    }

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            Initialise();
        }
        else if(instance != null)
                Destroy(gameObject);
    }

    public void LoadScene(string sceneName)
    {
        SetObject(true);
        StartCoroutine(LoadCoroutine(sceneName));
    }

    IEnumerator LoadCoroutine(string sceneName)
    {
        m_Anim.SetBool(HashShowAnim, true);

        if(sceneName == "GameScene")
        {
            if (transitionInformationLabel != null)
                transitionInformationLabel.text = quotesStart[Random.Range(0, quotesStart.Length - 1)];
        }
        else if(sceneName == "StartScene")
        {
            if (transitionInformationLabel != null)
                transitionInformationLabel.text = quotesAfterGame[Random.Range(0, quotesStart.Length - 1)];
        }

        UpdateProgressValue(0);


        yield return new WaitForSeconds(0.5f);
        var sceneAsync = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);

        while (!sceneAsync.isDone)
        {
            UpdateProgressValue(sceneAsync.progress);

            yield return null;
        }

        UpdateProgressValue(1);
        m_Anim.SetBool(HashShowAnim, false);

        SetObject(false);
    }

    private void Initialise()
    {
        m_Anim = GetComponent<Animator>();

        DontDestroyOnLoad(gameObject);
    }

    void UpdateProgressValue(float progressValue)
    {
        if(progressSlider != null)
            progressSlider.value = progressValue;
        if(progressLabel != null)
            progressLabel.text = $"{progressValue * 100}%";
    }

    void SetObject(bool transitionFinished)
    {
        gameObject.SetActive(transitionFinished);
    }
}
