ClearRisk Audit

ClearRisk Audit is a web-based smart contract risk evaluation platform designed to provide deterministic, transparent, and reproducible scoring of cryptocurrency token contracts. The system analyzes objective on-chain indicators and produces a structured risk score accompanied by a clear breakdown and blockchain-based audit verification.

Project Purpose
Retail cryptocurrency participants often have access to raw blockchain data but lack structured tools to interpret that data in a meaningful way. ClearRisk Audit addresses this gap by implementing a deterministic scoring framework that evaluates measurable contract attributes and presents results in plain, understandable terms.

Unlike opaque or purely heuristic scoring systems, this project emphasizes:
Transparency in scoring methodology
Reproducibility of results
Verifiable audit integrity
Structured risk interpretation

This approach ensures that identical inputs produce identical outputs, reinforcing reliability and academic rigor.

Core Features
Deterministic risk scoring model (0–100 scale)
Weighted risk category breakdown
Structured risk tier classification (Low, Moderate, High, Extreme)
SHA-256 cryptographic report hashing
Blockchain-based audit log (test network)
Web-based user interface
Risk Scoring Model

The system evaluates five primary risk categories:
Ownership Privilege Risk
Liquidity Risk
Distribution Concentration Risk
Code Transparency Risk
Activity / Behavior Risk

Each category produces a normalized score between 0 and 1. The final risk score is calculated using a weighted formula:

Risk Score = (0.25O + 0.30L + 0.20D + 0.15C + 0.10A) × 100

Risk tiers are defined as:
0–25: Low
26–50: Moderate
51–75: High
76–100: Extreme

This classification provides clarity while maintaining analytical consistency.

System Architecture

ClearRisk Audit is composed of three primary components:
Frontend user interface (HTML, CSS, JavaScript)
Backend API (.NET Minimal API)
Blockchain-based audit logging mechanism

Workflow:
User submits a token contract address.
Backend retrieves objective contract data.
Risk engine computes category scores.
Final score and breakdown are returned.
SHA-256 hash of the report is generated.
Hash is recorded on a blockchain test network for verification.
This layered design ensures modularity, separation of concerns, and maintainability.
Technology Stack

Backend:
.NET 8 Minimal API

Frontend:
HTML5
CSS3
JavaScript (Vanilla)

Blockchain:
Ethereum test network
Minimal Solidity contract (for audit hash storage)

Cryptography:
SHA-256 hashing (System.Security.Cryptography)

Version Control:
Git
GitHub

Environment:
Windows 11
WSL2 (Ubuntu 22.04)

Academic Context

This project was developed as part of:

Bachelor of Science in Computer Science
CS499 – Computer Science Capstone
University of Arkansas at Grantham

The project emphasizes structured design methodology, reproducibility, and technical integration consistent with upper-level computer science standards.

Development Status

Current Phase:
Repository initialized
Architecture defined
Backend API in development
Frontend prototype pending integration

Future Enhancements:
Real-time blockchain data integration
Expanded risk indicators
Historical score tracking
Production deployment
Commercial scalability evaluation