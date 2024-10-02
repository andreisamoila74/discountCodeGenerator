using System.Net.Sockets;
using System.Text.Json;

namespace DiscountCodeGenerator.Client
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            Console.WriteLine("Choose an operation:");
            Console.WriteLine("1 - Generate Discount Codes");
            Console.WriteLine("2 - Use Discount Code");
            var choice = Console.ReadLine();

            try
            {
                using (TcpClient client = new TcpClient("127.0.0.1", 5000)) // Connect to server on localhost and port 5000
                using (NetworkStream stream = client.GetStream())
                using (StreamWriter writer = new StreamWriter(stream) { AutoFlush = true })
                using (StreamReader reader = new StreamReader(stream))
                {
                    switch (choice)
                    {
                        case "1":
                            await SendGenerateRequest(writer, reader);
                            break;
                        case "2":
                            await SendUseCodeRequest(writer, reader);
                            break;
                        default:
                            Console.WriteLine("Invalid choice");
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        private static async Task SendGenerateRequest(StreamWriter writer, StreamReader reader)
        {
            Console.WriteLine("Enter the number of discount codes to generate:");
            var count = ushort.Parse(Console.ReadLine());

            Console.WriteLine("Enter the length of each discount code:");
            var length = byte.Parse(Console.ReadLine());

            // Create and send GenerateRequest
            var generateRequest = new GenerateRequest { Count = count, Length = length };
            var requestJson = JsonSerializer.Serialize(generateRequest);
            await writer.WriteLineAsync(requestJson);
            Console.WriteLine("Generate request sent to server.");

            // Read and process the response
            var responseJson = await reader.ReadLineAsync();
            var generateResponse = JsonSerializer.Deserialize<GenerateResponse>(responseJson);

            if (generateResponse.Result)
            {
                Console.WriteLine("Discount codes generated successfully.");
            }
            else
            {
                Console.WriteLine("Failed to generate discount codes.");
            }
        }

        private static async Task SendUseCodeRequest(StreamWriter writer, StreamReader reader)
        {
            Console.WriteLine("Enter the discount code to use:");
            var code = Console.ReadLine();

            // Create and send UseCodeRequest
            var useCodeRequest = new UseCodeRequest { Code = code };
            var requestJson = JsonSerializer.Serialize(useCodeRequest);
            await writer.WriteLineAsync(requestJson);
            Console.WriteLine("Use code request sent to server.");

            // Read and process the response
            var responseJson = await reader.ReadLineAsync();
            var useCodeResponse = JsonSerializer.Deserialize<UseCodeResponse>(responseJson);

            if (useCodeResponse.Success)
            {
                Console.WriteLine("Discount code used successfully.");
            }
            else
            {
                Console.WriteLine($"Failed to use discount code");
            }
        }
    }

    // Request/Response models
    public class GenerateRequest
    {
        public ushort Count { get; set; }
        public byte Length { get; set; }
    }

    public class GenerateResponse
    {
        public bool Result { get; set; }
    }

    public class UseCodeRequest
    {
        public required string Code { get; set; }
    }

    public class UseCodeResponse
    {
        public bool Success { get; set; }
    }
}
