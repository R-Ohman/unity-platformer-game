using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointFollower : MonoBehaviour
{
    [SerializeField] private Transform[] waypoints;
    private int currentWaypoint = 0;
    private float speed = 1.0f;
    void Update()
    {


        if (currentWaypoint < waypoints.Length)
        {
            Vector3 targetPosition = waypoints[currentWaypoint].position;
            float distance = Vector3.Distance(transform.position, targetPosition);

            if (distance < 0.1f)
            {
                currentWaypoint++;
                currentWaypoint = currentWaypoint % waypoints.Length;
            }

            Vector3 newPosition = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
            transform.position = newPosition;
        }
    }
}
