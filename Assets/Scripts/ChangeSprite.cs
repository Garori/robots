using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeSprite : MonoBehaviour
{
    public Sprite[] sprites;
    public int currentID;

    public void Start()
    {
        currentID = 0;
    }

    public void ChangeImage()
    {
        currentID = (currentID + 1) % sprites.Length;
        this.GetComponent<Image>().sprite = sprites[currentID];
    }
}