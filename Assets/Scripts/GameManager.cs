using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    public enum SweetsType
    {
        EMPTY,
        NORMAL,
        BARRIER,
        ROW_CLEAR,
        CONUMN_CLEAR,
        RAINBOWCANDY,
        COUNT //标记类型
    }

    public Vector3 CorrectPositon(int newX, int newY)
    {
        return new Vector3(newX - 4.5f, newY + 4.5f - newY * 2);
    }

    //甜品的预设体字典,我们可以通过甜品的种类得到相应的甜品字典
    private Dictionary<SweetsType, GameObject> sweetPrefabDict;

    [Serializable]
    public struct SweetPrefab
    {
        public SweetsType type;
        public GameObject prefab;
    }

    public SweetPrefab[] sweetPrefabs;

    public static GameManager _instance;

    //糖果
    private GameSweet[,] sweets;

    //糖果存放路径
    private Transform sweetsTransform;

    //大网格的行列
    public int xColumn;

    public int yRow;
    private GameObject gridPrefab;
    private Transform grids;
    public float fileTime;

    private GameSweet pressedSweet;
    private GameSweet enterSweet;

    private void Awake()
    {
        _instance = this;
        //背景格子存放路径
        grids = GameObject.Find("Grids").transform;
        //糖果存放路径
        sweetsTransform = GameObject.Find("Sweets").transform;
    }

    void Start()
    {
        #region 糖果字典的初始化

        sweetPrefabDict = new Dictionary<SweetsType, GameObject>();
        for (int i = 0; i < sweetPrefabs.Length; i++)
        {
            if (!sweetPrefabDict.ContainsKey(sweetPrefabs[i].type))
            {
                sweetPrefabDict.Add(sweetPrefabs[i].type, sweetPrefabs[i].prefab);
            }
        }

        #endregion

        #region 格子初始化

        gridPrefab = Resources.Load("Grid") as GameObject;
        for (int x = 0; x < 10; x++)
        {
            for (int y = 0; y < 10; y++)
            {
                GameObject chocolate = Instantiate(gridPrefab, new Vector3(x - 4.5f, y + 4.5f - y * 2, 0),
                    Quaternion.identity);
                chocolate.transform.parent = grids;
            }
        }

        #endregion

        sweets = new GameSweet[xColumn, yRow];
        for (int x = 0; x < 10; x++)
        {
            for (int y = 0; y < 10; y++)
            {
                /* GameObject newSweet = Instantiate(sweetPrefabDict[SweetsType.NORMAL],
                     new Vector3(x - 4.5f, y + 4.5f - y * 2, 0), Quaternion.identity);
                 newSweet.transform.parent = sweetsTransform;
                 sweets[x, y] = newSweet.GetComponent<GameSweet>();
                 sweets[x, y].Init(x, y, this, SweetsType.NORMAL);
                 if (sweets[x,y].CanColor())
                 {
                     sweets[x,y].ColoredComponent.SetColor((ColorSweet.ColorType)(Random.Range(0, sweets[x, y].ColoredComponent.NumColors)));
                 }*/
                CreateNewSweet(x, y, SweetsType.EMPTY);
            }
        }
        Destroy(sweets[4, 4].gameObject);
        CreateNewSweet(4, 4, SweetsType.BARRIER);
        StartCoroutine(AllFill());
//        AllFill();
    }


    //产生甜品的方法
    public GameSweet CreateNewSweet(int x, int y, SweetsType type)
    {
        GameObject newSweet = Instantiate(sweetPrefabDict[type], CorrectPositon(x, y), Quaternion.identity);
        newSweet.transform.SetParent(sweetsTransform);
        sweets[x, y] = newSweet.GetComponent<GameSweet>();
        sweets[x, y].Init(x, y, this, type);
        return sweets[x, y];
    }

    //全部填充
    public IEnumerator AllFill()
    {
        while (Fill())
        {
            yield return new WaitForSeconds(fileTime);
        }
    }

    public bool Fill()
    {
        bool filledNotFinished = false;
        for (int y = yRow - 2; y >= 0; y--)
        {
            for (int x = 0; x < xColumn; x++)
            {
                GameSweet sweet = sweets[x, y]; //得到当前原始的位置
                if (sweet.CanMove()) //如果可以移动
                {
                    GameSweet sweetBelow = sweets[x, y + 1];
                    if (sweetBelow.Type == SweetsType.EMPTY)
                    {
                        sweet.MoveComponent.Move(x, y + 1, fileTime);
                        sweets[x, y + 1] = sweet;
                        CreateNewSweet(x, y, SweetsType.EMPTY);
                        filledNotFinished = true;
                    }
                    else
                    {
                        //-1是左边,1是右边
                        for (int down = -1; down <= 1; down++)
                        {
                            if (down != 0)
                            {
                                int downX = x + down;
                                //如果不是最左边或者最后边
                                if (downX >= 0 && downX < xColumn)
                                {
                                    GameSweet downSweet = sweets[downX, y + 1];
                                    //如果为空
                                    if (downSweet.Type == SweetsType.EMPTY)
                                    {
                                        bool canfill = true; //用来判断垂直填充是否可以满足填充需求
                                        for (int aboveY = y; aboveY >= 0; aboveY--)
                                        {
                                            GameSweet sweetAbove = sweets[downX, aboveY];
                                            if (sweetAbove.CanMove())
                                            {
                                                break;
                                            }
                                            else if (!sweetAbove.CanMove() && sweetAbove.Type != SweetsType.EMPTY)
                                            {
                                                canfill = false;
                                                break;
                                            }
                                        }
                                        if (!canfill)
                                        {
                                            Destroy(downSweet.gameObject);
                                            sweet.MoveComponent.Move(downX, y + 1, fileTime);
                                            sweets[downX, y + 1] = sweet;
                                            CreateNewSweet(x, y, SweetsType.EMPTY);
                                            filledNotFinished = true;
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        //最上排特殊情况,生成一行糖块
        for (int x = 0; x < xColumn; x++)
        {
            GameSweet sweet = sweets[x, 0]; //获取当前糖块
            if (sweet.Type == SweetsType.EMPTY) //如果当前位置糖块为空
            {
                //创建一块糖块,设置糖块的位置为-1行
                GameObject newSweet =
                    Instantiate(sweetPrefabDict[SweetsType.NORMAL], CorrectPositon(x, -1), Quaternion.identity);
                //设置糖块的父物体
                newSweet.transform.parent = sweetsTransform;

                sweets[x, 0] = newSweet.GetComponent<GameSweet>();
                sweets[x, 0].Init(x, -1, this, SweetsType.NORMAL);
                sweets[x, 0].MoveComponent.Move(x, 0, fileTime);
                sweets[x, 0].ColoredComponent
                    .SetColor((ColorSweet.ColorType) Random.Range(0, sweets[x, 0].ColoredComponent.NumColors));
                filledNotFinished = true;
            }
        }
        return filledNotFinished;
    }

    //判断是否相邻
    private bool IsFriend(GameSweet sweet1, GameSweet sweet2)
    {
        return sweet1.X == sweet2.X && Mathf.Abs(sweet1.Y - sweet2.Y) == 1 ||
               (sweet1.Y == sweet2.Y && Mathf.Abs(sweet1.X - sweet2.X) == 1);
    }

    //交换两甜品
    private void ExchangeSweets(GameSweet sweet1, GameSweet sweet2)
    {
        if (sweet1.CanMove() && sweet2.CanMove())
        {
            sweets[sweet1.X, sweet1.Y] = sweet2;
            sweets[sweet2.X, sweet2.Y] = sweet1;

            int TempX = sweet1.X;
            int TempY = sweet1.Y;

            sweet1.MoveComponent.Move(sweet2.X, sweet2.Y, fileTime);
            sweet2.MoveComponent.Move(TempX, TempY, fileTime);
        }
    }

    public void PressSweet(GameSweet sweet)
    {
        pressedSweet = sweet;
    }

    public void EnterSweet(GameSweet sweet)
    {
        enterSweet = sweet;
    }

    public void ReleaseSweet()
    {
        if (IsFriend(pressedSweet, enterSweet))
        {
            ExchangeSweets(pressedSweet, enterSweet);
        }
    }
}