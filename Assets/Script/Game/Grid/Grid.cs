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
            CheckIfAnyLineIsFull();
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

    private void CheckIfAnyLineIsFull()
    {
        
    }

    void CheckIfAnyLineIsCompleted()
    {
        List<int[]> lines = new List<int[]>();

        //columns
        foreach(var column in _lineIndicator.columnIndexes)
        {
            lines.Add(_lineIndicator.GetVerticalLine(column));
        }

        //rows
        for(var row = 0; row < 9; row++)
        {
          List<int> data = new List<int>(9);
          for(var index = 0; index < 9; index++)
            {
                data.Add(_lineIndicator.line_data[row, index]);
            }

            lines.Add(data.ToArray());
        }

        var completedLines = CheckIfSquaresAreCompleted(lines);

        if(completedLines > 2)
        {
            //TODO:PLAY BONUS ANIMATION
        }
        //TODO:ADD SCORES

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
            var completed = false;

            foreach(var squareIndex in line)
            {
                var comp = _gridSquares[squareIndex].GetComponent<GridSquare>();
                comp.Deactivate();
                completed = true;
            }

            foreach(var squareIndex in line)
            {
                var comp = _gridSquares[squareIndex].GetComponent<GridSquare>();
                comp.ClearOccupied();
            }

            if(completed)
            {
                linesCompleted++;
            }
        }

        return linesCompleted;
    }
}