using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum VariableTarget
{
    Player,
    Enemy,
}

enum VariableType
{
    Health,
    Shield,
    Charge,
    MaxHealth,
}

enum VariableQuantity
{
    Current,
    Half,
    Double,
}

public class VariableSelector : MonoBehaviour
{
    [SerializeField] private Transform selectedVariable;
    private VariableTarget target;
    private VariableType type;
    private VariableQuantity quantity;
    private Transform[][][] variables;
    
    void Start()
    {
        target = VariableTarget.Player;
        type = VariableType.Health;
        quantity = VariableQuantity.Current;

        CreateMatrix();
        EnableSelectedVariable();
    }

    public void TargetButton(int target)
    {
        DisableCurrentVariable();
        this.target = (VariableTarget)target;
        EnableSelectedVariable();
    }

    public void TypeButton(int type)
    {
        DisableCurrentVariable();
        this.type = (VariableType)type;
        EnableSelectedVariable();
    }

    public void QuantityButton(int quantity)
    {
        DisableCurrentVariable();
        this.quantity = (VariableQuantity)quantity;
        EnableSelectedVariable();
    }

    private void CreateMatrix()
    {
        variables = new Transform[2][][];
        
        for (int i = 0; i < variables.Length; i++)
        {
            variables[i] = new Transform[4][];
            for (int j = 0; j < variables[i].Length; j++)
            {
                variables[i][j] = new Transform[3];
                for (int k = 0; k < variables[i][j].Length; k++)
                {
                    variables[i][j][k] = selectedVariable.GetChild(k + j * 3 + i * 12);
                }
            }
        }
    }

    private void DisableCurrentVariable()
    {
        variables[(int)target][(int)type][(int)quantity].gameObject.SetActive(false);
    }

    private void EnableSelectedVariable()
    {
        variables[(int)target][(int)type][(int)quantity].gameObject.SetActive(true);
    }
}
