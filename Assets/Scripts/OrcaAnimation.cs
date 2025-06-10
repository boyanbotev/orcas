using UnityEngine;

public class OrcaAnimation : MonoBehaviour
{
    Animator anim;

    void Awake()
    {
        anim = GetComponent<Animator>();
    }
    public void StartBoost()
    {
        anim.SetTrigger("boostStart");
    }

    public void StopBoost()
    {
        anim.SetTrigger("boostEnd");
    }
}
