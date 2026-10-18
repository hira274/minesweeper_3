using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class BoardManager : MonoBehaviour
{
    public static BoardManager Instance;

    public int width = 5;
    public int height = 5;

    public float startX = -4.5f;
    public float startY = -3.5f;
    public float spacing = 1.1f;

    private PicrossCell[,] cells;

    public GameObject clearPanel;
    public GameObject gameOverPanel;

    private int correctTotal = 0;
    private int correctNow = 0;

    private int missCount = 0;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        StartCoroutine(InitBoard());
    }

    IEnumerator InitBoard()
    {
        yield return null;

        cells = new PicrossCell[width, height];

        var allCells = FindObjectsOfType<PicrossCell>();
        foreach (var c in allCells)
        {
            int col = Mathf.RoundToInt((c.transform.position.x - startX) / spacing);
            int row = Mathf.RoundToInt((c.transform.position.y - startY) / spacing);

            if (col < 0 || col >= width || row < 0 || row >= height)
            {
                Debug.Log("ƒZƒ‹‚ª”z—ñ”ÍˆÍŠO‚Å‚·: " + c.name);
                continue;
            }

            cells[col, row] = c;
        }

        var correctCells = FindObjectsOfType<PicrossCorrectCell>();
        correctTotal = correctCells.Length;

        CountMineNumbers();
    }

    void CountMineNumbers()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                var cell = cells[x, y];

                if (cell == null) continue;
                if (cell is MineCell) continue;

                int count = 0;

                for (int dx = -1; dx <= 1; dx++)
                {
                    for (int dy = -1; dy <= 1; dy++)
                    {
                        if (dx == 0 && dy == 0) continue;

                        int nx = x + dx;
                        int ny = y + dy;

                        if (nx < 0 || nx >= width || ny < 0 || ny >= height)
                            continue;

                        if (cells[nx, ny] is MineCell)
                            count++;
                    }
                }

                cell.SetNumber(count);
            }
        }
    }

    public void AddCorrect()
    {
        correctNow++;
        CheckClear();
    }

    public void RemoveCorrect()
    {
        correctNow--;
        CheckClear();
    }

    public void AddMiss()
    {
        missCount++;
        CheckClear();
    }

    public void RemoveMiss()
    {
        if (missCount > 0)
            missCount--;

        CheckClear();
    }

    void CheckClear()
    {
        if (correctNow == correctTotal && missCount == 0)
        {
            Debug.Log("CLEAR!");

            if (clearPanel != null)
                clearPanel.SetActive(true);
        }
    }

    public void GameOver()
    {
        Debug.Log("GAME OVER");

        var mines = FindObjectsOfType<MineCell>();
        foreach (var m in mines)
        {
            m.ShowMine();
        }

        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);
    }

    public void ReturnToTitle()
    {
        SceneManager.LoadScene("TitleScene");
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
