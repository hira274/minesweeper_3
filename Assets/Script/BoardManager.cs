using UnityEngine;
using UnityEngine.SceneManagement;

public class BoardManager : MonoBehaviour
{
    public static BoardManager Instance;

    public int width = 5;
    public int height = 5;

    public float startX = -4.5f;
    public float startY = -3.5f;
    public float spacing = 1.1f;

    private PicrossCell[,] cells;

    // クリア演出用
    public GameObject clearPanel;

    // 正解セル数
    private int correctTotal = 0;
    private int correctNow = 0;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        cells = new PicrossCell[width, height];

        // Scene 上のセルを読み取って 2次元配列に格納
        var allCells = FindObjectsOfType<PicrossCell>();
        foreach (var c in allCells)
        {
            int col = Mathf.RoundToInt((c.transform.position.x - startX) / spacing);
            int row = Mathf.RoundToInt((c.transform.position.y - startY) / spacing);

            // 配列範囲外なら無視（ログだけ出す）
            if (col < 0 || col >= width || row < 0 || row >= height)
            {
                Debug.Log("セルが配列範囲外です: " + c.name + " pos=" + c.transform.position);
                continue;
            }

            cells[col, row] = c;
        }

        // 正解セルの総数を数える（配列に入らなくても数える）
        var correctCells = FindObjectsOfType<PicrossCorrectCell>();
        correctTotal = correctCells.Length;
        Debug.Log("CorrectTotal = " + correctTotal);

        CountMineNumbers();
    }

    // マインスイーパー数字を計算して PicrossCell にセット
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

                // 周囲8マスを調べる
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

    // PicrossCorrectCell が青になったら呼ばれる
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

            if (clearPanel != null)
                clearPanel.SetActive(true);
        }
    }

    // MineCell が青になったら呼ばれる
    public void GameOver()
    {
        Debug.Log("GAME OVER");

        var mines = FindObjectsOfType<MineCell>();
        foreach (var m in mines)
        {
            m.ShowMine();
        }
    }

    // タイトルへ戻るボタン用
    public void ReturnToTitle()
    {
        SceneManager.LoadScene("TitleScene");
    }
}
