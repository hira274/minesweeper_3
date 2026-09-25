using UnityEngine;

public class BoardManager : MonoBehaviour
{
    public static BoardManager Instance;

    public int width = 5;
    public int height = 5;

    public float startX = -4.5f;
    public float startY = -3.5f;
    public float spacing = 1.1f;

    private PicrossCell[,] cells;

    private int correctTotal = 0;
    private int correctNow = 0;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        cells = new PicrossCell[width, height];

        var allCells = FindObjectsOfType<PicrossCell>();
        foreach (var c in allCells)
        {
            int col = Mathf.RoundToInt((c.transform.position.x - startX) / spacing);
            int row = Mathf.RoundToInt((c.transform.position.y - startY) / spacing);

            cells[col, row] = c;
        }

        CountMineNumbers();

        // 正解セルの総数を数える
        var correctCells = FindObjectsOfType<PicrossCorrectCell>();
        correctTotal = correctCells.Length;
    }

    // PicrossCorrectCell が呼ぶ
    public void AddCorrect()
    {
        correctNow++;
        CheckClear();
    }

    public void RemoveCorrect()
    {
        correctNow--;
    }

    void CheckClear()
    {
        if (correctNow == correctTotal)
        {
            Debug.Log("CLEAR!");
        }
    }

    // MineCell が呼ぶ
    public void GameOver()
    {
        Debug.Log("GAME OVER");

        var mines = FindObjectsOfType<MineCell>();
        foreach (var m in mines)
        {
            m.ShowMine();
        }
    }

    // マインスイーパー数字計算
    void CountMineNumbers()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                var cell = cells[x, y];

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
}
