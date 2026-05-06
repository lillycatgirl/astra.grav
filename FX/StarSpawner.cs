using System.Collections.Generic;
using FX;
using UnityEngine;
using UnityEngine.Serialization;

public class StarSpawner : MonoBehaviour
{
    public int amount;

    public List<GameObject> stars;

    public Vector2 origin;

    public Vector2 size;

    public Vector2 speedChange;
    public Vector2 parallaxChange;
    private void Start()
    {
        for (int i = 0; i < amount; i++)
        {
            var position = origin + new Vector2(Random.Range(-size.x, size.x),  Random.Range(-size.y, size.y));
            var star = Instantiate(stars[Random.Range(0, stars.Count)], position, Quaternion.identity, transform);
            star.GetComponent<Animator>().speed = Random.Range(speedChange.x, speedChange.y);
            star.GetComponent<Animator>().SetFloat("Offset",  Random.Range(0.0f, 1.0f));
            star.GetComponent<BackgroundParallaxEffect>().parallaxEffectMultiplier = Random.Range(parallaxChange.x, parallaxChange.y);
        }
    }
}
