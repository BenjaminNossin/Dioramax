using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableEntityRemote : InteractableEntity
{
    [SerializeField] private List<GameObject> controlledEntities = new(); 
}
