using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SceneManagement;

public class mapGeneration : MonoBehaviour
{
    [SerializeField]
    [Range(0, 20)]
    int roomMaxWidth, roomMaxHeight, roomMinHeight, roomMinWidth, rooms;

    [SerializeField] GameObject primaryRock, border, chestPrefab, enemy, PLAYER, boxPrefab;
    [SerializeField] public int width, height;

    System.Random rnd = new System.Random();
    public int[,] map;


    void Awake()
    {
        InitializeMap();

        Tuple<int, int> previousRoom = new Tuple<int, int>(-1, -1);

        for (int i = 0; i < rooms; i++)
        {
            Tuple<int, int> currentRoom = GenerateRoom();
            if (previousRoom.Item1 != -1)
            {
                CreatePaths(currentRoom, previousRoom);
            }
            else
            {
                previousRoom = currentRoom;
                PLAYER.transform.position = new Vector3(previousRoom.Item1, previousRoom.Item2, -1);
            }
           
        }

        InstantiateBorders();
    }

    void InitializeMap()
    {
        map = new int[width, height];
    }

    Tuple<int, int> GenerateRoom()
    {
        int roomX, roomY, roomHeight, roomWidth;
        do
        {
            roomX = rnd.Next(16, width - 16);
            roomY = rnd.Next(16, height - 16);
            roomHeight = rnd.Next(roomMinHeight, roomMaxHeight);
            roomWidth = rnd.Next(roomMinWidth, roomMaxWidth);
        } while (!IsAValidRoomSpawn(roomX, roomY, roomWidth + roomX, roomHeight + roomY));

        int mitadX = roomWidth / 2 + roomX, mitadY = roomHeight / 2 + roomY;

        map[roomY, mitadX] = 4;
        map[roomY + roomHeight, mitadX] = 4;
        map[mitadY, roomX + roomWidth] = 4;
        map[mitadY, roomX] = 4;

        Tuple<int, int> cords = Tuple.Create(roomY, mitadX);

        for (int y = roomY; y <= roomY + roomHeight; y++)
        {
            for (int x = roomX; x <= roomX + roomWidth; x++)
            {
                if ((y == roomY || y == roomY + roomHeight) && map[y, x] == 0)
                {
                    map[y, x] = 2;
                }
                if ((x == roomX || x == roomX + roomWidth) && map[y, x] == 0)
                {
                    map[y, x] = 2;
                }
                if (map[y, x] == 0)
                {
                    map[y, x] = 1;
                    Instantiate(primaryRock, new Vector2(x, y), quaternion.identity);
                }

                //Create Monsters or boxes
                if (map[y, x] == 1 && !isBlockingADoor(x, y))
                {
                    if (rnd.Next(10) == 1)
                    {
                        Instantiate(enemy, new Vector3(x, y, -1), Quaternion.identity);
                    }
                    else if (rnd.Next(8) == 1)
                    {
                        map[y, x] = 5; // Cajas
                        Instantiate(boxPrefab, new Vector3(x, y, -1), Quaternion.identity);
                    }
                }


            }
        }
        return cords;
    }

    bool IsAValidRoomSpawn(int startX, int startY, int finX, int finY)
    {
        for (int y = startY; y < finY; y++)
        {
            for (int x = startX; x < finX; x++)
            {
                if (map[y, x] != 0) { return false; }
            }
        }
        return true;
    }

    void CreatePaths(Tuple<int, int> inicio, Tuple<int, int> fin)
    {
        int[] directionsX = { -1, 1, 0, 0 }, directionsY = { 0, 0, -1, 1 };
        Tuple<int, int>[,] parent = new Tuple<int, int>[width, height]; // Utilizar las variables 'width' y 'height'
        bool[,] visited = new bool[width, height]; // Utilizar las variables 'width' y 'height'

        Queue<Tuple<int, int>> q = new Queue<Tuple<int, int>>();
        visited[inicio.Item1, inicio.Item2] = true;
        q.Enqueue(inicio);

        bool foundPath = false;
        while (q.Count > 0)
        {
            Tuple<int, int> cords = q.Dequeue();

            if (cords.Item2 == fin.Item2 && cords.Item1 == fin.Item1)
            {
                foundPath = true;
                break;
            }

            for (int i = 0; i < 4; i++)
            {
                int x = cords.Item2 + directionsX[i];
                int y = cords.Item1 + directionsY[i];

                if (x >= 0 && y >= 0 && x < width && y < height && !visited[y, x] && map[y, x] != 2)
                {
                    visited[y, x] = true;
                    parent[y, x] = cords;
                    q.Enqueue(new Tuple<int, int>(y, x));
                }
            }
        }

        // Reconstruir el camino si se encontró uno
        if (foundPath)
        {
            Queue<Tuple<int, int>> q2 = new Queue<Tuple<int, int>>();

            Tuple<int, int> current = fin;
            while (current != inicio)
            {
                q2.Enqueue(current);
                current = parent[current.Item1, current.Item2];
            }
            HashSet<Tuple<int, int>> visitedQ2 = new HashSet<Tuple<int, int>>();

            while (q2.Count > 0)
            {
                Tuple<int, int> current2 = q2.Dequeue();
                visited[current2.Item1, current2.Item2] = true;
                visitedQ2.Add(current2); // Agregar la coordenada actual al conjunto 'visitedQ2'

                for (int i = 0; i < 4; i++)
                {
                    int x = current2.Item2 + directionsX[i];
                    int y = current2.Item1 + directionsY[i];

                    if (x >= 0 && y >= 0 && x < width && y < height && map[y, x] != 2 && !visitedQ2.Contains(new Tuple<int, int>(y, x)))
                    {
                        if (rnd.Next(6) / 5 == 1)
                        {
                            q2.Enqueue(new Tuple<int, int>(y, x));
                        }
                        if (map[y, x] == 0)
                        {
                            Instantiate(primaryRock, new Vector2(x, y), Quaternion.identity);
                            map[y, x] = 3;
                        }
                        visited[y, x] = true;
                    }
                }
            }
            Instantiate(primaryRock, new Vector2(inicio.Item2, inicio.Item1), quaternion.identity); // Corregir 'current' por 'inicio'
        }
    }

    void InstantiateBorders()
    {
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (y == 0 || y == height - 1 || x == 0 || x == width - 1)
                {
                    Instantiate(border, new Vector2(x, y), Quaternion.identity);
                }
                else if (map[y, x] == 0 && (map[y + 1, x] == 3 || map[y - 1, x] == 3 || map[y, x + 1] == 3 || map[y, x - 1] == 3))
                {
                    Instantiate(border, new Vector2(x, y), Quaternion.identity);
                }
                else if (map[y, x] == 2)
                {
                    Instantiate(border, new Vector2(x, y), Quaternion.identity);
                }

            }
        }
    }

    void ChestSpawns(int chests)
    {
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (map[y, x] == 3 && rnd.Next(21) / 20 == 1 && chests > 0)
                {
                    chests--;
                    map[y, x] = 4;
                    Instantiate(chestPrefab, new Vector3(x, y, -1), quaternion.identity);
                }
            }
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(0);
        }
    }

    private bool isBlockingADoor(int x, int y)
    {
        int[] directionsX = { -1, 1, 0, 0 }, directionsY = { 0, 0, -1, 1 };
        for (int i = 0; i < 4; i++)
        {
            if(map[y + directionsY[i], x + directionsX[i]] == 4)
            {
                return true;
            }
        }
        return false;
    }
}

