using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TicTacToeMgr : MonoBehaviour
{
    GlobalsMgr g;
    public Material matLines;
    public Material matSolutions;
    GameObject goSolutions;
    const float distNear = .1f;
    const float delay = .2f;
    const float delayWin = 1.5f;
    float timeStartWin;
    public bool ynWon;
    Color colorX;
    Color colorO;
    Color colorEmpty;
    Color colorWin;
    Color colorAlmost;
    SolutionItemType solutionItemTypeWin = SolutionItemType.empty;
    GameObject goSolutionWin;
    GameObject goSpots;
    public SolutionItemType solutionItemTypeCurrent;
    SolutionItemType solutionItemTypeCurrentLast;
    public Vector3 posLocalEmpty;
    GameObject goPadX;
    GameObject goPadO;
    GameObject goAssetNearPointerLast;
    int cntSpotsFilled;
    public GameObject goAssetNearPointer;

    private void Awake()
    {
        g = GetComponent<GlobalsMgr>();
        goSolutions = GameObject.Find("Solutions");
        goSpots = GameObject.Find("Spots");
        colorX = (Color.blue + Color.clear) / 2; 
        colorO = (Color.red + Color.clear) / 2; 
        colorEmpty = (Color.clear + Color.clear) / 2;
        colorWin = (Color.green + Color.clear) / 2; 
        colorAlmost = colorWin;
        goPadX = GameObject.Find("PadXbody");
        goPadO = GameObject.Find("PadObody");
    }

    // Update is called once per frame
    void Update()
    {
        if (g.mode == ModeType.place) return;
        if (g.gameType != GameType.TicTacToe) return;
        UpdateGoAssetNearPointer();
        UpdateScore();
        UpdateWin();
        UpdateSolutionItemTypeCurrent();
        UpdateTicTacToeNumCompletedProgress();
        UpdateProgressTicTacToe();
        UpdateAuto();
        solutionItemTypeCurrentLast = solutionItemTypeCurrent;
        goAssetNearPointerLast = goAssetNearPointer;
    }

    void UpdateAuto()
    {
        if (g.ynAuto == true)
        {
            g.autoMgr.ResetTimeLastAuto();
            AutoTicTacToe();
        }
    }

    void AutoTicTacToe()
    {
        GameObject goPiece = FindNextPiece();
        if (goPiece != null)
        {
            GameObject goSpot = FindEmptySpot();
            if (goSpot != null)
            {
                g.assetMgr.MoveAssetToAsset(goPiece, goSpot);
            }
            else
            {
                Debug.Log("auto advance tictactoe\n");
                g.assetMgr.ButtonAdvanceAssetsClickedReLoad();
            }
        }
        else
        {
            Debug.Log("No piece Advance\n");
            g.assetMgr.ButtonAdvanceAssetsClickedReLoad();
        }
    }

    void UpdateTicTacToeNumCompletedProgress()
    {
        g.numCompletedProgressX = 0;
        g.numCompletedProgressO = 0;
        foreach (Transform t in goSpots.transform)
        {
            SolutionItemType solutionItemType = SolutionItemType.empty;
            GameObject go = GetPieceNearGo(t.gameObject);
            if (go != null)
            {
                solutionItemType = GetAssetSolutionItemType(go);
                if (solutionItemType == SolutionItemType.X)
                {
                    g.numCompletedProgressX++;
                } else
                {
                    g.numCompletedProgressO++;
                }
            }
        }
    }

    void UpdateProgressTicTacToe()
    {
        if (g.progressMgr.numCompletedProgressLastX != g.numCompletedProgressX)
        {
            UpdateIndicatorsTicTacToe(SolutionItemType.X);
            if (g.numCompletedProgressX == g.numTotalProgress)
            {
                CelebrateTicTacToe(SolutionItemType.X);
            }
        }
        if (g.progressMgr.numCompletedProgressLastO != g.numCompletedProgressO)
        {
            UpdateIndicatorsTicTacToe(SolutionItemType.O);
            if (g.numCompletedProgressO == g.numTotalProgress)
            {
                CelebrateTicTacToe(SolutionItemType.O);
            }
        }
    }

    void UpdateIndicatorsTicTacToe(SolutionItemType solutionItemType)
    {
        Color colorOn = (Color.blue * 1.5f + Color.clear) / 2;
        Color colorOff = (Color.white * 1.5f + Color.clear) / 2;
        int numCompletedProg = g.numCompletedProgressX;
        if (solutionItemType == SolutionItemType.O)
        {
            numCompletedProg = g.numCompletedProgressO;
            colorOn = (Color.red * 1.5f + Color.clear) / 2;
        }
        foreach (GameObject goList in g.goProgressLists)
        {
            if (g.progressMgr.GetProgressListType(goList) == solutionItemType)
            {
                for (int n = 0; n < goList.transform.childCount; n++)
                {
                    GameObject go = goList.transform.GetChild(n).gameObject;
                    if (n < numCompletedProg)
                    {
                        go.GetComponentInChildren<Renderer>().material.color = colorOn;
                    }
                    else
                    {
                        go.GetComponentInChildren<Renderer>().material.color = colorOff;
                    }
                }
            }
        }
    }

    void CelebrateTicTacToe(SolutionItemType solutionItemType)
    {
        Debug.Log("Celebrate TicTacToe.................\n");
        //SwitchSides();
        //Invoke("EndCelebration", 2);
    }

    public int GetNumTotalProgress()
    {
        return 4;
    }

    int GetCountSpotsFilled()
    {
        int cnt = 0;
        foreach(Transform t in goSpots.transform)
        {
            GameObject go = GetPieceNearGo(t.gameObject);
            if (go != null)
            {
                cnt++;
            }
        }
        return cnt;
    }

    public void UpdateGoAssetNearPointer()
    {
        goAssetNearPointer = null;
        foreach(Transform t in g.goAssets.transform)
        {
            if (g.assetMgr.IsPiece(t.gameObject) == true)
            {
                float dist = Vector3.Distance(t.position, g.goPointer.transform.position);
                if (dist < distNear)
                {
                    goAssetNearPointer = t.gameObject;
                    return;
                }
            }
        }
    }

    public void ColorSideIndicators(Color color)
    {
        goPadX.GetComponent<Renderer>().material.color = color;
        goPadO.GetComponent<Renderer>().material.color = color;
    }

    void UpdateSolutionItemTypeCurrent()
    {
        if (goAssetNearPointerLast != goAssetNearPointer)
        {
            if (goAssetNearPointer != null)
            {
                solutionItemTypeCurrent = GetAssetSolutionItemType(goAssetNearPointer);
                cntSpotsFilled = GetCountSpotsFilled();
            }
            else
            {
                int cntNow = GetCountSpotsFilled();
                if (cntNow == cntSpotsFilled + 1)
                {
                    SwitchSolutionItemTypeCurrent();
                }
            }
        }
        if (solutionItemTypeCurrentLast != solutionItemTypeCurrent)
        {
            if (solutionItemTypeCurrent == SolutionItemType.empty)
            {
                goPadX.GetComponent<Renderer>().material.color = Color.white;
                goPadO.GetComponent<Renderer>().material.color = Color.white;
            }
            if (solutionItemTypeCurrent == SolutionItemType.X)
            {
                goPadX.GetComponent<Renderer>().material.color = Color.green;
                goPadO.GetComponent<Renderer>().material.color = Color.white;
            }
            if (solutionItemTypeCurrent == SolutionItemType.O)
            {
                goPadX.GetComponent<Renderer>().material.color = Color.white;
                goPadO.GetComponent<Renderer>().material.color = Color.green;
            }
        }
    }

    void UpdateWin()
    {
        if (ynWon == true) return;
        if (goSolutionWin == null)
        {
            timeStartWin = Time.realtimeSinceStartup;
        }
        else
        {
            if (Time.realtimeSinceStartup - timeStartWin > delayWin)
            {
                ynWon = true;
                Win();
            }
        }
    }

    void Win()
    {
        if (solutionItemTypeWin == SolutionItemType.empty) return;
        if (solutionItemTypeWin == SolutionItemType.X)
        {
            g.numCompletedProgressX++;
        }
        else
        {
            g.numCompletedProgressO++;
        }
        if (g.numCompletedProgressX == g.numTotalProgress || g.numCompletedProgressO == g.numTotalProgress)
        {
            g.numCompletedProgressX = 0;
            g.numCompletedProgressO = 0;
        }
        Debug.Log("Win Advance\n");
        g.assetMgr.ButtonAdvanceAssetsClickedReLoad();
    }

    void UpdateScore()
    {
        if (ynWon == true) return;
        MatchAssetsWithSpots();
        MatchAssetsWithSolutionItems();
        ScoreSolutions();
    }

    void ScoreSolutions()
    {
        goSolutionWin = null;
        solutionItemTypeWin = SolutionItemType.empty;
        foreach (Transform tSolution in goSolutions.transform)
        {
            ScoreSolution(tSolution);
            if(goSolutionWin != null)
            {
                break;
            }
        }
    }

    void ScoreSolution(Transform tSolution)
    {
        List<GameObject>listX = GetSolutionItemsOfType(tSolution, SolutionItemType.X);
        List<GameObject> listO = GetSolutionItemsOfType(tSolution, SolutionItemType.O);
        List<GameObject> listEmpty = GetSolutionItemsOfType(tSolution, SolutionItemType.empty);
        tSolution.name = "Solution cntX " + listX.Count + " cntO " + listO.Count;
        if (listX.Count == 3)
        {
            solutionItemTypeWin = SolutionItemType.X;
            goSolutionWin = tSolution.gameObject;
            HighlightSolutionSpots(tSolution, colorWin);
            return;
        }
        if (listO.Count == 3)
        {
            solutionItemTypeWin = SolutionItemType.O;
            goSolutionWin = tSolution.gameObject;
            HighlightSolutionSpots(tSolution, colorWin);
            return;
        }
        if (listX.Count == 2 && listEmpty.Count == 1)
        {
            HighlightGoChildrenOfList(listX, colorX);
            HighlightGoChildrenOfList(listEmpty, colorAlmost);
        }
        if (listO.Count == 2 && listEmpty.Count == 1)
        {
            HighlightGoChildrenOfList(listO, colorO);
            HighlightGoChildrenOfList(listEmpty, colorAlmost);
        }
    }

    void HighlightGoChildrenOfList(List<GameObject> gos, Color color)
    {
        foreach(GameObject go in gos)
        {
            HighlightGoChildren(go, color);
        }
    }

    void HighlightSolutionSpots(Transform tSolution, Color color)
    {
        foreach(Transform t in tSolution)
        {
            GameObject go = GetSpotUnderGo(t.gameObject);
            HighlightGoChildren(go, color);
        }
    }

    GameObject GetSpotNearToGo(GameObject go)
    {
        foreach (Transform tSpot in goSpots.transform)
        {
            if (g.IsNear(go, tSpot.gameObject, distNear) == true)
            {
                return tSpot.gameObject;
            }
        }
        return null;
    }

    List<GameObject>GetSolutionItemsOfType(Transform tSolution, SolutionItemType solutionItemType)
    {
        List<GameObject>matches = new List<GameObject>();
        foreach (Transform t in tSolution)
        {

            if (GetGoSolutionItemType(t.gameObject) == solutionItemType)
            {
                matches.Add(t.gameObject);
            }
        }
        return matches;
    }

    SolutionItemType GetGoSolutionItemType(GameObject go)
    {
        SolutionItemType solutionItemType = (SolutionItemType)System.Enum.Parse(typeof(SolutionItemType), go.name);
        return solutionItemType;
    }

    void MatchAssetsWithSolutionItems()
    {
        foreach(Transform tt in goSolutions.transform)
        {
            foreach(Transform t in tt)
            {
                SolutionItemType solutionItemType = SolutionItemType.empty;
                GameObject go = GetPieceNearGo(t.gameObject);
                if (go != null)
                {
                    solutionItemType = GetAssetSolutionItemType(go);
                }
                AdjustGoSolutionItemType(t.gameObject, solutionItemType);
                Color color = GetColorForSolutionItemType(solutionItemType);
                HighlightGoChildren(t.gameObject, color);
            }
        }
    }

    GameObject GetPieceNearGo(GameObject go)
    {
        foreach (Transform tAsset in g.goAssets.transform)
        {
            if (g.assetMgr.IsPiece(tAsset.gameObject) == true)
            {
                if (g.IsNear(go, tAsset.gameObject, distNear) == true)
                {
                    return tAsset.gameObject;
                }
            }
        }
        return null;
    }

    void MatchAssetsWithSpots()
    {
        foreach (Transform t in goSpots.transform)
        {
            SolutionItemType solutionItemType = SolutionItemType.empty;
            GameObject go = GetPieceNearGo(t.gameObject);
            if (go != null)
            {
                solutionItemType = GetAssetSolutionItemType(go);
            }
            AdjustGoSolutionItemType(t.gameObject, solutionItemType);
            Color color = GetColorForSolutionItemType(solutionItemType);
            HighlightGoChildren(t.gameObject, color);
        }
    }

    GameObject GetSpotUnderGo(GameObject go)
    {
        foreach (Transform t in goSpots.transform)
        {
            if (g.IsNear(t.gameObject, go, distNear) == true)
            {
                return t.gameObject;
            }
        }
        return null;
    }

    public void SwitchSolutionItemTypeCurrent()
    {
        if (solutionItemTypeCurrent == SolutionItemType.X)
        {
            solutionItemTypeCurrent = SolutionItemType.O;
        }
        else
        {
            solutionItemTypeCurrent = SolutionItemType.X;
        }
    }

    public GameObject FindNextPiece()
    {
        List<GameObject> choices = new List<GameObject>();
        GameObject goPiece = null;
        SwitchSolutionItemTypeCurrent();
        for (int n = 0; n < g.goAssets.transform.childCount; n++)
        {
            GameObject go = g.goAssets.transform.GetChild(n).gameObject;
            if (g.assetMgr.IsPiece(go) == true)
            {
                if (GetSpotUnderGo(go) == false)
                {
                    if (GetAssetSolutionItemType(go) == solutionItemTypeCurrent)
                    {
                        choices.Add(go);
                    }
                }
            }
        }
        if (choices.Count > 0)
        {
            int nChoice = Random.Range(0, choices.Count - 1);
            goPiece = choices[nChoice];
        }
        return goPiece;
    }

    public GameObject FindEmptySpot()
    {
        GameObject goSpot = null;
        List<GameObject> choices = new List<GameObject>();
        foreach(Transform t in goSpots.transform)
        {
            GameObject goPiece = GetPieceNearGo(t.gameObject);
            if (goPiece == null)
            {
                choices.Add(t.gameObject);
            }
        }
        if (choices.Count > 0)
        {
            int nChoice = Random.Range(0, choices.Count - 1);
            goSpot = choices[nChoice];
        }
        return goSpot;
    }

    public void ResetTicTacToe()
    {
        ynWon = false;
        goSolutionWin = null;
        solutionItemTypeCurrent = SolutionItemType.empty;
        g.goAsset = null;
    }

    void AdjustGoSolutionItemType(GameObject go, SolutionItemType solutionItemType)
    {
        go.name = solutionItemType.ToString();
    }

    SolutionItemType GetSolutionItemType(Transform t)
    {
        foreach (Transform tAsset in g.goAssets.transform)
        {
            if (g.assetMgr.IsPiece(tAsset.gameObject) == true)
            {
                if (g.IsNear(t.gameObject, tAsset.gameObject, distNear) == true)
                {
                    return GetAssetSolutionItemType(tAsset.gameObject);
                }
            }
        }
        return SolutionItemType.empty;
    }

    public GameObject FindEmptySolutionItemX()
    {
        List<GameObject> choices = new List<GameObject>();
        foreach (Transform tParent in goSolutions.transform)
        {
            foreach (Transform t in tParent)
            {
                bool ynFound = false;
                foreach (Transform tAsset in g.goAssets.transform)
                {
                    if (g.assetMgr.IsPiece(tAsset.gameObject) == true)
                    {
                        if (g.IsNear(t.gameObject, tAsset.gameObject, distNear) == true)
                        {
                            ynFound = true;
                            break;
                        }
                    }
                }
                if (ynFound == false)
                {
                    choices.Add(t.gameObject);
//                    return t.gameObject;
                }
            }
        }
        if (choices.Count > 0) { 
            int n = Random.Range(0, choices.Count - 1);
            return choices[n];
        }
        else
        {
            return null;
        }
    }

    public SolutionItemType GetAssetSolutionItemType(GameObject go)
    {
        if (go.name.Contains("PieceX") == true)
        {
            return SolutionItemType.X;
        }
        if (go.name.Contains("PieceO") == true)
        {
            return SolutionItemType.O;
        }
        return SolutionItemType.empty;
    }

    void ScaleSolutions(float sca)
    {
        foreach (Transform t in goSolutions.transform)
        {
            ScaleSolution(t, sca);
        }
    }

    void ScaleSolution(Transform tParent, float sca)
    {
        foreach (Transform t in tParent)
        {
            ScaleSolutionItem(t.gameObject, sca);
        }
    }

    void ScaleSolutionItem(GameObject go, float sca)
    {
        go.transform.localScale = Vector3.one * sca;
    }

    void HighlightGreatGrandChildren(GameObject go, Color color)
    {
        foreach (Transform t in go.transform)
        {
            HighlightGoGrandChildren(t.gameObject, color);
        }
    }

    void HighlightGoGrandChildren(GameObject go, Color color)
    {
        foreach (Transform t in go.transform)
        {
            HighlightGoChildren(t.gameObject, color);
        }
    }

    Color GetColorForSolutionItemType(SolutionItemType solutionItemType)
    {
        if (solutionItemType == SolutionItemType.X)
        {
            return colorX;
        }
        if (solutionItemType == SolutionItemType.O)
        {
            return colorO;
        }
        if (solutionItemType == SolutionItemType.empty)
        {
            return colorEmpty;
        }
        return Color.magenta;
    }

    void HighlightGoChildren(GameObject go, Color color)
    {
        Renderer[] rends = go.GetComponentsInChildren<Renderer>();
        foreach(Renderer rend in rends)
        {
            rend.material.color = color;
        }
    }
}