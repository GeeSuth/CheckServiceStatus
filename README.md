
# Check Service Status

Check Service Status is a powerful C# console application that allows you to monitor the status of multiple services across different communication types. It provides a user-friendly interface to display service statuses in real-time.

![Check Service Status Screenshot](https://github.com/user-attachments/assets/c05c0d97-c404-4a8f-8873-1423b2c07206)

## Features

- **Multi-Service Support**: Monitor multiple services defined in a JSON configuration file.
- **Real-time Updates**: View service statuses in a live-updating console table.
- **Flexible Configuration**: Easily configure services with different communication types, paths, and success criteria.
- **HTTP Support**: Check the status of HTTP/HTTPS services, including GET and POST requests.
- **Custom Success Criteria**: Define success based on response codes or content.
- **Performance Metrics**: Display the time spent checking each service.
- **Error Handling**: Robust error handling with informative messages for each service check.

## Installation

1. Clone this repository:
   ```
   git clone https://github.com/GeeSuth/CheckServiceStatus
   ```
2. Navigate to the project directory:
   ```
   cd CheckServiceStatus
   ```
3. Build the project:
   ```
   dotnet build
   ```

## Usage

1. Configure your services in the `ServiceList.json` file.
2. Run the application:
   ```
   dotnet run
   ```
3. The application will display a table with the status of each configured service.

## Configuration

Services are configured in the `ServiceList.json` file. Each service can have the following properties:

- `ServiceName`: Name of the service
- `CommunicationType`: Type of communication (e.g., "Http")
- `ServicePath`: URL or path to the service
- `SuccessExpression`: Criteria for determining if the service is up
- `Timeout`: Maximum time (in seconds) to wait for a response
- `ServiceRequired`: Additional requirements for the service check (e.g., POST method and body)
- `Priority`: (Optional) Priority of the service
- `AuthenticationType`: (Optional) Type of authentication required
- `AuthenticationValue`: (Optional) Authentication credentials

Example configuration:

```json
[
  {
    "ServiceName": "API PRODUCTION",
    "CommunicationType": "Http",
    "ServicePath": "https://v1.com/check",
    "SuccessExpression": {
      "SuccessExpressionType": "ResponseCode",
      "SuccessValue": "200"
    },
    "Timeout": 30
  },
  // ... more services ...
]
```

## Dependencies

This project uses the following main dependencies:

- [Spectre.Console](https://github.com/spectreconsole/spectre.console): For creating beautiful console applications
- [System.Net.Http.Json](https://docs.microsoft.com/en-us/dotnet/api/system.net.http.json): For handling JSON in HTTP requests

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.


## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Acknowledgments

- Special thanks to the Spectre.Console team for their amazing library.

## Support

If you encounter any issues or have questions, please [open an issue](https://github.com/GeeSuth/CheckServiceStatus/issues) on GitHub.

## Roadmap


- Add notification system for service status changes

## Author

- **GeeSuth** - [GitHub Profile](https://github.com/GeeSuth)

---

Happy monitoring!