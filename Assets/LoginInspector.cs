using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public class LoginInspector : MonoBehaviour
{
    Firebase.DependencyStatus dependencyStatus = Firebase.DependencyStatus.UnavailableOther;

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

    private string GetURLFB(){
        Debug.Log("Current Data:");
        Debug.Log("path: " +
               Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance
               .GetValue("path").StringValue);

        string url = Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.GetValue("path").StringValue;

        return url;
    }

    private string GetModel(){
        string model;
        model = SystemInfo.deviceModel;

        return model;
    }

    private bool GetSIM(){
        AndroidJavaObject TM = new AndroidJavaObject("android.telephony.TelephonyManager");
        string reg = TM.Call<string>("getSimCountryIso");

        if(reg == null)
            return false;
        else
            return true;
    }


    // private async void ones()  {
    //   try {
    //   await OneSignal.shared.promptUserForPushNotificationPermission();
    //   await OneSignal.shared.setAppId(“добавляем наш ключ, который скинем в
    //   текстовом файле”);
    //   } catch (e) {
    //   print(e);
    //   }
    // }
}
