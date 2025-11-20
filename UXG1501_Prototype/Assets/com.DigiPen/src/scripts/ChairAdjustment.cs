using UnityEngine;

public class ChairAdjustment : MonoBehaviour
{
    public bool chairisdown = true;
    public bool chairismovedforward = true;
    Animator animator;
    public GameObject chairSeat;

    private void Start()
    {
       animator = GetComponent<Animator>();
    }
    public void MoveChairDown()
    {
        if (!chairisdown)
        {
            chairisdown = true;
            chairSeat.GetComponent<Animator>().Play("ChairMoveDown");
        }
    }

    public void MoveChairUp()
    {
        if (chairisdown)
        {
            chairSeat.GetComponent<Animator>().Play("ChairMoveUp");
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
