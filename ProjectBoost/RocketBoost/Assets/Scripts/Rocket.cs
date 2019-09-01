using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Rocket : MonoBehaviour
{
    // config params
    [SerializeField] float mainThrust = 800f;
    [SerializeField] float rcsThrust = 100f;
    [SerializeField] AudioClip mainEngine;
    [SerializeField] AudioClip death;
    [SerializeField] AudioClip finish;

    enum State {Alive, Dying, Transcending};
    State state = State.Alive;

    // cached references
    Rigidbody rigidBody;
    AudioSource audioSource;
    int currentScene;
    int nextScene;

    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
        currentScene = SceneManager.GetActiveScene().buildIndex;
        nextScene = currentScene + 1 > SceneManager.sceneCount ? currentScene : currentScene + 1;
    }

    // Update is called once per frame
    void Update()
    {
        if (state == State.Alive)
        {
            RespondToThrustInput();
            RespondToRotateInput();
        }
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        // ignore collisions if not alive
        if (state != State.Alive) {return;}

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
        state = State.Transcending;
        audioSource.Stop();
        audioSource.PlayOneShot(finish);
        Invoke("LoadNextScene", 1f);
    }

    private void BeginDeathSequence()
    {
        state = State.Dying;
        audioSource.Stop();
        audioSource.PlayOneShot(death);
        Invoke("LoadStartScene", 1f);
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
            audioSource.Stop();
        }
    }

    private void ApplyThrust()
    {
        float thrustThisFrame = mainThrust * Time.deltaTime;

        rigidBody.AddRelativeForce(Vector3.up * thrustThisFrame);

        if (!audioSource.isPlaying)
        {
            audioSource.PlayOneShot(mainEngine);
        }
    }

    private void RespondToRotateInput()
    {
        // take manual control of rotation
        rigidBody.freezeRotation = true;
        float rotationThisFrame = rcsThrust * Time.deltaTime;

        if (Input.GetKey(KeyCode.A))
        {
            transform.Rotate(Vector3.forward * rotationThisFrame);
        } else if (Input.GetKey(KeyCode.D))
        {
            transform.Rotate(Vector3.back * rotationThisFrame);
        }

        // resume physics control of rotation
        rigidBody.freezeRotation = true;
    }

}
