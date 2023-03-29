using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Panda;
using UnityEngine.AI;

public class AlertEnemy : MonoBehaviour
{
	public float speed = 5f;
	public float detectionRange = 100f;
	public LayerMask playerLayer;
	public float patrolRange = 500f;
	public delegate void foundPlayer(Vector3 pos);
	public static event foundPlayer Found;
	private Transform target;
	private Vector3 playerPosition;
	private Vector3 startingPosition;
	[SerializeField] NavMeshAgent agent;

	private void Start()
	{
		target = GameObject.FindGameObjectWithTag("Player").transform;
		startingPosition = transform.position;
	}

	[Task]
	private bool PlayerInRange()
	{
		return Vector3.Distance(transform.position, target.position) <= detectionRange;
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
		ThisTask.Succeed();
	}

	[Task]
	public void Alarm()
	{
		Found(playerPosition);
		ThisTask.Succeed();
	}

	[Task]
	public void GoesBackToStartPosition()
	{

		agent.SetDestination(startingPosition);
		ThisTask.Succeed();
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
		if (agent.velocity.sqrMagnitude < 0.1f)
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
		ThisTask.Succeed();
	}

	[Task]
	private void RandomMovement()
	{
		Vector3 randomDirection = Random.insideUnitSphere * patrolRange;
		randomDirection += transform.position;
		NavMeshHit hit;
		NavMesh.SamplePosition(randomDirection, out hit, patrolRange, NavMesh.AllAreas);
		agent.SetDestination(hit.position);
		ThisTask.Succeed();
	}
}
