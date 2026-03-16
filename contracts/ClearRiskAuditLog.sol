// SPDX-License-Identifier: MIT
pragma solidity ^0.8.20;

contract ClearRiskAuditLog {
    struct AuditRecord {
        uint256 recordId;
        address submittedBy;
        string tokenContractAddress;
        uint256 finalScore;
        string riskTier;
        string reportHash;
        uint256 timestamp;
    }

    uint256 private nextRecordId = 1;

    mapping(uint256 => AuditRecord) public auditRecords;

    event AuditRecordLogged(
        uint256 indexed recordId,
        address indexed submittedBy,
        string tokenContractAddress,
        uint256 finalScore,
        string riskTier,
        string reportHash,
        uint256 timestamp
    );

    function logAuditRecord(
        string memory tokenContractAddress,
        uint256 finalScore,
        string memory riskTier,
        string memory reportHash
    ) public {
        auditRecords[nextRecordId] = AuditRecord({
            recordId: nextRecordId,
            submittedBy: msg.sender,
            tokenContractAddress: tokenContractAddress,
            finalScore: finalScore,
            riskTier: riskTier,
            reportHash: reportHash,
            timestamp: block.timestamp
        });

        emit AuditRecordLogged(
            nextRecordId,
            msg.sender,
            tokenContractAddress,
            finalScore,
            riskTier,
            reportHash,
            block.timestamp
        );

        nextRecordId++;
    }

    function getAuditRecord(uint256 recordId) public view returns (AuditRecord memory) {
        return auditRecords[recordId];
    }
}