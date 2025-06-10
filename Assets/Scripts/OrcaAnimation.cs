using UnityEngine;

public class OrcaAnimation : MonoBehaviour
{
    Animator anim;
    BubblesController bubbles;

    void Awake()
    {
        anim = GetComponent<Animator>();
        bubbles = GetComponentInChildren<BubblesController>();
        bubbles.SetIdle();
    }
    public void StartBoost()
    {
        anim.SetTrigger("boostStart");
        bubbles.SetActive();
    }

    public void StopBoost()
    {
        anim.SetTrigger("boostEnd");
        bubbles.SetIdle();
    }
}
