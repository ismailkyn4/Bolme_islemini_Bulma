using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private GameObject karePrefab;

    [SerializeField]
    private Transform karelerPaneli;

    [SerializeField]
    private Transform soruPaneli;

    [SerializeField]
    private Text soruText;

    [SerializeField]
    private Sprite[] kareSprites;

    [SerializeField]
    AudioSource audioSource;

    public AudioClip butonSesi,butonSesi2;

    bool butonaBasilsinmi;

    private GameObject[] karelerdizisi = new GameObject[25];

    List<int> bolumDegerleriListesi = new List<int>();

    int bolunenSayi, bolenSayi;
    int dogruSonuc;
    int kacinciSoru;
    int butonDegeri;
    int kalanHak;

    string sorununZorlukDerecesi;
    kalanHaklarManager KalanHaklarManager; //kalanHaklarManager scriptine erişiyoruz.
    puanManager puan_manager;

    GameObject gecerliKare;

    [SerializeField]
    private GameObject sonucPaneli;

    private void Awake()
    {
        kalanHak = 3;

        audioSource = GetComponent<AudioSource>();

        sonucPaneli.GetComponent<RectTransform>().localScale = Vector3.zero; //sonuç panelini başlangıçta göstermiyoruz.
        KalanHaklarManager = Object.FindObjectOfType<kalanHaklarManager>(); //kalanHaklarManager scriptine ulaşıyoruz.
        puan_manager = Object.FindObjectOfType<puanManager>();
        KalanHaklarManager.kalanHaklariKontrolEt(kalanHak);
    }
    void Start()
    {
        butonaBasilsinmi = false;
        soruPaneli.GetComponent<RectTransform>().localScale = Vector3.zero; //soru panelinin ebatlarını 0'a çektik.
        kareleriOlustur();
    }
    public void kareleriOlustur()
    {
        for (int i = 0; i < 25; i++)
        {
            GameObject kare = Instantiate(karePrefab, karelerPaneli);
            kare.transform.GetChild(1).GetComponent<Image>().sprite = kareSprites[Random.Range(0, kareSprites.Length)];
            //karenin içine index 1e image eklediğimiz için, 1. indexe erişip imageni random olarak attık. 
            kare.transform.GetComponent<Button>().onClick.AddListener(() => butonaBasildi());
            //butonu dinamik olarak ayarladık. 
            karelerdizisi[i] = kare; 
        }
        bolumDegerleriniTexteYazdır();

        StartCoroutine(doFadeRoutine());

        Invoke("soruPaneliniAc", 2f);
    }
    void butonaBasildi()
    {
        if (butonaBasilsinmi)
        {
            
            butonDegeri = int.Parse(UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.transform.GetChild(0).GetComponent<Text>().text);

            gecerliKare = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject; //geçerli karemiz şuanda tıklanan kare diyoruz.
            sonucuKontrolEt();
        }
        //butona basılmasıyla çalışır.
        //eventsystem içinn içindeki o anda basılı olan objeyi bul transformu içerisindeki childı 0 olanı bul
        //bunun içindeki text nesnesinin text değerini al bana söyle.
        //tekrar tekrar bu işlemi yapmamak için integer değişkene attık.
    }

    private void sonucuKontrolEt()
    {
        if (butonDegeri==dogruSonuc)
        {
            audioSource.PlayOneShot(butonSesi);//birkez çalıştır diyoruz. 
            gecerliKare.transform.GetChild(1).GetComponent<Image>().enabled = true; //eğer doğru sonuca tıklanırsa tıklanılan butonun image kısmını aktif ediyoruz.
            gecerliKare.transform.GetChild(0).GetComponent<Text>().text = "";//text kısmını kaldırıyoruz.
            gecerliKare.transform.GetComponent<Button>().interactable = false; //aynı kareye bidaha basılamaması için butonun enable veya interactable özelliğini false yaptık.
            puan_manager.puaniArtir(sorununZorlukDerecesi);
            bolumDegerleriListesi.RemoveAt(kacinciSoru); //aynı soruyu sormamak için listeden bolumdeğerleri listesinde sorulan soruyu çıkardık.
            Debug.Log(bolumDegerleriListesi.Count);
            if (bolumDegerleriListesi.Count>0)
            {
                soruPaneliniAc();
            }
            else
            {
                Debug.Log("oyun bitti");
            }

        }
        else
        {
            audioSource.PlayOneShot(butonSesi2);
            kalanHak--;
            KalanHaklarManager.kalanHaklariKontrolEt(kalanHak);
        }
        if (kalanHak<=0)
        {
            Invoke("OyunBitti", 1f); 
        }
    }
    void OyunBitti()
    {
        butonaBasilsinmi = false;
        sonucPaneli.GetComponent<RectTransform>().DOScale(1, 0.3f).SetEase(Ease.OutBack);
    }

    IEnumerator doFadeRoutine()
    {
        foreach (var kare in karelerdizisi)
        {
            kare.GetComponent<CanvasGroup>().DOFade(1, .2f); //kare değerlerinin alphasını 1e çekip ekranda gösteriyoruz.

            yield return new WaitForSeconds(0.07f);
        }
    }
    void bolumDegerleriniTexteYazdır()
    {
        foreach (var kare in karelerdizisi)
        {
            int rastgeleDeger = Random.Range(1, 13);
            bolumDegerleriListesi.Add(rastgeleDeger);//rastgele değerleri listeye attık.
            kare.transform.GetChild(0).GetComponent<Text>().text=rastgeleDeger.ToString();
            //kare prefabının içindeki texte eriştik. ilk child olduğu için 0  
        }
    }
    void soruPaneliniAc()
    {
        butonaBasilsinmi = true;
        soruyuSor();
        soruPaneli.GetComponent<RectTransform>().DOScale(1, 0.3f).SetEase(Ease.OutBack); //dotween fonksiyanları bunlar
    }
    void soruyuSor()
    {

        bolenSayi = Random.Range(2, 11);

        kacinciSoru = Random.Range(0,bolumDegerleriListesi.Count);

        dogruSonuc = bolumDegerleriListesi[kacinciSoru];
        bolunenSayi = bolenSayi * dogruSonuc;
        if (bolunenSayi<=40)
        {
            sorununZorlukDerecesi = "kolay";
        }
        else if (bolunenSayi>40 && bolunenSayi <=80)
        {
            sorununZorlukDerecesi = "orta";
        }
        else
        {
            sorununZorlukDerecesi = "zor";
        }

        soruText.text = bolunenSayi.ToString()+" : "+bolenSayi.ToString();
    }
}
