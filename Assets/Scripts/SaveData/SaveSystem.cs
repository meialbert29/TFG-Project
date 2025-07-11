using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
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

    [SerializeField] private Transform entryContainer;
    [SerializeField] private Transform entryTemplate;
    private List<Transform> scoreEntryTransformList;

    [SerializeField] private RawImage logoRawImage;
    public Texture dayLogoTexture;
    public Texture nightLogoTexture;

    private Color sunMorning = ColorsPalette.LogosColors.sunMorning;
    private Color sunEvening = ColorsPalette.LogosColors.sunEvening;

    private int entriesPerPage = 7;
    public int currentPage = 0;
    private List<ScoreEntry> allEntries = new List<ScoreEntry>();

    // getters & setters
    public int TotalEntries {  get { return allEntries.Count; } }
    public int EntriesPerPage { get { return entriesPerPage; } }

    private void Awake()
    {
        entryTemplate.gameObject.SetActive(false);

        string jsonString = PlayerPrefs.GetString("latestScoreTable");

        ScoresList latestScores = JsonUtility.FromJson<ScoresList>(jsonString);

        if (latestScores == null || latestScores.scoreEntryList == null)
        {
            latestScores = new ScoresList { scoreEntryList = new List<ScoreEntry>() };
        }

        // sort list by date & time
        for (int i = 0; i < latestScores.scoreEntryList.Count; i++)
        {
            for (int j = i + 1; j < latestScores.scoreEntryList.Count; j++)
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

        allEntries = latestScores.scoreEntryList;
        scoreEntryTransformList = new List<Transform>();

        ShowPage(0);
    }

    private void Update()
    {
    }

    private void CreateLatestScoresEntryTransform(ScoreEntry newScoreEntry, Transform container, List<Transform> transformList)
    {
        float templateHeight = 100f;
        string today = DateTime.Now.ToString("dd/MM/yy");
        string yesterday = DateTime.Now.AddDays(-1).ToString("dd/MM/yy");

        Transform entryTransform = Instantiate(entryTemplate, container);
        RectTransform entryRectTransform = entryTransform.GetComponent<RectTransform>();
        entryRectTransform.anchoredPosition = new Vector2(0, -templateHeight * transformList.Count);
        entryRectTransform.gameObject.SetActive(true);

        string dateString = newScoreEntry.date;

        if (dateString == today)
            dateString = "Today";
        else if (dateString == yesterday)
            dateString = "Yesterday";

        string timeString = newScoreEntry.time;
        int score = newScoreEntry.score;

        entryTransform.Find("dateText").GetComponent<Text>().text = dateString;
        entryTransform.Find("timeText").GetComponent<Text>().text = timeString;
        entryTransform.Find("scoreText").GetComponent<Text>().text = score.ToString();

        entryTransform.Find("backgroundRow").gameObject.SetActive(transformList.Count % 2 == 0);

        int entryHour = int.Parse(newScoreEntry.time.Substring(0, 2));
        RawImage logo = entryTransform.Find("timeLogo").GetComponent<RawImage>();

        if (entryHour >= 6 && entryHour < 13)
        {
            logo.texture = dayLogoTexture;
            logo.color = sunMorning;
        }
        else if (entryHour >= 13 && entryHour < 18)
        {
            logo.texture = dayLogoTexture;
            logo.color = sunEvening;
        }
        else
        {
            logo.texture = nightLogoTexture;
            logo.color = Color.white;
        }

        transformList.Add(entryRectTransform);
    }
    public void SaveNewScore(int score)
    {
        AddNewScoreEntry(DateTime.Now.ToString("dd/MM/yy"), DateTime.Now.ToString("HH:mm"), score);
    }
    private void AddNewScoreEntry(string date, string time, int score)
    {
        ScoreEntry entry = new ScoreEntry { date = date, time = time, score = score };

        string jsonString = PlayerPrefs.GetString("latestScoreTable");
        ScoresList latestScores = JsonUtility.FromJson<ScoresList>(jsonString);

        if (latestScores == null || latestScores.scoreEntryList == null)
        {
            latestScores = new ScoresList { scoreEntryList = new List<ScoreEntry>() };
        }

        latestScores.scoreEntryList.Add(entry);

        string json = JsonUtility.ToJson(latestScores);
        PlayerPrefs.SetString("latestScoreTable", json);
        PlayerPrefs.Save();
    }

    public void ClearAllScoreEntries()
    {
        ScoresList emptyList = new ScoresList
        {
            scoreEntryList = new List<ScoreEntry>()
        };

        string json = JsonUtility.ToJson(emptyList);
        PlayerPrefs.SetString("latestScoreTable", json);
        PlayerPrefs.Save();

        RefreshScoreTable();
    }

    private void ShowPage(int pageNumber)
    {
        foreach (Transform child in entryContainer)
        {
            if (child != entryTemplate)
                Destroy(child.gameObject);
        }

        scoreEntryTransformList.Clear();

        int startIndex = pageNumber * entriesPerPage;
        int endIndex = Mathf.Min(startIndex + entriesPerPage, allEntries.Count);

        for (int i = startIndex; i < endIndex; i++)
        {
            CreateLatestScoresEntryTransform(allEntries[i], entryContainer, scoreEntryTransformList);
        }

        currentPage = pageNumber;
    }

    public void NextPage()
    {
        int maxPage = Mathf.CeilToInt((float)allEntries.Count / entriesPerPage) - 1;
        if (currentPage < maxPage)
        {
            currentPage++;
            ShowPage(currentPage);
        }
    }

    public void PreviousPage()
    {
        if (currentPage > 0)
        {
            currentPage--;
            ShowPage(currentPage);
        }

    }

    public void LoadPage(int pageIndex)
    {
        ShowPage(pageIndex - 1);
    }

    public void RefreshScoreTable()
    {
        string jsonString = PlayerPrefs.GetString("latestScoreTable");
        ScoresList latestScores = JsonUtility.FromJson<ScoresList>(jsonString);

        if (latestScores == null || latestScores.scoreEntryList == null)
        {
            latestScores = new ScoresList { scoreEntryList = new List<ScoreEntry>() };
        }

        latestScores.scoreEntryList.Sort((a, b) =>
        {
            DateTime dateA = DateTime.ParseExact(a.date + " " + a.time, "dd/MM/yy HH:mm", null);
            DateTime dateB = DateTime.ParseExact(b.date + " " + b.time, "dd/MM/yy HH:mm", null);
            return dateB.CompareTo(dateA); // M�s recientes primero
        });

        allEntries = latestScores.scoreEntryList;
        ShowPage(currentPage);
    }
}
