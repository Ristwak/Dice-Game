using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PawnMovement : MonoBehaviour
{
    public IEnumerator MoveAlongPath(
        List<Transform> pathPoints,
        int currentIndex,
        int steps,
        System.Action<int> onMoveComplete)
    {
        if (pathPoints == null || pathPoints.Count == 0)
        {
            Debug.LogError("PawnMovement: Path is empty or missing.");
            onMoveComplete?.Invoke(0);
            yield break;
        }

        currentIndex = Mathf.Clamp(currentIndex, 0, pathPoints.Count - 1);
        steps = Mathf.Max(0, steps);

        int targetIndex = Mathf.Min(
            currentIndex + steps,
            pathPoints.Count - 1
        );

        float baseY = transform.position.y;

        while (currentIndex < targetIndex)
        {
            Transform nextPoint = pathPoints[currentIndex + 1];

            if (nextPoint == null)
            {
                Debug.LogError(
                    "PawnMovement: Missing path point at index " + (currentIndex + 1)
                );

                break;
            }

            Vector3 start = transform.position;
            Vector3 end = nextPoint.position;

            start.y = baseY;
            end.y = baseY;

            float elapsed = 0f;
            const float duration = 0.5f;
            const float jumpHeight = 1.5f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;

                float t = Mathf.Clamp01(elapsed / duration);
                float yOffset = Mathf.Sin(Mathf.PI * t) * jumpHeight;

                transform.position =
                    Vector3.Lerp(start, end, t) +
                    Vector3.up * yOffset;

                yield return null;
            }

            transform.position = end;
            currentIndex++;
        }

        onMoveComplete?.Invoke(currentIndex);
    }
}