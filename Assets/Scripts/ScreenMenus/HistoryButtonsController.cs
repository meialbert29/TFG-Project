using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using TMPro;

public class HistoryButtonsController : MonoBehaviour
{
    AudioManager audioManager;

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

    private void Awake()
    {
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
    }

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

    private void OnEnable()
    {
        // Reinicia sprites a su estado normal
        if (backButton != null)
            backButton.sprite = normalBack_Sprite;

        if (previousPageButton != null)
            previousPageButton.sprite = normalPrevious_Sprite;

        if (nextPageButton != null)
            nextPageButton.sprite = normalNextPage_Sprite;

        // Reinicia índice de página y los textos
        currentPageIndex = 1;

        if (saveSystem != null)
        {
            int totalEntries = saveSystem.TotalEntries;
            int entriesPerPage = saveSystem.EntriesPerPage;

            totalPageCount = (totalEntries / entriesPerPage) + 1;
        }
        else
        {
            totalPageCount = 1;
        }

        if (currentPage != null)
            currentPage.text = currentPageIndex.ToString();

        if (totalPages != null)
            totalPages.text = "/" + totalPageCount.ToString();
    }

    private void ButtonSound()
    {
        audioManager.PlaySFX(audioManager.buttonHover);
    }

    private void ButtonError()
    {
        audioManager.PlaySFX(audioManager.error);
    }

    public void OnBackMainButtonEnter()
    {
        if (backButton != null)
        {
            backButton.sprite = hoverBack_Sprite;
            audioManager.PlaySFX(audioManager.buttonPressed);
        }
            
    }
    public void OnBackMainButtonExit()
    {
        if (backButton != null)
            backButton.sprite = normalBack_Sprite;
    }
    public void OnPreviousPageButtonEnter()
    {
        if (previousPageButton != null)
        {
            previousPageButton.sprite = hoverPreviousPage_Sprite;
            audioManager.PlaySFX(audioManager.buttonPressed);
        }
    }
    public void OnPreviousPageButtonExit()
    {
        if (previousPageButton != null)
            previousPageButton.sprite = normalPrevious_Sprite;
    }
    public void OnNextPageButtonEnter()
    {
        if (nextPageButton != null)
        {
            nextPageButton.sprite = hoverNextPage_Sprite;
            audioManager.PlaySFX(audioManager.buttonPressed);
        }
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
            ButtonSound();
            currentPageIndex--;
            currentPage.text = currentPageIndex.ToString();
        }
        else
        {
            ButtonError();
        }
    }

    public void OnClickNextPageButton()
    {
        if(nextPageButton != null && currentPageIndex < totalPageCount)
        {
            ButtonSound();
            currentPageIndex++;
            currentPage.text = currentPageIndex.ToString();
        }
        else
        {
            ButtonError();
        }
    }
}
