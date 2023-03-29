using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Panda;
using UnityEngine.AI;

public class RandomMovement : MonoBehaviour
{
	public float speed = 5f;
	public float detectionRange = 100f;
	public LayerMask playerLayer;
	public float patrolRange = 500f;
	private Transform target;
	private Vector3 playerPosition;
	[SerializeField] NavMeshAgent agent;

	private void Start()
	{
		target = GameObject.FindGameObjectWithTag("Player").transform;
		AlertEnemy.Found += Alarm;
	}

	[Task]
	private bool PlayerInRange()
	{
		return Vector3.Distance(transform.position, target.position) <= detectionRange;
	}
	private void Alarm(Vector3 pos)
	{
		hasViewedPlayer = true;
		playerPosition = pos;
	}
	[Task]
	private bool hasViewedPlayer = false;

	[Task]
	private bool isTroubled = false;

	[Task]
	public void StopTrouble()
	{
		isTroubled = false;
		hasViewedPlayer = false;
	}

	[Task]
	private bool LineOfSightToPlayer()
	{
		RaycastHit hit;
		if (Physics.Raycast(transform.position, target.transform.position - transform.position, out hit))
		{
			if (hit.transform.CompareTag("Player"))
			{
				hasViewedPlayer = true;
				playerPosition = hit.transform.position;
				return true;
			}
		}
		return false;
	}
	[Task]
	public void IsInPlayerPosition()
	{
		if (transform.position == playerPosition)
		{
			isTroubled = true;
			ThisTask.Succeed();
		}
		ThisTask.Fail();
	}
	[Task]
	bool IsMoving()
	{
		if (agent.velocity == Vector3.zero)
		{
			return false;
		}
		return true;
	}
	[Task]
	private void MoveTowardsPlayer()
	{
	
		agent.SetDestination(playerPosition);
		ThisTask.Succeed();
	}

	[Task]
	private void WaitRandomTime(float minTime, float maxTime)
	{
		float waitTime = Random.Range(minTime, maxTime);
		Invoke("test", waitTime);
	}

	[Task]
	private void RandomMove()
	{
		Vector3 randomDirection = Random.insideUnitSphere * patrolRange;
		randomDirection += transform.position;
		NavMeshHit hit;
		NavMesh.SamplePosition(randomDirection, out hit, patrolRange, NavMesh.AllAreas);
		agent.SetDestination(hit.position);
		ThisTask.Succeed();
	}
}