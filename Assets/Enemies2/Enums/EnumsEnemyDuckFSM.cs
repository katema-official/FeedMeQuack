using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DuckEnemies
{
    public class EnemyDuckFSMEnumState
    {
        public enum State
        {
            HubState,           //state where the duck decides what's the new most important action to do.
            Chilling,           //where the duck stays still for some moments
            Roaming,            //the duck moves to a random point
            FoodSeen,          //the duck has seen some bread. It's an intermediate state that will decide what to do after seeing a bread
            Dashing,            //the duck is dashing directly towards the bread
            FoodSeeking,       //the duck is going after the bread following a path
            Bite,               //after dashing/breadSeeking, this state is used to check if the bread is still there
            Eating,             //the duck eats the bread
            StealingPassive,    //the duck is getting robbed by the player
            Chasing,            //the duck is chasing the player to steal bread from him
            TryStealActive,     //when the duck reaches the player, this state is used to check if it is possible to steal it or not
            StealingActive,     //when the duck starts stealing bread from the player
            Exiting             //the lake has ended, the duck can fly away
        }
    }
}