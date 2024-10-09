using CheckServiceStatus.Models;

namespace CheckServiceStatus.Services;

public class ServiceTalk
{

    public Task<bool> CheckServiceStatus(ServiceModel service)
    {
        Console.WriteLine($"Service Name: {service.ServiceName}, Communication Type: {service.CommunicationType}");
        switch (service.CommunicationType)
        {
            case CommunicationType.Http:
                return HttpServiceHelper.CheckHttpService(service);
            case CommunicationType.Tcp:
                return TcpServiceHelper.CheckTcpService(service);
            case CommunicationType.Udp:
                return UdpServiceHelper.CheckUdpService(service);
            case CommunicationType.Icmp:
                return IcmpServiceHelper.CheckIcmpService(service);
            case CommunicationType.Ssh:
                return SshServiceHelper.CheckSshService(service);
            case CommunicationType.Telnet:
                return TelnetServiceHelper.CheckTelnetService(service);
            default:
                throw new ArgumentException($"Unsupported communication type: {service.CommunicationType}");
        }
    }
}
