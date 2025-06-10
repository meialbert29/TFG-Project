using UnityEngine;
using UnityEngine.UI;

public class ScoreSystem : MonoBehaviour
{
    public GeneralController generalController;
    public GameObject pausedUI;

    public Image lotusIcon;
    public Text pointsScoreText;
    public Text pointsEarnedText;

    public Sprite lotus_1;
    public Sprite lotus_2;
    public Sprite lotus_3;
    public Sprite lotus_4;
    public Sprite lotus_5;
    public Sprite lotus_6;
    public Sprite lotus_7;

    private int score;
    private int points;
    private float moodTimer = 0f;
    private float moodUpdateInterval = 1f; // cada cuánto tiempo se suman puntos según el mood

    private Color transparent = ColorsPalette.LotusColors.transparent;

    public int Score {  get { return score;} set { score = value; } }
    public int PointsEarned { get { return points; } set { points = value; } }

    void Start()
    {
        if (generalController == null)
            Debug.Log("GeneralController script not found");

        lotusIcon.sprite = null;
        lotusIcon.color = transparent;

        score = 0;
        pointsScoreText.text = score.ToString();
    }

    void Update()
    {
        HandleInputPoints();
        UpdateMoodPoints();
        UpdatePointsEarned();
        UpdateScorePoints();
    }

    private void UpdateMoodPoints()
    {
       
        moodTimer += Time.deltaTime;

        if (moodTimer >= moodUpdateInterval)
        {
            PointsByMood(generalController.Mood);
            moodTimer = 0f;
        }
    }

    public void UpdateScorePoints()
    {
        pointsScoreText.text = score.ToString();
        ChangeSpriteByPoints();
    }

    public void UpdatePointsEarned()
    {
        pointsEarnedText.text = points.ToString();
    }

    private void HandleInputPoints()
    {
        if (Input.GetKeyDown(KeyCode.U))
            score += 10;
        else if (Input.GetKeyDown(KeyCode.I))
            score += 20;
        else if (Input.GetKeyDown(KeyCode.O))
            score += 50;
        else if (Input.GetKeyDown(KeyCode.Keypad0))
            score += 100;
    }

    private void ChangeSpriteByPoints()
    {
            
        if (score > 200)
        {
            lotusIcon.color = Color.white;
            lotusIcon.sprite = lotus_7;
        }
        else if (score > 150)
        {
            lotusIcon.color = Color.white;
            lotusIcon.sprite = lotus_6;
        }      
        else if (score > 100)
        {
            lotusIcon.color = Color.white;
            lotusIcon.sprite = lotus_5;
        }
            
        else if (score > 75)
        {
            lotusIcon.color = Color.white;
            lotusIcon.sprite = lotus_4;
        }
            
        else if (score > 50)
        {
            lotusIcon.color = Color.white;
            lotusIcon.sprite = lotus_3;
        }
            
        else if (score > 25)
        {
            lotusIcon.color = Color.white;
            lotusIcon.sprite = lotus_2;
        }
            
        else if(score > 10)
        {
            lotusIcon.color = Color.white;
            lotusIcon.sprite = lotus_1;
        }
    }

    private void PointsByMood(string mood)
    {
        switch (mood)
        {
            case "sad":
                points = 2;
                break;
            case "stressed":
                points = 1;
                break;
            case "neutral":
                points = 3;
                break;
            case "calm":
                points = 4;
                break;
            case "anxious":
                points = 0;
                break;
        }

        score += points;
    }
}
