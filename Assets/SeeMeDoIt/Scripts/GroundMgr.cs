using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundMgr : MonoBehaviour
{
    GlobalsMgr g;
    GameType gameTypeLast;

    private void Awake()
    {
        g = GetComponent<GlobalsMgr>();     
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        UpdateGameChange();
        gameTypeLast = g.gameType;
    }

    void UpdateGameChange()
    {
        if (g.gameType != gameTypeLast)
        {
            //TurnOnOffTicTacToe(false);
            //TurnOnOffConnect(false);
            //TurnOnOffScrew(false);
            //TurnOnOffChess(false);
            switch (g.gameType)
            {
                case GameType.TicTacToe:
                    TurnOnTicTacToe();
                    break;
                case GameType.Connect:
                    TurnOnConnect();
                    break;
                case GameType.Screw:
                    TurnOnScrew();
                    break;
                case GameType.Chess:
                    TurnOnChess();
                    break;
                case GameType.Pong:
                    TurnOnPong();
                    break;
                case GameType.Sculpt:
                    TurnOnSculpt();
                    break;
                case GameType.Noise:
                    TurnOnNoise();
                    break;
            }
        }
    }

    void TurnOnSculpt()
    {
        TurnOnOffSpotsLinesSolutionsTilesBases(false);
        TurnOnOffGroundBodyBaseSculpt(true);
    }

    void TurnOnNoise()
    {
        TurnOnOffSpotsLinesSolutionsTilesBases(false);
        TurnOnOffGroundBodyBaseNoise(true);
    }

    void TurnOnConnect()
    {
        TurnOnOffSpotsLinesSolutionsTilesBases(false);
        TurnOnOffGroundBodyBaseConnect(true);
    }

    void TurnOnScrew()
    {
        TurnOnOffSpotsLinesSolutionsTilesBases(false);
        TurnOnOffGroundBodyBaseScrew(true);
    }

    void TurnOnTicTacToe()
    {
        TurnOnOffSpotsLinesSolutionsTilesBases(true);
        TurnOnOffGroundBodyBaseTicTacToe(true);
    }

    void TurnOnChess()
    {
        TurnOnOffSpotsLinesSolutionsTilesBases(false);
        TurnOnOffGroundBodyBaseChess(true);
    }

    void TurnOnOffSpotsLinesSolutionsTilesBases(bool yn)
    {
        TurnOnOffSolutions(yn);
        TurnOnOffSpots(yn);
        TurnOnOffLines(yn);
        TurnOnOffGroundBodyTiles(yn);
        TurnOnOffGroundBodyBaseTicTacToe(yn);
        TurnOnOffGroundBodyBaseChess(yn);
        TurnOnOffGroundBodyBaseScrew(yn);
        TurnOnOffGroundBodyBasePong(yn);
        TurnOnOffGroundBodyBaseConnect(yn);
        TurnOnOffGroundBodyBaseSculpt(yn);
        TurnOnOffGroundBodyBaseNoise(yn);
    }

    void TurnOnPong()
    {
        TurnOnOffSpotsLinesSolutionsTilesBases(false);
        TurnOnOffGroundBodyBasePong(true);
    }

    void TurnOnOffGroundBodyBaseTicTacToe(bool yn)
    {
        GameObject go = GameObject.Find("GroundBodyBaseTicTacToe");
        TurnOnOffGoRenderers(go, yn);
    }

    void TurnOnOffGroundBodyBaseChess(bool yn)
    {
        GameObject go = GameObject.Find("GroundBodyBaseChess");
        TurnOnOffGoRenderers(go, yn);
    }

    void TurnOnOffGroundBodyBaseScrew(bool yn)
    {
        GameObject go = GameObject.Find("GroundBodyBaseScrew");
        TurnOnOffGoRenderers(go, yn);
    }

    void TurnOnOffGroundBodyBasePong(bool yn)
    {
        GameObject go = GameObject.Find("GroundBodyBasePong");
        TurnOnOffGoRenderers(go, yn);
    }

    void TurnOnOffGroundBodyBaseConnect(bool yn)
    {
        GameObject go = GameObject.Find("GroundBodyBaseConnect");
        TurnOnOffGoRenderers(go, yn);
    }

    void TurnOnOffGroundBodyBaseSculpt(bool yn)
    {
        GameObject go = GameObject.Find("GroundBodyBaseSculpt");
        TurnOnOffGoRenderers(go, yn);
    }

    void TurnOnOffGroundBodyBaseNoise(bool yn)
    {
        GameObject go = GameObject.Find("GroundBodyBaseNoise");
        TurnOnOffGoRenderers(go, yn);
    }

    void TurnOnOffGroundBodyTiles(bool yn)
    {
        GameObject go = GameObject.Find("GroundBodyTiles");
        TurnOnOffGoRenderers(go, yn);
    }

    void TurnOnOffSolutions(bool yn)
    {
        GameObject go = GameObject.Find("Solutions");
        TurnOnOffGoRenderers(go, yn);
    }

    void TurnOnOffSpots(bool yn)
    {
        GameObject go = GameObject.Find("Spots");
        TurnOnOffGoRenderers(go, yn);
    }

    void TurnOnOffLines(bool yn)
    {
        GameObject go = GameObject.Find("Lines");
        TurnOnOffGoRenderers(go, yn);
    }

    void TurnOnOffGoRenderers(GameObject go, bool yn)
    {
        Renderer[] rends = go.GetComponentsInChildren<Renderer>();
        foreach(Renderer rend in rends)
        {
            rend.enabled = yn;
        }
    }
}
