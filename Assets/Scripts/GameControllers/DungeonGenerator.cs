using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;
using UnityEngine;

public class DungeonGenerator : MonoBehaviour
{
    public class Cell   // room 
    {
        public bool visited = false;    // if room has been checked
        public bool[] status = new bool[4];     // directions from room
    }

    [System.Serializable]
    public class Rule
    {
        public GameObject room;
        public Vector2Int minPos;
        public Vector2Int maxPos;

        public bool obligatory;

        public int ProbabilityOfSpawning(int x, int y)
        {
            // 0 - cannot spawn, 1 - can spawn, 2 - has to spawn

            if (x >= minPos.x && x <= maxPos.x && y >= minPos.y && y <= maxPos.y)
            {
                return obligatory ? 2 : 1;
            }

            return 0;
        }
    }

    public Vector2Int size;
    public Vector2 offset;
    public int startPos = 0;
    public Rule[] rooms;

    List<Cell> board;

    void Start()
    {
        CrusadeGenerator();
    }

    void GenerateDungeon()
    {
        for (int i = 0; i < size.x; i++)
        {
            for (int j = 0; j < size.y; j++)
            {
                Cell currentCell = board[Mathf.FloorToInt(i + j * size.x)];

                if (currentCell.visited)
                {
                    int randomRoom = -1;

                    List<int> availableRooms = new List<int>();

                    for (int k = 0; k < rooms.Length; k++)
                    {
                        int p = rooms[k].ProbabilityOfSpawning(i, j);

                        if (p == 2)
                        {
                            randomRoom = k;
                            break;
                        }
                        else if (p == 1)
                        {
                            availableRooms.Add(k);
                        }
                    }

                    if (randomRoom == -1)
                    {
                        if (availableRooms.Count > 0)
                        {
                            randomRoom = availableRooms[Random.Range(0, availableRooms.Count)];
                        }
                        else
                        {
                            randomRoom = 0;
                        }
                    }

                    var newRoom = Instantiate(rooms[randomRoom].room, new Vector3(i * offset.x, 0, -j * offset.y), Quaternion.identity, transform).GetComponent<RoomController>();
                    newRoom.UpdateRoom(currentCell.status);

                    newRoom.name += " " + i + "-" + j;
                }
            }
        }
    }

    void CrusadeGenerator()
    {
        board = new List<Cell>();  // create board and cells

        for (int i = 0; i < size.x; i++)
        {
            for (int j = 0; j < size.y; j++)
            {
                board.Add(new Cell());
            }
        }

        int currentCell = startPos;     // track cells

        Stack<int> path = new();

        int k = 0;      // track generation

        while (k < 100)    // number of times the loop can continue (up to 100 rooms)
        {
            k++;

            board[currentCell].visited = true;

            if (currentCell == board.Count - 1)
            {
                break;
            }

            // Check cell's neighbours
            List<int> neighbours = CheckNeighbours(currentCell);

            if (neighbours.Count == 0)
            {
                if (path.Count == 0)
                {
                    break;
                }
                else
                {
                    currentCell = path.Pop();
                }
            }
            else
            {
                path.Push(currentCell);

                int newCell = neighbours[Random.Range(0, neighbours.Count)];

                if (newCell > currentCell)
                {
                    // down or right
                    if (newCell - 1 == currentCell)
                    {
                        board[currentCell].status[1] = true;
                        currentCell = newCell;
                        board[currentCell].status[3] = true;
                    }
                    else
                    {
                        board[currentCell].status[2] = true;
                        currentCell = newCell;
                        board[currentCell].status[0] = true;
                    }
                }
                else
                {
                    // up or left
                    if (newCell + 1 == currentCell)
                    {
                        board[currentCell].status[3] = true;
                        currentCell = newCell;
                        board[currentCell].status[1] = true;
                    }
                    else
                    {
                        board[currentCell].status[0] = true;
                        currentCell = newCell;
                        board[currentCell].status[2] = true;
                    }
                }
            }
        }

        GenerateDungeon();
    }

    List<int> CheckNeighbours(int cell)     // check neighbouring cells for rooms
    {
        List<int> neighbours = new List<int>();

        // check north neighbour
        if (cell - size.x >= 0 && !board[cell - size.x].visited)
        {
            neighbours.Add(cell - size.x);
        }

        // check east neighbour
        if (cell + size.x < board.Count && !board[cell + size.x].visited)
        {
            neighbours.Add(cell + size.x);
        }

        // check south neighbour
        if ((cell + 1) % size.x != 0 && !board[cell + 1].visited)
        {
            neighbours.Add(cell + 1);
        }

        // check west neighbour
        if (cell % size.x != 0 && !board[cell - 1].visited)
        {
            neighbours.Add(cell - 1);
        }

        return neighbours;
    }
}
