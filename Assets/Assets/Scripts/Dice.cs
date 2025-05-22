using UnityEngine;

public class Dice : MonoBehaviour
{
    public int Roll()
    {
        return Random.Range(1, 7); // returns 1 to 6
    }
}
