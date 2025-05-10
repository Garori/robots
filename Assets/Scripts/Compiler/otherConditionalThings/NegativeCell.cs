using System.Collections.Generic;

[System.Serializable]
public class NegativeCell
{
    // private Commands variable1;
    // private Commands variable2;

    // public NotCell(Commands variable1, Commands variable2) : base()
    // {
    //     this.variable1 = variable1;
    //     this.variable2 = variable2;
    // }

    public float Evaluate(float v1)
    {
        return v1 * -1;
    }

    public Commands GetCommand()
    {
        return Commands.NEGATIVE;
    }
}

