using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DuckTypeManager : MonoBehaviour
{
    [SerializeField] private PlayerDuckDescriptionSO[] DuckTypes;

    public PlayerDuckDescriptionSO getTypeFromName(string typeName)
    {
        foreach(var type in DuckTypes)
        {
            if (type.Name == typeName)
                return type;
        }

        return new PlayerDuckDescriptionSO();
    }
    public PlayerDuckDescriptionSO getTypeFromIndex(int index)
    {
        return DuckTypes[index];
    }
}
