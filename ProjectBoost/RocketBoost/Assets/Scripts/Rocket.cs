using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Rocket : MonoBehaviour
{
    // config params
    [SerializeField] float mainThrust = 800f;
    [SerializeField] float rcsThrust = 100f;
    [SerializeField] float levelLoadDelay = 2f;

    [SerializeField] AudioClip mainEngine;
    [SerializeField] AudioClip death;
    [SerializeField] AudioClip finish;

    [SerializeField] ParticleSystem mainEngineParticles;
    [SerializeField] ParticleSystem deathParticles;
    [SerializeField] ParticleSystem finishParticles;

    bool isTranscending = false;

    // cached references
    Rigidbody rigidBody;
    AudioSource audioSource;
    int currentScene;
    int nextScene;
    bool colliderEnabled = true;

    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
        currentScene = SceneManager.GetActiveScene().buildIndex;
        nextScene = (currentScene + 1) % SceneManager.sceneCountInBuildSettings;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isTranscending)
        {
            RespondToThrustInput();
            RespondToRotateInput();
        }

        if (Debug.isDebugBuild)
        {
            RespondToDebugKeys();
        }
        
    }

    private void RespondToDebugKeys()
    {
        // debuggin l key allows you to go to next level
        if (Input.GetKeyDown(KeyCode.L))
        {
            LoadNextScene();
        }

        // c key toggles whether collider is enabled or not
        if (Input.GetKeyDown(KeyCode.C))
        {
            colliderEnabled = !colliderEnabled;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // ignore collisions if not alive
        if (isTranscending || !colliderEnabled) {return;}

        switch (collision.gameObject.tag)
        {
            case "Friendly":
                Debug.Log("OK");
                break;
            case "Finish":
                BeginSuccessSequence();
                break;
            default:
                BeginDeathSequence();
                break;
        }
    }

    private void BeginSuccessSequence()
    {
        isTranscending = true;
        audioSource.Stop();
        mainEngineParticles.Stop();
        audioSource.PlayOneShot(finish);
        finishParticles.Play();
        Invoke("LoadNextScene", levelLoadDelay);
    }

    private void BeginDeathSequence()
    {
        isTranscending = true;
        audioSource.Stop();
        mainEngineParticles.Stop();
        audioSource.PlayOneShot(death);
        deathParticles.Play();
        Invoke("LoadStartScene", levelLoadDelay);
    }

    private void LoadStartScene()
    {
        SceneManager.LoadScene(0);
    }

    private void LoadNextScene()
    {
        SceneManager.LoadScene(nextScene);
    }

    private void RespondToThrustInput()
    {
        // GetKey works all the time while getkeydown only applies when it first goes down
        // can thrust while rotating
        if (Input.GetKey(KeyCode.Space))
        {
            ApplyThrust();

        }
        else
        {
            StopApplyingThrust();
        }
    }

    private void StopApplyingThrust()
    {
        audioSource.Stop();
        mainEngineParticles.Stop();
    }

    private void ApplyThrust()
    {
        float thrustThisFrame = mainThrust * Time.deltaTime;

        // frame rate independent because using Time.deltaTime
        rigidBody.AddRelativeForce(Vector3.up * thrustThisFrame);

        if (!audioSource.isPlaying)
        {
            audioSource.PlayOneShot(mainEngine);
        }
        mainEngineParticles.Play();
    }

    private void RespondToRotateInput()
    {
        float rotationThisFrame = rcsThrust * Time.deltaTime;

        if (Input.GetKey(KeyCode.A))
        {
            RotateManually(rotationThisFrame);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            RotateManually(-rotationThisFrame);
        }

        
    }

    private void RotateManually(float rotationThisFrame)
    {
        // take manual control of rotation
        rigidBody.freezeRotation = true;
        transform.Rotate(Vector3.forward * rotationThisFrame);
        // resume physics control of rotation
        rigidBody.freezeRotation = true;
    }
}
