using UnityEngine;
using UnityEngine.Animations;
using RTSModularSystem;

public class GenericUnitEvents : MonoBehaviour
{
    public void MoveBegin(GameObject go)
    {
        go.GetComponent<Animator>().SetBool("IsMoving", true);
    }


    public void MoveEnd(GameObject go)
    {
        go.GetComponent<Animator>().SetBool("IsMoving", false);
    }


    public void Attack(GameObject go)
    {
        go.GetComponent<Animator>().SetTrigger("Attacking");
    }
}
