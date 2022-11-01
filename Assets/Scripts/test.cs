using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Threading.Tasks;

public class test : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI modeltxt;
    [SerializeField]
    TextMeshProUGUI SIMtxt;

    //firebase
    public GUISkin fb_GUISkin;
    private Vector2 controlsScrollViewVector = Vector2.zero;
    private Vector2 scrollViewVector = Vector2.zero;
    bool UIEnabled = true;
    private string logText = "";
    const int kMaxLogSize = 16382;
    Firebase.DependencyStatus dependencyStatus = Firebase.DependencyStatus.UnavailableOther;
    protected bool isFirebaseInitialized = false;

    void Start()
    {
        Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task => {
        dependencyStatus = task.Result;
        if (dependencyStatus == Firebase.DependencyStatus.Available) {
          InitializeFirebase();
        } else {
          Debug.LogError(
            "Could not resolve all Firebase dependencies: " + dependencyStatus);
        }
      });

      FetchDataAsync();
    }

    private void InitializeFirebase(){
        Debug.Log("Remote config ready");
    }

    private void DisplayData(){
        Debug.Log("Current Data:");
      Debug.Log("path: " +
               Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance
               .GetValue("path").StringValue);
    }

    public Task FetchDataAsync() {
      Debug.Log("Fetching data...");
      System.Threading.Tasks.Task fetchTask =
      Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.FetchAsync(
          System.TimeSpan.Zero);
      return fetchTask.ContinueWith(FetchComplete);
    }

    void FetchComplete(Task fetchTask) {
      if (fetchTask.IsCanceled) {
        Debug.Log("Fetch canceled.");
      } else if (fetchTask.IsFaulted) {
        Debug.Log("Fetch encountered an error.");
      } else if (fetchTask.IsCompleted) {
        Debug.Log("Fetch completed successfully!");
      }

      var info = Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.Info;
      switch (info.LastFetchStatus) {
        case Firebase.RemoteConfig.LastFetchStatus.Success:
          Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.ActivateAsync()
          .ContinueWith(task => {
            Debug.Log(System.String.Format("Remote data loaded and ready (last fetch time {0}).",
                                 info.FetchTime));
          });

          break;
        case Firebase.RemoteConfig.LastFetchStatus.Failure:
          switch (info.LastFetchFailureReason) {
            case Firebase.RemoteConfig.FetchFailureReason.Error:
              Debug.Log("Fetch failed for unknown reason");
              break;
            case Firebase.RemoteConfig.FetchFailureReason.Throttled:
              Debug.Log("Fetch throttled until " + info.ThrottledEndTime);
              break;
          }
          break;
        case Firebase.RemoteConfig.LastFetchStatus.Pending:
          Debug.Log("Latest Fetch call still pending.");
          break;
      }
    }

    // Update is called once per frame
    void Update()
    {
        modeltxt.text = GetModel();
        SIMtxt.text = GetSIM();

        //DisplayData();
    }

    private string GetModel(){
        string model;
        model = SystemInfo.deviceModel;

        return model;
    }

    private string GetSIM(){
        AndroidJavaObject TM = new AndroidJavaObject("android.telephony.TelephonyManager");
        string reg = TM.Call<string>("getSimCountryIso");
        //ReturnSIMSerialNumber

        return reg;
    }

    

}
