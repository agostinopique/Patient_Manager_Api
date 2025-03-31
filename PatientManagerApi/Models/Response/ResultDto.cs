namespace PatientManagerApi.Models.Response;


public class ResultDto<T> : ResultDto
{
    public ResultDto(T data, int code, string message) : base(code, message)
    {
        Data = data;
    }

    public T Data { get; private set; }

    public void SetData(T data)
    {
        Data = data;
    }
}

public class ResultDto
{
    internal ResultDto(int code, string message)
    {
        Code = code;
        Message = message;
    }

    public int Code { get; }
    public string Message { get; }
    
}