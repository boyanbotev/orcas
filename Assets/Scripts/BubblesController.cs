using UnityEngine;

public class BubblesController : MonoBehaviour
{
    ParticleSystem bubbles;
    float defaultEmissionRate;

    private void Awake()
    {
        bubbles = GetComponent<ParticleSystem>();
        defaultEmissionRate = bubbles.emissionRate;
    }

    public void SetActive()
    {
        bubbles.emissionRate = defaultEmissionRate;
    }

    public void SetIdle()
    {
        bubbles.emissionRate = 1f;
    }
}
