using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Text;
using System;

[System.Serializable]
public class UserData
{
    public string userId;
    public string tournamentId;
    public string roundId;
    public bool isTrial;
    public int baseDifficulty;
}

public class WebGLBridge : MonoBehaviour
{
    public bool isTrial;
    public static WebGLBridge Instance;

    [Header("Game Mode UI Objects")]
    public GameObject trialGameObject;      // ✅ Assign UI/GameObject for Trial Mode
    public GameObject nonTrialGameObject;   // ✅ Assign UI/GameObject for Non-Trial Mode

    private string baseUrl = "https://maidaan-api-server-44cf74tcjq-el.a.run.app/api/v1/webgl-game";
    private UserData userData = new UserData();

    public int baseDifficulty;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ReceiveDataFromReact(string jsonData)
    {
        Debug.Log("📥 Received from React: " + jsonData);
        try
        {
            userData = JsonUtility.FromJson<UserData>(jsonData);
            Debug.Log($"✅ Stored User Data -> User ID: {userData.userId}, Tournament: {userData.tournamentId}, Round: {userData.roundId}, IsTrial: {userData.isTrial} , BaseDifficulty: {userData.baseDifficulty}");
            baseDifficulty = userData.baseDifficulty;
            isTrial = userData.isTrial;

            // ✅ Toggle trial and non-trial objects
            if (trialGameObject != null)
                trialGameObject.SetActive(userData.isTrial);

            if (nonTrialGameObject != null)
                nonTrialGameObject.SetActive(!userData.isTrial);

            if (!userData.isTrial)
            {
                StartCoroutine(StartGameWithDelay());
                Debug.Log("🚀 Starting main game since it's NOT a trial.");
            }
        }
        catch (Exception e)
        {
            Debug.LogError("❌ JSON Parse Error: " + e.Message);
        }
    }

    IEnumerator StartGameWithDelay()
    {
        yield return new WaitForSeconds(0.2f);
        // Simulate game start here if needed
    }

    public void StartGame()
    {
        string startTime = DateTime.UtcNow.ToString("o");

        string json = $"{{" +
            $"\"userId\": \"{userData.userId}\", " +
            $"\"tournamentId\": \"{userData.tournamentId}\", " +
            $"\"roundId\": \"{userData.roundId}\", " +
            $"\"startTime\": \"{startTime}\", " +
            $"\"isTrial\": {userData.isTrial.ToString().ToLower()}" +
            $"}}";

        StartCoroutine(SendGameData("start-time", json));
    }

    public void UpdateScore(int score, string jsonData)
    {
        string json = $"{{" +
            $"\"userId\": \"{userData.userId}\", " +
            $"\"tournamentId\": \"{userData.tournamentId}\", " +
            $"\"roundId\": \"{userData.roundId}\", " +
            $"\"score\": {score}, " +
            $"\"attemptedWord\": {jsonData}" +
            $"}}";

        if (!userData.isTrial)
        {
            StartCoroutine(SendGameData("update-score", json));
        }
    }

    public void EndGame()
    {
        string endpoint = userData.isTrial ? "end-trial" : "end-game";

        string json = $"{{" +
            $"\"userId\": \"{userData.userId}\", " +
            $"\"tournamentId\": \"{userData.tournamentId}\", " +
            $"\"roundId\": \"{userData.roundId}\", " +
            $"\"TrialEnded\": {userData.isTrial.ToString().ToLower()}, " +
            $"\"GameEnded\": {(!userData.isTrial).ToString().ToLower()}" +
            $"}}";

        StartCoroutine(SendGameData(endpoint, json));
    }

    private IEnumerator SendGameData(string action, string json)
    {
        string url = $"{baseUrl}/{action}";
        Debug.Log($"📡 Sending [{action.ToUpper()}] request to: {url}");
        Debug.Log($"📜 Payload: {json}");

        using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
        {
            byte[] jsonBytes = Encoding.UTF8.GetBytes(json);
            request.uploadHandler = new UploadHandlerRaw(jsonBytes);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log($"✅ [{action.ToUpper()}] API Success: {request.downloadHandler.text}");
            }
            else
            {
                Debug.LogError($"❌ [{action.ToUpper()}] API Error: {request.error}");
            }
        }
    }
}
