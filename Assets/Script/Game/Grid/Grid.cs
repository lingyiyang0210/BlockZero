using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    public ShapeStorage shapeStorage;
    public int columns = 9;
    public int rows = 9;
    public float squaresGap = 5f;
    public GameObject gridSquare;
    public Vector2 startPosition = new Vector2(0f, 0f);
    public float squareScale = 1f;
    public float everySquareOffset = 0f;

    private Vector2 _offset = Vector2.zero;
    private List<GameObject> _gridSquares = new List<GameObject>();

    private LineIndicator _lineIndicator;

    private void OnEnable()
    {
        GameEvents.CheckIfShapeCanBePlaced += CheckIfShapeCanBePlaced;
    }

    private void OnDisable()
    {
        GameEvents.CheckIfShapeCanBePlaced -= CheckIfShapeCanBePlaced;
    }

    void Start()
    {
        _lineIndicator = GetComponent<LineIndicator>();
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
        for (var row = 0; row < rows; row++)
        {
            for (var column = 0; column < columns; column++)
            {
                GameObject instance = Instantiate(gridSquare) as GameObject;
                _gridSquares.Add(instance);
                var gs = instance.GetComponent<GridSquare>();
                gs.SquareIndex = squareIndex;
                instance.transform.SetParent(this.transform);
                instance.transform.localScale = new Vector3(squareScale, squareScale, squareScale);
                gs.SetImage(_lineIndicator.GetGridSquareIndex(squareIndex) % 2 == 0);
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
            rect.anchoredPosition = new Vector2(startPosition.x + posXOffset, startPosition.y - posYOffset);
            columnNumber++;
        }
    }

    private void CheckIfShapeCanBePlaced()
    {
        var squareIndexes = new List<int>();
        foreach (var square in _gridSquares)
        {
            var gridSquare = square.GetComponent<GridSquare>();
            if (gridSquare.Selected && !gridSquare.SquareOccupied)
            {
                squareIndexes.Add(gridSquare.SquareIndex);
            }
        }

        var currentSelectedShape = shapeStorage.GetCurrentSelectedShape();
        if (currentSelectedShape == null) return;

        if (currentSelectedShape.TotalSquareNumber == squareIndexes.Count)
        {
            foreach (var squareIndex in squareIndexes)
            {
                _gridSquares[squareIndex].GetComponent<GridSquare>().PlaceShapeOnBoard();
            }
            currentSelectedShape.DeactivateShape();
            currentSelectedShape.gameObject.SetActive(false);
            
            ResetGridSquares();

            int shapeLeft = 0;
            foreach (var shape in shapeStorage.shapeList)
            {
                if (shape.gameObject.activeSelf) shapeLeft++;
            }
            if (shapeLeft == 0)
            {
                GameEvents.RequestNewShapes?.Invoke();
            }

            CheckIfAnyLineIsCompleted();
        }
        else
        {
            GameEvents.MoveShapeToStartPosition?.Invoke();
            ResetGridSquares();
        }
    }

    private void ResetGridSquares()
    {
        foreach (var square in _gridSquares)
        {
            var gs = square.GetComponent<GridSquare>();
            gs.Selected = false;
            gs.hooverImage.gameObject.SetActive(false);
        }
    }

    void CheckIfAnyLineIsCompleted()
    {
        List<int[]> lines = new List<int[]>();

        foreach(var column in _lineIndicator.columnIndexes)
        {
            lines.Add(_lineIndicator.GetVerticalLine(column));
        }

        for(var row = 0; row < 9; row++)
        {
            List<int> data = new List<int>(9);
            for(var index = 0; index < 9; index++)
            {
                data.Add(_lineIndicator.line_data[row, index]);
            }

            lines.Add(data.ToArray());
        }

        for(var square = 0; square < 9; square++)
        {
            List<int> data = new List<int>(9);
            for(var index = 0; index < 9; index++)
            {
                data.Add(_lineIndicator.square_data[square, index]);
            }

            lines.Add(data.ToArray());
        }

        var completedLines = CheckIfSquaresAreCompleted(lines);

        var totalScores = 10 * completedLines;
        GameEvents.AddScores?.Invoke(totalScores);
        CheckIfPlayerLost();
    }

    private int CheckIfSquaresAreCompleted(List<int[]> data)
    {
        List<int[]> completedLines = new List<int[]>();
        var linesCompleted = 0;

        foreach(var line in data)
        {
            var lineCompleted = true;
            foreach(var squareIndex in line)
            {
                var comp = _gridSquares[squareIndex].GetComponent<GridSquare>();
                if(comp.SquareOccupied == false)
                {
                    lineCompleted = false;
                }  
            }

            if(lineCompleted)
            {
                completedLines.Add(line);
            }
        }

        foreach(var line in completedLines)
        {
            foreach(var squareIndex in line)
            {
                var comp = _gridSquares[squareIndex].GetComponent<GridSquare>();
                comp.Deactivate();
                comp.ClearOccupied();
            }
            linesCompleted++;
        }

        return linesCompleted;
    }

    private void CheckIfPlayerLost()
    {
        var validShapes = 0;

        for(var index = 0; index < shapeStorage.shapeList.Count; index++)
        {
            var currentShape = shapeStorage.shapeList[index];
            var isShapeActive = currentShape.IsAnyOfShapeSquareActive();
            if(isShapeActive && CheckIfShapeCanBePlacedOnGrid(currentShape))
            {
                currentShape.ActivateShape(); 
                validShapes++;
            }
        }

        if(validShapes == 0)
        {
            GameEvents.GameOver?.Invoke(false);
        }
    }

    private bool CheckIfShapeCanBePlacedOnGrid(Shape currentShape)
    {
        var currentShapeData = currentShape.CurrentShapeData;
        var shapeColumns = currentShapeData.columns;
        var shapeRows = currentShapeData.rows;

        //All indexex of filled up squares
        List<int> originalShapeFilledUpSquares = new List<int>();
        var squareIndex = 0;

        for(var rowIndex = 0; rowIndex < shapeRows; rowIndex++)
        {
            for(var columnIndex = 0; columnIndex < shapeColumns; columnIndex++)
            {
                if(currentShapeData.board[rowIndex].column[columnIndex])
                {
                    originalShapeFilledUpSquares.Add(squareIndex);
                }
                squareIndex++;
            }
        }

        var squareList = GetAllSquaresCombination(shapeColumns, shapeRows);
        bool canBePlaced = false;

        foreach(var combination in squareList)
        {
            bool shapeCanBePlacedOnTheBoard = true; 
            foreach(var squareIndexToCheck in originalShapeFilledUpSquares)
            {
                var comp = _gridSquares[combination[squareIndexToCheck]].GetComponent<GridSquare>();
                if(comp.SquareOccupied)
                {
                    shapeCanBePlacedOnTheBoard = false;
                    break;
                }
            }

            if(shapeCanBePlacedOnTheBoard)
            {
                canBePlaced = true;
                break;
            }
        }

        return canBePlaced;
    }

    private List<int[]> GetAllSquaresCombination(int shapeColumns, int shapeRows)
    {
        var squareList = new List<int[]>();
        var lastColumnIndex = 0;
        var lastRowIndex = 0;
        int safeIndex = 0;

        while(lastRowIndex + (shapeRows - 1) < 9)
        {
            var rowData = new List<int>();

            for(var row = lastRowIndex; row < lastRowIndex + shapeRows; row++)
            {
                for(var col = lastColumnIndex; col < lastColumnIndex + shapeColumns; col++)
                {
                    rowData.Add(_lineIndicator.line_data[row, col]);
                }
            }

            squareList.Add(rowData.ToArray());
            lastColumnIndex++;

            if(lastColumnIndex + (shapeColumns - 1) >= 9)
            {
                lastRowIndex++;
                lastColumnIndex = 0;
            }

            safeIndex++;
            if(safeIndex > 100)
            {
                break;
            }
        }

        return squareList;
    }
}