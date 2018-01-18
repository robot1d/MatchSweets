using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovedSweet : MonoBehaviour
{
    private IEnumerator moveCoroutine; //这样得到其他指令的时候我们可以终止这个协程

    private GameSweet sweet;

    private void Awake()
    {
        sweet = GetComponent<GameSweet>();
    }
    //移动方法,调用协程使移动时更加顺滑
    public void Move(int newX, int newY,float time)
    {
        if (moveCoroutine != null)
        {
            StopCoroutine(moveCoroutine);
        }
        moveCoroutine = MoveCoroutine(newX, newY, time);
        StartCoroutine(moveCoroutine);

    }

    private IEnumerator MoveCoroutine(int newX, int newY,float time)
    {
        sweet.X = newX;
        sweet.Y = newY;
        //每一帧移动一点点
        Vector3 startPos = transform.position;
        Vector3 endPos = sweet.gameManager.CorrectPositon(newX, newY);
        for (float t = 0; t < time; t+=Time.deltaTime)
        {
            sweet.transform.position = Vector3.Lerp(startPos,endPos,t/time);
            yield return 0;
        }
        sweet.transform.position = endPos;
    }
}