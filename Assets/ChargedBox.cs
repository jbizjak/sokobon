using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ChargedBox : Pushable
{
    void Start(){
        ChargedObjs.Add(this);
        Pushables.Add(this);
    }

    public Vector2 ResolvePos(){
        // direction we would move based on adjacent charges
        Vector3 position = transform.position;
        Vector3 forces = Vector2.zero;
        foreach(var chargedObj in ChargedObjs){
            var relativePos = position - chargedObj.transform.position;
            if(relativePos.sqrMagnitude == 1 && charge == chargedObj.charge){
                // charged object directly adjacent
                forces += relativePos;
            }
        }
        return (Vector2)forces;
    }
}
