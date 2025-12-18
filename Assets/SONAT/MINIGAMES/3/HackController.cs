using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

[System.Serializable]
public class HackRound
{
    [Header("Resimler")]
    public Sprite funnyPhoto;   // Komik/Ýfþa (HEDEF BU)
    public Sprite[] normalPhotos; // 3 Tane Güzel Foto (TUZAK)
}

public class HackController : MonoBehaviour
{
    [Header("UI Baðlantýlarý")]
    [SerializeField] private Button[] gridButtons; // 4 Butonu buraya sürükle
    [SerializeField] private Slider timerSlider;
    [SerializeField] private TextMeshProUGUI statusText;

    [Header("Ayarlar")]
    [SerializeField] private float timePerRound = 3f; // Seçmek için kaç saniyen var?
    [SerializeField] private List<HackRound> rounds; // Ýfþa arþivini buraya ekle

    // Durum
    private int currentRoundIndex = 0;
    private float timer;
    private bool isHackActive = false;

    // Skor
    private int sabotageScore = 0; // Kaç tane ifþa attýk?

    private void OnEnable()
    {
        StartHack();
    }

    private void StartHack()
    {
        currentRoundIndex = 0;
        sabotageScore = 0;
        isHackActive = true;
        LoadRound();
    }

    private void LoadRound()
    {
        // Tur bitti mi?
        if (currentRoundIndex >= rounds.Count)
        {
            EndHack();
            return;
        }

        timer = timePerRound;
        statusText.text = "HEDEFÝ SEÇ...";
        statusText.color = Color.white;

        HackRound currentData = rounds[currentRoundIndex];

        // --- KARIÞTIRMA ALGORÝTMASI ---
        // 4 butona 1 komik + 3 normal resmi rastgele daðýtacaðýz.

        // 1. Önce resimleri geçici bir listeye doldur
        List<Sprite> roundSprites = new List<Sprite>();
        roundSprites.Add(currentData.funnyPhoto); // 0. eleman her zaman komik (þimdilik)
        foreach (var s in currentData.normalPhotos) roundSprites.Add(s);

        // 2. Butonlarý rastgele bir sýrayla gezmek için index listesi oluþtur (0,1,2,3)
        List<int> buttonIndices = new List<int> { 0, 1, 2, 3 };

        // 3. Resimleri butonlara daðýt
        // roundSprites[0] -> Funny Photo
        // roundSprites[1,2,3] -> Normal Photos

        int funnyButtonIndex = -1;

        // Rastgele bir butona Komik resmi ver
        int randomSlot = Random.Range(0, 4);
        funnyButtonIndex = buttonIndices[randomSlot];

        // Geriye kalan butonlara normalleri ver
        buttonIndices.RemoveAt(randomSlot); // Artýk bu slot dolu

        // Komik resmi ata
        gridButtons[funnyButtonIndex].image.sprite = currentData.funnyPhoto;
        // Týklama olayýný ayarla (TRUE = Komik)
        gridButtons[funnyButtonIndex].onClick.RemoveAllListeners();
        gridButtons[funnyButtonIndex].onClick.AddListener(() => OnPhotoClicked(true));

        // Diðer 3 butona normal resimleri ata
        for (int i = 0; i < 3; i++)
        {
            int slot = buttonIndices[i];
            gridButtons[slot].image.sprite = currentData.normalPhotos[i];
            // Týklama olayýný ayarla (FALSE = Normal)
            gridButtons[slot].onClick.RemoveAllListeners();
            gridButtons[slot].onClick.AddListener(() => OnPhotoClicked(false));
        }
    }

    private void Update()
    {
        if (!isHackActive) return;

        timer -= Time.deltaTime;
        if (timerSlider) timerSlider.value = timer / timePerRound;

        if (timer <= 0)
        {
            // Süre bitti, fýrsat kaçtý! (Normal atmýþ gibi sayalým veya pas geçelim)
            OnPhotoClicked(false);
        }
    }

    public void OnPhotoClicked(bool isFunny)
    {
        if (!isHackActive) return;

        if (isFunny)
        {
            sabotageScore++;
            statusText.text = "ÝFÞALANDI!";
            statusText.color = Color.green;
            // Efekt: Ekrana "UPLOADED" yazýsý vs. çýkabilir
        }
        else
        {
            // Güzel foto attýk, kýza yaradý :(
            statusText.text = "YANLIÞ FOTO!";
            statusText.color = Color.red;
        }

        currentRoundIndex++;
        // Çok hýzlý geçmesin, sonucu 0.5sn görelim
        isHackActive = false;
        Invoke("NextRoundDelay", 0.5f);
    }

    private void NextRoundDelay()
    {
        isHackActive = true;
        LoadRound();
    }

    private void EndHack()
    {
        isHackActive = false;

        // --- SONUÇLAR ---
        Debug.Log($"Hack Bitti. Ýfþa Skoru: {sabotageScore} / {rounds.Count}");

        // Yarýdan fazlasýný ifþaladýysak baþarýlý
        if (sabotageScore >= rounds.Count * 0.5f)
        {
            // BAÞARILI: Kýz rezil oldu
            // Sporcu kýzdan soðudu (+Love bize)
            GameScenarioManager.Instance.StatsManager.addSportsLove(15);
            // Serseri bu kaosu sevdi
            GameScenarioManager.Instance.StatsManager.addPunkLove(10);

            statusText.text = "HACK BAÞARILI!";
        }
        else
        {
            // BAÞARISIZ: Kýza iyilik yaptýk
            GameScenarioManager.Instance.StatsManager.addSportsLove(-5);

            statusText.text = "PLAN TUTMADI...";
        }

        Invoke("FinishMinigame", 2f);
    }

    private void FinishMinigame()
    {
        gameObject.SetActive(false);
        // Eðer parantez içinde bir success/fail deðiþkeni varsa aynen kalsýn
        GameScenarioManager.Instance.OnMinigame3Finished_Logic(true);
        // NOT: HackController'da baþarý durumunu tutan bir bool deðiþken varsa 'true' yerine onu yaz.
        // Örneðin: OnMinigame3Finished_Logic(basariliMi);
    }
}