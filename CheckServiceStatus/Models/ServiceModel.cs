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

    public CustomHeader[] customHeaders { get; set; } = new CustomHeader[0];
    public ServiceRequired? ServiceRequired { get; set; } = new ServiceRequired();

    public bool Enabled { get; set; } = true;
}

public class ServiceRequired
{
    public ServiceRequired()
    {
        this.CommunicationMethod = CommunicationMethod.Basic;
        this.RequiredValue = "";
    }
    public CommunicationMethod CommunicationMethod { get; set; }
    public string? RequiredValue { get; set; }
}

public class CustomHeader
{
    public string HeaderKey { get; set; } = "Header-X";
    public string? HeaderValue { get; set; }
}
public enum CommunicationMethod
{
    Basic,
    Get,
    Post,
    Options
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
