using UnityEngine;
using UnityEngine.SceneManagement;

public class Rocket : MonoBehaviour {
    
    [SerializeField] float mainThrust = 100f;
    [SerializeField] float rcsThrust = 100f;
    [SerializeField] AudioClip mainEngine, deathSound, successSound;

    Rigidbody rigidBody;
    AudioSource audioSource;

    enum State {  Alive, Dying, Transcending }
    State state = State.Alive;

    // Use this for initialization
    void Start () {
        rigidBody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
	}
	
	// Update is called once per frame
	void Update () {
        if (state == State.Alive) {
            RespondToThrustInput();
            RespondtoRotateInput();
        }
    }

    void OnCollisionEnter(Collision collision) {

        if (state != State.Alive) { return; } // ignore collisions when dead

        switch (collision.gameObject.tag) {
            case "Friendly":
                // do nothing
                break;
            case "Finish":
                StartSuccessSequence();
                break;
            default:
                StartDeathSequence();
                break;

        }
    }

    private void StartSuccessSequence() {
        state = State.Transcending;
        audioSource.Stop();
        audioSource.PlayOneShot(successSound);
        Invoke("LoadNextLevel", 1f); // parameterise time
    }

    private void StartDeathSequence() {
        state = State.Dying;
        audioSource.Stop();
        audioSource.PlayOneShot(deathSound);
        Invoke("LoadFirstLevel", 1f); // parameterise time
    }

    private void LoadNextLevel() {
        SceneManager.LoadScene(1); // todo allow for more than 2 levels
        audioSource.PlayOneShot(successSound);
    }

    private void LoadFirstLevel() {
        SceneManager.LoadScene(0);
        audioSource.PlayOneShot(successSound);
    }


    private void RespondToThrustInput() {
        if (Input.GetKey(KeyCode.Space)) { // can thrust while rotating
            ApplyThrust();
        }
        else {
            audioSource.Stop();
        }
    }

    private void ApplyThrust() {
        rigidBody.AddRelativeForce(Vector3.up * mainThrust);
        if (!audioSource.isPlaying) { // so it doesn't layer
            audioSource.PlayOneShot(mainEngine);
        }
    }

    private void RespondtoRotateInput() {
        rigidBody.freezeRotation = true;

        float rotationThisFrame = rcsThrust * Time.deltaTime;

        if (Input.GetKey(KeyCode.A)) {
            transform.Rotate(Vector3.forward * rotationThisFrame);
        }
        else if (Input.GetKey(KeyCode.D)) {
            transform.Rotate(-Vector3.forward * rotationThisFrame);
        }
        rigidBody.freezeRotation = false;
    }
}
