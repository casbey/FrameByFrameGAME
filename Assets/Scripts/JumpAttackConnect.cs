using UnityEngine;

public class JumpAttackConnect : MonoBehaviour
{
    public PlayerShooting playershooting;
    public GameObject player;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playershooting = player.GetComponent<PlayerShooting>();
    }

    public void DisableJumpAttackHitboxA()
    {
        playershooting.DisableJumpAttackHitbox();
    }

}
