#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

namespace qtools.qmaze
{
    [ExecuteInEditMode]
    public class MazeEngine : MonoBehaviour
    {
        [Serializable] public class QMazeGeneratedEvent : UnityEvent<MazeEngine> { }
        [Serializable] public class QMazePieceGeneratedEvent : UnityEvent<MazePieceData> { }
        [Serializable] public class QMazeGenerateProgressEvent : UnityEvent<float> { }

        [SerializeField] private MazePiecePack piecePack;

        [SerializeField] private int mazeWidth = 25;
        [SerializeField] private int mazeHeight = 25;

        [SerializeField] private float mazePieceWidth = 10;
        [SerializeField] private float mazePieceHeight = 10;

        [SerializeField] private float mazeScale = 1.0f;

        [SerializeField] private bool startRandomPosition = false;
        [SerializeField] private int startRandomPositionCount = 0;
        [SerializeField] private List<MazeVector2IntDir> startPositionList = new List<MazeVector2IntDir>();

        [SerializeField] private bool finishRandomPosition = false;
        [SerializeField] private int finishRandomPositionCount = 0;
        [SerializeField] private List<MazeVector2IntDir> finishPositionList = new List<MazeVector2IntDir>();

        [SerializeField] private List<MazeVector2IntDir> exitPositionList = new List<MazeVector2IntDir>();

        [SerializeField] private bool obstacleIsNone = false;
        [SerializeField] private List<MazeVector2Int> obstaclePositionList = new List<MazeVector2Int>();

        [SerializeField] private bool onlyPath = false;

        [SerializeField] private bool useSeed = false;
        [SerializeField] private int seed = 0;

        [SerializeField] private MazePieceData[][] mazeArray;
        [SerializeField] private bool inited = false;

        public QMazeGeneratedEvent mazeGeneratedEvent = new QMazeGeneratedEvent();
        public QMazePieceGeneratedEvent mazePieceGeneratedEvent = new QMazePieceGeneratedEvent();
        public QMazeGenerateProgressEvent mazeGenerateProgressEvent = new QMazeGenerateProgressEvent();

        private float generationProgress;
        private float instantiatingProgress;
        private List<CheckTask> checkTaskList = new List<CheckTask>();
        private List<MazeVector2Int> path;
        private MazeOutputDirection lastDirection;
        private int startFinishLeft;

        private void Awake()
        {
            if (!inited)
            {
                inited = true;

                if (gameObject.GetComponent<MazePiecePack>() == null)
                    piecePack = gameObject.AddComponent<MazePiecePack>();
            }
        }

        public MazePiecePack getMazePiecePack()
        {
            return piecePack;
        }

        public void setMazePiecePack(MazePiecePack piecePack)
        {
            this.piecePack = piecePack;
        }

        public int getMazeWidth()
        {
            return mazeWidth;
        }

        public void setMazeWidth(int mazeWidth)
        {
            this.mazeWidth = mazeWidth;
        }

        public int getMazeHeight()
        {
            return mazeHeight;
        }

        public void setMazeHeight(int mazeHeight)
        {
            this.mazeHeight = mazeHeight;
        }

        public float getMazePieceWidth()
        {
            return mazePieceWidth;
        }

        public void setMazePieceWidth(float mazePieceWidth)
        {
            this.mazePieceWidth = mazePieceWidth;
        }

        public float getMazePieceHeight()
        {
            return mazePieceHeight;
        }

        public void setMazePieceHeight(float mazePieceHeight)
        {
            this.mazePieceHeight = mazePieceHeight;
        }

        public void setMazePieceSize(float mazePieceSize)
        {
            this.mazePieceWidth = this.mazePieceHeight = mazePieceSize;
        }

        public void setMazePieceSize(float mazePieceWidth, float mazePieceHeight)
        {
            this.mazePieceWidth = mazePieceWidth;
            this.mazePieceHeight = mazePieceHeight;
        }

        public void setMazeScale(float mazeScale)
        {
            this.mazeScale = mazeScale;
        }

        public float getMazeScale()
        {
            return mazeScale;
        }

        public bool isStartRandomPosition()
        {
            return startRandomPosition;
        }

        public void setStartRandomPosition(bool startRandomPosition)
        {
            this.startRandomPosition = startRandomPosition;
            if (startRandomPosition) startPositionList.Clear();
            else startRandomPositionCount = 0;
        }

        public int getStartRandomPositionCount()
        {
            return startRandomPositionCount;
        }

        public void setStartRandomPositionCount(int startRandomPositionCount)
        {
            startRandomPosition = true;
            this.startRandomPositionCount = startRandomPositionCount;
            startPositionList.Clear();
        }

        public List<MazeVector2IntDir> getStartPositionList()
        {
            return startPositionList;
        }

        public void setStartPositionList(List<MazeVector2IntDir> startPositionList)
        {
            startRandomPosition = false;
            startRandomPositionCount = 0;
            this.startPositionList = startPositionList;
        }

        public bool isFinishRandomPosition()
        {
            return finishRandomPosition;
        }

        public void setFinishRandomPosition(bool finishRandomPosition)
        {
            this.finishRandomPosition = finishRandomPosition;
            if (finishRandomPosition) finishPositionList.Clear();
            else finishRandomPositionCount = 0;
        }

        public int getFinishRandomPositionCount()
        {
            return finishRandomPositionCount;
        }

        public List<MazeVector2IntDir> getFinishPositionList()
        {
            return finishPositionList;
        }

        public void setFinishRandomPositionCount(int finishRandomPositionCount)
        {
            finishRandomPosition = true;
            this.finishRandomPositionCount = finishRandomPositionCount;
            finishPositionList.Clear();
        }

        public void setFinishPositionList(List<MazeVector2IntDir> finishPositionList)
        {
            finishRandomPosition = false;
            finishRandomPositionCount = 0;
            this.finishPositionList = finishPositionList;
        }

        public List<MazeVector2IntDir> getExitPositionList()
        {
            return exitPositionList;
        }

        public void setExitPositionList(List<MazeVector2IntDir> exitPositionList)
        {
            this.exitPositionList = exitPositionList;
        }

        public bool isObstacleIsNone()
        {
            return obstacleIsNone;
        }

        public void setObstacleIsNone(bool obstacleIsNone)
        {
            this.obstacleIsNone = obstacleIsNone;
        }

        public List<MazeVector2Int> getObstaclePositionList()
        {
            return obstaclePositionList;
        }

        public void setObstaclePositionList(List<MazeVector2Int> obstaclePositionList)
        {
            this.obstaclePositionList = obstaclePositionList;
        }

        public bool isOnlyPathMode()
        {
            return onlyPath;
        }

        public void setOnlyPathMode(bool onlyPath)
        {
            this.onlyPath = onlyPath;
        }

        public bool isUseSeed()
        {
            return useSeed;
        }

        public void setUseSeed(bool useSeed)
        {
            this.useSeed = useSeed;
        }

        public int GetSeed()
        {
            return seed;
        }

        public void SetSeed(int _seed)
        {
            seed = _seed;
        }

        public void GenerateMaze()
        {
            if (useSeed)
            {
                MazeMath.SetSeed(seed);
            }

            IEnumerator generationEnumerator = Generate(false);

            while (generationEnumerator.MoveNext()) ;
        }

        public void destroyImmediateMazeGeometry()
        {
            for (int i = transform.childCount - 1; i >= 0; i--)
            {
                DestroyImmediate(transform.GetChild(i).gameObject);
            }
        }

        public float getGenerationProgress()
        {
            return (generationProgress + instantiatingProgress) / (2 * mazeWidth * mazeHeight);
        }

        public MazePieceData[][] getMazeData()
        {
            return mazeArray;
        }

        public bool PointInMaze(MazeVector2Int _point)
        {
            bool inMaze = _point.x >= 0 && _point.x < mazeWidth && _point.y >= 0 && _point.y < mazeHeight;
            bool notInObstacle = !ListUtilities.Has(obstaclePositionList, _point);
            return inMaze && notInObstacle;
        }

        private delegate int CheckTask(MazeVector2Int _currentPosition, MazeVector2Int _newPosition, List<MazeOutput> _newPositionOutputs, MazeOutputDirection _direction);
        private const int CHECK_CONTINUE = 0;
        private const int CHECK_BREAK = 1;
        private const int CHECK_FAILED = 2;

        private IEnumerator Generate(bool asynchronous = true, float maxTime = 0.1f)
        {
            generationProgress = 0;

            startFinishLeft = 0;

            if (startRandomPosition)
            {
                startPositionList.Clear();
                generateRandomPoints(startPositionList, startRandomPositionCount);
            }
            startFinishLeft += startPositionList.Count;

            if (finishRandomPosition)
            {
                finishPositionList.Clear();
                generateRandomPoints(finishPositionList, finishRandomPositionCount);
            }
            startFinishLeft += finishPositionList.Count;

            MazeVector2Int startGenerationPoint = new MazeVector2Int(MazeMath.GetRandom(0, mazeWidth), MazeMath.GetRandom(0, mazeHeight));
            while (ListUtilities.Has(startPositionList, startGenerationPoint) ||
                   ListUtilities.Has(finishPositionList, startGenerationPoint) ||
                   ListUtilities.Has(obstaclePositionList, startGenerationPoint))
            {
                startGenerationPoint.x = MazeMath.GetRandom(0, mazeWidth);
                startGenerationPoint.y = MazeMath.GetRandom(0, mazeHeight);
            }

            path = new List<MazeVector2Int>();
            mazeArray = new MazePieceData[mazeWidth][];
            for (int px = 0; px < mazeWidth; px++)
            {
                mazeArray[px] = new MazePieceData[mazeHeight];
                for (int py = 0; py < mazeHeight; py++)
                {
                    mazeArray[px][py] = new MazePieceData(px, py);
                }
            }

            lastDirection = MazeOutputDirection.NotSpecified;
            MazeVector2Int currentPosition = new MazeVector2Int(startGenerationPoint.x, startGenerationPoint.y);

            MazeOutput output = new MazeOutput();
            mazeArray[currentPosition.x][currentPosition.y].outputs = new List<MazeOutput>();
            mazeArray[currentPosition.x][currentPosition.y].outputs.Add(output);

            path.Add(new MazeVector2Int(currentPosition.x, currentPosition.y));

            checkTaskList.Clear();
            if (startPositionList.Count > 0) checkTaskList.Add(checkStartPoint);
            if (finishPositionList.Count > 0) checkTaskList.Add(checkFinishPoint);
            checkTaskList.Add(checkStandard);
            if (piecePack.GetPiece(MazePieceType.Intersection).use) checkTaskList.Add(checkUnder);
            if (piecePack.GetPiece(MazePieceType.Crossing).use && !onlyPath) checkTaskList.Add(checkCrossing);
            if (piecePack.GetPiece(MazePieceType.Triple).use && !onlyPath) checkTaskList.Add(checkTripple);
            if (piecePack.GetPiece(MazePieceType.DoubleCorner).use) checkTaskList.Add(checkDoubleCorner);
            if (piecePack.GetPiece(MazePieceType.DeadlockCorner).use) checkTaskList.Add(checkDeadlockCorner);
            if (piecePack.GetPiece(MazePieceType.DeadlockLine).use) checkTaskList.Add(checkDeadlockLine);
            if (piecePack.GetPiece(MazePieceType.DeadlockTriple).use) checkTaskList.Add(checkDeadlockTriple);
            if (piecePack.GetPiece(MazePieceType.DeadlockCrossing).use) checkTaskList.Add(checkDeadlockCrossing);
            if (piecePack.GetPiece(MazePieceType.TripleDeadlock).use) checkTaskList.Add(checkTripleDeadlock);
            if (piecePack.GetPiece(MazePieceType.LineDeadlock).use) checkTaskList.Add(checkLineDeadlock);
            if (piecePack.GetPiece(MazePieceType.LineDeadlockLine).use) checkTaskList.Add(checkLineDeadlockLine);
            if (piecePack.GetPiece(MazePieceType.CornerDeadlockLeft).use) checkTaskList.Add(checkCornerDeadlock1);
            if (piecePack.GetPiece(MazePieceType.CornerDeadlockRight).use) checkTaskList.Add(checkCornerDeadlock2);
            if (piecePack.GetPiece(MazePieceType.CornerDeadlockCorner).use) checkTaskList.Add(checkCornerDeadlockCorner);
            if (piecePack.GetPiece(MazePieceType.None).use) checkTaskList.Add(checkNone);

            float time = Time.realtimeSinceStartup;

            do
            {
                int lastPathIndex = path.Count - 1;
                currentPosition.set(path[lastPathIndex].x, path[lastPathIndex].y);

                lastDirection = MazeOutputDirection.NotSpecified;
                MazeOutput outputArray = MazeOutput.GetShuffleOutput();

                foreach (MazeOutputDirection dir in outputArray.outputDirList)
                {
                    MazeVector2Int newPosition = new MazeVector2Int(currentPosition.x + MazeOutput.dx[dir], currentPosition.y + MazeOutput.dy[dir]);
                    if (PointInMaze(newPosition))
                    {
                        if (mazeArray[currentPosition.x][currentPosition.y].outputs.Count == 1)
                        {
                            List<MazeOutput> newPositionOutputs = mazeArray[newPosition.x][newPosition.y].outputs;

                            int checkResult = 0;
                            for (int i = 0; i < checkTaskList.Count; i++)
                            {
                                CheckTask checkTask = checkTaskList[i];
                                checkResult = checkTask(currentPosition, newPosition, newPositionOutputs, dir);
                                if (checkResult != CHECK_FAILED) break;
                            }

                            MazeVector2IntDir exit = ListUtilities.Get(exitPositionList, currentPosition);
                            if (exit != null)
                            {
                                if (!mazeArray[currentPosition.x][currentPosition.y].outputs[0].outputDirList.Contains(exit.direction))
                                    mazeArray[currentPosition.x][currentPosition.y].outputs[0].outputDirList.Add(exit.direction);
                            }

                            if (checkResult == CHECK_CONTINUE) continue;
                            if (checkResult == CHECK_BREAK)
                            {
                                generationProgress++;
                                break;
                            }
                        }
                    }
                }

                if (lastDirection == MazeOutputDirection.NotSpecified)
                    path.RemoveAt(path.Count - 1);

                if (asynchronous && Time.realtimeSinceStartup - time > maxTime)
                {
                    time = Time.realtimeSinceStartup;
                    yield return null;
                    if (mazeGenerateProgressEvent != null)
                        mazeGenerateProgressEvent.Invoke(getGenerationProgress());
                }
            }
            while (path.Count > 0);

            List<MazePiece> pieces = piecePack.GetMazePieceList();
            for (int i = 0; i < pieces.Count; i++)
            {
                if ((!pieces[i].use && !pieces[i].isRequire()) ||
                    pieces[i].getType() == MazePieceType.Start ||
                    pieces[i].getType() == MazePieceType.Finish)
                {
                    pieces.RemoveAt(i);
                    i--;
                }
            }

            float count = 0;
            instantiatingProgress = 0;
            bool wasError = false;
            float mazeSize = mazeWidth * mazeHeight;

            for (int ix = 0; ix < mazeWidth; ix++)
            {
                for (int iy = 0; iy < mazeHeight; iy++)
                {
                    MazePieceData mazePieceData = mazeArray[ix][iy];

                    MazePiece targetPiece = null;

                    if (ListUtilities.has(startPositionList, ix, iy) && mazePieceData.outputs != null && piecePack.GetPiece(MazePieceType.Start).checkFit(mazePieceData.outputs))
                    {
                        targetPiece = piecePack.GetPiece(MazePieceType.Start);
                    }
                    else if (ListUtilities.has(finishPositionList, ix, iy) && mazePieceData.outputs != null && piecePack.GetPiece(MazePieceType.Finish).checkFit(mazePieceData.outputs))
                    {
                        targetPiece = piecePack.GetPiece(MazePieceType.Finish);
                    }
                    else
                    {
                        ListUtilities.Shuffle<MazePiece>(pieces);
                        for (int i = 0; i < pieces.Count; i++)
                        {
                            if (pieces[i].checkFit(mazePieceData.outputs))
                            {
                                targetPiece = pieces[i];
                                break;
                            }
                        }
                    }

                    if (targetPiece == null)
                    {
                        if (PointInMaze(new MazeVector2Int(ix, iy)) || obstacleIsNone)
                        {
                            targetPiece = piecePack.GetPiece(MazePieceType.None);
                        }
                        else
                        {
                            continue;
                        }
                    }
                    else if (targetPiece.geometryList.Count == 0)
                    {
                        if (PointInMaze(new MazeVector2Int(ix, iy)))
                        {
                            if (!wasError)
                            {
                                wasError = true;
                                Debug.LogWarning("QMaze: Geometry for " + targetPiece.getType() + " piece is not found. Please check that geometry is specified for it in the piece pack.");
                            }
                        }
                        continue;
                    }

                    mazePieceData.type = targetPiece.getType();
                    mazePieceData.rotation = -targetPiece.getRotation();

                    generateGeometry(mazePieceData);

                    count++;
                    instantiatingProgress = count / mazeSize;

                    if (mazePieceGeneratedEvent != null)
                        mazePieceGeneratedEvent.Invoke(mazePieceData);

                    if (Time.realtimeSinceStartup - time > maxTime)
                    {
                        time = Time.realtimeSinceStartup;
                        yield return null;
                        if (mazeGenerateProgressEvent != null)
                            mazeGenerateProgressEvent.Invoke(getGenerationProgress());
                    }
                }
            }

            if (mazeGeneratedEvent != null)
                mazeGeneratedEvent.Invoke(this);
        }

        private void generateGeometry(MazePieceData pieceData)
        {
            MazePiece targetPiece = piecePack.GetPiece(pieceData.type);

            GameObject prefab = targetPiece.geometryList[MazeMath.GetRandom(0, targetPiece.geometryList.Count)];
            GameObject go;
#if UNITY_EDITOR
            if (Application.isPlaying)
            {
                go = (GameObject)GameObject.Instantiate(prefab, new Vector3(), Quaternion.AngleAxis(pieceData.rotation, Vector3.up));
            }
            else
            {
                PrefabType type = PrefabUtility.GetPrefabType(prefab);
                if (type == PrefabType.Prefab)
                {
                    go = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
                }
                else
                {
                    go = (GameObject)GameObject.Instantiate(prefab, new Vector3(), Quaternion.AngleAxis(pieceData.rotation, Vector3.up));
                }
            }
#else
				go = (GameObject)GameObject.Instantiate(prefab, new Vector3(), Quaternion.AngleAxis(-targetPiece.getRotation(), Vector3.up));
#endif
            go.transform.parent = transform;
            go.transform.localPosition = new Vector3(pieceData.x * mazePieceWidth * mazeScale, 0, -pieceData.y * mazePieceHeight * mazeScale);
            go.transform.rotation = transform.rotation * Quaternion.AngleAxis(pieceData.rotation, Vector3.up);

            Vector3 scale = go.transform.localScale;
            go.transform.localScale = scale * mazeScale;

            pieceData.geometry = go;
        }

        private void generateRandomPoints(List<MazeVector2IntDir> pointList, int randomCount)
        {
            for (int i = 0; i < randomCount; i++)
            {
                MazeVector2IntDir newPoint = new MazeVector2IntDir(MazeMath.GetRandom(0, mazeWidth), MazeMath.GetRandom(0, mazeHeight), MazeOutputDirection.NotSpecified);
                while (!PointInMaze(newPoint) || ListUtilities.Has(startPositionList, newPoint) || ListUtilities.Has(finishPositionList, newPoint) || ListUtilities.Has(exitPositionList, newPoint))
                {
                    newPoint.x = MazeMath.GetRandom(0, mazeWidth);
                    newPoint.y = MazeMath.GetRandom(0, mazeHeight);
                }
                pointList.Add(newPoint);
            }
        }

        private int checkStartPoint(MazeVector2Int currentPosition, MazeVector2Int newPosition, List<MazeOutput> newPositionOutputs, MazeOutputDirection dir)
        {
            if (ListUtilities.Has(startPositionList, newPosition))
            {
                if (mazeArray[newPosition.x][newPosition.y].outputs == null)
                {
                    MazeVector2IntDir startPoint = ListUtilities.Get(startPositionList, new MazeVector2IntDir(newPosition.x, newPosition.y, MazeOutput.opposite[dir]));
                    if (startPoint != null)
                    {
                        MazeOutput output = mazeArray[currentPosition.x][currentPosition.y].outputs[mazeArray[currentPosition.x][currentPosition.y].outputs.Count - 1];
                        output.outputDirList.Add(dir);

                        output = new MazeOutput();
                        output.outputDirList.Add(MazeOutput.opposite[dir]);
                        mazeArray[newPosition.x][newPosition.y].outputs = new List<MazeOutput>();
                        mazeArray[newPosition.x][newPosition.y].outputs.Add(output);

                        if (startPoint.direction == MazeOutputDirection.NotSpecified)
                            startPoint.direction = MazeOutput.opposite[dir];
                        startFinishLeft--;
                    }
                    else
                    {
                        return CHECK_CONTINUE;
                    }
                }
                return CHECK_CONTINUE;
            }
            else if (ListUtilities.Has(startPositionList, currentPosition))
            {
                MazeVector2IntDir startPoint = ListUtilities.Get(startPositionList, new MazeVector2IntDir(currentPosition.x, currentPosition.y, MazeOutput.opposite[dir]));
                if (startPoint != null && startPoint.direction == MazeOutputDirection.NotSpecified)
                    startPoint.direction = MazeOutput.opposite[dir];
                return CHECK_BREAK;
            }
            return CHECK_FAILED;
        }

        private int checkFinishPoint(MazeVector2Int currentPosition, MazeVector2Int newPosition, List<MazeOutput> newPositionOutputs, MazeOutputDirection dir)
        {
            if (ListUtilities.Has(finishPositionList, newPosition))
            {
                if (mazeArray[newPosition.x][newPosition.y].outputs == null)
                {
                    MazeVector2IntDir finishPoint = ListUtilities.Get(finishPositionList, new MazeVector2IntDir(newPosition.x, newPosition.y, MazeOutput.opposite[dir]));
                    if (finishPoint != null)
                    {
                        MazeOutput output = mazeArray[currentPosition.x][currentPosition.y].outputs[mazeArray[currentPosition.x][currentPosition.y].outputs.Count - 1];
                        output.outputDirList.Add(dir);

                        output = new MazeOutput();
                        output.outputDirList.Add(MazeOutput.opposite[dir]);
                        mazeArray[newPosition.x][newPosition.y].outputs = new List<MazeOutput>();
                        mazeArray[newPosition.x][newPosition.y].outputs.Add(output);

                        if (finishPoint.direction == MazeOutputDirection.NotSpecified)
                            finishPoint.direction = MazeOutput.opposite[dir];
                        startFinishLeft--;
                    }
                    else
                    {
                        return CHECK_CONTINUE;
                    }
                }
                return CHECK_CONTINUE;
            }
            else if (ListUtilities.Has(finishPositionList, currentPosition))
            {
                MazeVector2IntDir finishPoint = ListUtilities.Get(finishPositionList, new MazeVector2IntDir(currentPosition.x, currentPosition.y, MazeOutput.opposite[dir]));
                if (finishPoint != null && finishPoint.direction == MazeOutputDirection.NotSpecified)
                    finishPoint.direction = MazeOutput.opposite[dir];
                return CHECK_BREAK;
            }
            return CHECK_FAILED;
        }

        private int checkStandard(MazeVector2Int currentPosition, MazeVector2Int newPosition, List<MazeOutput> newPositionOutputs, MazeOutputDirection dir)
        {
            if (mazeArray[newPosition.x][newPosition.y].outputs == null)
            {
                MazeOutput output = mazeArray[currentPosition.x][currentPosition.y].outputs[mazeArray[currentPosition.x][currentPosition.y].outputs.Count - 1];
                output.outputDirList.Add(dir);

                output = new MazeOutput();
                output.outputDirList.Add(MazeOutput.opposite[dir]);
                mazeArray[newPosition.x][newPosition.y].outputs = new List<MazeOutput>();
                mazeArray[newPosition.x][newPosition.y].outputs.Add(output);

                path.Add(new MazeVector2Int(newPosition.x, newPosition.y));
                lastDirection = dir;

                return CHECK_BREAK;
            }
            else
            {
                bool found = false;
                for (int i = 0; i < mazeArray[currentPosition.x][currentPosition.y].outputs.Count; i++)
                {
                    MazeOutput mazeOutput = mazeArray[currentPosition.x][currentPosition.y].outputs[i];

                    for (int k = 0; k < mazeOutput.outputDirList.Count; k++)
                    {
                        if (mazeOutput.outputDirList[k] == dir)
                        {
                            found = true;
                            break;
                        }
                    }
                }

                if (!found)
                {
                    for (int i = 0; i < mazeArray[newPosition.x][newPosition.y].outputs.Count; i++)
                    {
                        MazeOutput mazeOutput = mazeArray[newPosition.x][newPosition.y].outputs[i];

                        for (int k = 0; k < mazeOutput.outputDirList.Count; k++)
                        {
                            if (mazeOutput.outputDirList[k] == MazeOutput.opposite[dir])
                            {
                                MazeOutput output = mazeArray[currentPosition.x][currentPosition.y].outputs[0];
                                output.outputDirList.Add(dir);
                            }
                        }
                    }
                }
            }
            return CHECK_FAILED;
        }

        private int checkUnder(MazeVector2Int currentPosition, MazeVector2Int newPosition, List<MazeOutput> newPositionOutputs, MazeOutputDirection dir)
        {
            if (MazeMath.GetRandom() < piecePack.GetPiece(MazePieceType.Intersection).frequency &&
                newPositionOutputs != null &&
                newPositionOutputs.Count == 1 &&
                newPositionOutputs[0].outputDirList.Count == 2 &&
                !newPositionOutputs[0].outputDirList.Contains(dir) &&
                !newPositionOutputs[0].outputDirList.Contains(MazeOutput.opposite[dir]))
            {
                MazeVector2Int newPosition2 = newPosition.clone();
                newPosition2.x += MazeOutput.dx[dir];
                newPosition2.y += MazeOutput.dy[dir];

                if (PointInMaze(newPosition2) &&
                    mazeArray[newPosition2.x][newPosition2.y].outputs == null &&
                    !ListUtilities.Has(startPositionList, newPosition2) &&
                    !ListUtilities.Has(finishPositionList, newPosition2))
                {
                    MazeOutput output = mazeArray[currentPosition.x][currentPosition.y].outputs[mazeArray[currentPosition.x][currentPosition.y].outputs.Count - 1];
                    output.outputDirList.Add(dir);

                    output = new MazeOutput();
                    output.outputDirList.Add(dir);
                    output.outputDirList.Add(MazeOutput.opposite[dir]);
                    mazeArray[newPosition.x][newPosition.y].outputs.Add(output);

                    output = new MazeOutput();
                    output.outputDirList.Add(MazeOutput.opposite[dir]);
                    mazeArray[newPosition2.x][newPosition2.y].outputs = new List<MazeOutput>();
                    mazeArray[newPosition2.x][newPosition2.y].outputs.Add(output);

                    path.Add(new MazeVector2Int(newPosition2.x, newPosition2.y));
                    lastDirection = dir;

                    return CHECK_BREAK;
                }
            }
            return CHECK_FAILED;
        }

        private int checkCrossing(MazeVector2Int currentPosition, MazeVector2Int newPosition, List<MazeOutput> newPositionOutputs, MazeOutputDirection dir)
        {
            if (MazeMath.GetRandom() < piecePack.GetPiece(MazePieceType.Crossing).frequency &&
                newPositionOutputs != null &&
                newPositionOutputs.Count == 1 &&
                newPositionOutputs[0].outputDirList.Count == 2 &&
                !newPositionOutputs[0].outputDirList.Contains(dir) &&
                !newPositionOutputs[0].outputDirList.Contains(MazeOutput.opposite[dir]))
            {
                MazeVector2Int newPosition2 = newPosition.clone();
                newPosition2.x += MazeOutput.dx[dir];
                newPosition2.y += MazeOutput.dy[dir];

                if (PointInMaze(newPosition2) &&
                    mazeArray[newPosition2.x][newPosition2.y].outputs == null)
                {
                    MazeOutput output = mazeArray[currentPosition.x][currentPosition.y].outputs[mazeArray[currentPosition.x][currentPosition.y].outputs.Count - 1];
                    output.outputDirList.Add(dir);

                    mazeArray[newPosition.x][newPosition.y].outputs[0].outputDirList.Add(dir);
                    mazeArray[newPosition.x][newPosition.y].outputs[0].outputDirList.Add(MazeOutput.opposite[dir]);

                    output = new MazeOutput();
                    output.outputDirList.Add(MazeOutput.opposite[dir]);
                    mazeArray[newPosition2.x][newPosition2.y].outputs = new List<MazeOutput>();
                    mazeArray[newPosition2.x][newPosition2.y].outputs.Add(output);

                    path.Add(new MazeVector2Int(newPosition2.x, newPosition2.y));
                    lastDirection = dir;

                    return CHECK_BREAK;
                }
            }
            return CHECK_FAILED;
        }

        private int checkTripple(MazeVector2Int currentPosition, MazeVector2Int newPosition, List<MazeOutput> newPositionOutputs, MazeOutputDirection dir)
        {
            if (MazeMath.GetRandom() < piecePack.GetPiece(MazePieceType.Triple).frequency &&
                newPositionOutputs.Count == 1 &&
                newPositionOutputs[0].outputDirList.Count == 2 &&
                newPositionOutputs[0].outputDirList.Contains(dir) &&
                !newPositionOutputs[0].outputDirList.Contains(MazeOutput.opposite[dir]))
            {
                MazeOutput output = mazeArray[currentPosition.x][currentPosition.y].outputs[mazeArray[currentPosition.x][currentPosition.y].outputs.Count - 1];
                output.outputDirList.Add(dir);

                newPositionOutputs[newPositionOutputs.Count - 1].outputDirList.Add(MazeOutput.opposite[dir]);

                return CHECK_CONTINUE;
            }
            return CHECK_FAILED;
        }

        private int checkDoubleCorner(MazeVector2Int currentPosition, MazeVector2Int newPosition, List<MazeOutput> newPositionOutputs, MazeOutputDirection dir)
        {
            if (MazeMath.GetRandom() < piecePack.GetPiece(MazePieceType.DoubleCorner).frequency &&
                newPositionOutputs.Count == 1 &&
                newPositionOutputs[0].outputDirList.Count == 2 &&
                newPositionOutputs[0].outputDirList.Contains(dir) &&
                !newPositionOutputs[0].outputDirList.Contains(MazeOutput.opposite[dir]))
            {
                MazeVector2Int newPos1 = new MazeVector2Int(newPosition.x + MazeOutput.dx[MazeOutput.rotateCW[dir]],
                                                      newPosition.y + MazeOutput.dy[MazeOutput.rotateCW[dir]]);
                MazeVector2Int newPos2 = new MazeVector2Int(newPosition.x + MazeOutput.dx[MazeOutput.rotateCCW[dir]],
                                                      newPosition.y + MazeOutput.dy[MazeOutput.rotateCCW[dir]]);

                if ((PointInMaze(newPos1) &&
                     mazeArray[newPos1.x][newPos1.y].outputs == null &&
                     newPositionOutputs[0].outputDirList.Contains(MazeOutput.rotateCCW[dir]) &&
                     !ListUtilities.Has(startPositionList, newPos1) &&
                     !ListUtilities.Has(finishPositionList, newPos1))
                    ||
                    (PointInMaze(newPos2) &&
                     mazeArray[newPos2.x][newPos2.y].outputs == null &&
                     newPositionOutputs[0].outputDirList.Contains(MazeOutput.rotateCW[dir]) &&
                    !ListUtilities.Has(startPositionList, newPos2) &&
                    !ListUtilities.Has(finishPositionList, newPos2)))
                {
                    MazeOutputDirection dir2 = dir;

                    MazeOutput output = mazeArray[currentPosition.x][currentPosition.y].outputs[mazeArray[currentPosition.x][currentPosition.y].outputs.Count - 1];
                    output.outputDirList.Add(dir);

                    output = new MazeOutput();
                    output.outputDirList.Add(MazeOutput.opposite[dir]);
                    if (!mazeArray[newPosition.x][newPosition.y].outputs[0].outputDirList.Contains(MazeOutput.rotateCW[dir]))
                    {
                        output.outputDirList.Add(MazeOutput.rotateCW[dir]);
                        dir2 = MazeOutput.rotateCW[dir];
                    }
                    else
                    {
                        output.outputDirList.Add(MazeOutput.rotateCCW[dir]);
                        dir2 = MazeOutput.rotateCCW[dir];
                    }
                    mazeArray[newPosition.x][newPosition.y].outputs.Add(output);

                    newPosition.x += MazeOutput.dx[dir2];
                    newPosition.y += MazeOutput.dy[dir2];

                    output = new MazeOutput();
                    output.outputDirList.Add(MazeOutput.opposite[dir2]);
                    mazeArray[newPosition.x][newPosition.y].outputs = new List<MazeOutput>();
                    mazeArray[newPosition.x][newPosition.y].outputs.Add(output);

                    path.Add(new MazeVector2Int(newPosition.x, newPosition.y));
                    lastDirection = dir2;

                    return CHECK_BREAK;
                }
            }
            return CHECK_FAILED;
        }

        private int checkDeadlockCorner(MazeVector2Int currentPosition, MazeVector2Int newPosition, List<MazeOutput> newPositionOutputs, MazeOutputDirection dir)
        {
            if (MazeMath.GetRandom() < piecePack.GetPiece(MazePieceType.DeadlockCorner).frequency &&
                newPositionOutputs != null &&
                newPositionOutputs.Count == 1 &&
                newPositionOutputs[0].outputDirList.Count == 1 &&
                (newPositionOutputs[0].outputDirList.Contains(MazeOutput.rotateCW[dir]) || newPositionOutputs[0].outputDirList.Contains(MazeOutput.rotateCCW[dir])))
            {
                MazeOutput output = mazeArray[currentPosition.x][currentPosition.y].outputs[mazeArray[currentPosition.x][currentPosition.y].outputs.Count - 1];
                output.outputDirList.Add(dir);

                output = new MazeOutput();
                output.outputDirList.Add(MazeOutput.opposite[dir]);
                mazeArray[newPosition.x][newPosition.y].outputs.Add(output);

                return CHECK_CONTINUE;
            }
            return CHECK_FAILED;
        }

        private int checkDeadlockLine(MazeVector2Int currentPosition, MazeVector2Int newPosition, List<MazeOutput> newPositionOutputs, MazeOutputDirection dir)
        {
            if (MazeMath.GetRandom() < piecePack.GetPiece(MazePieceType.DeadlockLine).frequency &&
                newPositionOutputs != null &&
                newPositionOutputs.Count == 1 &&
                newPositionOutputs[0].outputDirList.Count == 1 &&
                newPositionOutputs[0].outputDirList.Contains(dir))
            {
                MazeOutput output = mazeArray[currentPosition.x][currentPosition.y].outputs[mazeArray[currentPosition.x][currentPosition.y].outputs.Count - 1];
                output.outputDirList.Add(dir);

                output = new MazeOutput();
                output.outputDirList.Add(MazeOutput.opposite[dir]);
                mazeArray[newPosition.x][newPosition.y].outputs.Add(output);

                return CHECK_CONTINUE;
            }
            return CHECK_FAILED;
        }

        private int checkDeadlockTriple(MazeVector2Int currentPosition, MazeVector2Int newPosition, List<MazeOutput> newPositionOutputs, MazeOutputDirection dir)
        {
            if (MazeMath.GetRandom() < piecePack.GetPiece(MazePieceType.DeadlockTriple).frequency &&
                newPositionOutputs != null &&
                newPositionOutputs.Count == 2 &&
                newPositionOutputs[0].outputDirList.Count == 1 &&
                !newPositionOutputs[0].outputDirList.Contains(MazeOutput.opposite[dir]) &&
                newPositionOutputs[1].outputDirList.Count == 1 &&
                !newPositionOutputs[1].outputDirList.Contains(MazeOutput.opposite[dir]))
            {
                MazeOutput output = mazeArray[currentPosition.x][currentPosition.y].outputs[mazeArray[currentPosition.x][currentPosition.y].outputs.Count - 1];
                output.outputDirList.Add(dir);

                output = new MazeOutput();
                output.outputDirList.Add(MazeOutput.opposite[dir]);
                mazeArray[newPosition.x][newPosition.y].outputs.Add(output);

                return CHECK_CONTINUE;
            }
            return CHECK_FAILED;
        }

        private int checkDeadlockCrossing(MazeVector2Int currentPosition, MazeVector2Int newPosition, List<MazeOutput> newPositionOutputs, MazeOutputDirection dir)
        {
            if (MazeMath.GetRandom() < piecePack.GetPiece(MazePieceType.DeadlockCrossing).frequency &&
                newPositionOutputs != null &&
                newPositionOutputs.Count == 3 &&
                newPositionOutputs[0].outputDirList.Count == 1 &&
                !newPositionOutputs[0].outputDirList.Contains(MazeOutput.opposite[dir]) &&
                newPositionOutputs[1].outputDirList.Count == 1 &&
                !newPositionOutputs[1].outputDirList.Contains(MazeOutput.opposite[dir]) &&
                newPositionOutputs[2].outputDirList.Count == 1 &&
                !newPositionOutputs[2].outputDirList.Contains(MazeOutput.opposite[dir]))
            {
                MazeOutput output = mazeArray[currentPosition.x][currentPosition.y].outputs[mazeArray[currentPosition.x][currentPosition.y].outputs.Count - 1];
                output.outputDirList.Add(dir);

                output = new MazeOutput();
                output.outputDirList.Add(MazeOutput.opposite[dir]);
                mazeArray[newPosition.x][newPosition.y].outputs.Add(output);

                return CHECK_CONTINUE;
            }
            return CHECK_FAILED;
        }

        private int checkTripleDeadlock(MazeVector2Int currentPosition, MazeVector2Int newPosition, List<MazeOutput> newPositionOutputs, MazeOutputDirection dir)
        {
            if (MazeMath.GetRandom() < piecePack.GetPiece(MazePieceType.TripleDeadlock).frequency &&
                newPositionOutputs != null &&
                newPositionOutputs.Count == 1 &&
                newPositionOutputs[0].outputDirList.Count == 3 &&
                !newPositionOutputs[0].outputDirList.Contains(MazeOutput.opposite[dir]))
            {
                MazeOutput output = mazeArray[currentPosition.x][currentPosition.y].outputs[mazeArray[currentPosition.x][currentPosition.y].outputs.Count - 1];
                output.outputDirList.Add(dir);

                output = new MazeOutput();
                output.outputDirList.Add(MazeOutput.opposite[dir]);
                mazeArray[newPosition.x][newPosition.y].outputs.Add(output);

                return CHECK_CONTINUE;
            }
            return CHECK_FAILED;
        }

        private int checkLineDeadlock(MazeVector2Int currentPosition, MazeVector2Int newPosition, List<MazeOutput> newPositionOutputs, MazeOutputDirection dir)
        {
            if (MazeMath.GetRandom() < piecePack.GetPiece(MazePieceType.LineDeadlock).frequency &&
                newPositionOutputs != null &&
                newPositionOutputs.Count == 1 &&
                newPositionOutputs[0].outputDirList.Count == 2 &&
                !newPositionOutputs[0].outputDirList.Contains(dir) &&
                !newPositionOutputs[0].outputDirList.Contains(MazeOutput.opposite[dir]))
            {
                MazeOutput output = mazeArray[currentPosition.x][currentPosition.y].outputs[mazeArray[currentPosition.x][currentPosition.y].outputs.Count - 1];
                output.outputDirList.Add(dir);

                output = new MazeOutput();
                output.outputDirList.Add(MazeOutput.opposite[dir]);
                mazeArray[newPosition.x][newPosition.y].outputs.Add(output);

                return CHECK_CONTINUE;
            }
            return CHECK_FAILED;
        }

        private int checkLineDeadlockLine(MazeVector2Int currentPosition, MazeVector2Int newPosition, List<MazeOutput> newPositionOutputs, MazeOutputDirection dir)
        {
            if (MazeMath.GetRandom() < piecePack.GetPiece(MazePieceType.LineDeadlockLine).frequency &&
                newPositionOutputs != null &&
                newPositionOutputs.Count == 2 &&
                !newPositionOutputs[0].outputDirList.Contains(MazeOutput.opposite[dir]) &&
                !newPositionOutputs[1].outputDirList.Contains(MazeOutput.opposite[dir]) &&
                ((newPositionOutputs[0].outputDirList.Count == 2 && newPositionOutputs[1].outputDirList.Count == 1 && newPositionOutputs[0].outputDirList[0] == MazeOutput.opposite[newPositionOutputs[0].outputDirList[1]]) ||
                 (newPositionOutputs[0].outputDirList.Count == 1 && newPositionOutputs[1].outputDirList.Count == 2 && newPositionOutputs[1].outputDirList[0] == MazeOutput.opposite[newPositionOutputs[1].outputDirList[1]])))
            {
                MazeOutput output = mazeArray[currentPosition.x][currentPosition.y].outputs[mazeArray[currentPosition.x][currentPosition.y].outputs.Count - 1];
                output.outputDirList.Add(dir);

                output = new MazeOutput();
                output.outputDirList.Add(MazeOutput.opposite[dir]);
                mazeArray[newPosition.x][newPosition.y].outputs.Add(output);

                return CHECK_CONTINUE;
            }
            return CHECK_FAILED;
        }

        private int checkCornerDeadlock1(MazeVector2Int currentPosition, MazeVector2Int newPosition, List<MazeOutput> newPositionOutputs, MazeOutputDirection dir)
        {
            if (MazeMath.GetRandom() < piecePack.GetPiece(MazePieceType.CornerDeadlockLeft).frequency &&
                newPositionOutputs != null &&
                newPositionOutputs.Count == 1 &&
                newPositionOutputs[0].outputDirList.Count == 2 &&
                newPositionOutputs[0].outputDirList.Contains(dir) &&
                newPositionOutputs[0].outputDirList.Contains(MazeOutput.rotateCW[dir]))
            {
                MazeOutput output = mazeArray[currentPosition.x][currentPosition.y].outputs[mazeArray[currentPosition.x][currentPosition.y].outputs.Count - 1];
                output.outputDirList.Add(dir);

                output = new MazeOutput();
                output.outputDirList.Add(MazeOutput.opposite[dir]);
                mazeArray[newPosition.x][newPosition.y].outputs.Add(output);

                return CHECK_CONTINUE;
            }
            return CHECK_FAILED;
        }

        private int checkCornerDeadlock2(MazeVector2Int currentPosition, MazeVector2Int newPosition, List<MazeOutput> newPositionOutputs, MazeOutputDirection dir)
        {
            if (MazeMath.GetRandom() < piecePack.GetPiece(MazePieceType.CornerDeadlockRight).frequency &&
                newPositionOutputs.Count == 1 &&
                newPositionOutputs[0].outputDirList.Count == 2 &&
                newPositionOutputs[0].outputDirList.Contains(dir) &&
                newPositionOutputs[0].outputDirList.Contains(MazeOutput.rotateCCW[dir]))
            {
                MazeOutput output = mazeArray[currentPosition.x][currentPosition.y].outputs[mazeArray[currentPosition.x][currentPosition.y].outputs.Count - 1];
                output.outputDirList.Add(dir);

                output = new MazeOutput();
                output.outputDirList.Add(MazeOutput.opposite[dir]);
                mazeArray[newPosition.x][newPosition.y].outputs.Add(output);

                return CHECK_CONTINUE;
            }
            return CHECK_FAILED;
        }

        private int checkCornerDeadlockCorner(MazeVector2Int currentPosition, MazeVector2Int newPosition, List<MazeOutput> newPositionOutputs, MazeOutputDirection dir)
        {
            if (MazeMath.GetRandom() < piecePack.GetPiece(MazePieceType.CornerDeadlockCorner).frequency &&
                newPositionOutputs.Count == 2 &&
                !newPositionOutputs[0].outputDirList.Contains(MazeOutput.opposite[dir]) &&
                !newPositionOutputs[1].outputDirList.Contains(MazeOutput.opposite[dir]) &&
                ((newPositionOutputs[0].outputDirList.Count == 2 && newPositionOutputs[1].outputDirList.Count == 1 && newPositionOutputs[0].outputDirList[0] != MazeOutput.opposite[newPositionOutputs[0].outputDirList[1]]) ||
                 (newPositionOutputs[0].outputDirList.Count == 1 && newPositionOutputs[1].outputDirList.Count == 2 && newPositionOutputs[1].outputDirList[0] != MazeOutput.opposite[newPositionOutputs[1].outputDirList[1]])))
            {
                MazeOutput output = mazeArray[currentPosition.x][currentPosition.y].outputs[mazeArray[currentPosition.x][currentPosition.y].outputs.Count - 1];
                output.outputDirList.Add(dir);

                output = new MazeOutput();
                output.outputDirList.Add(MazeOutput.opposite[dir]);
                mazeArray[newPosition.x][newPosition.y].outputs.Add(output);

                return CHECK_CONTINUE;
            }
            return CHECK_FAILED;
        }

        private int checkNone(MazeVector2Int currentPosition, MazeVector2Int newPosition, List<MazeOutput> newPositionOutputs, MazeOutputDirection dir)
        {
            if (startFinishLeft == 0 &&
                MazeMath.GetRandom() < piecePack.GetPiece(MazePieceType.None).frequency &&
                newPositionOutputs.Count == 1 &&
                newPositionOutputs[0].outputDirList.Count == 1 &&
                newPositionOutputs[0].outputDirList.Contains(MazeOutput.opposite[dir]))
            {
                newPositionOutputs.Clear();
                newPositionOutputs.Add(new MazeOutput());

                List<MazeOutput> currentOutputs = mazeArray[currentPosition.x][currentPosition.y].outputs;
                for (int i = 0; i < currentOutputs.Count; i++)
                {
                    currentOutputs[i].outputDirList.Remove(dir);
                }

                return CHECK_BREAK;
            }
            return CHECK_FAILED;
        }
    }
}
