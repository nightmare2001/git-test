using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
public class AStarGrid : MonoBehaviour, IPointerClickHandler
{
    public int x;
    public int y;

    public bool isStart;
    public bool isEnd;
    public bool isObstacle;
    public Image img;
    public TextMeshProUGUI gTxt;
    public TextMeshProUGUI hTxt;
    public TextMeshProUGUI fTxt;



    private void Awake()
    {
        img = GetComponent<Image>();
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if (AStar.Inst.isFinding) return;
        if(Input.GetKey(KeyCode.S))//start
        {
            AStar.Inst.SetStart(x, y);
        }
        else if(Input.GetKey(KeyCode.O))//obstacle
        {
            AStar.Inst.SetObstacle(x, y);
        }
        else if(Input.GetKey(KeyCode.E))//end
        {
            AStar.Inst.SetEnd(x, y);
        }
    }

    
}
