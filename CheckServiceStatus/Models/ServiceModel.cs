namespace CheckServiceStatus.Models;

public class ServiceModel
{
    public string? ServiceName { get; set; }
    public CommunicationType CommunicationType { get; set; }
    public string? ServicePath { get; set; }
    public SuccessExpression? SuccessExpression { get; set; }
    public Priority? Priority { get; set; }
    public AuthenticationType? AuthenticationType { get; set; }
    public string? AuthenticationValue { get; set; }
    public int? Timeout { get; set; }
}
public enum CommunicationType
{
    Http,
    Tcp,
    Udp,
    Icmp,
    Ssh,
    Telnet
}

public class SuccessExpression
{
    public SuccessExpressionType SuccessExpressionType { get; set; }
    public string? SuccessValue { get; set; }
}

public enum SuccessExpressionType
{
    ResponseCode,
    Regex,
    Contains,
    StartsWith,
    EndsWith
}

public enum Priority
{
    Critical,
    High,
    Medium,
    Low
}

public enum AuthenticationType
{
    None,
    Basic,
    Bearer,
    Digest,
    NTLM,
    OAuth
}


public class ServiceResponse
{
    public bool IsSuccess { get; set; }
    public string? ErrorMessage { get; set; }
}
