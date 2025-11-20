using UnityEngine;

public class ChairAdjustment : MonoBehaviour
{
    public bool chairisdown = false;
    public bool chairismovedforward = true;
    Animator animator;

    private void Start()
    {
       animator = GetComponent<Animator>();
    }
    public void MoveChairDown()
    {
        if (!chairisdown)
        {
            chairisdown = true;
            animator.Play("ChairMoveDown");
        }
    }

    public void MoveChairUp()
    {
        if (chairisdown)
        {
            animator.Play("ChairMoveUp");
            chairisdown = false;
        }
    }

    public void MoveChairBack()
    {
        if (chairismovedforward)
        {
            chairismovedforward = false;
            animator.Play("ChairMoveBack");
        }
    }

    public void MoveChairForward()
    {
        if (!chairismovedforward)
        {
            chairismovedforward = true;
            animator.Play("ChairMoveForward");
        }
    }
}
