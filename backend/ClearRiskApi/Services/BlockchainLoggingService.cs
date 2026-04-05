using Nethereum.Web3;
using Nethereum.Web3.Accounts;
using Nethereum.Contracts;
using Microsoft.Extensions.Configuration;
using System.Numerics;
using System.Threading.Tasks;

namespace ClearRiskApi.Services
{
    public class BlockchainLoggingService
    {
        private readonly IConfiguration _config;

        private readonly string _rpcUrl;
        private readonly string _contractAddress;
        private readonly string _privateKey;

        private readonly Web3 _web3;

        // Contract ABI (matches Solidity EXACTLY)
        private const string ABI = @"[
        {
            ""inputs"": [
                { ""internalType"": ""string"", ""name"": ""tokenContractAddress"", ""type"": ""string"" },
                { ""internalType"": ""uint256"", ""name"": ""finalScore"", ""type"": ""uint256"" },
                { ""internalType"": ""string"", ""name"": ""riskTier"", ""type"": ""string"" },
                { ""internalType"": ""string"", ""name"": ""reportHash"", ""type"": ""string"" }
            ],
            ""name"": ""logAuditRecord"",
            ""outputs"": [],
            ""stateMutability"": ""nonpayable"",
            ""type"": ""function""
        }
        ]";

        public BlockchainLoggingService(IConfiguration config)
        {
            _config = config;

            _rpcUrl = config["Blockchain:RpcUrl"]
                ?? throw new Exception("RpcUrl not configured");

            _contractAddress = config["Blockchain:ContractAddress"]
                ?? throw new Exception("ContractAddress not configured");

            _privateKey = config["Blockchain:PrivateKey"]
                ?? throw new Exception("PrivateKey not configured");

            var account = new Account(_privateKey);
            _web3 = new Web3(account, _rpcUrl);
        }

        public async Task<string> LogAuditAsync(
            string tokenContractAddress,
            int riskScore,
            string riskTier,
            string reportHash)
        {
            var contract = _web3.Eth.GetContract(ABI, _contractAddress);
            var function = contract.GetFunction("logAuditRecord");

            // Send transaction and wait for confirmation
            var receipt = await function.SendTransactionAndWaitForReceiptAsync(
                from: _web3.TransactionManager.Account.Address,
                gas: new Nethereum.Hex.HexTypes.HexBigInteger(500000),
                value: null,
                functionInput: new object[]
                {
                    tokenContractAddress,
                    new BigInteger(riskScore),
                    riskTier,
                    reportHash
                }
            );

            return receipt.TransactionHash;
        }
    }
}