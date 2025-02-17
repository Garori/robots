[System.Serializable]
public class OrCell
{
    public Commands GetCommand()
    {
        return Commands.OR;
    }

    public static bool Evaluate(bool v1, bool v2)
    {
        return v1 || v2;
    }
}
