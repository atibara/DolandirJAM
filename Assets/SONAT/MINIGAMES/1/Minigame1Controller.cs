using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class Minigame1Controller : MonoBehaviour
{
    public static Minigame1Controller Instance; // ARTIK INSTANCE VAR (Hata çözüldü)

    [Header("UI Ayarlarý")]
    [SerializeField] private Slider timerSlider;
    [SerializeField] private float gameDuration = 15f;
    [SerializeField] private RectTransform spawnArea; // Itemlarýn doðabileceði alan (Panelin kendisi)

    [Header("Hedefler")]
    [HideInInspector] public int totalNerdItemsInScene;
    [HideInInspector] public int totalPlayerItemsInScene;

    // Anlýk toplananlar
    private int collectedNerd = 0;
    private int collectedPlayer = 0;

    private float timer;
    private bool isGameActive = false;
    //fasfas

    private void Awake()
    {
        // Singleton Kurulumu
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void OnEnable()
    {
        // Oyun her açýldýðýnda itemlarý say
        MinigameItem[] allItems = FindObjectsByType<MinigameItem>(FindObjectsSortMode.None);
        totalNerdItemsInScene = 0;
        totalPlayerItemsInScene = 0;

        foreach (var item in allItems)
        {
            item.gameObject.SetActive(true); // Belki kapanmýþ vardýr, aç
            if (item.type == ItemType.NerdItem) totalNerdItemsInScene++;
            else if (item.type == ItemType.PlayerItem) totalPlayerItemsInScene++;
        }

        timer = gameDuration;
        isGameActive = true;
        collectedNerd = 0;
        collectedPlayer = 0;
    }

    private void Update()
    {
        if (!isGameActive) return;

        timer -= Time.deltaTime;
        if (timerSlider) timerSlider.value = timer / gameDuration;

        if (timer <= 0) FinishGame();

        // Hepsi bitti mi?
        if ((collectedNerd + collectedPlayer) >= (totalNerdItemsInScene + totalPlayerItemsInScene))
        {
            FinishGame();
        }
    }

    public void AddScore(ItemType type)
    {
        if (type == ItemType.NerdItem) collectedNerd++;
        if (type == ItemType.PlayerItem) collectedPlayer++;
    }

    // YANLIÞ KUTUYA ATILAN ITEM'I IÞINLA
    public void RespawnItemRandomly(MinigameItem item)
    {
        // Panelin içinde rastgele bir x,y (örnek deðerler, panel boyutuna göre ayarla)
        float rangeX = 300f;
        float rangeY = 150f;

        if (spawnArea != null)
        {
            rangeX = spawnArea.rect.width / 2 - 50;
            rangeY = spawnArea.rect.height / 2 - 50;
        }

        Vector3 randomPos = new Vector3(Random.Range(-rangeX, rangeX), Random.Range(-rangeY, rangeY), 0);
        item.transform.localPosition = randomPos;

        // Item'ý sýfýrla
        item.ResetState();
    }

    private void FinishGame()
    {
        isGameActive = false;

        // --- SONUÇLARI GÖNDER ---
        if (collectedNerd >= totalNerdItemsInScene * 0.5f) GameScenarioManager.Instance.StatsManager.addNerdLove(10);
        else
        {
            GameScenarioManager.Instance.StatsManager.addNerdLove(-5);
            GameScenarioManager.Instance.StatsManager.addPunkLove(10);
        }

        if (collectedPlayer < totalPlayerItemsInScene) GameScenarioManager.Instance.StatsManager.addSportsLove(10);

        gameObject.SetActive(false);
        GameScenarioManager.Instance.OnMinigame1Finished_Logic();
    }
}