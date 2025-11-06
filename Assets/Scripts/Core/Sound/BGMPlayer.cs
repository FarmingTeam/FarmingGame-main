using UnityEngine;
using UnityEngine.SceneManagement;

public class BGMPlayer : Singleton<BGMPlayer>
{

    private static BGMPlayer instance;
    private AudioSource audioSource;

    public AudioClip mainBGM; //스타트씬
    public AudioClip farmBGM;
    public AudioClip houseBGM;
    public AudioClip midwayBGM;
    public AudioClip shopBGM;
    public AudioClip sleepBGM;
    public AudioClip townBGM;
    public AudioClip openingBGM;


    protected override void Initialize()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.loop = true;
        audioSource.playOnAwake = false;

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void Start()
    {
        if (SceneManager.GetActiveScene().name == "StartScene")
        {
            PlayBGM(mainBGM);
        }
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    //Refactor : 씬 이름 변경시 이부분도 같이 변경
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        switch (scene.name)
        {
            case "StartScene":
                PlayBGM(mainBGM);
                break;
            case "TestInHouseScene":
                PlayBGM(houseBGM);
                break;
            case "TestFarmScene":
                PlayBGM(farmBGM);
                break;
            case "TestInShopScene":
                PlayBGM(shopBGM);
                break;
            case "TestMidwayScene":
                PlayBGM(midwayBGM);
                break;
            case "TestTownScene":
                PlayBGM(townBGM);
                break;
            case "OpeningSequenceScene":
                PlayBGM(openingBGM);
                break;
            default:
                break;
                
        }
    }

    public void PlayBGM(AudioClip clip)
    {
        if (clip == null)
        {
            return;
        }

        if (audioSource.clip == clip && audioSource.isPlaying)
        {
            return;
        }

        audioSource.clip = clip;
        audioSource.Play();
    }

}
