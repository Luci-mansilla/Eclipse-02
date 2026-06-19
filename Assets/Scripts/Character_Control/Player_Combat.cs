using UnityEngine;

public class Player_Combat : MonoBehaviour
{
    public Animator anim; 

    private Vector2 lastDirection;

   
    public void Attack()
    {
       Debug.Log("Attack called");

       if (anim == null)
       {
           Debug.LogError("Animator is NULL!");
           return;
       }

       anim.SetBool("IsAttacking", true);
    }

    public void FinishedAttacking()
    {
        anim.SetBool("IsAttacking", false);

    }




}
 