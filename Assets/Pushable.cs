using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Pushable : MonoBehaviour
{
    
    public static GameObject[] Obstacles;
    public static List<Pushable> Pushables = new List<Pushable>();
    public static List<ChargedBox> ChargedObjs = new List<ChargedBox>();
    public static Player player;
    public int charge = 0;
    public Vector2 north = new Vector2(0, 0);


    void Start()
    {
        // Add this object to the list when it's created
        Pushables.Add(this);
    }

    void Update()
    {

    }

    public bool Move(Vector2 direction){
        HashSet<Pushable> visited = new();
        return NoRepeatMove(direction, visited);
    }
    public bool NoRepeatMove(Vector2 direction, HashSet<Pushable> visited){
        if(visited.Contains(this)){
            return true;
        }
        visited.Add(this);

        if(!MovePushablesIfPossible(transform.position, direction, visited)){
            return false;
        }
        transform.Translate(direction);
        return true;
    }
    public bool MovePushablesIfPossible(Vector3 position, Vector2 direction, HashSet<Pushable> visited){
        // return true if we are able to move in target direction, false otherwise
        Vector2 newpos = (Vector2)transform.position + direction;
        Vector2 leftPos = (Vector2)transform.position + new Vector2(-direction.y, direction.x);
        Vector2 rightPos = (Vector2)transform.position + new Vector2(direction.y, -direction.x);


        foreach(var obstacle in Obstacles){
            if(newpos == (Vector2)obstacle.transform.position){
                return false;
            }
        }

        Vector2 newPosAdjacent = newpos + direction;
        foreach(var pushable in Pushables){
            
            if(newpos == (Vector2)pushable.transform.position){
                if(pushable.NoRepeatMove(direction, visited)){
                    continue;
                }else{
                    return false;
                }
            }

            if(newPosAdjacent == (Vector2)pushable.transform.position){
                if(charge != 0 && pushable.charge == charge){
                    Debug.Log("Blocked by a charge");
                    if(pushable.NoRepeatMove(direction, visited)){
                        continue;
                    }else{
                        return false; //should we not move if we cannot push?
                    }
                }
            }
        }
        // if we reached here we were able to move, now move things next to us if we can
        foreach(var pushable in Pushables){
            var relativePos = position - pushable.transform.position;
            if(relativePos.sqrMagnitude == 1 && charge != 0 && charge == -1*pushable.charge){
                    if(pushable.NoRepeatMove(direction, visited)){
                        continue;
                    }else{
                        continue; //false; //should we not move if we cannot push?
                    }
            }
        }   

        return true;
    }

    public int Resistance(Vector2 direction){
        Vector2 newpos = (Vector2)transform.position + direction;
        foreach(var obstacle in Obstacles){
            if(newpos == (Vector2)obstacle.transform.position){
                return 1000;
            }
        }

        foreach(var pushable in Pushables){
            if(pushable == gameObject || pushable == player){
                continue;
            }
            if(newpos == (Vector2)pushable.transform.position){

                return 1 + pushable.Resistance(direction);
            }


            Vector2 newPosAdjacent = newpos + direction;
            if(newPosAdjacent == (Vector2)pushable.transform.position){
                if(charge != 0){
                    if(pushable.charge == charge){
                        return 1 + pushable.Resistance(direction);

                    }
                }
            }

        }

        if(newpos == (Vector2)player.transform.position){
            return 500;
        }
        return 1;
    }
}
