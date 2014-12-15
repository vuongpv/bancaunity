using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIHelpNavigator : MonoBehaviour
{
    const float DRAG_SCROLLING_SPEED = 50.0f;
    const float AUTO_SCROLLING_SPEED = 2500.0f;

    const int TOTAL_PAGES = 5;
    const int PAGE_WIDTH = 920;
    const int PAGER_WIDTH = 40;
    const int DRAG_THRESHOLD_WIDTH = 0;
    
    public UIHelpHandler controller;
    public Transform pageContainer;
    public Transform pagerContainer;
    public GameObject arrowLeft;
    public GameObject arrowRight;

    public int[] defaultHelp;
    public int[] singleHelp;
    public int[] onlineHelp;
    public int[] multiHelp;

    bool isScrolling;
    float finishX;
    Vector3 direction;

    GameObject[] pages;
    UISprite[] pagers;

    int[] currentHelp;
    float posXMin, posXMax;
    int numberPages, currentPage;
    Vector3 currentPosition, pressedPosition;

    void Awake()
    {
        pages = new GameObject[TOTAL_PAGES];
        pagers = new UISprite[TOTAL_PAGES];

        for (int i = 0; i < TOTAL_PAGES; i++)
        {
            pages[i] = pageContainer.transform.FindChild("Page" + i.ToString()).gameObject;
            pagers[i] = pagerContainer.transform.FindChild("Pager" + i.ToString()).GetComponentInChildren<UISprite>();
        }
    }

    public void Setup()
    {
        GetCurrentHelp();
        if (currentHelp == null)
            return;

        gameObject.collider.enabled = true;

        for (int i = 0; i < TOTAL_PAGES; i++)
        {
            pages[i].SetActiveRecursively(false);
            pagers[i].gameObject.SetActiveRecursively(false);
        }

        numberPages = currentHelp.Length;

        int pagePosX = 0;
        int pagerPosX = 0;
        for (int i = 0; i < currentHelp.Length; i++)
        {
            int index = currentHelp[i];

            pages[index].SetActiveRecursively(true);
            pages[index].transform.localPosition = new Vector3(pagePosX, 0.0f, 0.0f);
            pagePosX += PAGE_WIDTH;

            pagers[index].gameObject.SetActiveRecursively(true);
            pagers[index].transform.localPosition = new Vector3(pagerPosX, 0.0f, 0.0f);
            pagerPosX += PAGER_WIDTH;
        }

        pagerContainer.transform.localPosition = new Vector3(-numberPages * PAGER_WIDTH / 2, pagerContainer.transform.localPosition.y, 0.0f);

        arrowLeft.SetActiveRecursively(true);
        arrowRight.SetActiveRecursively(true);

        posXMin = -(numberPages - 1) * PAGE_WIDTH;
        posXMax = 0;
        currentPage = 0;
        currentPosition = Vector3.zero;
        pageContainer.localPosition = currentPosition;

        UpdateCurrentPage();
        SetPageStatus(0, true);

        isScrolling = false;
    }

    void GetCurrentHelp()
    {
        currentHelp = null;

        switch (Application.loadedLevelName)
        {
            case FHScenes.Single:
                currentHelp = singleHelp;
                break;

            case FHScenes.Online:
                currentHelp = onlineHelp;
                break;

            case FHScenes.Multi:
                currentHelp = multiHelp;
                break;

            default:
                currentHelp = defaultHelp;
                break;
        }

        if (currentHelp == null || currentHelp.Length < 1)
            currentHelp = defaultHelp;

        gameObject.collider.enabled = !(currentHelp.Length == 1);
    }

    void SetPageStatus(int page, bool active)
    {
        pagers[currentHelp[page]].spriteName = (active ? "multi_tutorial_map_on" : "multi_tutorial_map_off");
    }

    public void ResetOnOpen()
    {
        for (int i = 0; i < numberPages;i++)
        {
            currentPage = i;
            SetPageStatus(currentPage, false);
        }
        currentPage = 0;
        SetPageStatus(currentPage, true);
        UpdateCurrentPage();
    }
    void UpdateCurrentPage()
    {
        int page = (int)(-currentPosition.x) / PAGE_WIDTH;
        if (currentPage != page)
        {
            for (int i = 0; i < numberPages - 1;i++)
            {
                currentPage = i;
                SetPageStatus(currentPage, false);
            }
            SetPageStatus(currentPage, false);
            SetPageStatus(page, true);
            currentPage = page;

        }

        if (page == 0)
            arrowLeft.SetActiveRecursively(false);
        else
        if (page == numberPages - 1)
            arrowRight.SetActiveRecursively(false);
        else
        if (numberPages > 1)
        {
            arrowLeft.SetActiveRecursively(true);
            arrowRight.SetActiveRecursively(true);
        }
    }

    void OnPress(bool isPressed)
    {
        if (isPressed)
        {
            pressedPosition = pageContainer.localPosition;
            currentPosition = pageContainer.localPosition;
        }
        else
        if (!isPressed)
        {
            int posX = (int)(-currentPosition.x);
            int page = posX / PAGE_WIDTH;
            int remain = posX % PAGE_WIDTH;

            if ((currentPosition.x < pressedPosition.x && remain >= DRAG_THRESHOLD_WIDTH) || (currentPosition.x > pressedPosition.x && remain >= PAGE_WIDTH - DRAG_THRESHOLD_WIDTH))
            {
                page++;
                if (page >= numberPages)
                    page = numberPages - 1;
            }

            finishX = -page * PAGE_WIDTH;

            if (finishX < currentPosition.x)
                direction = -Vector3.right;
            else
                direction = Vector3.right;

            isScrolling = true;

            //Debug.LogError("Page = " + page);
            //Debug.LogError("Direction = " + direction);
            //Debug.LogError("Finish X = " + finishX);
        }
    }
    
    void OnDrag(Vector2 delta)
    {
        currentPosition += delta.x * Vector3.right * DRAG_SCROLLING_SPEED * Time.deltaTime;
        
        if (currentPosition.x <= posXMin)
            currentPosition.x = posXMin;
        else
        if (currentPosition.x >= posXMax)
            currentPosition.x = posXMax;

        pageContainer.localPosition = currentPosition;
    }

    void Update()
    {
        if (!isScrolling)
            return;

        currentPosition += direction * AUTO_SCROLLING_SPEED * Time.deltaTime;

        if ((direction == Vector3.right && currentPosition.x >= finishX) || (direction == -Vector3.right && currentPosition.x <= finishX))
        {
            currentPosition.x = finishX;
            isScrolling = false;
        }

        pageContainer.localPosition = currentPosition;
        UpdateCurrentPage();
    }

    public void MoveLeft()
    {
        OnPress(true);

        currentPosition.x += 1.0f;
        pageContainer.localPosition = currentPosition;

        OnPress(false);
    }

    public void MoveRight()
    {
        OnPress(true);

        currentPosition.x -= 1.0f;
        pageContainer.localPosition = currentPosition;

        OnPress(false);
    }
}