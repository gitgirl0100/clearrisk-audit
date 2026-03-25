using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace ClearRiskApi.Services
{
    public class BlockchainValidationService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<BlockchainValidationService> _logger;

        public BlockchainValidationService(
            HttpClient httpClient,
            IConfiguration configuration,
            ILogger<BlockchainValidationService> logger)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<BlockchainValidationResult> ValidateContractExistsAsync(string contractAddress)
        {
            string? rpcUrl = _configuration["Blockchain:RpcUrl"];

            if (string.IsNullOrWhiteSpace(rpcUrl))
            {
                return new BlockchainValidationResult
                {
                    IsSuccess = false,
                    ContractExists = false,
                    ErrorMessage = "Blockchain RPC URL is not configured."
                };
            }

            var requestBody = new JsonRpcRequest
            {
                JsonRpc = "2.0",
                Method = "eth_getCode",
                Params = new object[] { contractAddress, "latest" },
                Id = 1
            };

            try
            {
                using var response = await _httpClient.PostAsJsonAsync(rpcUrl, requestBody);

                if (!response.IsSuccessStatusCode)
                {
                    string responseText = await response.Content.ReadAsStringAsync();

                    _logger.LogWarning(
                        "Blockchain RPC returned non-success status {StatusCode}. Response: {ResponseText}",
                        response.StatusCode,
                        responseText);

                    return new BlockchainValidationResult
                    {
                        IsSuccess = false,
                        ContractExists = false,
                        ErrorMessage = "Unable to verify contract on the blockchain at this time."
                    };
                }

                var rpcResponse = await response.Content.ReadFromJsonAsync<JsonRpcResponse>();

                if (rpcResponse is null)
                {
                    return new BlockchainValidationResult
                    {
                        IsSuccess = false,
                        ContractExists = false,
                        ErrorMessage = "Blockchain RPC returned an empty response."
                    };
                }

                if (rpcResponse.Error is not null)
                {
                    _logger.LogWarning(
                        "Blockchain RPC error {Code}: {Message}",
                        rpcResponse.Error.Code,
                        rpcResponse.Error.Message);

                    return new BlockchainValidationResult
                    {
                        IsSuccess = false,
                        ContractExists = false,
                        ErrorMessage = $"Blockchain RPC error: {rpcResponse.Error.Message}"
                    };
                }

                string code = rpcResponse.Result ?? "0x";
                bool contractExists = !string.Equals(code, "0x", StringComparison.OrdinalIgnoreCase);

                return new BlockchainValidationResult
                {
                    IsSuccess = true,
                    ContractExists = contractExists,
                    ErrorMessage = contractExists
                        ? null
                        : "Address format is valid, but no deployed smart contract was found at this address."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while validating contract existence on blockchain.");

                return new BlockchainValidationResult
                {
                    IsSuccess = false,
                    ContractExists = false,
                    ErrorMessage = "An error occurred while contacting the blockchain validation service."
                };
            }
        }

        private class JsonRpcRequest
        {
            [JsonPropertyName("jsonrpc")]
            public string JsonRpc { get; set; } = "2.0";

            [JsonPropertyName("method")]
            public string Method { get; set; } = string.Empty;

            [JsonPropertyName("params")]
            public object[] Params { get; set; } = Array.Empty<object>();

            [JsonPropertyName("id")]
            public int Id { get; set; }
        }

        private class JsonRpcResponse
        {
            [JsonPropertyName("jsonrpc")]
            public string? JsonRpc { get; set; }

            [JsonPropertyName("id")]
            public int Id { get; set; }

            [JsonPropertyName("result")]
            public string? Result { get; set; }

            [JsonPropertyName("error")]
            public JsonRpcError? Error { get; set; }
        }

        private class JsonRpcError
        {
            [JsonPropertyName("code")]
            public int Code { get; set; }

            [JsonPropertyName("message")]
            public string Message { get; set; } = string.Empty;
        }
    }

    public class BlockchainValidationResult
    {
        public bool IsSuccess { get; set; }
        public bool ContractExists { get; set; }
        public string? ErrorMessage { get; set; }
    }
}