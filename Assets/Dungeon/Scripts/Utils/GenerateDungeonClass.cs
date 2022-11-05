using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace LevelStageNamespace
{
    public static class GenerateDungeonClass
    {

        private enum _bitMapDirection
        {
            Up,
            Down,
            Left,
            Right
        };


        public static bool[,] GenerateStage(StageSO currentStage)
        {

            int randomSeed = (int)System.DateTime.Now.Ticks;
            Random.InitState(randomSeed);

            //this method needs to generate the current stage.
            int mapDimensionX = 128;
            int mapDimensionY = 128;
            int startMapX = 64;
            int startMapY = 64;

            //a stage is a spatially ordered collection of lakes. The lakes can be represented inside a matrix.

            //first of all, let's decide the way the map of the current stage is organized by using the bitmap.
            //0) we initialize the bitmap to false (there is no lake)
            bool[,] currentStageBitMap = new bool[mapDimensionX, mapDimensionY];
            for (int i = 0; i < mapDimensionX; i++)
            {
                for (int j = 0; j < mapDimensionY; j++)
                {
                    currentStageBitMap[i, j] = false;   //false = WALL, true = EMPTY SPACE
                }
            }
            //1) we always have an initial lake
            currentStageBitMap[startMapX, startMapY] = true;

            //2) how many lakes do we need to generate, actually?
            int nLakes = Random.Range(currentStage.MinNumberOfLakes, currentStage.MaxNumberOfLakes + 1);
            nLakes -= 1;        //since we already fixed the initial room.
            Debug.Log("[64,64] = " + currentStageBitMap[64, 64] + ", nLakes = " + nLakes);

            //3) now, to explain how we will generate the dungeon, let's use a metaphore:
            //there is a miner, that is inside a cave. He wants to find diamonds. Do to so, he wants to explore as much as possible
            //in different directions. So, starting from its initial room, he decides to dig in a certain direction for a certain number
            //of rooms (here you can imagine that room = meter). Once it has finished doing so, he goes back to the initial room.
            //There he wanders in a certain direction, untill he reaches a wall. When it does, with a certain probability, he starts to
            //dig that wall. With another probability, he changes direction.
            //So, let's define some parameters to do so. 
            int minStepsToWalkIfPossible = 2;         //arbitrary value
            int maxStepsToWalkIfPossible = 3;         //arbitrary value
            int minRoomsToDig = 1;
            int maxRoomsToDig = 3;

            int currentX = startMapX;
            int currentY = startMapY;

            while (nLakes > 0)
            {
                //starting from the current room, choose a random direction.
                _bitMapDirection currentDirection = (_bitMapDirection)Random.Range(0, 4);

                //can you move in that direction?
                currentX = updateCurrentX(currentX, currentDirection);
                currentY = updateCurrentY(currentY, currentDirection);
                if (currentStageBitMap[currentX, currentY] == false)
                {
                    //If no, dig in that direction for a certain number of steps
                    int howMuchToDig = Mathf.Min(nLakes, Random.Range(minRoomsToDig, maxRoomsToDig + 1));
                    for(int i = 0; i < howMuchToDig; i++)
                    {
                        dig(currentStageBitMap, currentX, currentY);
                        currentX = updateCurrentX(currentX, currentDirection);
                        currentY = updateCurrentY(currentY, currentDirection);
                        //printDebugXY(currentX, currentY);
                    }
                    nLakes -= howMuchToDig;
                    currentX = startMapX;
                    currentY = startMapY;
                }
                else
                {
                    //If yes, move up to a certain number of steps in that direction
                    int howMuchToMove = Random.Range(minStepsToWalkIfPossible, maxStepsToWalkIfPossible + 1) - 1;   //-1 because actually we already did one step before the if!
                    for(int i = 0; i < howMuchToMove; i++)
                    {
                        //I keep moving in that direction only if I would not hit a wall.
                        if(!doISeeAWall(currentStageBitMap, currentX, currentY, currentDirection))
                        {
                            currentX = updateCurrentX(currentX, currentDirection);
                            currentY = updateCurrentY(currentY, currentDirection);
                        }
                        else
                        {
                            //if I would hit a wall, I stop.
                            break;
                        }
                    }




                }


            }

            return currentStageBitMap;

        }





        private static void printDebugXY(int x, int y)
        {
            Debug.Log("DBG: [x,y] = [" + x + "," + y + "]");
        }


        private static bool doISeeAWall(bool[,] bitMap, int currentX, int currentY, _bitMapDirection direction)
        {
            switch (direction)
            {
                case _bitMapDirection.Up:
                    return bitMap[currentX, currentY - 1] == false;
                case _bitMapDirection.Down:
                    return bitMap[currentX, currentY + 1] == false;
                case _bitMapDirection.Left:
                    return bitMap[currentX - 1, currentY] == false;
                case _bitMapDirection.Right:
                    return bitMap[currentX + 1, currentY] == false;
                default:
                    Debug.Log("What are you doing here!?");
                    return true;    //the code should never arrive here
            }
        }

        

        private static void dig(bool[,] bitMap, int currentX, int currentY)
        {
            bitMap[currentX, currentY] = true;
        }

        private static int updateCurrentY(int currentY, _bitMapDirection direction)
        {
            switch (direction)
            {
                case _bitMapDirection.Up:
                    return currentY - 1;
                case _bitMapDirection.Down:
                    return currentY + 1;
                default:
                    return currentY;
            }
        }

        private static int updateCurrentX(int currentX, _bitMapDirection direction)
        {
            switch (direction)
            {
                case _bitMapDirection.Left:
                    return currentX - 1;
                case _bitMapDirection.Right:
                    return currentX + 1;
                default:
                    return currentX;
            }
        }



    }
}