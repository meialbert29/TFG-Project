using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class SaveSystem : MonoBehaviour
{
    [Serializable]
    private class ScoreEntry
    {
        public string date;
        public string time;
        public int score;
    }

    private class ScoresList
    {
        public List<ScoreEntry> scoreEntryList;
    }

    private Transform entryContainer;
    private Transform entryTemplate;
    private List<Transform> scoreEntryTransformList;

    private void Awake()
    {
        entryContainer = transform.Find("ScoreTableEntryContainer");
        entryTemplate = entryContainer.Find("ScoreEntryTemplate");

        entryTemplate.gameObject.SetActive(false);

        //AddScoreEntry(DateTime.Now.ToString("dd/MM/yy", DateTime.ParseExact(time, "HH:mm", null));

        string jsonString = PlayerPrefs.GetString("latestScoreTable");
        ScoresList latestScores = JsonUtility.FromJson<ScoresList>(jsonString);

        // sort list by date & time

        for (int i = 0; i < latestScores.scoreEntryList.Count; i++)
        {
            for(int j = i + 1; j < latestScores.scoreEntryList.Count; j++)
            {
                DateTime dateI = DateTime.ParseExact(latestScores.scoreEntryList[i].date + " " + latestScores.scoreEntryList[i].time, "dd/MM/yy HH:mm", null);
                DateTime dateJ = DateTime.ParseExact(latestScores.scoreEntryList[j].date + " " + latestScores.scoreEntryList[j].time, "dd/MM/yy HH:mm", null);

                if (dateJ > dateI)
                {
                    ScoreEntry tmp = latestScores.scoreEntryList[i];
                    latestScores.scoreEntryList[i] = latestScores.scoreEntryList[j];
                    latestScores.scoreEntryList[j] = tmp;
                }
                
            }
        }

        scoreEntryTransformList = new List<Transform>();

        foreach(ScoreEntry latestScoreEntry in latestScores.scoreEntryList)
        {
            CreateLatestScoreEntryTransform(latestScoreEntry, entryContainer, scoreEntryTransformList);
        }
    }

    private void CreateLatestScoreEntryTransform(ScoreEntry newScoreEntry, Transform container, List<Transform> transformList)
    {
        float templateHeight = 70f;
        string today = DateTime.Now.ToString("dd/MM/yy");
        string yesterday = DateTime.Now.AddDays(-1).ToString("dd/MM/yy");

        Transform entryTransform = Instantiate(entryTemplate, container);
        RectTransform entryRectTransform = entryTransform.GetComponent<RectTransform>();
        entryRectTransform.anchoredPosition = new Vector2(0, -templateHeight * transformList.Count);
        entryRectTransform.gameObject.SetActive(true);

        string dateString = newScoreEntry.date;

        if(dateString == today.ToString())
        {
            dateString = "Today";
        }
        else if(dateString == yesterday.ToString())
        {
            dateString = "Yesterday";
        }

        string timeString = newScoreEntry.time;

        int score = newScoreEntry.score;

        entryTransform.Find("dateText").GetComponent<Text>().text = dateString;
        entryTransform.Find("timeText").GetComponent<Text>().text = timeString;
        entryTransform.Find("scoreText").GetComponent<Text>().text = score.ToString();

        transformList.Add(entryRectTransform);
    }

    private void AddNewScoreEntry(string date, string time, int score)
    {

        // create new score entry
        ScoreEntry entry = new ScoreEntry { date = date, time = time, score = score };
        
        // load saved scores
        string jsonString = PlayerPrefs.GetString("latestScoreTable");
        ScoresList latestScores = JsonUtility.FromJson<ScoresList>(jsonString);

        // add new entry to LatestScores
        latestScores.scoreEntryList.Add(entry);

        // save updated LatestScores
        string json = JsonUtility.ToJson(latestScores);
        PlayerPrefs.SetString("latestScoreTable", json);
        PlayerPrefs.Save();
    }
}
