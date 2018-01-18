using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorSweet : MonoBehaviour {

	public enum ColorType
	{
	    YELLOW,
        PURPLE,
        RED,
        BLUE,
        GREEN,
        PINK,
        ANY,
        COUNT
	}
    [Serializable]
    public struct ColorSprite
    {
        public ColorType color;
        public Sprite sprite;
    }

    private ColorType clolr;
    public ColorSprite[] ColorSprites;
    private Dictionary<ColorType,Sprite> colorSpriteDict ;

    private SpriteRenderer sprite;

    public int NumColors
    {
        get { return ColorSprites.Length; }
    }

    public ColorType Clolr
    {
        get
        {
            return clolr;
        }

        set
        {
            SetColor(value);
        }
    }

    private void Awake()
    {
        sprite = transform.Find("Sweet").GetComponent<SpriteRenderer>();
        colorSpriteDict = new Dictionary<ColorType, Sprite>();
        for (int i = 0; i < ColorSprites.Length; i++)
        {
            if (!colorSpriteDict.ContainsKey(ColorSprites[i].color))
            {
                colorSpriteDict.Add(ColorSprites[i].color,ColorSprites[i].sprite);
            }
        }
    }

    public void SetColor(ColorType newColor)
    {
        if (colorSpriteDict.ContainsKey(newColor))
        {
            sprite.sprite = colorSpriteDict[newColor];
            clolr = newColor;
        }
    }

}
