//This is a basic implementation of A* pathfinding algorithm. For the algorithm to work, 
//the h value, that is, the distance to the target node, the g value, the distance to the starting node, 
//and finally the f value, that is, the sum of the g and h values, must be defined for 
//each neighbour node until it reaches the target node. When the searching part ends, 
//algorithm will retrace the path from the end to start node and finds the shortest path.
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Pathfinder : MonoBehaviour
{
    //Contains map data
   public Map map;
   [SerializeField] Transform startTransform,targetTransform;
   //Contains available nodes
   List<Node> open = new List<Node>();
   //Contains used nodes
   HashSet<Node> closed = new HashSet<Node>();
   //Contains the shortest path
   HashSet<Node> path = new HashSet<Node>();
   List<Vector2> directions = new List<Vector2>(){
                                new Vector2(1,0),
                                new Vector2(0,1),
                                new Vector2(-1,0),
                                new Vector2(0,-1)
                                                };
    
   Node goalnode;
   Node startnode;
   Node lastPos;
   bool done;

    void BeginSearch()
    {
        done=false;
        //This method determines wall positions as 1 and floor positions as 0
        map.SetMap();

        //Clear both just in case
        open.Clear();
        closed.Clear();
        //Set goal and start nodes
        startnode = new Node(new Vector2(startTransform.position.x,startTransform.position.z),0f,0f,null);
        goalnode = new Node(new Vector2(targetTransform.position.x,targetTransform.position.z),0f,0f,null);
        open.Add(startnode);
        lastPos=startnode;

    }
    void Search(Node node)
    {
        if(node==null)return;
        if(node.Equals(goalnode)){done=true;return;}
        //You will look at each neighbour nodes of the current node in here
        foreach (var dir in directions)
        {
            Vector2 neighbour = dir + node.pos;

            //Checks whether neighbour node is in map's bounds
            if (neighbour.x < 0 || neighbour.x > map.mapWidth || neighbour.y < 0 || neighbour.y > map.mapHeight) continue;
            
            //If its neighbour is an unwalkable element, continue
            if(map.map[(int)neighbour.x,(int)neighbour.y]==1)continue;

            //Checks neighbour node is in the closed list
            if(IsClosed(neighbour))continue;

            //Calcultes the distance from from start node
            float _g = Vector2.Distance(node.pos,neighbour)+node.g;
            //Calculates the shortest distance to the target node
            float _h = Vector2.Distance(neighbour,goalnode.pos);
            //Calculates the f value of neighbour node with summation of g value and h value 
            float _f = _g+_h;

            //Checks whether the open list has this node or not. 
            if(!UpdateNode(neighbour,_g,_h,node))
            open.Add(new Node(neighbour,_g,_h,node));

        }
        //Sorts the list by lowest f value, then sorts nodes with equal f values ​​according to their h values
        open=open.OrderBy(p=>p.f).ThenBy(n=>n.h).ToList<Node>();

        //Picks the first element in the list which will be node that has the lowest f cost
        Node currentnode =(Node)open.ElementAt(0);

        //Adds the already selected node to the closed list and remove from the open list
        closed.Add(currentnode);
        open.RemoveAt(0);

        //Updates the last node with selected node
        lastPos=currentnode;
    }
    bool UpdateNode(Vector2 pos, float g, float h, Node parent)
    {
        foreach (var p in open)
        {
            if(p.pos.Equals(pos))
            {
                p.g=g;
                p.h=h;
                p.parent=parent;
                return true;
            }
        }
        return false;
    }
    private void Update() {
        if (Input.GetKeyDown(KeyCode.P)) BeginSearch();
        if (!done) Search(lastPos);
        if (Input.GetKeyDown(KeyCode.Tab)) Retrace();
    }
    bool IsClosed(Vector2 location)
    {
        foreach (var p in closed)
        {
            if(p.pos.Equals(location))
            return true;
        }
        return false;
    }
    
    //When search process ends, this method will be execute and give you the shortest path
    private void Retrace()
    {
        Node begin = lastPos;
        while (!begin.Equals(startnode)&&begin!=null)
        {
            path.Add(begin);
            begin = begin.parent;
            Debug.Log(begin.pos);
        }
    }
}
public class Node
{
    //Holds position vector
    Vector3 _pos;
    public Vector2 pos { set => _pos = new Vector3(value.x, 0, value.y); get => new Vector2(_pos.x, _pos.z); }

    public float h, g;
    public float f { get => h + g; }

    //You will need this parent variable for retracing part
    public Node parent;

    public Node(Vector2 _pos, float _g, float _h, Node _parent)
    {
        pos = _pos;
        g = _g;
        h = _h;
        parent = _parent;
    }
    public override bool Equals(object obj)
    {
        if ((obj == null) || !this.GetType().Equals(obj.GetType()))
            return false;
        else
            return pos.Equals(((Node)obj).pos);
    }

    public override int GetHashCode()
    {
        return 0;
    }
}
//this is an example class, change it according to your need.
[System.Serializable]
public class Map
{
    public byte mapWidth, mapHeight;
    public byte[,] map;
    public List<Vector2> walls = new List<Vector2>();
    public void SetMap()
    {
        map = new byte[mapWidth, mapHeight];
        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                if (walls.Contains(new Vector2(x, y)))
                    map[x, y] = 1;
                else
                    map[x, y] = 0;
            }
        }
    }
   
}

