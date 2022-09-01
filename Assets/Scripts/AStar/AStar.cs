using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
    //当前节点到起点
    public int gVal;
    //当前节点到终点
    public int hVal;

    public int fVal;
    //当前节点是由哪个节点转移过来的
    public (int x,int y) parent;

    public Node(int gval,int hval)
    {
        gVal = gval;
        hVal = hval;
        fVal = gval + hval;
    }
    public Node() { }

}

public class AStar : MonoBehaviour
{
    private static AStar inst;
    public static AStar Inst { get => inst;}

    [Header("GridColor")]
    public Color startColor;
    public Color endColor;
    public Color obstacleColor;
    public Color findColor;
    public Color curColor;
    public Color pathColor;
    [Header("MapSize")]
    public int Row;
    public int Col;

    public GameObject gridPrefab;
    public Transform canvas;
    public bool isFinding = false;

    AStarGrid[,] maps;

    List<(int x,int y)> openList;
    List<(int x,int y)> closedList;
    (int x, int y) start=(-1,-1);
    (int x, int y) end=(-1,-1);
    Node[,] nodes;

    private void Awake()
    {
        inst = this;
        init();
    }
    void init()
    {
        maps = new AStarGrid[Row, Col];
        openList = new List<(int x, int y)>();
        closedList = new List<(int x, int y)>();
        for(int i=0;i<Row;++i)
        {
            for(int j=0;j<Col;++j)
            {
                maps[i,j]=Instantiate(gridPrefab,canvas,false).GetComponent<AStarGrid>();
                maps[i, j].x = i;
                maps[i, j].y = j;
            }
        }
    }
    private void Update()
    {
        if(!isFinding&&Input.GetKeyDown(KeyCode.Space))
        {
            StartFind();
        }
        if(isFinding&&Input.GetKeyDown(KeyCode.N))
        {
            StepFind();
        }
    }
    void StartFind()
    {
        if (start.x == -1 || start.y == -1 || end.x == -1 || end.y == -1
            ||maps[start.x,start.y].isObstacle||maps[end.x,end.y].isObstacle) return;
        nodes = new Node[Row, Col];
        for(int i=0;i<Row;++i)
        {
            for(int j=0;j<Col;++j)
            {
                nodes[i, j] = new Node();
            }
        }
        openList.Add(start);
        isFinding = true;
        Debug.Log("start.");
    }
    int[,] offset = new int[8,2] { { -1, -1 }, { -1, 0 }, { -1, 1 }, { 0, -1 }, { 0, 1 }, { 1, -1 }, { 1, 0 }, { 1, 1 } };
    void StepFind()
    {
        if (!isFinding) return;
        if(openList.Count==0)
        {
            isFinding = false;
            return;
        }
        Debug.Log("find");
        //find min
        var node = openList[0];
        for(int i=1; i<openList.Count;++i)
        {
            var n = openList[i];
            if(nodes[n.x,n.y].fVal<nodes[node.x,node.y].fVal)
            {
                node = n;
            }
            else if(nodes[n.x, n.y].fVal == nodes[node.x, node.y].fVal&& nodes[n.x, n.y].hVal < nodes[node.x, node.y].hVal)
            {
                node = n;
            }
        }
        maps[node.x, node.y].img.color = curColor;
        for(int i=0;i<8;++i)
        {
            int x = node.x + offset[i,0];
            int y = node.y + offset[i, 1];
            if (closedList.Contains((x, y))||x<0||x>=Row||y<0||y>=Col||
                maps[x,y].isObstacle) continue;
            if ((x, y) == end)
            {
                nodes[x, y].parent = node;
                isFinding = false;
                FindPath((x, y));
                return;
            }
            if(openList.Contains((x,y)))
            {
                int newGVal= nodes[node.x, node.y].gVal + GetGVal(x, y, node.x, node.y);
                if (nodes[x,y].gVal>newGVal)
                {
                    nodes[x, y].gVal = newGVal;
                    nodes[x, y].fVal = nodes[x, y].hVal + nodes[x, y].gVal;
                    nodes[x, y].parent = node;
                }
            }
            else
            {
                nodes[x, y].hVal = GetHVal(x, y);
                nodes[x, y].gVal = nodes[node.x, node.y].gVal + GetGVal(x, y, node.x, node.y);
                nodes[x, y].fVal = nodes[x, y].hVal + nodes[x, y].gVal;
                nodes[x, y].parent = node;
                openList.Add((x, y));
            }
            maps[x, y].img.color = findColor;
            maps[x, y].hTxt.text = nodes[x, y].hVal.ToString();
            maps[x, y].gTxt.text = nodes[x, y].gVal.ToString();
            maps[x, y].fTxt.text = nodes[x, y].fVal.ToString();
        }
        openList.Remove(node);
        closedList.Add(node);

    }
    void FindPath((int x,int y)n)
    {
        maps[n.x, n.y].img.color = pathColor;
        if (n == start) return;
        FindPath(nodes[n.x,n.y].parent);
    }

    int GetGVal(int x,int y,int a,int b)
    {
        return (int)(Mathf.Sqrt((x - a) * (x - a) + (y - b) * (y - b))*10.0f);
    }
    int GetHVal(int x,int y)
    {
        return (int)(Mathf.Sqrt((x - end.x) * (x - end.x) + (y - end.y) * (y - end.y))*10.0f);
    }





    public void SetStart(int x,int y)
    {
        if(start.x!=x||start.y!=y)
        {
            ClearStart();
        }
        start = (x, y);
        maps[x, y].isStart = true;
        maps[x, y].img.color = startColor;
    }
    private void ClearStart()
    {
        if (start.x != -1 && start.y != -1)
        {
            maps[start.x, start.y].isStart = false;
            maps[start.x, start.y].img.color = new Color(1f, 1f, 1f, 1f);
        }
    }
    public void SetEnd(int x,int y)
    {
        if (end.x != x || end.y != y)
        {
            ClearEnd();
        }
        end = (x, y);
        maps[x, y].isEnd = true;
        maps[x, y].img.color = endColor;
    }
    private void ClearEnd()
    {
        if (end.x != -1 && end.y != -1)
        {
            maps[end.x, end.y].isEnd = false;
            maps[end.x, end.y].img.color = new Color(1f, 1f, 1f, 1f);
        }
    }
    public void SetObstacle(int x,int y)
    {
        if(maps[x,y].isObstacle)
        {
            maps[x, y].isObstacle = false;
            maps[x, y].img.color = new Color(1f, 1f, 1f, 1f);
        }
        else
        {
            maps[x, y].isObstacle = true;
            maps[x, y].img.color = obstacleColor;
        }
    }
}
