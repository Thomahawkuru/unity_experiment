
using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using Varjo;
using System.Collections;
using RosSharp.RosBridgeClient;

public class Logger : MonoBehaviour
{
    StreamWriter GAZEwriter = null;
    StreamWriter HMDwriter = null;
    StreamWriter PANDAwriter = null;
    StreamWriter HANDwriter = null;
    StreamWriter EXPERIMENTwriter = null;

    List<VarjoPlugin.GazeData> dataSinceLastUpdate;

    private string subjectDataFolder;
    private string Scene;

    Vector3 hmdPosition;
    Vector3 hmdRotation;

    [Header("Should only the latest data be logged on each update")]
    public bool oneGazeDataPerFrame = true;

    [Header("Start logging after calibration")]
    public bool startAutomatically = true;

    [Header("Press to start or end logging")]
    public KeyCode toggleLoggingKey = KeyCode.L;

    [Header("Hands for logging")]
    public Transform Lpalm;
    public Transform Rpalm;

    bool logging = false;
    static double time;

    static readonly string[] ColumnNamesGaze = { "Frame", "DeltaTime", "Time", "Status", "GazeX", "GazeY", "GazeZ", "GazePosition", "FocusDistance", "FocusStability", "StatusL", "LeftX", "LeftY", "LeftZ", "PositionL", "PupilL", "StatusR", "RightX", "RightY", "RightZ", "PositionR", "PupilR"};
    static readonly string[] ColumnNamesHMD = { "Frame", "DeltaTime", "Time", "HMDPositionX", "HMDPositionY", "HMDPositionZ", "HMDRotationX", "HMDRotationY", "HMDRotationZ" };
    static readonly string[] ColumnNamesPanda = { "Frame", "DeltaTime", "Time", "PositionX", "PositionY", "PositionZ", "Gripper" };
    static readonly string[] ColumnNamesHands = { "Frame", "DeltaTime", "Time", "LPositionX", "LPositionY", "LPositionZ", "LGrab", "LPinch", "RPositionX", "RPositionY", "RPositionZ", "RGrab", "RPinch" };
    static readonly string[] ColumnNamesExperiment = { "Frame", "DeltaTime", "Time", "LevelNumber", "TrailNumber", "MolePosition", "WhackCheck" };
            
    const string ValidString = "VALID";
    const string InvalidString = "INVALID";

    private TransformSubscriber transformsubscriber;
    private HandReader handreader;
    private TrailNumberSubscriber trailNumberSubscriber;
    private LevelNumberSubscriber levelNumberSubscriber;
    private MolePositionSubscriber molePositionSubscriber;
    private WhackCheckSubscriber whackCheckSubscriber;

    private IEnumerator Start()
    {
        yield return new WaitUntil(() => ExperimentManager.IsStarted);

        if (!VarjoPlugin.InitGaze())
        {
            Debug.LogError("Failed to initialize gaze");
        }

        transformsubscriber = gameObject.AddComponent<TransformSubscriber>();
        handreader = gameObject.AddComponent<HandReader>();
        trailNumberSubscriber = gameObject.AddComponent<TrailNumberSubscriber>();
        levelNumberSubscriber = gameObject.AddComponent<LevelNumberSubscriber>();
        molePositionSubscriber = gameObject.AddComponent<MolePositionSubscriber>();
        whackCheckSubscriber = gameObject.AddComponent<WhackCheckSubscriber>();

        subjectDataFolder = ExperimentManager.subjectDataFolder;
        Debug.Log("Read subject data folder = " + subjectDataFolder);
        Scene = ExperimentManager.sceneArray[ExperimentManager.sceneIndex];
        Debug.Log("Current scene = " + Scene);  
    }

    void Update()
    {
        time = time + Time.deltaTime;

        // Do not run update if the application is not visible
        if (!VarjoManager.Instance.IsLayerVisible() || VarjoManager.Instance.IsInStandBy())
        {
            return;
        }

        if (Input.GetKeyDown(toggleLoggingKey))
        {
            if (!logging)
            {
                StartLogging();
            }
            else
            {
                StopLogging(GAZEwriter);
                StopLogging(HMDwriter);
                StopLogging(PANDAwriter);
                StopLogging(HANDwriter);
                StopLogging(EXPERIMENTwriter);
            }
            return;
        }

        if (logging)
        {
            if (oneGazeDataPerFrame)
            {
                // Get and log latest gaze data
                LogGazeData(VarjoPlugin.GetGaze());
                LogHMDData(VarjoPlugin.GetGaze());
                LogPandaData(VarjoPlugin.GetGaze());
                LogHandData(VarjoPlugin.GetGaze());
                LogExperimentData(VarjoPlugin.GetGaze());
            }
            else
            {
                // Get and log all gaze data since last update
                dataSinceLastUpdate = VarjoPlugin.GetGazeList();
                foreach (var data in dataSinceLastUpdate)
                {
                    LogGazeData(data);
                    LogHMDData(data);
                    LogPandaData(data);
                    LogHandData(data);
                    LogExperimentData(data);
                }
            }
        }
        else if (startAutomatically)
        {
            if (VarjoPlugin.GetGaze().status == VarjoPlugin.GazeStatus.VALID)
            {
                StartLogging();
                Debug.Log("automatic start");
            }
        }
    }

    void LogGazeData(VarjoPlugin.GazeData data)
    {
        string[] logData = new string[22];

        // Gaze data frame number
        logData[0] = data.frameNumber.ToString();

        // Gaze data capture time (nanoseconds)
        logData[1] = Time.deltaTime.ToString(); //data.DeltaTime.ToString();

        // Log time (milliseconds)
        logData[2] = time.ToString(); //DateTime.Now.Ticks.ToString();

        // Combined gaze
        bool invalid = data.status == VarjoPlugin.GazeStatus.INVALID;
        logData[3] = invalid ? InvalidString : ValidString;
        logData[4] = invalid ? "" : data.gaze.forward[0].ToString();
        logData[5] = invalid ? "" : data.gaze.forward[1].ToString();
        logData[6] = invalid ? "" : data.gaze.forward[2].ToString();
        logData[7] = invalid ? "" : data.gaze.position[0].ToString();

        // Focus
        logData[8] = invalid ? "" : data.focusDistance.ToString();
        logData[9] = invalid ? "" : data.focusStability.ToString();

        // Left eye
        bool leftInvalid = data.leftStatus == VarjoPlugin.GazeEyeStatus.EYE_INVALID;
        logData[10] = leftInvalid ? InvalidString : ValidString;
        logData[11] = leftInvalid ? "" : data.left.forward[0].ToString();
        logData[12] = leftInvalid ? "" : data.left.forward[1].ToString();
        logData[13] = leftInvalid ? "" : data.left.forward[2].ToString();
        logData[14] = leftInvalid ? "" : data.left.position[0].ToString();
        logData[15] = leftInvalid ? "" : data.leftPupilSize.ToString();

        // Right eye
        bool rightInvalid = data.rightStatus == VarjoPlugin.GazeEyeStatus.EYE_INVALID;
        logData[16] = rightInvalid ? InvalidString : ValidString;
        logData[17] = rightInvalid ? "" : data.right.forward[0].ToString();
        logData[18] = rightInvalid ? "" : data.right.forward[1].ToString();
        logData[19] = rightInvalid ? "" : data.right.forward[2].ToString();
        logData[20] = rightInvalid ? "" : data.right.position[0].ToString();
        logData[21] = rightInvalid ? "" : data.rightPupilSize.ToString();

        

        Log(logData,GAZEwriter);
    }

    void LogHMDData(VarjoPlugin.GazeData data)
    {
        // Get HMD position and rotation
        hmdPosition = VarjoManager.Instance.HeadTransform.position;
        hmdRotation = VarjoManager.Instance.HeadTransform.rotation.eulerAngles;

        string[] logData = new string[9];

        // Gaze data frame number
        logData[0] = data.frameNumber.ToString();

        // Gaze data capture time (nanoseconds)
        logData[1] = Time.deltaTime.ToString(); //data.DeltaTime.ToString();

        // Log time (milliseconds)
        logData[2] = time.ToString(); //DateTime.Now.Ticks.ToString();

        // HMD
        logData[3] = hmdPosition[0].ToString("F3");
        logData[4] = hmdPosition[1].ToString("F3");
        logData[5] = hmdPosition[2].ToString("F3");

        logData[6] = hmdRotation[0].ToString("F3");
        logData[7] = hmdRotation[1].ToString("F3");
        logData[8] = hmdRotation[2].ToString("F3");

        Log(logData, HMDwriter);

    }

    void LogPandaData(VarjoPlugin.GazeData data)
    {
        float[] Panda = transformsubscriber.Transform();
        string[] logData = new string[7];

        // Gaze data frame number
        logData[0] = data.frameNumber.ToString();

        // Gaze data capture time (nanoseconds)
        logData[1] = Time.deltaTime.ToString(); //data.DeltaTime.ToString();

        // Log time (milliseconds)
        logData[2] = time.ToString(); //DateTime.Now.Ticks.ToString();

        // Panda
        logData[3] = Panda[0].ToString("F3");
        logData[4] = Panda[1].ToString("F3");
        logData[5] = Panda[2].ToString("F3");

        logData[6] = 0.ToString("F3");

        Log(logData, PANDAwriter);
    }

    void LogHandData(VarjoPlugin.GazeData data)
    {
        string[] logData = new string[13];
        double[] HandData = handreader.Read();

        // Gaze data frame number
        logData[0] = data.frameNumber.ToString();

        // Gaze data capture time (nanoseconds)
        logData[1] = Time.deltaTime.ToString(); //data.DeltaTime.ToString();

        // Log time (milliseconds)
        logData[2] = time.ToString(); //DateTime.Now.Ticks.ToString();

        // Panda
        logData[3] = Lpalm.position[0].ToString("F3");
        logData[4] = Lpalm.position[1].ToString("F3");
        logData[5] = Lpalm.position[2].ToString("F3");
        logData[6] = HandData[0].ToString("F3");
        logData[7] = HandData[1].ToString("F3");

        logData[8] = Rpalm.position[0].ToString("F3");
        logData[9] = Rpalm.position[1].ToString("F3");
        logData[10] = Rpalm.position[2].ToString("F3");
        logData[11] = HandData[2].ToString("F3");
        logData[12] = HandData[3].ToString("F3");

        Log(logData, HANDwriter);
    }

    void LogExperimentData(VarjoPlugin.GazeData data)
    {
        string[] logData = new string[7];
        double mole_position = molePositionSubscriber.Mole();
        long trail_number = trailNumberSubscriber.Trail();
        long level_number = levelNumberSubscriber.Trail();
        bool whacked = whackCheckSubscriber.Whack();

        // Gaze data frame number
        logData[0] = data.frameNumber.ToString();

        // Gaze data capture time (nanoseconds)
        logData[1] = Time.deltaTime.ToString(); //data.DeltaTime.ToString();

        // Log time (milliseconds)
        logData[2] = time.ToString(); //DateTime.Now.Ticks.ToString();

        // Panda
        logData[3] = level_number.ToString("F3");
        logData[4] = trail_number.ToString("F3"); 
        logData[5] = mole_position.ToString("F3");
        logData[6] = whacked.ToString();
        
        Log(logData, EXPERIMENTwriter);
    }

    // Write given values in the log file
    void Log(string[] values, StreamWriter writer)
    {
        if (!logging || writer == null)
            return;

        string line = "";
        for (int i = 0; i < values.Length; ++i)
        {
            values[i] = values[i].Replace("\r", "").Replace("\n", ""); // Remove new lines so they don't break csv
            line += values[i] + (i == (values.Length - 1) ? "" : ","); // Do not add semicolon to last data string
        }
        writer.WriteLine(line);
    }

    public void StartLogging()
    {
        if (logging)
        {
            Debug.LogWarning("Logging was on when StartLogging was called. No new log was started.");
            return;
        }

        logging = true;

        string logPath = subjectDataFolder + "/" + Scene;
        Directory.CreateDirectory(logPath);
        Debug.Log("Scene log directory created");

        //DateTime now = DateTime.Now;
        //string fileName = string.Format("{0}-{1:00}-{2:00}-{3:00}-{4:00}", now.Year, now.Month, now.Day, now.Hour, now.Minute);
        //string path = logPath + "/" + fileName + ".csv";

        GAZEwriter = new StreamWriter(logPath + "/Gaze.csv");
        HMDwriter = new StreamWriter(logPath + "/HMD.csv");
        PANDAwriter = new StreamWriter(logPath + "/Panda.csv");
        HANDwriter = new StreamWriter(logPath + "/Hand.csv");
        EXPERIMENTwriter = new StreamWriter(logPath + "/Experiment.csv");

        Log(ColumnNamesGaze, GAZEwriter);
        Log(ColumnNamesHMD, HMDwriter);
        Log(ColumnNamesPanda, PANDAwriter);
        Log(ColumnNamesHands, HANDwriter);
        Log(ColumnNamesExperiment, EXPERIMENTwriter);

        Debug.Log("Logs started at: " + logPath);
    }

    void StopLogging(StreamWriter writer)
    {
        if (!logging)
            return;

        if (writer != null)
        {
            writer.Flush();
            writer.Close();
            writer = null;
        }
        logging = false;
        Debug.Log("Logging ended");
    }

    void OnApplicationQuit()
    {
        StopLogging(GAZEwriter);
        StopLogging(HMDwriter);
        StopLogging(PANDAwriter);
        StopLogging(HANDwriter);
        StopLogging(EXPERIMENTwriter);
    }

    public static string Double3ToString(double[] doubles)
    {
        return doubles[0].ToString() + ". " + doubles[1].ToString() + ". " + doubles[2].ToString();
    }
}
