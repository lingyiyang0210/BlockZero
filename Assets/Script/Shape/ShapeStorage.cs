using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShapeStorage : MonoBehaviour
{
    public List<ShapeData> shapeData;
    public List<Shape> shapeList;
    public SquareTextureData squareTextureData;

    private Config.SquareColor _currentShapeColor;

    private void OnEnable()
    {
        GameEvents.RequestNewShapes += RequestNewShapes;
        GameEvents.UpdateSquareColor += OnUpdateSquareColor;
    }

    private void OnDisable()
    {
        GameEvents.RequestNewShapes -= RequestNewShapes;
        GameEvents.UpdateSquareColor -= OnUpdateSquareColor;
    }

    void Start()
    {
        _currentShapeColor = squareTextureData.activeSquareTextures[0].squareColor;

        foreach (var shape in shapeList)
        {
            var shapeIndex = UnityEngine.Random.Range(0, shapeData.Count);
            shape.CreateShape(shapeData[shapeIndex]);
        }
    }

    private void OnUpdateSquareColor(Config.SquareColor color)
    {
        _currentShapeColor = color;
    }

    public Shape GetCurrentSelectedShape()
    {
        foreach (var shape in shapeList)
        {
            if (shape.IsOnStartPosition() == false && shape.IsAnyOfShapeSquareActive())
                return shape;
        }
        return null;
    }

    private void RequestNewShapes()
    {
        foreach (var shape in shapeList)
        {
            var shapeIndex = UnityEngine.Random.Range(0, shapeData.Count);

            shape.gameObject.SetActive(true);
            shape.RequestNewShape(shapeData[shapeIndex]);
            shape.ActivateShape();
        }
    }

    public void ResetStorage()
    {
        Config.SquareColor initialColor = squareTextureData.activeSquareTextures[0].squareColor;
        _currentShapeColor = initialColor;

        GameEvents.UpdateSquareColor?.Invoke(initialColor);

        RequestNewShapes();
    }
}