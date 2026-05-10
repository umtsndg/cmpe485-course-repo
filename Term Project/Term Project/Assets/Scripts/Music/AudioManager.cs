using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Sources")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioSource loopSfxSource;

    [Header("Music")]
    [SerializeField] private AudioClip mainMenuMusic;
    [SerializeField] private AudioClip normalGameMusic;

    [Header("Player Movement SFX")]
    [SerializeField] private AudioClip runClip;
    [SerializeField] private AudioClip jumpClip;
    [SerializeField] private AudioClip wallRunClip;
    [SerializeField] private AudioClip dashClip;
    [SerializeField] private AudioClip slideClip;

    [Header("Combat SFX")]
    [SerializeField] private AudioClip enemyMeleeHitClip;
    [SerializeField] private AudioClip enemyRangedShootClip;
    [SerializeField] private AudioClip blockClip;
    [SerializeField] private AudioClip parryClip;
    [SerializeField] private AudioClip swordSwingClip;
    [SerializeField] private AudioClip enemyDieClip;
    [SerializeField] private AudioClip enemyWalkClip;
    [SerializeField] private AudioClip playerDieClip;

    [Header("Volume")]
    [Range(0f, 1f)][SerializeField] private float musicVolume = 0.35f;
    [Range(0f, 1f)][SerializeField] private float sfxVolume = 0.8f;
    [Range(0f, 1f)][SerializeField] private float loopSfxVolume = 0.45f;

    private string currentLoopName = "";

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (musicSource != null) musicSource.volume = musicVolume;
        if (sfxSource != null) sfxSource.volume = sfxVolume;
        if (loopSfxSource != null) loopSfxSource.volume = loopSfxVolume;
    }

    // ---------- MUSIC ----------

    public void PlayMainMenuMusic()
    {
        PlayMusic(mainMenuMusic);
    }

    public void PlayNormalGameMusic()
    {
        PlayMusic(normalGameMusic);
    }

    private void PlayMusic(AudioClip clip)
    {
        if (musicSource == null || clip == null) return;

        if (musicSource.clip == clip && musicSource.isPlaying) return;

        musicSource.Stop();
        musicSource.clip = clip;
        musicSource.loop = true;
        musicSource.volume = musicVolume;
        musicSource.Play();
    }

    // ---------- ONE-SHOT SFX ----------

    private void PlaySFX(AudioClip clip)
    {
        if (sfxSource == null || clip == null) return;

        sfxSource.PlayOneShot(clip, sfxVolume);
    }

    public void PlayJump() => PlaySFX(jumpClip);
    public void PlayDash() => PlaySFX(dashClip);
    public void PlaySlide() => PlaySFX(slideClip);
    public void PlayEnemyMeleeHit() => PlaySFX(enemyMeleeHitClip);
    public void PlayEnemyRangedShoot() => PlaySFX(enemyRangedShootClip);
    public void PlayBlock() => PlaySFX(blockClip);
    public void PlayParry() => PlaySFX(parryClip);
    public void PlaySwordSwing() => PlaySFX(swordSwingClip);
    public void PlayEnemyDie() => PlaySFX(enemyDieClip);
    public void PlayPlayerDie() => PlaySFX(playerDieClip);

    // ---------- LOOPING SFX ----------

    public void StartRunLoop()
    {
        StartLoop(runClip, "Run");
    }

    public void StartWallRunLoop()
    {
        StartLoop(wallRunClip, "WallRun");
    }

    public void StartEnemyWalkLoop()
    {
        StartLoop(enemyWalkClip, "EnemyWalk");
    }

    public void StopLoop()
    {
        if (loopSfxSource == null) return;

        loopSfxSource.Stop();
        loopSfxSource.clip = null;
        currentLoopName = "";
    }

    private void StartLoop(AudioClip clip, string loopName)
    {
        if (loopSfxSource == null || clip == null) return;

        if (currentLoopName == loopName && loopSfxSource.isPlaying) return;

        loopSfxSource.clip = clip;
        loopSfxSource.loop = true;
        loopSfxSource.volume = loopSfxVolume;
        loopSfxSource.Play();

        currentLoopName = loopName;
    }

    public void StopAllSFX()
    {
        if (sfxSource != null)
        {
            sfxSource.Stop();
        }

        if (loopSfxSource != null)
        {
            loopSfxSource.Stop();
            loopSfxSource.clip = null;
        }

        currentLoopName = "";
    }
}