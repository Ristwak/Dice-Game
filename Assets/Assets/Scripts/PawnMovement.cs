using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PawnMovement : MonoBehaviour
{
    public IEnumerator MoveAlongPath(List<Transform> pathPoints, int currentIndex, int steps, System.Action<int> onMoveComplete)
    {
        int targetIndex = Mathf.Min(currentIndex + steps, pathPoints.Count - 1);

        float baseY = transform.position.y;

        while (currentIndex < targetIndex)
        {
            Vector3 start = transform.position;
            Vector3 end = pathPoints[currentIndex + 1].position;
            start.y = baseY;
            end.y = baseY;

            float t = 0f;
            float duration = 0.5f;
            float jumpHeight = 1.5f;

            while (t < 1f)
            {
                t += Time.deltaTime / duration;
                float yOffset = Mathf.Sin(Mathf.PI * t) * jumpHeight;
                transform.position = Vector3.Lerp(start, end, t) + Vector3.up * yOffset;
                yield return null;
            }

            transform.position = new Vector3(end.x, baseY, end.z);
            currentIndex++;
        }

        onMoveComplete?.Invoke(currentIndex);
    }
}