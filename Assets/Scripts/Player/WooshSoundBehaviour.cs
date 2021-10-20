using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[RequireComponent(typeof(AudioSource))]
public class WooshSoundBehaviour : MonoBehaviour
{
    public Rigidbody objectRB;

    public float minWooshSpeed;

    private bool canWoosh;

    private AudioSource aud;

    // Start is called before the first frame update
    private void Start()
    {
        canWoosh = true;
    }

#if UNITY_EDITOR
    private void Reset()
    {
        AudioMixer mixer = Resources.Load("AudioMaster") as AudioMixer;
        aud = GetComponent<AudioSource>();
        aud.outputAudioMixerGroup = mixer.FindMatchingGroups("SFX")[0];
        aud.playOnAwake = false;
        aud.spatialBlend = 1;
        aud.rolloffMode = AudioRolloffMode.Custom;
        aud.minDistance = 50;
        aud.maxDistance = 200;
    }
#endif

    private void Awake()
    {
        aud = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 velocity = objectRB.velocity;

        if(velocity.magnitude > minWooshSpeed)
        {
            PlayWoosh();
        }

        if(velocity.magnitude < minWooshSpeed)
        {
            //aud.Stop();
            if(aud.volume != 0)
            {
                aud.volume -= Time.deltaTime * 10;
            }
            canWoosh = true;
        }
    }

    public void PlayWoosh()
    {
        if(canWoosh)
        {
            float t = (objectRB.velocity.magnitude - minWooshSpeed)*2 / 100;
            aud.volume = Mathf.Lerp(0, 1, t);
            //aud.Play();

            //canWoosh = false;
        }
    }
}
