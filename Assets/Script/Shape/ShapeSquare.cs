using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShapeSquare : MonoBehaviour
{
    public Image occupiedImage;

    void Start()
    {
        occupiedImage.gameObject.SetActive(false);
    }

    public void DeactivateShape()
    {
        occupiedImage.gameObject.SetActive(false);
        gameObject.GetComponent<BoxCollider2D>().enabled = false;
        gameObject.SetActive(false);
    }

    public void ActivateShape()
    {
        occupiedImage.gameObject.SetActive(false);
        gameObject.GetComponent<BoxCollider2D>().enabled = true;
        gameObject.SetActive(true);
    }

    public void SetOccupied()
    {
        occupiedImage.gameObject.SetActive(true);
    }

    public void UnSetOccupied()
    {
        occupiedImage.gameObject.SetActive(false);
    }
}