using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]

public class Oscillator : MonoBehaviour
{
    [SerializeField] Vector3 movementVector;
    // period of sine wave is 2 seconds
    [SerializeField] float period = 2f;

    // 0 for no move, 1 for fully moved
    float movementFactor;

    Vector3 startingPos;

    // Start is called before the first frame update
    void Start()
    {
        startingPos = transform.position;
        
    }

    // Update is called once per frame
    void Update()
    {
        // if period is 0 stop movement so we don't divide by 0
        if (Mathf.Abs(period) <= Mathf.Epsilon)
        {
            return;
        }

        // number of cycles of sin that have happened in game
        float cycles = Time.time / period;
        const float tau = Mathf.PI * 2;
        float rawSinWave = Mathf.Sin(cycles * tau);

        // make amplitude be between 0 and 1
        movementFactor = rawSinWave / 2f + 0.5f;

        Vector3 offset = movementFactor * movementVector;
        transform.position = startingPos + offset;
        
    }


}
