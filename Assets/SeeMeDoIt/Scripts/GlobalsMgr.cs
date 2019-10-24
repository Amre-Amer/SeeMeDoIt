using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Collections.Generic;

public class GlobalsMgr : MonoBehaviour
{
    public BoundsMgr boundsMgr;
    public UdpMgr udpMgr;
    public TouchMouseMgr touchMouseMgr;
    public AssetMgr assetMgr;
    public PlaneFinderMgr planeFinderMgr;
    public MaterialMgr materialMgr;
    public SenderReceiverCamMgr senderReceiverCamMgr; 
    public HighlightMgr highlightMgr;
    public SmoothMgr smoothMgr;
    public PointerMgr pointerMgr;
    public AvoidMgr avoidMgr;
    public HitPlaceMgr hitPlaceMgr;
    public TicTacToeMgr ticTacToeMgr;
    public ConnectMgr connectMgr;
    public ScrewMgr screwMgr;
    public ChessMgr chessMgr;
    public PongMgr pongMgr;
    public ProgressMgr progressMgr;
    public AutoMgr autoMgr;
    public SculptMgr sculptMgr;
    public NoiseMgr noiseMgr;
    public ModeMgr modeMgr;
    public AssetBundlesMgr assetBundlesMgr;
    public ShakeMgr shakeMgr;
    //
    public GameObject goAssets;
    public GameObject goAssetsHolder;
    public GameObject goAsset;
    public GameObject goBoundingBox;
    public GameObject goDownloadList;
    public GameObject goBundleDownloadList;
    public GameObject goList;
    public GameObject goBundleList;
    public GameObject goWorkArea;
    public GameObject goCloudContent;
    public GameObject goMarkerFrom;
    public GameObject goMarkerTo;
    public GameObject goProgressIndicator;
    public List<GameObject> goProgressLists;
    public GameObject goPointerOrig;
    public GameObject goPointer;
    public GameObject goListItem;
    public GameObject goBundleListItem;
    public GameObject goCubeList;
    public GameObject goBundleCubeList;
    public GameObject goGround;
    public GameObject goGroundBase;
    public GameObject goDownloadPad;
    public GameObject goDownloads;
    public GameObject goShadowPlane;
    public GameObject goReceiverCam;
    public GameObject goReceiverCamOrig;
    public GameObject goSenderCam;
    public GameObject goSenderCamOrig;
    public GameObject goHeightUp;
    public GameObject goHeightDown;
    public GameObject goWorkAreaBody;
    public GameObject goWorkAreaComponents;
    public List<GameObject> goAdvances;
    public List<GameObject> goRetreats;
    public List<GameObject> goClouds;
    public List<GameObject> goAdvanceLights;
    public List<GameObject> goRetreatLights;
    public List<GameObject> goCloudLights;
    public GameObject goPlaneFinder;
    public GameObject goProtractor;
    //
    public bool ynShowBoundingBox;
    public bool ynFirstTime;
    public bool ynList;
    public bool ynBundleList;
    public bool ynAssemble;
    public bool ynInfo;
    public bool ynDetect;
    public bool ynRotateGround;
    public bool ynAuto;
    public bool ynPause;
    //
    public Vector3 eulRotate;
    //
    public float yaw;
    public float yStartAnimate;
    public float yDeltaAnimate;
    public float sizePointer;
    public float maxRayDistance;
    public float deltaHeight;
    public float timeLastTouch;
    public float timeLastTouchAuto;
    public float timeLastTouchAutoAdvance;
    public float delayAutoOrig;
    public float delayAutoFirst;
    public float delayAuto;
    public float delayAutoAdvance;
    public float deltaMove = .01f;
    public float deltaRotate = .25f;
    //
    public Bounds boundingBox;
    //
    public Text textLog;
    public Text textFps;
    public Text textPlace;
    public Text textLogo;
    //
    public int cntFrames;
    public int cntFps;
    public int numTotalProgress;
    public int numCompletedProgressX;
    public int numCompletedProgressO;
    public int numCompletedProgress;
    public int nAssetsHolder;
    public int textLogMax;
    int touchCount;
    //
    public string f8;
    //
    public InputField inputFieldUrl;
    //
    public Camera camMain;
    public Camera camThumb;
    //
    public Image imagePlane;
    public Image imageInfo;
    //
    public LayerMask layerTable = 1 << 11;  
    public LayerMask layerContent = 1 << 12;  
    public LayerMask layerPlane = 1 << 13; 
    public LayerMask layerGroundPivot = 1 << 14;
    public LayerMask layerGround = 1 << 15;
    public LayerMask layerAdvance = 1 << 16;
    public LayerMask layerRetreat = 1 << 17;
    public LayerMask layerCloud = 1 << 18;
    //
    public RoleType role;
    public Button buttonInfo;
    public Button buttonLog;
    public Canvas canvas;
    public ModeType mode;
    //
    public GameType gameType;

    private void Awake()
    {
        boundsMgr = GetComponent<BoundsMgr>();
        udpMgr = GetComponent<UdpMgr>();
        touchMouseMgr = GetComponent<TouchMouseMgr>();
        assetMgr = GetComponent<AssetMgr>();
        planeFinderMgr = GetComponent<PlaneFinderMgr>();
        materialMgr = GetComponent<MaterialMgr>();
        senderReceiverCamMgr = GetComponent<SenderReceiverCamMgr>();
        highlightMgr = GetComponent<HighlightMgr>();
        smoothMgr = GetComponent<SmoothMgr>();
        pointerMgr = GetComponent<PointerMgr>();
        avoidMgr = GetComponent<AvoidMgr>();
        hitPlaceMgr = GetComponent<HitPlaceMgr>();
        ticTacToeMgr = GetComponent<TicTacToeMgr>();
        connectMgr = GetComponent<ConnectMgr>();
        screwMgr = GetComponent<ScrewMgr>();
        chessMgr = GetComponent<ChessMgr>();
        pongMgr = GetComponent<PongMgr>();
        progressMgr = GetComponent<ProgressMgr>();
        autoMgr = GetComponent<AutoMgr>();
        sculptMgr = GetComponent<SculptMgr>();
        noiseMgr = GetComponent<NoiseMgr>();
        modeMgr = GetComponent<ModeMgr>();
        assetBundlesMgr = GetComponent<AssetBundlesMgr>();
        shakeMgr = GetComponent<ShakeMgr>();
        //
        goAssetsHolder = GameObject.Find("AssetsHolder");

        goDownloadList = GameObject.Find("DownloadList");
        goBundleDownloadList = GameObject.Find("BundleDownloadList");
        goList = GameObject.Find("List");
        goBundleList = GameObject.Find("BundleList");
        textFps = GameObject.Find("TextFps").GetComponent<Text>();
        imagePlane = GameObject.Find("ImagePlane").GetComponent<Image>();

        textLog = GameObject.Find("TextLog").GetComponent<Text>();
        goProgressIndicator = GameObject.Find("ProgressIndicator");
        goProgressLists = new List<GameObject>();
        LoadProgressLists();
        goPointerOrig = GameObject.Find("PointerOrig");
        goPointer = GameObject.Find("Pointer");
        camMain = GameObject.Find("ARCamera").GetComponent<Camera>();
        goGround = GameObject.Find("Ground");
        goGroundBase = GameObject.Find("GroundBase");
        goBoundingBox = GameObject.Find("BoundingBox");
        goAdvances = new List<GameObject>();
        goRetreats = new List<GameObject>();
        goClouds = new List<GameObject>();
        LoadAdvances();
        LoadRetreats();
        LoadClouds();
        goAdvanceLights = new List<GameObject>();
        goRetreatLights = new List<GameObject>();
        goCloudLights = new List<GameObject>();
        LoadAdvanceLights();
        LoadRetreatLights();
        LoadCloudLights();
        buttonInfo = GameObject.Find("ButtonInfo").GetComponent<Button>();
        buttonLog = GameObject.Find("ButtonLog").GetComponent<Button>();
        buttonInfo.onClick.AddListener(ButtonInfoClicked);
        buttonLog.onClick.AddListener(ButtonLogClicked);
        imageInfo = GameObject.Find("ImageInfo").GetComponent<Image>();
        textPlace = GameObject.Find("TextPlace").GetComponent<Text>();
        goReceiverCamOrig = GameObject.Find("ReceiverCamOrig");
        goReceiverCam = GameObject.Find("ReceiverCam");
        goSenderCamOrig = GameObject.Find("SenderCamOrig");
        goSenderCam = GameObject.Find("SenderCam");
        goPlaneFinder = GameObject.Find("Plane Finder");
        goProtractor = GameObject.Find("Protractor");
        //
        textLogo = GameObject.Find("TextLogo").GetComponent<Text>();
        udpMgr.port = 26000;
        role = RoleType.sender;
        textLogMax = 1500;
        Application.targetFrameRate = 60;
        f8 = "F6";
    }

    private void Start()
    {
        DebugLog(System.DateTime.Now.ToString());
        DebugLog("------------------------------------------");
        textLogo.text = "SeeMeDoIt " + Application.version + " me@amre-amer.com";
        //
        DebugLog(textLogo.text);
        DebugLog("Role " + role);
        //
        deltaHeight = .1f;
        sizePointer = .01f;
        maxRayDistance = 30.0f;
        yStartAnimate = 1.5f;
        yDeltaAnimate = .5f;
        delayAutoOrig = 4;
        delayAutoFirst = 12;
        delayAuto = delayAutoFirst;
        delayAutoAdvance = 20;
        //
        InitMarkers();
        InvokeRepeating("UpdateFps", 1, 1);
        //
        imageInfo.gameObject.SetActive(false);
        buttonLog.gameObject.SetActive(false);
        textLog.gameObject.SetActive(true);  // false
    }

    private void Update()
    {
        touchCount = touchMouseMgr.GetTouchMouseCount();
        UpdateKeyPress();
        UpdateTextFpsActive();
        cntFrames++;
        cntFps++;
    }

    void UpdateTextFpsActive()
    {
        bool yn = true;
        if (touchCount > 0)
        {
            yn = false;
        }
        textFps.gameObject.SetActive(yn);
    }

    void UpdateKeyPress()
    {
        if (Input.GetKey(KeyCode.A) == true && Input.GetKey(KeyCode.LeftShift) == true)
        {
            camMain.transform.Rotate(0, -deltaRotate, 0);
            autoMgr.ResetTimeLastTouch();
            return;
        }
        if (Input.GetKey(KeyCode.F) == true && Input.GetKey(KeyCode.LeftShift) == true)
        {
            camMain.transform.Rotate(0, deltaRotate, 0);
            autoMgr.ResetTimeLastTouch();
            return;
        }
        if (Input.GetKey(KeyCode.W) == true && Input.GetKey(KeyCode.LeftShift) == true)
        {
            camMain.transform.Rotate(-deltaRotate, 0, 0);
            autoMgr.ResetTimeLastTouch();
            return;
        }
        if (Input.GetKey(KeyCode.X) == true && Input.GetKey(KeyCode.LeftShift) == true)
        {
            camMain.transform.Rotate(deltaRotate, 0, 0);
            autoMgr.ResetTimeLastTouch();
            return;
        }
        if (Input.GetKey(KeyCode.A) == true)
        {
            camMain.transform.position += camMain.transform.right * deltaMove * -1;
            autoMgr.ResetTimeLastTouch();
            return;
        }
        if (Input.GetKey(KeyCode.F) == true)
        {
            camMain.transform.position += camMain.transform.right * deltaMove;
            autoMgr.ResetTimeLastTouch();
            return;
        }
        if (Input.GetKey(KeyCode.D) == true)
        {
            camMain.transform.position += camMain.transform.up * deltaMove;
            autoMgr.ResetTimeLastTouch();
            return;
        }
        if (Input.GetKey(KeyCode.S) == true)
        {
            camMain.transform.position += camMain.transform.up * deltaMove * -1;
            autoMgr.ResetTimeLastTouch();
            return;
        }
        if (Input.GetKey(KeyCode.W) == true)
        {
            camMain.transform.position += camMain.transform.forward * deltaMove;
            autoMgr.ResetTimeLastTouch();
            return;
        }
        if (Input.GetKey(KeyCode.X) == true)
        {
            camMain.transform.position += camMain.transform.forward * deltaMove * -1;
            autoMgr.ResetTimeLastTouch();
            return;
        }
    }

    void LoadProgressLists()
    {
        Transform[] ts = GameObject.Find("Progresses").GetComponentsInChildren<Transform>();
        foreach (Transform t in ts)
        {
            if (t.name == "ProgressListX" || t.name == "ProgressListO")
            {
                goProgressLists.Add(t.gameObject);
            }
        }
    }

    void LoadAdvances() {
        GameObject go = GameObject.Find("Advances");
        foreach(Transform t in go.transform)
        {
            goAdvances.Add(t.gameObject);
        }
    }

    void LoadAdvanceLights()
    {
        Transform[] ts = GameObject.Find("Advances").GetComponentsInChildren<Transform>();
        foreach (Transform t in ts)
        {
            if (t.name == "AdvanceLight")
            {
                goAdvanceLights.Add(t.gameObject);
            }
        }
    }

    void LoadRetreats()
    {
        GameObject go = GameObject.Find("Retreats");
        foreach (Transform t in go.transform)
        {
            goRetreats.Add(t.gameObject);
        }
    }

    void LoadClouds()
    {
        GameObject go = GameObject.Find("Clouds");
        foreach (Transform t in go.transform)
        {
            goClouds.Add(t.gameObject);
        }
    }

    void LoadCloudLights()
    {
        Transform[] ts = GameObject.Find("Clouds").GetComponentsInChildren<Transform>();
        foreach (Transform t in ts)
        {
            if (t.name == "CloudLight")
            {
                goCloudLights.Add(t.gameObject);
            }
        }
    }

    void LoadRetreatLights()
    {
        Transform[] ts = GameObject.Find("Retreats").GetComponentsInChildren<Transform>();
        foreach (Transform t in ts)
        {
            if (t.name == "RetreatLight")
            {
                goRetreatLights.Add(t.gameObject);
            }
        }
    }

    void ButtonLogClicked()
    {
        Debug.Log("ButtonLogClicked");
        textLog.gameObject.SetActive(!textLog.isActiveAndEnabled);
    }

    void ButtonInfoClicked()
    {
        Debug.Log("ButtonInfoClicked");
        ynInfo = !ynInfo;
        imageInfo.gameObject.SetActive(ynInfo);
        buttonLog.gameObject.SetActive(ynInfo);
    }

    int GetSecondsSince1970()
    {
        System.DateTime epochStart = new System.DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc);
        int cur_time = (int)(System.DateTime.UtcNow - epochStart).TotalSeconds;
        return cur_time;
    }

    public void UpdateFps()
    {
        string txt = "";
        if (udpMgr.ynConnected == true)
        {
            txt += role + " fps " + cntFps;
            txt += "\nbytes " + udpMgr.bytesSent + "/" + udpMgr.bytesReceived + " max [" + udpMgr.maxPayload + "]";
            string txtViewer = RoleType.sender.ToString();
            if (role == RoleType.sender)
            {
                txtViewer = RoleType.receiver.ToString();
            }
            txt += "\nip " + udpMgr.ipLocal + " " + txtViewer + " " + udpMgr.ipFrom;
        }
        else {
            txt += "fps " + cntFps;
            txt += "\nip " + udpMgr.ipLocal;
        }
        txt += "\n" + gameType;
        if (gameType == GameType.Sculpt && mode == ModeType.play)
        {
            txt += "\nTap to toggle (green),\nadd from screen center"; 
            txt += "\n(" + udpMgr.GetCountTxtConfirms() + ")";
        }
        textFps.text = txt;
        //
        cntFps = 0;
        udpMgr.bytesSent = 0;
        udpMgr.bytesReceived = 0;
        udpMgr.maxPayload = 0;
    }

    void InitMarkers()
    {
        goMarkerFrom = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        goMarkerFrom.name = "MarkerFrom";
        goMarkerFrom.transform.localScale = Vector3.one * .03f;
        goMarkerFrom.GetComponent<Renderer>().material.color = Color.red;
        SphereCollider sc = goMarkerFrom.GetComponent<SphereCollider>();
        sc.enabled = false;
        //
        goMarkerTo = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        goMarkerTo.name = "MarkerTo";
        goMarkerTo.transform.localScale = Vector3.one * .03f;
        goMarkerTo.GetComponent<Renderer>().material.color = Color.green;
        sc = goMarkerTo.GetComponent<SphereCollider>();
        sc.enabled = false;
    }

    public void DebugLog(string txt)
    {
        if (txt.Contains("startTimeApp") == true)
        {
            txt = ".";
        }
        if (textLog.enabled == true)
        {
            string txtAll = textLog.text += "\n" + txt;
            if (txtAll.Length > textLogMax)
            {
                txtAll = txtAll.Substring(txtAll.Length - textLogMax);
            }
            textLog.text = txtAll;
        }
        if (Application.isEditor == true)
        {
            SaveLog(txt);
        }
    }

    void SaveLog(string txt)
    {
        string path = Path.Combine(Application.dataPath, "log.txt");
        //Debug.Log("SaveLog " + path + "\n");
        File.AppendAllText(path, txt + "\n");
    }

    public bool IsNear(GameObject go, GameObject goCheck, float distNear)
    {
        float dist = Vector3.Distance(go.transform.position, goCheck.transform.position);
        if (dist < distNear)
        {
            return true;
        }
        return false;
    }

    public void ColorGo(GameObject go, Color color)
    {
        Renderer[] rends = go.GetComponentsInChildren<Renderer>();
        foreach (Renderer rend in rends)
        {
            rend.material.color = color;
        }
    }

    public void RenameChild(GameObject go, string txtName, string txtNameNew)
    {
        Transform[] ts = go.GetComponentsInChildren<Transform>();
        foreach (Transform t in ts)
        {
            if (t.name == txtName)
            {
                t.name = txtNameNew;
                break;
            }
        }
    }

    public void ColorAlphaImage(Image image, float alpha)
    {
        Color color = ColorAlpha(image.color, alpha);
        image.color = color;
    }

    public void ColorAlphaGo(GameObject go, float alpha)
    {
        Renderer[] rends = go.GetComponentsInChildren<Renderer>();
        foreach (Renderer rend in rends)
        {
            Color color = ColorAlpha(rend.material.color, alpha);
            rend.material.color = color;
        }
    }

    public Color ColorAlpha(Color color, float alpha)
    {
        color.a = alpha;
        return color;
    }
}

public enum RoleType
{
    sender,
    receiver
}

public enum ModeType
{
    place,
    play
}

public enum SolutionItemType
{
    X,
    O,
    empty
}

public enum GameType
{
    TicTacToe,
    Connect,
    Screw,
    Chess,
    Pong,
    Sculpt,
    Noise,
    Towers,
    Art,
    Learn
}
