using UnityEngine;

public class PicrossCell : MonoBehaviour
{
    [Header("セル設定")]
    public bool isNumberCell = false;

    public Sprite whiteSprite;
    public Sprite blueSprite;
    public Sprite crossSprite;
    public Sprite flagSprite;

    public Sprite[] numberSprites;
    protected SpriteRenderer sr;
    protected SpriteRenderer numberRenderer;

    protected int state = 0; // 0=白,1=青,2=×
    protected bool isFlagged = false;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();

        Transform numObj = transform.Find("Number");
        if (numObj != null)
        {
            numberRenderer = numObj.GetComponent<SpriteRenderer>();
        }

        UpdateSprite();
    }

    protected virtual void Update()
    {
        // 左クリック
        if (Input.GetMouseButtonDown(0))
        {
            if (IsMouseOver())
            {
                if (isFlagged) return;

                int oldState = state;

                // 状態を進める（0→1→2→0）
                state++;
                if (state > 2) state = 0;

                Debug.Log($"{name} oldState={oldState} newState={state}");

                // ★ 正解セルは missCount を操作しない
                if (!(this is PicrossCorrectCell))
                {
                    if (oldState == 0 && state == 1)
                    {
                        Debug.Log($"{name} MISS++");
                        BoardManager.Instance.AddMiss();
                    }
                    else if (oldState == 1 && state == 2)
                    {
                        Debug.Log($"{name} MISS--");
                        BoardManager.Instance.RemoveMiss();
                    }
                }

                UpdateSprite();
            }
        }

        // 右クリック（旗）
        if (Input.GetMouseButtonDown(1))
        {
            if (IsMouseOver())
            {
                isFlagged = !isFlagged;
                UpdateSprite();
            }
        }
    }

    protected bool IsMouseOver()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Collider2D col = Physics2D.OverlapPoint(mousePos);
        return col != null && col.gameObject == this.gameObject;
    }

    protected void UpdateSprite()
    {
        if (isFlagged)
        {
            sr.sprite = flagSprite;
            return;
        }

        switch (state)
        {
            case 0: sr.sprite = whiteSprite; break;
            case 1: sr.sprite = blueSprite; break;
            case 2: sr.sprite = crossSprite; break;
        }

        if (numberRenderer != null)
        {
            numberRenderer.enabled = isNumberCell;
        }

        Debug.Log($"{name} UpdateSprite state={state}");

        OnStateChanged(state);
    }

    public void SetNumber(int n)
    {
        if (!isNumberCell) return;
        if (numberRenderer == null) return;

        if (numberSprites != null && n >= 0 && n < numberSprites.Length)
        {
            numberRenderer.sprite = numberSprites[n];
        }
    }

    protected virtual void OnStateChanged(int newState)
    {
        Debug.Log($"{name} OnStateChanged newState={newState}");
    }

    public bool IsBlue() => state == 1;
}
