using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShapeStorage : MonoBehaviour
{
    public List<ShapeData> shapeData;
    public List<Shape> shapeList;
    public SquareTextureData squareTextureData;

    public static Config.SquareColor CurrentColor;

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
        squareTextureData.SetStartColor();
        CurrentColor = squareTextureData.currentColor;

        foreach (var shape in shapeList)
        {
            var shapeIndex = UnityEngine.Random.Range(0, shapeData.Count);
            shape.CreateShape(shapeData[shapeIndex]);
        }
    }

    private void OnUpdateSquareColor(Config.SquareColor color)
    {
        CurrentColor = color;
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

    public void RequestNewShapes()
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
        squareTextureData.SetStartColor();

        CurrentColor = squareTextureData.currentColor;

        GameEvents.UpdateSquareColor?.Invoke(CurrentColor);

        RequestNewShapes();
    }
}