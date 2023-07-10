using UnityEngine;
using System.Collections;

public class Chip : MonoBehaviour
{
    public ChipType chipType;
    private int row;
    private int col;
    public float chipSize = 200f; 
    public float spacing = 10f; 
    public float moveDuration = 0.5f; 

    private void Start()
    {
        transform.localScale = new Vector3(chipSize, chipSize, 1f);
        UpdateChipPosition();
    }

    public void SetPosition(int row, int col)
    {
        this.row = row;
        this.col = col;
        UpdateChipPosition();
    }

    public void MoveToPosition(int newRow, int newCol)
    {
        StartCoroutine(MoveChip(newRow, newCol));
    }

    private IEnumerator MoveChip(int newRow, int newCol)
    {
        Vector2 startPosition = transform.localPosition;
        Vector2 endPosition = CalculateChipPosition(newRow, newCol);
        float elapsedTime = 0f;

        while (elapsedTime < moveDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / moveDuration);
            transform.localPosition = Vector2.Lerp(startPosition, endPosition, t);
            yield return null;
        }

        row = newRow;
        col = newCol;
        UpdateChipPosition();
    }

    private void UpdateChipPosition()
    {
        transform.localPosition = CalculateChipPosition(row, col);
    }

    private Vector2 CalculateChipPosition(int row, int col)
    {
        float xPos = col * (chipSize + spacing);
        float yPos = -row * (chipSize + spacing);
        return new Vector2(xPos, yPos);
    }
}

public enum ChipType
{
    Green,
    Yellow,
    Blue,
    Empty
}
