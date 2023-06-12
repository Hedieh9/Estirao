using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class RandomPosition : MonoBehaviour
{
    [SerializeField] private float randomRange = .5f;
    [SerializeField] private float minTime = .5f;
    [SerializeField] private float maxTime = 10f;
    [SerializeField] private float minWait = 0f;
    [SerializeField] private float maxWait = 2f;

    private Vector3 startPosition;
    

    private void OnEnable()
    {
        startPosition = transform.position;
        StartCoroutine(RandomPositionRoutine());
    }

    private IEnumerator RandomPositionRoutine()
    {

        while (true)
        {
            Vector3 newPosition = startPosition + new Vector3(Random.Range(-randomRange, randomRange),
                Random.Range(-randomRange, randomRange), Random.Range(-randomRange, randomRange));
            float randomTime = Random.Range(minTime, maxTime);
            float time = 0f;
            
            while(time <= randomTime)
            {
                time += Time.deltaTime;

                float relativeTime = time / randomTime;

                transform.position = Vector3.Lerp(startPosition, newPosition, relativeTime);
                yield return null;
            }

            yield return new WaitForSeconds(Random.Range(minWait, maxWait));

            time = 0f;
            
            while(time <= randomTime)
            {
                time += Time.deltaTime;

                float relativeTime = time / randomTime;

                transform.position = Vector3.Lerp(newPosition, startPosition, relativeTime);
                yield return null;
            }

            yield return new WaitForSeconds(Random.Range(minWait, maxWait));
        }
    }

    #if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow.With(a: 0.2f); 
        Gizmos.DrawSphere(transform.position, randomRange);
    }
    #endif
}