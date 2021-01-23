using System.Collections;
using System.Collections.Generic;
using System;
using System.Text;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using GooglePlayGames.BasicApi.SavedGame;
using UnityEngine;

#if (UNITY_ANDROID || (UNITY_IPHONE && !NO_GPGS))
public static class GPGSManager
{
    private static string                         handleData       = string.Empty;
    private static Action                         onLogined        = null;
    private static Action<string>                 onLoginFailed    = null;
    private static Action                         onSaveSuccessed  = null;
    private static Action<string>                 onLoadSuccessed  = null;
    private static Action<SavedGameRequestStatus> onSaveLoadFailed = null;

    public static bool IsLogined { get { return Social.localUser.authenticated; } }

    public static string LocalUserName
    {
        get
        {
            if (IsLogined)
                return Social.localUser.userName;
            else
                return string.Empty;
        }
    }

    [RuntimeInitializeOnLoadMethod]
    private static void Initialize()
    {
        PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder().EnableSavedGames().Build();
        PlayGamesPlatform.InitializeInstance(config); 
        PlayGamesPlatform.DebugLogEnabled = false;
        PlayGamesPlatform.Activate();
    }

    public static void SaveToCloud(string data, Action onSaveSuccessed, Action<SavedGameRequestStatus> onEventFailed = null)
    {
        handleData = data;
        GPGSManager.onSaveSuccessed = onSaveSuccessed;
        onSaveLoadFailed = onEventFailed;

        PlayGamesPlatform.
            Instance.
            SavedGame.
            OpenWithAutomaticConflictResolution(
            Application.productName,
            DataSource.ReadCacheOrNetwork,
            ConflictResolutionStrategy.UseLongestPlaytime,
            OnFileOpenToSave);
    }

    public static void LoadFromCloud(Action<string> onLoadSuccessed, Action<SavedGameRequestStatus> onEventFailed = null)
    {
        GPGSManager.onLoadSuccessed = onLoadSuccessed;
        onSaveLoadFailed = onEventFailed;

        PlayGamesPlatform.
            Instance.
            SavedGame.
            OpenWithAutomaticConflictResolution(
            Application.productName,
            DataSource.ReadCacheOrNetwork,
            ConflictResolutionStrategy.UseLongestPlaytime,
            OnFileOpenToLoad);
    }

    private static void OnFileOpenToSave(SavedGameRequestStatus status, ISavedGameMetadata metaData)
    {
        switch (status)
        {
            case SavedGameRequestStatus.Success:
                var savedGame = PlayGamesPlatform.Instance.SavedGame;

                byte[] handleDataAsBytes = Encoding.UTF8.GetBytes(handleData);
                SavedGameMetadataUpdate.Builder bulider = new SavedGameMetadataUpdate.Builder().
                                                              WithUpdatedPlayedTime(DateTime.Now.TimeOfDay);
                SavedGameMetadataUpdate updatedMetadata = bulider.Build();
                savedGame.CommitUpdate(metaData, updatedMetadata, handleDataAsBytes, OnSaveFinished);
                break;

            default:
                onSaveLoadFailed?.Invoke(status);
                break;
        }
    }

    private static void OnFileOpenToLoad(SavedGameRequestStatus status, ISavedGameMetadata metaData)
    {
        switch (status)
        {
            case SavedGameRequestStatus.Success:
                PlayGamesPlatform.
                    Instance.
                    SavedGame.
                    ReadBinaryData(metaData, OnLoadFinished);
                break;

            default:
                onSaveLoadFailed?.Invoke(status);
                break;
        }
    }

    private static void OnSaveFinished(SavedGameRequestStatus status, ISavedGameMetadata mataData)
    {
        switch (status)
        {
            case SavedGameRequestStatus.Success:
                onSaveSuccessed.Invoke();
                break;

            default:
                onSaveLoadFailed?.Invoke(status);
                break;
        }

        handleData = null;
        onSaveSuccessed = null;
        onSaveLoadFailed = null;
    }

    private static void OnLoadFinished(SavedGameRequestStatus status, byte[] data)
    {
        switch (status)
        {
            case SavedGameRequestStatus.Success:
                if (data.Length == 0)
                    onLoadSuccessed.Invoke(string.Empty);
                else
                    onLoadSuccessed.Invoke(Encoding.UTF8.GetString(data));
                break;

            default:
                onSaveLoadFailed?.Invoke(status);
                break;
        }

        onLoadSuccessed  = null;
        onSaveLoadFailed = null;
    }

    private static void OnLogined(bool result, string log)
    {
        if (result)
            onLogined();
        else
            onLoginFailed?.Invoke(log);
    }

    public static void Login(Action onLoginSuccess, Action<string> onLoginFailed = null)
    {
        if (IsLogined)
            return;

        Social.localUser.Authenticate(OnLogined);
    }

    public static void Logout()
    {
        if (!IsLogined)
            return;

        PlayGamesPlatform.Instance.SignOut();
    }
}
#endif