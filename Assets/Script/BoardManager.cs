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

    // �N���A���o�p
    public GameObject clearPanel;

    // GAME OVER �p�p�l��
    public GameObject gameOverPanel;

    // �����Z����
    private int correctTotal = 0;
    private int correctNow = 0;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        // �� 1�t���[���҂��Ă���Ֆʂ��������iRestart��̃Y���h�~�j
        StartCoroutine(InitBoard());
    }

    IEnumerator InitBoard()
    {
        // �� Transform �����肷��܂� 1 �t���[���҂�
        yield return null;

        cells = new PicrossCell[width, height];

        // Scene ��̃Z����ǂݎ���� 2�����z��Ɋi�[
        var allCells = FindObjectsOfType<PicrossCell>();
        foreach (var c in allCells)
        {
            int col = Mathf.RoundToInt((c.transform.position.x - startX) / spacing);
            int row = Mathf.RoundToInt((c.transform.position.y - startY) / spacing);

            // �z��͈͊O�Ȃ疳���i���O�����o���j
            if (col < 0 || col >= width || row < 0 || row >= height)
            {
                Debug.Log("�Z�����z��͈͊O�ł�: " + c.name + " pos=" + c.transform.position);
                continue;
            }

            cells[col, row] = c;
        }

        // �����Z���̑����𐔂���
        var correctCells = FindObjectsOfType<PicrossCorrectCell>();
        correctTotal = correctCells.Length;
        Debug.Log("CorrectTotal = " + correctTotal);

        CountMineNumbers();
    }

    // �}�C���X�C�[�p�[�������v�Z���� PicrossCell �ɃZ�b�g
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

                // ����8�}�X�𒲂ׂ�
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

    // PicrossCorrectCell ���ɂȂ�����Ă΂��
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

    // MineCell �����܂ꂽ��Ă΂��
    public void GameOver()
    {
        Debug.Log("GAME OVER");

        // �n����S���\��
        var mines = FindObjectsOfType<MineCell>();
        foreach (var m in mines)
        {
            m.ShowMine();
        }

        // GAME OVER �p�l����\��
        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);
    }

    // �^�C�g���֖߂�{�^���p
    public void ReturnToTitle()
    {
        SceneManager.LoadScene("TitleScene");
    }

    // ���߂����蒼���{�^���p
    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
