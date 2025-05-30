using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using TMPro;

public class HistoryButtonsController : MonoBehaviour
{
    [SerializeField] private SaveSystem saveSystem;
    // images variables
    public Image backButton;
    public Image previousPageButton;
    public Image nextPageButton;

    // sprites variables
    public Sprite normalBack_Sprite;
    public Sprite hoverBack_Sprite;

    public Sprite normalPrevious_Sprite;
    public Sprite hoverPreviousPage_Sprite;

    public Sprite normalNextPage_Sprite;
    public Sprite hoverNextPage_Sprite;

    public Text currentPage;
    public Text totalPages;
    private int currentPageIndex;
    private int totalPageCount;

    public void Start()
    {
        currentPageIndex = 1;

        if (saveSystem != null)
        {
            int totalEntries = saveSystem.TotalEntries;
            int entriesPerPage = saveSystem.EntriesPerPage;

            totalPageCount = (totalEntries / entriesPerPage) + 1;
        }
        else
        {
            totalPageCount = 1; // fallback
        }

        currentPage.text = currentPageIndex.ToString();

        totalPages.text = "/" + totalPageCount.ToString();
    }

    public void OnBackMainButtonEnter()
    {
        if (backButton != null)
            backButton.sprite = hoverBack_Sprite;
    }
    public void OnBackMainButtonExit()
    {
        if (backButton != null)
            backButton.sprite = normalBack_Sprite;
    }
    public void OnPreviousPageButtonEnter()
    {
        if (previousPageButton != null)
            previousPageButton.sprite = hoverPreviousPage_Sprite;
    }
    public void OnPreviousPageButtonExit()
    {
        if (previousPageButton != null)
            previousPageButton.sprite = normalPrevious_Sprite;
    }
    public void OnNextPageButtonEnter()
    {
        if (nextPageButton != null)
            nextPageButton.sprite = hoverNextPage_Sprite;
    }
    public void OnNextPageButtonExit()
    {
        if (nextPageButton != null)
            nextPageButton.sprite = normalNextPage_Sprite;
    }

    // click buttons
    public void OnClickPreviousPageButton()
    {
        if(previousPageButton != null && currentPageIndex > 1)
        {
            currentPageIndex--;
            currentPage.text = currentPageIndex.ToString();
        }    
    }

    public void OnClickNextPageButton()
    {
        if(nextPageButton != null && currentPageIndex < totalPageCount)
        {
            currentPageIndex++;
            currentPage.text = currentPageIndex.ToString();
        }
    }
}
