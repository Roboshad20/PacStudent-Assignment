using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip introMusic;
    public AudioClip ghostNormalState;

    // Start is called before the first frame update
    void Start()
    {
        audioSource.clip = introMusic;
        audioSource.Play();

        float waitTime = Mathf.Min(introMusic.length, 3f);
        Invoke("GhostNormalMusic", waitTime);
    }

    // Update is called once per frame
    void GhostNormalMusic()
    {
        audioSource.clip = ghostNormalState;
        audioSource.loop = true;
        audioSource.Play();
    }
}
