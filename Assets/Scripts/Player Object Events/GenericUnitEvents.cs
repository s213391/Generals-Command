using UnityEngine;

public class GenericUnitEvents : MonoBehaviour
{
    public void MoveBegin(GameObject go)
    {
        if (GameData.instance.isHost)
        {
            Animator animator = go.GetComponent<Animator>();
            if (animator)
                animator.SetBool("IsMoving", true);
        }
    }


    public void MoveEnd(GameObject go)
    {
        if (GameData.instance.isHost)
        {
            Animator animator = go.GetComponent<Animator>();
            if (animator)
                animator.SetBool("IsMoving", false);
        }
    }


    public void Attack(GameObject go)
    {
        if (GameData.instance.isHost)
        {
            Animator animator = go.GetComponent<Animator>();
            if (animator)
                animator.SetTrigger("Attacking");
        }
    }
}
