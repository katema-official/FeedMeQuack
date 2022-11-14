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

        private static int _mapDimensionX = 128;
        private static int _mapDimensionY = 128;
        public static int StartMapX = 64;
        public static int StartMapY = 64;


        public static bool[,] GenerateStageLayout(StageSO currentStage)
        {

            int randomSeed = (int)System.DateTime.Now.Ticks;
            Random.InitState(randomSeed);

            //this method needs to generate the current stage.
            //a stage is a spatially ordered collection of lakes. The lakes can be represented inside a matrix.

            //first of all, let's decide the way the map of the current stage is organized by using the bitmap.
            //0) we initialize the bitmap to false (there is no lake)
            bool[,] currentStageBitMap = new bool[_mapDimensionX, _mapDimensionY];
            for (int i = 0; i < _mapDimensionX; i++)
            {
                for (int j = 0; j < _mapDimensionY; j++)
                {
                    currentStageBitMap[i, j] = false;   //false = WALL, true = EMPTY SPACE
                }
            }
            //1) we always have an initial lake
            currentStageBitMap[StartMapX, StartMapY] = true;

            //2) how many lakes do we need to generate, actually?
            int nLakes = Random.Range(currentStage.MinNumberOfLakes, currentStage.MaxNumberOfLakes + 1);
            int totalNumberOfLakes = nLakes;

            nLakes -= 1;        //since we already fixed the initial room.

            //3) now, to explain how we will generate the dungeon, let's use a metaphore:
            //there is a miner, that is inside a cave. He wants to find diamonds. Do to so, he wants to explore as much as possible
            //in different directions. So, starting from its initial room, he decides to dig in a certain direction for a certain number
            //of rooms (here you can imagine that room = meter). Once it has finished doing so, he goes back to the initial room.
            //There he wanders in a certain direction, untill he reaches a wall. When it does, with a certain probability, he starts to
            //dig that wall. With another probability, he changes direction.
            //So, let's define some parameters to do so. 
            int minStepsToWalkIfPossible = 2;         //arbitrary value
            int maxStepsToWalkIfPossible = 3;         //arbitrary value
            int minRoomsToDig = totalNumberOfLakes/8 + 1;
            int maxRoomsToDig = totalNumberOfLakes/4 + 1;

            int currentX = StartMapX;
            int currentY = StartMapY;

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
                    currentX = StartMapX;
                    currentY = StartMapY;
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

            debugGeneratedStageBitmap(currentStageBitMap);

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



        //#######################################################################################################################################################
        //#######################################################################################################################################################
        //#######################################################################################################################################################
        //#######################################################################################################################################################
        //#######################################################################################################################################################


        public static LakeDescriptionSO[,] GenerateStageWithActualLakeDescriptionSO(bool[,] bitMapDungeon, StageSO stageDescription)
        {
            LakeDescriptionSO[,] finalMap = new LakeDescriptionSO[_mapDimensionX, _mapDimensionY];

            float farthestLength = 0f;
            LakeDescriptionSO farthestLake = null;


            //we just need to copy the bitmap. The main difference is that, instead of having "true"s and "false"s, we have
            //LakeDescriptionSO, the datas of which are generated based on stageDescription
            for(int i = 0; i < _mapDimensionX; i++)
            {
                for(int j = 0; j < _mapDimensionY; j++)
                {
                    if(bitMapDungeon[i,j] == false)
                    {
                        //there is no lake here
                        finalMap[i,j] = null;
                    }
                    else
                    {
                        //we have to generate the lake.
                        //if the lake is the starting one, let's remember it.
                        //otherwise, let's see if the current room is the farthest from the initial room
                        if(i == StartMapX && j == StartMapY)
                        {
                            finalMap[i,j] = createInitialLakeDescriptionSO();
                        }
                        else
                        {
                            finalMap[i,j] = createIntermediateLakeDescriptionSO(stageDescription);
                            float x = Mathf.Abs(StartMapX - i);
                            float y = Mathf.Abs(StartMapY - j);
                            float dist = Mathf.Sqrt(Mathf.Pow(x, 2f) + Mathf.Pow(y, 2f));
                            if(dist > farthestLength)
                            {
                                //we save the furthest room so we can then flag it as the final room
                                farthestLake = finalMap[i,j];
                                farthestLength = dist;
                            }
                        }

                        //to set the adjacent rooms of the current room, we can use the bitmap
                        if (bitMapDungeon[i - 1, j] == true)
                        {
                            finalMap[i, j].HasNorthRiver = true;
                        }
                        if (bitMapDungeon[i + 1, j] == true)
                        {
                            finalMap[i, j].HasSouthRiver = true;
                        }
                        if (bitMapDungeon[i, j - 1] == true)
                        {
                            finalMap[i, j].HasWestRiver = true;
                        }
                        if (bitMapDungeon[i, j + 1] == true)
                        {
                            finalMap[i, j].HasEastRiver = true;
                        }

                        debugGeneratedLake(finalMap[i, j], i, j);
                        
                    }
                }
            }
            farthestLake._isFinalRoom = true;
            Debug.Log("farthest lake: " + farthestLake._isFinalRoom);
            return finalMap;

        }


        //function used to create the LakeDescriptionSO of the initial lake of the stage 
        private static LakeDescriptionSO createInitialLakeDescriptionSO()
        {
            LakeDescriptionSO ret = ScriptableObject.CreateInstance<LakeDescriptionSO>();

            ret.EnemiesToSpawnMap = null;
            ret.Dimension = EnumsDungeon.LakeDimension.Small;   //let's assume that the initial lake is always small (it's not an absurd assumption)
            ret.BreadToSpawnMap = null;
            ret._isLakeCleared = false;      //a small trick: will be set to true as soon as the player spawns at its center. If it's false, the player spawns at the center.
                                               //if it's true, he will spawn from where he came from.
            ret._isStartingRoom = true;        //and always by definition, is the starting room lol
            ret._isFinalRoom = false;       

            return ret;
        }

        //function used to create the LakeDescriptionSO of an intermediate stage (not the initial one basically)
        private static LakeDescriptionSO createIntermediateLakeDescriptionSO(StageSO stageDescription)
        {
            LakeDescriptionSO ret = ScriptableObject.CreateInstance<LakeDescriptionSO>();

            generateEnemiesForLake(ret, stageDescription);
            decideLakeDimensionForLake(ret, stageDescription);
            decideBreadToSpawnMapForLake(ret, stageDescription);

            ret._isLakeCleared = false;
            ret._isStartingRoom = false;
            ret._isFinalRoom = false;


            return ret;
        }

        /*private static LakeDescriptionSO createFinalLakeDescriptionSO(StageSO stageDescription)
        {
            LakeDescriptionSO ret = createIntermediateLakeDescriptionSO(stageDescription);
            ret._isFinalRoom = true;
            return ret;
        }*/







        private static void generateEnemiesForLake(LakeDescriptionSO lake, StageSO stageDescription)
        {
            //first, let's decide how many enemies there must be
            int totNumberOfEnemies = Random.Range(stageDescription.MinNumberOfEnemiesPerLake, stageDescription.MaxNumberOfEnemiesPerLake + 1);
            lake.EnemiesToSpawnMap.Add(EnumsDungeon.EnemyType.Mallard, 0);
            lake.EnemiesToSpawnMap.Add(EnumsDungeon.EnemyType.Coot, 0);
            lake.EnemiesToSpawnMap.Add(EnumsDungeon.EnemyType.Goose, 0);
            lake.EnemiesToSpawnMap.Add(EnumsDungeon.EnemyType.Fish, 0);
            lake.EnemiesToSpawnMap.Add(EnumsDungeon.EnemyType.Seagull, 0);

            //then, let's set the minimum amount of enemies requested
            lake.EnemiesToSpawnMap[EnumsDungeon.EnemyType.Mallard] += stageDescription.MinNumberOfEachEnemy[(int)EnumsDungeon.EnemyType.Mallard];
            totNumberOfEnemies -= lake.EnemiesToSpawnMap[EnumsDungeon.EnemyType.Mallard];

            lake.EnemiesToSpawnMap[EnumsDungeon.EnemyType.Coot] += stageDescription.MinNumberOfEachEnemy[(int)EnumsDungeon.EnemyType.Coot];
            totNumberOfEnemies -= lake.EnemiesToSpawnMap[EnumsDungeon.EnemyType.Coot];

            lake.EnemiesToSpawnMap[EnumsDungeon.EnemyType.Goose] += stageDescription.MinNumberOfEachEnemy[(int)EnumsDungeon.EnemyType.Goose];
            totNumberOfEnemies -= lake.EnemiesToSpawnMap[EnumsDungeon.EnemyType.Goose];

            lake.EnemiesToSpawnMap[EnumsDungeon.EnemyType.Fish] += stageDescription.MinNumberOfEachEnemy[(int)EnumsDungeon.EnemyType.Fish];
            totNumberOfEnemies -= lake.EnemiesToSpawnMap[EnumsDungeon.EnemyType.Fish];

            lake.EnemiesToSpawnMap[EnumsDungeon.EnemyType.Seagull] += stageDescription.MinNumberOfEachEnemy[(int)EnumsDungeon.EnemyType.Seagull];
            totNumberOfEnemies -= lake.EnemiesToSpawnMap[EnumsDungeon.EnemyType.Seagull];

            //we might still have some enemies to generate. Let's generate them randomly.
            for(int j = 0; j < totNumberOfEnemies; j++)
            {
                float r = Random.Range(0f, 1f);
                float accum = 0f;
                int enemyChosenIndex = -1;
                for (int i = 0; i < stageDescription.ProbabilitiesOfEnemies.Length; i++)
                {
                    accum += stageDescription.ProbabilitiesOfEnemies[i];
                    if (r < accum)
                    {
                        enemyChosenIndex = i;
                        break;
                    }
                }

                EnumsDungeon.EnemyType enemyTypeChosen = (EnumsDungeon.EnemyType)enemyChosenIndex;
                lake.EnemiesToSpawnMap[enemyTypeChosen]++;
            }
        }

        private static void decideLakeDimensionForLake(LakeDescriptionSO lake, StageSO stageDescription)
        {
            float r = Random.Range(0f, 1f);
            float accum = 0f;
            int lakeDimensionChosenIndex = -1;
            for (int i = 0; i < stageDescription.ProbabilitiesOfLake.Length; i++)
            {
                accum += stageDescription.ProbabilitiesOfLake[i];
                if (r < accum)
                {
                    lakeDimensionChosenIndex = i;
                    break;
                }
            }

            switch (lakeDimensionChosenIndex)
            {
                case (int)EnumsDungeon.LakeDimension.Small:
                    lake.Dimension = EnumsDungeon.LakeDimension.Small;
                    break;
                case (int)EnumsDungeon.LakeDimension.Medium:
                    lake.Dimension = EnumsDungeon.LakeDimension.Medium;
                    break;
                case (int)EnumsDungeon.LakeDimension.Large:
                    lake.Dimension = EnumsDungeon.LakeDimension.Large;
                    break;
                default:
                    Debug.Log("wtf again you shouldn't be here");
                    break;
            }
        }

        private static void decideBreadToSpawnMapForLake(LakeDescriptionSO lake, StageSO stageDescription)
        {
            //first, let's decide hoe much bread do we want to spawn
            int breadToSpawn = Random.Range(stageDescription.MinNumberOfBreadPerLake, stageDescription.MaxNumberOfBreadPerLake + 1);
            lake.BreadToSpawnMap.Add(EnumsDungeon.BreadType.Small, 0);
            lake.BreadToSpawnMap.Add(EnumsDungeon.BreadType.Medium, 0);
            lake.BreadToSpawnMap.Add(EnumsDungeon.BreadType.Large, 0);

            //generate, according to the given probability, the number of bread pieces of different kind to spawn
            for (int j = 0; j < breadToSpawn; j++)
            {
                float r = Random.Range(0f, 1f);
                float accum = 0f;
                int breadTypeChosenIndex = -1;
                for (int i = 0; i < stageDescription.ProbabilitiesOfBread.Length; i++)
                {
                    accum += stageDescription.ProbabilitiesOfBread[i];
                    if (r < accum)
                    {
                        breadTypeChosenIndex = i;
                        break;
                    }
                }

                EnumsDungeon.BreadType breadTypeChosen = (EnumsDungeon.BreadType) breadTypeChosenIndex;
                lake.BreadToSpawnMap[breadTypeChosen]++;
            }

        }






        private static void debugGeneratedLake(LakeDescriptionSO lake, int i, int j)
        {
            if(lake._isStartingRoom == true) { return; }
            Debug.LogFormat("room [{0},{1}] has:\n" +
                            "{2} mallards, {3} coots, {4} goose\n" +
                            "northRiver: {5}, southRiver: {6}, westRiver: {7}, eastRiver: {8}\n" +
                            "dimension: {9}\n" +
                            "breadSmall: {10}, breadMedium: {11}, breadMedium: {12}\n", i, j,
                            lake.EnemiesToSpawnMap[EnumsDungeon.EnemyType.Mallard],
                            lake.EnemiesToSpawnMap[EnumsDungeon.EnemyType.Coot],
                            lake.EnemiesToSpawnMap[EnumsDungeon.EnemyType.Goose],
                            lake.HasNorthRiver, lake.HasSouthRiver, lake.HasWestRiver, lake.HasEastRiver,
                            lake.Dimension,
                            lake.BreadToSpawnMap[EnumsDungeon.BreadType.Small], lake.BreadToSpawnMap[EnumsDungeon.BreadType.Medium], lake.BreadToSpawnMap[EnumsDungeon.BreadType.Large]);
                            
        }

        private static void debugGeneratedStageBitmap(bool[,] bitmap)
        {
            string s = "";
            for(int i = 55; i < 75; i++)
            {
                for(int j = 55; j < 75; j++)
                {
                    if(bitmap[i,j] == false)
                    {
                        s += "-";
                    }
                    else
                    {
                        s += "+";
                    }
                }
                s += "\n";
            }
            Debug.Log(s);
        }


    }
}