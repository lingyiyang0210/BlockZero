using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    public int columns = 9;
    public int rows = 9;
    public float squaresGap = 5f;
    public GameObject gridSquare;
    public Vector2 startPosition = new Vector2(0f, 0f);
    public float squareScale = 1f;
    public float everySquareOffset = 0f;

    private Vector2 _offset = Vector2.zero;
    private List<GameObject> _gridSquares = new List<GameObject>();

    void Start()
    {
        CreateGrid();
    }

    private void CreateGrid()
    {
        SpawnGridSquares();
        SetGridSquaresPositions();
    }

    private void SpawnGridSquares()
    {
        int squareIndex = 0;

        for (int row = 0; row < rows; row++)
        {
            for (int column = 0; column < columns; column++)
            {
                GameObject square = Instantiate(gridSquare);
                square.transform.SetParent(transform, false);
                square.transform.localScale = Vector3.one * squareScale;

                square.GetComponent<GridSquare>().SetImage(squareIndex % 2 == 0);
                _gridSquares.Add(square);

                squareIndex++;
            }
        }
    }

    private void SetGridSquaresPositions()
    {
        int columnNumber = 0;
        int rowNumber = 0;

        Vector2 squareGapNumber = Vector2.zero;
        bool rowMoved = false;

        RectTransform squareRect = _gridSquares[0].GetComponent<RectTransform>();

        _offset.x = squareRect.rect.width * squareRect.localScale.x + everySquareOffset;
        _offset.y = squareRect.rect.height * squareRect.localScale.y + everySquareOffset;

        foreach (GameObject square in _gridSquares)
        {
            if (columnNumber >= columns)
            {
                columnNumber = 0;
                rowNumber++;
                squareGapNumber.x = 0;
                rowMoved = false;
            }

            float posXOffset = _offset.x * columnNumber + squareGapNumber.x * squaresGap;
            float posYOffset = _offset.y * rowNumber + squareGapNumber.y * squaresGap;

            if (columnNumber > 0 && columnNumber % 3 == 0)
            {
                squareGapNumber.x++;
                posXOffset += squaresGap;
            }

            if (rowNumber > 0 && rowNumber % 3 == 0 && !rowMoved)
            {
                squareGapNumber.y++;
                posYOffset += squaresGap;
                rowMoved = true;
            }

            RectTransform rect = square.GetComponent<RectTransform>();
            rect.anchoredPosition = new Vector2(
                startPosition.x + posXOffset,
                startPosition.y - posYOffset
            );

            columnNumber++;
        }
    }
}
