using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour
{
    float moveSpeed = 0.5f;

    // Update is called once per frame
    void Update()
    {
        if(Input.touchCount == 2)
        {
            MoveTiles();
        }
    }

    void MoveTiles()
    {
        Vector3 dir = Vector3.zero;
        dir.x = -Input.acceleration.y;
        dir.z = Input.acceleration.x;

        // clamp acceleration vector to unit sphere
        if (dir.sqrMagnitude > 1)
            dir.Normalize();

        // Make it move 10 meters per second instead of 10 meters per frame...
        dir *= Time.deltaTime;

        // Move object
        transform.Translate(dir * moveSpeed * -transform.position.z);
        // Start is called before the first frame update
    }
}
