[System.Serializable]
public class ProgramNotInstantiated : System.Exception
{
    public ProgramNotInstantiated() { }
    public ProgramNotInstantiated(string message) : base("The Program class was not instantiated") { }
}