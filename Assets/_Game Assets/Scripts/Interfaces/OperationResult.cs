public class OperationResult
{
    public bool success;
    public string message;

    public OperationResult(bool success, string message = "Success!")
    {
        this.success = success;
        this.message = message;
    }
}