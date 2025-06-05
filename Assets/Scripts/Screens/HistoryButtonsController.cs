using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

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

    private bool canPlayErrorSound = true;

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
        // Reset normal sprite
        if (backButton != null)
            backButton.sprite = normalBack_Sprite;

        if (previousPageButton != null)
            previousPageButton.sprite = normalPrevious_Sprite;

        if (nextPageButton != null)
            nextPageButton.sprite = normalNextPage_Sprite;

        StartCoroutine(DelayedPageLoad());
    }

    private IEnumerator DelayedPageLoad()
    {
        yield return null; // wait one frame

        if (saveSystem != null)
        {
            currentPageIndex = 1;
            int totalEntries = saveSystem.TotalEntries;
            int entriesPerPage = saveSystem.EntriesPerPage;
            totalPageCount = (totalEntries / entriesPerPage) + 1;

            currentPage.text = currentPageIndex.ToString();
            totalPages.text = "/" + totalPageCount.ToString();

            saveSystem.LoadPage(currentPageIndex);
        }
    }

    private void ButtonSound()
    {
        audioManager.PlaySFX(audioManager.buttonPressed);
    }

    private void ButtonError()
    {
        if (canPlayErrorSound)
        {
            audioManager.PlaySFX(audioManager.error);
            StartCoroutine(ErrorSoundCooldown());
        }
    }

    private IEnumerator ErrorSoundCooldown()
    {
        canPlayErrorSound = false;
        yield return new WaitForSeconds(0.5f);
        canPlayErrorSound = true;
    }

    public void OnBackMainButtonEnter()
    {
        if (backButton != null)
        {
            backButton.sprite = hoverBack_Sprite;
            audioManager.PlaySFX(audioManager.buttonHover);
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
            audioManager.PlaySFX(audioManager.buttonHover);
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
            audioManager.PlaySFX(audioManager.buttonHover);
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
