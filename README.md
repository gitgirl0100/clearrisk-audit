
# ClearRisk Audit

ClearRisk Audit is a web-based smart contract risk evaluation platform designed to provide deterministic, transparent, and reproducible scoring of cryptocurrency token contracts. The system analyzes objective on-chain indicators and produces a structured risk score accompanied by a clear breakdown and blockchain-based audit verification.

---

## Project Purpose

Retail cryptocurrency participants often have access to raw blockchain data but lack structured tools to interpret that data in a meaningful way. ClearRisk Audit addresses this gap by implementing a deterministic scoring framework that evaluates measurable contract attributes and presents results in plain, understandable terms.

Unlike opaque or purely heuristic scoring systems, this project emphasizes:

- Transparency in scoring methodology  
- Reproducibility of results  
- Verifiable audit integrity  
- Structured risk interpretation  

This approach ensures that identical inputs produce identical outputs, reinforcing reliability and academic rigor.

---

## Core Features

- Deterministic risk scoring model (0–100 scale)  
- Weighted risk category breakdown  
- Structured risk tier classification (Low, Moderate, High, Extreme)  
- SHA-256 cryptographic report hashing  
- Blockchain-based audit logging (Ethereum Sepolia test network)  
- SQLite database persistence for audit records  
- Web-based user interface with expandable report history  
- Accessibility-aware UI with help and support section  

---

## Risk Scoring Model

The system evaluates five primary risk categories:

- Ownership Privilege Risk  
- Liquidity Risk  
- Distribution Concentration Risk  
- Code Transparency Risk  
- Activity / Behavior Risk  

Each category produces a normalized score between 0 and 1. The final risk score is calculated using a weighted formula:

Risk Score = (0.25O + 0.30L + 0.20D + 0.15C + 0.10A) × 100

Risk tiers are defined as:

- 0–25: Low  
- 26–50: Moderate  
- 51–75: High  
- 76–100: Extreme  

This classification provides clarity while maintaining analytical consistency.

---

## System Architecture

ClearRisk Audit is composed of three primary components:

- Frontend user interface (HTML, CSS, JavaScript)  
- Backend API (.NET 8 Minimal API)  
- Blockchain-based audit logging mechanism (Solidity smart contract via Nethereum)  

### Workflow

1. User submits a token contract address  
2. Backend evaluates contract risk factors  
3. Risk engine computes category scores  
4. Final score and breakdown are returned  
5. SHA-256 hash of the report is generated  
6. Audit record is stored in SQLite database  
7. Audit hash and metadata are recorded on blockchain  
8. Transaction hash is returned and persisted  

This layered design ensures modularity, separation of concerns, and maintainability.

---

## Technology Stack

### Backend
- .NET 8 Minimal API  
- Entity Framework Core  
- SQLite  

### Frontend
- HTML5  
- CSS3  
- JavaScript (Vanilla)  

### Blockchain
- Ethereum Sepolia Test Network  
- Solidity Smart Contract  
- Nethereum  

### Cryptography
- SHA-256 hashing (System.Security.Cryptography)

### Version Control
- Git  
- GitHub  

### Environment
- Windows 11  
- WSL2 (Ubuntu 22.04)

---

## Academic Context

This project was developed as part of:

**Bachelor of Science in Computer Science**  
**CS499 – Computer Science Capstone**  
**University of Arkansas at Grantham**

The project emphasizes structured design methodology, reproducibility, and technical integration consistent with upper-level computer science standards.

---

## Development Status

### Current Status (Week 7 Complete)

- Full backend API implemented  
- Deterministic risk scoring engine operational  
- SQLite database persistence verified  
- Blockchain audit logging integrated and tested  
- End-to-end workflow (API → DB → Blockchain) validated  
- Frontend interface completed with interactive report history  
- Accessibility and usability improvements implemented  

---

## Future Enhancements

- Real-time blockchain data integration  
- Expanded risk indicators  
- Advanced analytics and visualization  
- Production deployment  
- Smart contract mainnet deployment  
- Commercial scalability evaluation  

---

## Disclaimer

This application is intended for educational and informational purposes only.  
Risk scores and blockchain data do not constitute financial or investment advice.