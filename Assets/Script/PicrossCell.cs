using UnityEngine;

public class PicrossCell : MonoBehaviour
{
    public Sprite whiteSprite;
    public Sprite blueSprite;
    public Sprite crossSprite;
    public Sprite flagSprite;

    public Sprite[] numberSprites; // 0〜8 の数字画像
    protected SpriteRenderer sr;
    protected SpriteRenderer numberRenderer;

    protected int state = 0; // 0=白,1=青,2=×
    protected bool isFlagged = false;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();

        // 子オブジェクト "Number" を探す（安全版）
        Transform numObj = transform.Find("Number");
        if (numObj != null)
        {
            numberRenderer = numObj.GetComponent<SpriteRenderer>();
        }
        else
        {
            numberRenderer = null; // 見つからない場合は null のまま
        }

        UpdateSprite();
    }

    protected void Update()
    {
        // 左クリック
        if (Input.GetMouseButtonDown(0))
        {
            if (IsMouseOver())
            {
                if (isFlagged) return;

                state++;
                if (state > 2) state = 0;

                UpdateSprite();
            }
        }

        // 右クリック
        if (Input.GetMouseButtonDown(1))
        {
            if (IsMouseOver())
            {
                isFlagged = !isFlagged;
                UpdateSprite();
            }
        }
    }

    bool IsMouseOver()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Collider2D col = Physics2D.OverlapPoint(mousePos);
        return col != null && col.gameObject == this.gameObject;
    }

    void UpdateSprite()
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

        OnStateChanged(state);
    }

    // BoardManager が数字をセットするためのメソッド
    public void SetNumber(int n)
    {
        Debug.Log($"SetNumber called: {n}");
        // numberRenderer が null の場合は何もしない（安全）
        if (numberRenderer == null) return;

        if (numberSprites != null && n >= 0 && n < numberSprites.Length)
        {
            numberRenderer.sprite = numberSprites[n];
        }
    }

    // PicrossCorrectCell や MineCell が override するためのメソッド
    protected virtual void OnStateChanged(int newState) { }

    public bool IsBlue() => state == 1;
}
