using UnityEngine;

public static class ChainPathUtility 
{
   public static GameObject CreatePath(Vector3[] points, GameObject chainPrefab, float spacing, bool closedLoop)
    {
        if (chainPrefab == null) return null;
        if(spacing <= 0f)
        {
            Debug.LogWarning("The spacing must be > 0");
            return null;
        }
        GameObject container = new GameObject("ChainPath");

        int segmentCount;

        if (closedLoop)
        {
            segmentCount = points.Length; // Last segment goes back to the first point
        }
        else
        {
            segmentCount = points.Length - 1;
        }

        for(int i = 0; i < segmentCount; i++)
        {
            Vector3 startPos = points[i]; 

            int nextIndex = i + 1;

            if(nextIndex >= points.Length)
            {
                nextIndex = 0;
            }

            Vector3 endPos = points[nextIndex];

            CreateSegment(startPos, endPos, chainPrefab, spacing, container.transform);
        }

        return container;
    }


    public static void CreateSegment(Vector3 startPos, Vector3 endPos, GameObject chainPrefab, float desiredSpacing, Transform parent)
    {
        Vector3 direction = endPos - startPos;

        float totalDistance = direction.magnitude;

        int numberOfChains = Mathf.CeilToInt(totalDistance / desiredSpacing);

        Vector3 normalizedDirection = direction.normalized;

        float actualSpacing = totalDistance / numberOfChains;

        for(int i = 0; i < numberOfChains; i++)
        {
            float distanceFromStart = i * actualSpacing;

            Vector3 chainPosition = startPos + (normalizedDirection * distanceFromStart);

            Object.Instantiate(chainPrefab, chainPosition, Quaternion.identity, parent);
        }
    }
}
