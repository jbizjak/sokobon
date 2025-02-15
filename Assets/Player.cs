using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Player : Pushable
{
    void Start()
    {
        player = this;        
        Obstacles = GameObject.FindGameObjectsWithTag("Obstacle");

        // Add this object to the list when it's created
        Pushables.Add(this);
    }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) // Move up
        {
            InputMove(Vector2.up);
        }
        else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow)) // Move down
        {
            InputMove(Vector2.down);
        }
        else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)) // Move left
        {
            InputMove(Vector2.left);
        }
        else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)) // Move right
        {
            InputMove(Vector2.right);
        }
    }

    public void InputMove(Vector2 direction){
        Move(direction);
        ResolveCharges();
    }

    public void ResolveCharges(){
        // charges have a "resistance"
        // which how hard it is to move the object
        // if two charges push on eachother the one with less resistance moves
        // unless both have same in which case both move
        
        // resistance is: inate resistance of object (1)
        // plus resistance of any object in the direction it would move
        // walls are 1000 (higher than anything else would get)
        
        int safety = 0;
        bool somethingChanged = true;
        while (somethingChanged){
            safety++;
            if(safety > 100){
                break;
            }
            somethingChanged = false;
            Dictionary<ChargedBox, (Vector2 vector, int resistance)> resolvedPositions = new Dictionary<ChargedBox, (Vector2, int)>();

            foreach(var chargedBox in ChargedObjs){
                //ChargedBox chargedBox = charged.GetComponent<ChargedBox>();
                Vector2 movement = chargedBox.ResolvePos();
                if(movement == Vector2.zero){
                    // dont bother doing calculations for objects not being pushed
                    continue;
                }

                //Push pushobj = chargedBox.GetComponent<Push>();

                // if a box is being pushed diagonally, resistance is lower of two direction values
                // and the push direction is set to that direction (vertical if tied)
                if(movement.x != 0 && movement.y != 0){
                    Debug.Log("diagonal push");
                    Vector2 vertVec = new Vector2(movement.x, 0);
                    Vector2 horizVec = new Vector2(0, movement.y);
                    int vertRes = chargedBox.Resistance(vertVec);
                    int horizRes = chargedBox.Resistance(horizVec);
                    if(vertRes <= horizRes){
                        resolvedPositions[chargedBox] = (vertVec, vertRes);
                    }else{
                        resolvedPositions[chargedBox] = (horizVec, horizRes);
                    }
                }else{
                    resolvedPositions[chargedBox] = (movement, chargedBox.Resistance(movement));

                }
            }
            


            // Dictionary to group ChargedBoxes by their resistance levels
            Dictionary<int, List<ChargedBox>> groupedByResistance = new Dictionary<int, List<ChargedBox>>();

            // Group ChargedBox objects by their resistance levels
            foreach (var entry in resolvedPositions)
            {
                ChargedBox chargedBox = entry.Key;
                //Push pushobj = chargedBox.GetComponent<Push>();

                int resistance = entry.Value.resistance;
                if (!groupedByResistance.ContainsKey(resistance))
                {
                    groupedByResistance[resistance] = new List<ChargedBox>();
                }
                groupedByResistance[resistance].Add(chargedBox);

            }

            // Sort the resistance levels in ascending order
            List<int> sortedResistanceLevels = groupedByResistance.Keys.Cast<int>().OrderBy(r => r).ToList();

            // Process ChargedBoxes group by group
            foreach (int resistance in sortedResistanceLevels)
            {
                List<ChargedBox> batch = groupedByResistance[resistance];

                // Apply the resolved positions (move ChargedBoxes)
                foreach (var chargedBox in batch)
                {
                    Vector2 resolvedPos = resolvedPositions[chargedBox].vector;

                    if(chargedBox.Move(resolvedPos)){
                        somethingChanged = true;
                    };
                }
                break;
            }
        }

    }
}
