using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public string enemyId = "kdsghfsdfoughsdfuo"; // must be unique for each enemy

    public void TakeDamage(float amount, int criticalDmgCode, Vector3 position, string sourceWeapon)
    {
        Debug.Log("Dealed "+amount+" damage with "+sourceWeapon+" from "+position+". critical code: "+criticalDmgCode);
    }
}
