using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    private float _speed = 5f;
    [SerializeField]
    private float _speedBoost = 8.5f;
    [SerializeField]
    private GameObject _speedBoostPrefab;
    [SerializeField]
    private GameObject _laserPrefab;
    [SerializeField]
    private GameObject _tripleshotPrefab;
    [SerializeField]
    private GameObject _shieldsVisualizer;
    [SerializeField]
    private float _fireRate = 0.5f;
    private float _canFire = -1f;
    [SerializeField]
    private int _lives = 3;
    private SpawnManager _spawnManager;
    private bool _isTripleShotActive = false;
    private bool _isSpeedBoostActive = false;
    private bool _isShieldsActive = false;
    [SerializeField]
    private int _score;
    private UIManager _uiManager;
    [SerializeField]
    private GameObject _rightEnginePrefab, _leftEnginePrefab;
    private AudioManager _audioManager;
    

    // Start is called before the first frame update
    void Start()
    {
        transform.position = new Vector3(0, 0, 0);
        _spawnManager = GameObject.Find("Spawn_Manager").GetComponent<SpawnManager>();
        _uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();
        _audioManager = GameObject.Find("Audio_Manager").GetComponent<AudioManager>();
        _rightEnginePrefab.SetActive(false);
        _leftEnginePrefab.SetActive(false);

        if (_spawnManager == null)
        {
            Debug.LogError("The Spawn Manager is NULL.");
        }

        if (_uiManager == null)
        {
            Debug.LogError("The UI Manager is NULL.");
        }

        if (_audioManager == null)
        {
            Debug.Log("The Audio Manager is NULL.");
        }    
    }

    // Update is called once per frame
    void Update()
    {
        CalculateMovement();

        if (Input.GetKeyDown(KeyCode.Space) && Time.time > _canFire)
        {
            FireLaser();
        }
    }

    void CalculateMovement()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector3 direction = new Vector3(horizontalInput, verticalInput, 0);

        if (_isSpeedBoostActive == false)
        {
            transform.Translate(direction * _speed * Time.deltaTime);
        }
        else if (_isSpeedBoostActive == true)
        {
            transform.Translate(direction * _speedBoost * Time.deltaTime);
            StartCoroutine(SpeedBoostPowerDownRoutine());
        }

        transform.position = new Vector3(transform.position.x, Mathf.Clamp(transform.position.y, -3.8f, 0), 0);

        if (transform.position.x >= 11.3f)
        {
            transform.position = new Vector3(-11.3f, transform.position.y, 0);
        }
        else if (transform.position.x <= -11.3f)
        {
            transform.position = new Vector3(11.3f, transform.position.y, 0);
        }
    }

    void FireLaser()
    {
        _canFire = Time.time + _fireRate;

        if (_isTripleShotActive == false)
        {
            Instantiate(_laserPrefab, transform.position + new Vector3(0, 1.05f, 0), Quaternion.identity);
        }
        else if (_isTripleShotActive == true)
        {
            Instantiate(_tripleshotPrefab, transform.position, Quaternion.identity);
        }

        //play the laser audio clip
        _audioManager.LaserShotAudio();
    }

    public void Damage()
    {

        if (_isShieldsActive == true)
        {
            _isShieldsActive = false;
            _shieldsVisualizer.SetActive(false);
            return;
        }
        
        _lives--;

        if (_lives == 2)
        {
            _rightEnginePrefab.SetActive(true);
        }
        else if (_lives == 1)
        {
            _leftEnginePrefab.SetActive(true);
        }

        _uiManager.UpdateLives(_lives);
        
        if (_lives < 1)
        {
            _spawnManager.OnPlayerDeath();
            Destroy(this.gameObject);
            _audioManager.ExplosionSoundAudio();
        }
    }

    public void TripleShotActive()
    {
        _isTripleShotActive = true;
        StartCoroutine(TripleShotPowerDownRoutine());
    }

    IEnumerator TripleShotPowerDownRoutine()
    {
        yield return new WaitForSeconds(5.0f);
        _isTripleShotActive = false;
    }    

    public void SpeedBoostActive()
    {
        _isSpeedBoostActive = true;
    }

    IEnumerator SpeedBoostPowerDownRoutine()
    {
        yield return new WaitForSeconds(5.0f);
        _isSpeedBoostActive = false;
    }

    public void ShieldsActive()
    {
        _isShieldsActive = true;
        _shieldsVisualizer.SetActive(true);
    }

    public void AddScore(int points)
    {
        _score += points;
        _uiManager.UpdateScore(_score);
    }
}
