using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class ChipGame : MonoBehaviour
{
    public GameObject[] chipPrefabs;
    public GameObject emptyCellPrefab;

    public Transform fieldParent;
    public Transform chipParent;
    public Transform columnButtonParent; 
    public Transform rowButtonParent; 
    public Transform rowButtonParent2; 
    public GameObject columnButtonPrefab;
    
    public GameObject rowButtonPrefab; 
    public GameObject rowButtonPrefab2;

    private Chip[,] field;
    private int numRows = 4;
    private int numColumns = 9;
    private Button[] columnButtons;
    private Button[] rowButtons; 
    private Button[] rowButtons2; 

    private bool[] isColumnButtonToggleOn;

    private bool firstLineMatched = false;
    private bool secondLineMatched = false;
    private bool thirdLineMatched = false;

    private AudioManager sound;

    private void Awake()
    {
        sound = FindObjectOfType<AudioManager>();
    }

    private void Start()
    {
        field = new Chip[numRows, numColumns];
        columnButtons = new Button[numColumns];
        rowButtons = new Button[2]; 
        rowButtons2 = new Button[2]; 

        InitializeField();
        CreateColumnButtons();
        CreateRowButtons(rowButtonParent, rowButtons);
        CreateRowButtons2(rowButtonParent2, rowButtons2);
    }

    private void InitializeField()
{
    for (int row = 0; row < numRows; row++)
    {
        for (int col = 0; col < numColumns; col++)
        {
            GameObject chipObject;
            if (row < numRows - 1)
            {
                int randomIndex = Random.Range(0, chipPrefabs.Length);
                chipObject = Instantiate(chipPrefabs[randomIndex], fieldParent);
            }
            else
            {
                chipObject = Instantiate(emptyCellPrefab, fieldParent);
            }

            Chip chip = chipObject.GetComponent<Chip>();
            chip.SetPosition(row, col);
            field[row, col] = chip;
        }
    }

    for (int row = 0; row < numRows - 1; row++)
    {
        ShuffleRowChips(row);
    }

    isColumnButtonToggleOn = new bool[numColumns];
}

private void ShuffleRowChips(int row)
{
    List<Chip> chips = new List<Chip>();
    for (int col = 0; col < numColumns; col++)
    {
        Chip chip = field[row, col];
        chips.Add(chip);
    }

    // Shuffle the chips randomly
    for (int i = 0; i < chips.Count; i++)
    {
        int randomIndex = Random.Range(i, chips.Count);
        Chip temp = chips[i];
        chips[i] = chips[randomIndex];
        chips[randomIndex] = temp;
    }

    for (int col = 0; col < numColumns; col++)
    {
        Chip chip = chips[col];
        chip.SetPosition(row, col);
        field[row, col] = chip;
    }
}


    private void CreateColumnButtons()
    {
        for (int col = 0; col < numColumns; col++)
        {
            int column = col; 

            GameObject buttonObject = Instantiate(columnButtonPrefab, columnButtonParent);
            Button button = buttonObject.GetComponent<Button>();

            // Add click event handler for the button
            button.onClick.AddListener(() => OnColumnButtonClicked(column));

            columnButtons[col] = button;
        }
    }

    private void CreateRowButtons(Transform parent, Button[] buttons)
    {
        for (int row = 1; row <= 2; row++)
        {
            int currentRow = row; 

            GameObject buttonObject = Instantiate(rowButtonPrefab, parent);
            Button button = buttonObject.GetComponent<Button>();

            // Add click event handler for the button
            button.onClick.AddListener(() => OnRowButtonClicked(currentRow));

            buttons[row - 1] = button;
        }
    }

    private void CreateRowButtons2(Transform parent, Button[] buttons)
    {
        for (int row = 1; row <= 2; row++)
        {
            int currentRow = row; 

            GameObject buttonObject = Instantiate(rowButtonPrefab2, parent);
            Button button = buttonObject.GetComponent<Button>();

            // Add click event handler for the button
            button.onClick.AddListener(() => OnRowButtonClicked2(currentRow));

            buttons[row - 1] = button;
        }
    }

    private void CheckLineChips()
    {
        if (firstLineMatched && secondLineMatched && thirdLineMatched)
        {
            Debug.Log("All Three Lines Matched Correctly!");
            sound.Play("Victory");
            return;
        }

        bool[] lineMatched = new bool[3];
        for (int row = 0; row < 3; row++)
        {
            bool lineMatch = true;
            for (int col = 0; col < numColumns; col++)
            {
                if (field[row, col].chipType != chipPrefabs[row].GetComponent<Chip>().chipType)
                {
                    lineMatch = false;
                    break;
                }
            }

            lineMatched[row] = lineMatch;
            if (lineMatch && !GetLineMatched(row))
            {
                Debug.Log($"Line {row + 1} Match!");
                sound.Play("LineDeveloped");
                SetLineMatched(row, true);
            }
        }

        if (lineMatched[0] && lineMatched[1] && lineMatched[2])
        {
            Debug.Log("All Three Lines Matched Correctly!");
            sound.Play("Victory");
        }

    
        for (int row = 0; row < numRows; row++)
        {
            bool lineMatch = true;
            ChipType firstChipType = field[row, 0].chipType; 
            for (int col = 1; col < numColumns; col++) 
            {
                if (field[row, col].chipType != firstChipType)
                {
                    lineMatch = false;
                    break;
                }
            }

            if (lineMatch)
            {
                Debug.Log("Line Match!");
            }
        }
    }

    private bool GetLineMatched(int lineIndex)
    {
        switch (lineIndex)
        {
            case 0:
                return firstLineMatched;
            case 1:
                return secondLineMatched;
            case 2:
                return thirdLineMatched;
            default:
                return false;
        }
    }

    private void SetLineMatched(int lineIndex, bool matched)
    {
        switch (lineIndex)
        {
            case 0:
                firstLineMatched = matched;
                break;
            case 1:
                secondLineMatched = matched;
                break;
            case 2:
                thirdLineMatched = matched;
                break;
        }
    }

    private void MoveChipsLeft()
    {
        for (int row = 0; row < numRows; row++)
        {
            Chip firstChip = field[row, 0];
            for (int col = 0; col < numColumns - 1; col++)
            {
                field[row, col] = field[row, col + 1];
                field[row, col].MoveToPosition(row, col);
            }
            field[row, numColumns - 1] = firstChip;
            field[row, numColumns - 1].MoveToPosition(row, numColumns - 1);
        }
        CheckLineChips();
    }

    private void MoveChipsRight()
    {
        for (int row = 0; row < numRows; row++)
        {
            Chip lastChip = field[row, numColumns - 1];
            for (int col = numColumns - 1; col > 0; col--)
            {
                field[row, col] = field[row, col - 1];
                field[row, col].MoveToPosition(row, col);
            }
            field[row, 0] = lastChip;
            field[row, 0].MoveToPosition(row, 0);
        }
        CheckLineChips();
    }

    private void OnRowButtonClicked(int row)
{
    int numColumnsToMove = numColumns - 1;
    Chip firstChip = field[row, 0];
    for (int col = 0; col < numColumnsToMove; col++)
    {
        field[row, col] = field[row, col + 1];
        field[row, col].MoveToPosition(row, col);
    }
    field[row, numColumnsToMove] = firstChip;
    field[row, numColumnsToMove].MoveToPosition(row, numColumnsToMove);
    CheckLineChips();
}

private void OnRowButtonClicked2(int row)
{
    int numColumnsToMove = numColumns - 1;
    Chip lastChip = field[row, numColumnsToMove];
    for (int col = numColumnsToMove; col > 0; col--)
    {
        field[row, col] = field[row, col - 1];
        field[row, col].MoveToPosition(row, col);
    }
    field[row, 0] = lastChip;
    field[row, 0].MoveToPosition(row, 0);
    CheckLineChips();
}


    private void OnColumnButtonClicked(int column)
    {
        if (isColumnButtonToggleOn[column])
        {
            for (int step = 0; step < 3; step++)
            {
                for (int row = numRows - 2; row >= 0; row--)
                {
                    Chip currentChip = field[row + 1, column];
                    field[row + 1, column] = field[row, column];
                    field[row + 1, column].MoveToPosition(row + 1, column);
                    field[row, column] = currentChip;
                    field[row, column].MoveToPosition(row, column);
                }
            }
        }
        else
        {
            for (int step = 0; step < 3; step++)
            {
                for (int row = 0; row < numRows - 1; row++)
                {
                    Chip currentChip = field[row, column];
                    field[row, column] = field[row + 1, column];
                    field[row, column].MoveToPosition(row, column);
                    field[row + 1, column] = currentChip;
                    field[row + 1, column].MoveToPosition(row + 1, column);
                }
            }
        }

        isColumnButtonToggleOn[column] = !isColumnButtonToggleOn[column]; 
        CheckLineChips();
    }

    public void OnLeftButtonClicked()
    {
        MoveChipsLeft();
    }

    public void OnRightButtonClicked()
    {
        MoveChipsRight();
    }
}