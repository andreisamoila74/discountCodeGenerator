using DiscountCodeGenerator.Application.Services;
using System.Net.Sockets;
using System.Net;
using DiscountCodeGenerator.Server.Requests;
using DiscountCodeGenerator.Server.Responses;
using System.Text.Json;

namespace DiscountCodeGenerator.Server
{
    public class TcpHandler(DiscountCodeService discountCodeService)
    {
        private readonly DiscountCodeService _discountCodeService = discountCodeService;

        public async Task StartListeningAsync()
        {
            TcpListener listener = new(IPAddress.Any, 5000);
            listener.Start();
            Console.WriteLine("Server is listening on port 5000...");

            while (true)
            {
                var client = await listener.AcceptTcpClientAsync();
                _ = Task.Run(() => HandleClient(client));
            }
        }

        private async Task HandleClient(TcpClient client)
        {
            using (NetworkStream stream = client.GetStream())
            using (StreamReader reader = new StreamReader(stream))
            using (StreamWriter writer = new StreamWriter(stream) { AutoFlush = true })
            {
                try
                {
                    // Read request data from client
                    var requestData = await reader.ReadLineAsync();
                    Console.WriteLine($"Received request: {requestData}");

                    // Attempt to detect and process the request type
                    var jsonDocument = JsonDocument.Parse(requestData);
                    if (jsonDocument.RootElement.TryGetProperty("Count", out _))
                    {
                        // Handle Generate Request
                        var generateRequest = JsonSerializer.Deserialize<GenerateRequest>(requestData);
                        var codes = await _discountCodeService.GenerateDiscountCodesAsync(generateRequest.Count, generateRequest.Length);

                        var response = new GenerateResponse { Result = codes.Count > 0 };
                        var responseData = JsonSerializer.Serialize(response);
                        await writer.WriteLineAsync(responseData);
                        Console.WriteLine($"Response sent: {responseData}");
                    }
                    else if (jsonDocument.RootElement.TryGetProperty("Code", out _))
                    {
                        // Handle UseCode Request
                        var useCodeRequest = JsonSerializer.Deserialize<UseCodeRequest>(requestData);
                        var success = await _discountCodeService.UseDiscountCodeAsync(useCodeRequest.Code);

                        var response = new UseCodeResponse
                        {
                            Success = success
                        };
                        var responseData = JsonSerializer.Serialize(response);
                        await writer.WriteLineAsync(responseData);
                        Console.WriteLine($"Response sent: {responseData}");
                    }
                    else
                    {
                        throw new Exception("Invalid request type.");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                    await writer.WriteLineAsync(JsonSerializer.Serialize(new UseCodeResponse { Success = false }));
                }
                finally
                {
                    client.Close();
                }
            }
        }
    }
}