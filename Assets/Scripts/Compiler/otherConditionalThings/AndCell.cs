[System.Serializable]
public class AndCell
{
    public Commands GetCommand()
    {
        return Commands.AND;
    }
    
    public static bool Evaluate(bool v1, bool v2)
    {
        return v1 && v2;
    }
}
