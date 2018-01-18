using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSweet : MonoBehaviour
{
    private int x;

    public int X
    {
        get { return x; }
        set
        {
            if (CanMove())
            {
                x = value;
            }
        }
    }

    private int y;

    public int Y
    {
        get { return y; }
        set
        {
            if (CanMove())
            {
                y = value;
            }
        }
    }

    public GameManager.SweetsType Type
    {
        get { return type; }
    }

    private MovedSweet moveComponent;

    public MovedSweet MoveComponent
    {
        get { return moveComponent; }
    }

    private ColorSweet coloredComponent;

    public ColorSweet ColoredComponent
    {
        get { return coloredComponent; }
    }
    //是否可以移动
    public bool CanMove()
    {
        return moveComponent != null;
    }
    public bool CanColor()
    {
        return coloredComponent != null;
    }

    private void Awake()
    {
        moveComponent = GetComponent<MovedSweet>();
        coloredComponent = GetComponent<ColorSweet>();
    }

    private GameManager.SweetsType type;
    [HideInInspector] public GameManager gameManager;

    public void Init(int _x, int _y, GameManager _gameManager, GameManager.SweetsType _type)
    {
        x = _x;
        y = _y;
        gameManager = _gameManager;
        type = _type;
    }

    // 当用户在 GUIElement 或碰撞器上按鼠标按钮时调用 OnMouseDown
    private void OnMouseDown()
    {
        gameManager.PressSweet(this);
    }

    // 当鼠标进入 GUIElement 或碰撞器时调用 OnMouseEnter
    private void OnMouseEnter()
    {
        gameManager.EnterSweet(this);
    }

    // 当用户松开鼠标按钮时调用 OnMouseUp
    private void OnMouseUp()
    {
        gameManager.ReleaseSweet();
    }
    

}