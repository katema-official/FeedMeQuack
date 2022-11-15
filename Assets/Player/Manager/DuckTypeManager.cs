using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DuckTypeManager : MonoBehaviour
{
    [SerializeField] private DuckDescription[] DuckTypes;

    public DuckDescription getTypeFromName(string typeName)
    {
        foreach(var type in DuckTypes)
        {
            if (type.Name == typeName)
                return type;
        }

        return new DuckDescription();
    }
    public DuckDescription getTypeFromIndex(int index)
    {
        return DuckTypes[index];
    }
}
