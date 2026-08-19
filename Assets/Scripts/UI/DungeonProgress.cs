using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Drives the dungeon progression HUD: a room counter, a row of minimap dots
/// (one per room), and a banner announcing the room type/clear state.
/// Works entirely on PLACEHOLDER data right now via GenerateMockRun(), so it
/// can be built and tested before the real Procedural Generation system
/// exists. Once that system is ready, swap the mock call for the generator's
/// real room list and everything else keeps working unchanged.
/// </summary>
public class DungeonProgressUI : MonoBehaviour
{
    public enum RoomType { Combat, Reward, Rest, Corridor }

    [System.Serializable]
    public class RoomInfo
    {
        public RoomType type;
        public bool cleared;
    }

    [Header("UI References")]
    public TextMeshProUGUI roomCounterText;      // "Room 3 / 8"
    public TextMeshProUGUI bannerText;           // "Entering Combat Room" / "Room Cleared"
    public CanvasGroup bannerGroup;              // for fade in/out
    public Transform minimapDotContainer;        // horizontal layout group
    public GameObject minimapDotPrefab;          // small Image prefab

    [Header("Banner Timing")]
    public float bannerVisibleTime = 1.4f;
    public float bannerFadeTime = 0.3f;

    [Header("Dot Colors")]
    public Color dotUpcoming = new Color(0.6f, 0.6f, 0.6f, 0.5f);
    public Color dotCurrent = new Color(1f, 0.85f, 0.3f);
    public Color dotCleared = new Color(0.4f, 0.8f, 0.5f);

    [Header("Test Mode (remove once real Procedural Generation exists)")]
    public bool useMockData = true;
    public int mockRoomCount = 8;

    private List<RoomInfo> rooms = new List<RoomInfo>();
    private List<Image> dotImages = new List<Image>();
    private int currentRoomIndex = -1;

    void Start()
    {
        if (useMockData)
        {
            GenerateMockRun(mockRoomCount);
        }
    }

    void GenerateMockRun(int count)
    {
        List<RoomInfo> mockRooms = new List<RoomInfo>();
        for (int i = 0; i < count; i++)
        {
            RoomType type = (i == count - 1) ? RoomType.Reward
                            : (i % 3 == 0) ? RoomType.Rest
                            : RoomType.Combat;
            mockRooms.Add(new RoomInfo { type = type, cleared = false });
        }
        InitializeRun(mockRooms);
    }

    /// <summary>Call this with the real room list once Procedural Generation exists.</summary>
    public void InitializeRun(List<RoomInfo> roomList)
    {
        rooms = roomList;
        currentRoomIndex = -1;

        // Clear old dots
        foreach (Transform child in minimapDotContainer) Destroy(child.gameObject);
        dotImages.Clear();

        // Build one dot per room
        foreach (var room in rooms)
        {
            GameObject dot = Instantiate(minimapDotPrefab, minimapDotContainer);
            Image img = dot.GetComponent<Image>();
            if (img != null)
            {
                img.color = dotUpcoming;
                dotImages.Add(img);
            }
        }

        EnterRoom(0);
    }

    /// <summary>Call this whenever the player enters a new room (real or mock).</summary>
    public void EnterRoom(int index)
    {
        if (index < 0 || index >= rooms.Count) return;

        currentRoomIndex = index;
        UpdateCounterText();
        UpdateDots();
        ShowBanner($"Entering {rooms[index].type} Room");
    }

    /// <summary>Call this when the player clears the current room.</summary>
    public void ClearCurrentRoom()
    {
        if (currentRoomIndex < 0 || currentRoomIndex >= rooms.Count) return;

        rooms[currentRoomIndex].cleared = true;
        UpdateDots();
        ShowBanner("Room Cleared");
    }

    void UpdateCounterText()
    {
        if (roomCounterText != null)
            roomCounterText.text = $"Room {currentRoomIndex + 1} / {rooms.Count}";
    }

    void UpdateDots()
    {
        for (int i = 0; i < dotImages.Count; i++)
        {
            if (rooms[i].cleared) dotImages[i].color = dotCleared;
            else if (i == currentRoomIndex) dotImages[i].color = dotCurrent;
            else dotImages[i].color = dotUpcoming;
        }
    }

    void ShowBanner(string text)
    {
        if (bannerText == null || bannerGroup == null) return;
        bannerText.text = text;
        StopAllCoroutines();
        StartCoroutine(BannerRoutine());
    }

    IEnumerator BannerRoutine()
    {
        // Fade in
        float t = 0f;
        while (t < bannerFadeTime)
        {
            t += Time.deltaTime;
            bannerGroup.alpha = Mathf.Lerp(0f, 1f, t / bannerFadeTime);
            yield return null;
        }
        bannerGroup.alpha = 1f;

        yield return new WaitForSeconds(bannerVisibleTime);

        // Fade out
        t = 0f;
        while (t < bannerFadeTime)
        {
            t += Time.deltaTime;
            bannerGroup.alpha = Mathf.Lerp(1f, 0f, t / bannerFadeTime);
            yield return null;
        }
        bannerGroup.alpha = 0f;
    }

    // --- Test-only controls, safe to delete once real systems exist ---
    void Update()
    {
        if (!useMockData) return;

        if (Input.GetKeyDown(KeyCode.RightArrow) && currentRoomIndex < rooms.Count - 1)
            EnterRoom(currentRoomIndex + 1);

        if (Input.GetKeyDown(KeyCode.Space))
            ClearCurrentRoom();
    }
}