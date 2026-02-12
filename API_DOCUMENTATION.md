# API Documentation Summary

## 📍 Service Information
- **Base URL**: http://localhost:8086
- **Version**: 1.0
- **Status**: Production Ready

## 🎯 Complete API Endpoints List

### 1. Health & Status Monitoring
```
GET  /health                                 - Basic health check
GET  /api/health                            - Detailed health check with MT5 status
```

### 2. Live Account Management
```
GET    /api/mt5/accounts                    - Get all live accounts
GET    /api/mt5/account/{loginId}           - Get single account details
POST   /api/mt5/account/create              - Create new account
PUT    /api/mt5/account/update              - Update account details
DELETE /api/mt5/account/delete              - Delete account
POST   /api/LivePasswordChange              - Change account password
POST   /api/LivePasswordReset               - Reset account password
POST   /api/LiveLeverageUpdate              - Update account leverage
POST   /api/LiveGroupUpdate                 - Change account group
PUT    /api/LiveAccountStatus               - Enable/disable account
GET    /api/LiveOnlineUser                  - Get online users
POST   /api/LiveAccountDisable              - Disable account
```

### 3. Demo Account Management
```
GET    /api/DemoAccount                     - Get all demo accounts
POST   /api/DemoAccount/Create              - Create demo account
POST   /api/DemoAccountStatus               - Update demo account status
POST   /api/DemoGroupUpdate                 - Update demo group
```

### 4. Trading Operations
```
GET  /api/LiveTradingHistory/{loginId}      - Get trading history
GET  /api/OpenTradeDetail/{loginId}         - Get open positions
POST /api/CloseTrace/ClosePosition          - Close specific position
GET  /api/LiveDealHistory/{loginId}         - Get deal history
GET  /api/LiveTradingData/{loginId}         - Get real-time trading data
```

### 5. Liquidation & Risk Management
```
GET  /api/liquidation/MT5Liquidation?accountId={id}  - Check and liquidate accounts
```

### 6. Deposit & Withdrawal
```
POST /api/CreditInOut/Deposit               - Deposit funds
POST /api/CreditInOut/Withdraw              - Withdraw funds (internal)
POST /api/LiveWithdrawal/Withdraw           - Process withdrawal
POST /api/AccountTransfer                   - Transfer between accounts
```

### 7. Performance & Analytics
```
GET  /api/AccountPerformance/{loginId}      - Get performance metrics
GET  /api/Leaderboard                       - Get top performers
GET  /api/ProfitBySymbol/{loginId}          - Profit by symbol
GET  /api/LiveDashboard                     - Dashboard statistics
GET  /api/TradeAccountOverview              - Account overview
GET  /api/AccountProfitChartByDate          - Profit chart data
```

### 8. Group Management
```
GET  /api/Group/GetAllGroups                - List all groups
GET  /api/Group/GetGroup/{name}             - Get group details
POST /api/Group/CreateGroup                 - Create new group
PUT  /api/Group/UpdateGroup                 - Update group settings
```

### 9. Symbol Management
```
GET  /api/Symbol/GetAllSymbols              - List all symbols
GET  /api/Symbol/GetSymbol/{name}           - Get symbol details
POST /api/DemoSymbol/UpdateSymbol           - Update symbol configuration
```

### 10. User Account Details
```
GET  /api/UserAccountDetails/{loginId}      - Get detailed user account info
GET  /api/UserAccountGetByGroup             - Get accounts by group
POST /api/AccountAvailabilityCheck          - Check account availability
```

## 📝 Request/Response Examples

### Example 1: Create Live Account
**Request:**
```http
POST /api/mt5/account/create
Content-Type: application/json

{
  "userId": 12345,
  "name": "John Doe",
  "email": "john.doe@example.com",
  "group": "PropTrading\\Live",
  "leverage": 100,
  "balance": 10000.00
}
```

**Response:**
```json
{
  "success": true,
  "message": "Account created successfully",
  "data": {
    "login": 5551001,
    "password": "Abc123!@#",
    "investor": "Inv456!@#",
    "server": "PropTradingMT5",
    "group": "PropTrading\\Live"
  }
}
```

### Example 2: Get Account Details
**Request:**
```http
GET /api/mt5/account/1000
```

**Response:**
```json
{
  "success": true,
  "message": "Account retrieved successfully",
  "data": {
    "login": 1000,
    "name": "John Doe",
    "email": "john.doe@example.com",
    "group": "PropTrading\\Live",
    "balance": 10000.00,
    "equity": 10250.50,
    "margin": 500.00,
    "marginFree": 9750.50,
    "marginLevel": 2050.10,
    "leverage": 100,
    "credit": 0.00,
    "profit": 250.50,
    "lastUpdate": "2025-01-13T10:30:00Z"
  }
}
```

### Example 3: Deposit Funds
**Request:**
```http
POST /api/CreditInOut/Deposit
Content-Type: application/json

{
  "login": 1000,
  "amount": 1000.00,
  "comment": "Deposit via wire transfer"
}
```

**Response:**
```json
{
  "success": true,
  "message": "Deposit processed successfully",
  "data": {
    "deal": 456789,
    "login": 1000,
    "amount": 1000.00,
    "previousBalance": 10000.00,
    "newBalance": 11000.00,
    "timestamp": "2025-01-13T10:30:00Z"
  }
}
```

### Example 4: Close Position
**Request:**
```http
POST /api/CloseTrace/ClosePosition
Content-Type: application/json

{
  "login": 1000,
  "position": 987654,
  "volume": 1.0,
  "symbol": "EURUSD"
}
```

**Response:**
```json
{
  "success": true,
  "message": "Position closed successfully",
  "data": {
    "deal": 789012,
    "position": 987654,
    "profit": 150.00,
    "closePrice": 1.0965,
    "commission": -2.50,
    "swap": -0.50,
    "netProfit": 147.00
  }
}
```

### Example 5: Get Performance Metrics
**Request:**
```http
GET /api/AccountPerformance/1000
```

**Response:**
```json
{
  "success": true,
  "message": "Performance data retrieved",
  "data": {
    "login": 1000,
    "totalTrades": 150,
    "winningTrades": 95,
    "losingTrades": 55,
    "winRate": 63.33,
    "totalProfit": 5250.00,
    "totalLoss": -2100.00,
    "netProfit": 3150.00,
    "profitFactor": 2.5,
    "sharpeRatio": 1.85,
    "maxDrawdown": -850.00,
    "maxDrawdownPercent": -8.5,
    "averageWin": 55.26,
    "averageLoss": -38.18,
    "largestWin": 450.00,
    "largestLoss": -280.00,
    "consecutiveWins": 8,
    "consecutiveLosses": 3
  }
}
```

### Example 6: Check Liquidations
**Request:**
```http
GET /api/liquidation/MT5Liquidation
```

**Response:**
```json
{
  "success": true,
  "message": "Liquidation check completed",
  "data": {
    "accountsChecked": 150,
    "accountsLiquidated": 3,
    "accountsAtRisk": 5,
    "timestamp": "2025-01-13T10:30:00Z",
    "liquidatedAccounts": [
      {
        "login": 1005,
        "reason": "Margin level below threshold",
        "marginLevel": 25.5,
        "equity": 250.00
      }
    ]
  }
}
```

### Example 7: Get Leaderboard
**Request:**
```http
GET /api/Leaderboard?limit=10
```

**Response:**
```json
{
  "success": true,
  "message": "Leaderboard retrieved",
  "data": [
    {
      "rank": 1,
      "login": 1005,
      "name": "Top Trader",
      "profit": 12500.00,
      "returnPercentage": 125.00,
      "totalTrades": 250,
      "winRate": 75.2
    },
    {
      "rank": 2,
      "login": 1008,
      "name": "Second Best",
      "profit": 9800.00,
      "returnPercentage": 98.00,
      "totalTrades": 180,
      "winRate": 68.9
    }
  ]
}
```

## 🔐 Authentication & Security

### Headers
All requests should include:
```http
Content-Type: application/json
X-Api-Key: your-api-key (if enabled)
```

### Security Features
- ✅ CORS enabled and configurable
- ✅ Security headers (X-Frame-Options, X-Content-Type-Options, etc.)
- ✅ Request ID tracking (X-Request-Id header in response)
- ✅ Response time tracking (X-Response-Time header)
- ✅ Global exception handling
- ✅ Structured logging with Serilog

## 📊 Standard Response Format

### Success Response
```json
{
  "success": true,
  "message": "Operation completed successfully",
  "data": { ... }
}
```

### Error Response
```json
{
  "success": false,
  "message": "Error description",
  "errors": [
    "Detailed error 1",
    "Detailed error 2"
  ],
  "requestId": "guid-here",
  "timestamp": "2025-01-13T10:30:00Z"
}
```

## 🎯 Testing with cURL

```bash
# Health check
curl http://localhost:8086/health

# Get all accounts
curl http://localhost:8086/api/mt5/accounts

# Get specific account
curl http://localhost:8086/api/mt5/account/1000

# Create account
curl -X POST http://localhost:8086/api/mt5/account/create \
  -H "Content-Type: application/json" \
  -d '{"userId":12345,"name":"John Doe","email":"john@example.com"}'

# Deposit
curl -X POST http://localhost:8086/api/CreditInOut/Deposit \
  -H "Content-Type: application/json" \
  -d '{"login":1000,"amount":1000,"comment":"Test deposit"}'
```

## 🎯 Testing with Postman

1. Import the base URL: `http://localhost:8086`
2. Create requests for each endpoint
3. Use environment variables for common values
4. Save response examples for documentation

## 📱 Integration Examples

### JavaScript/TypeScript
```javascript
const baseUrl = 'http://localhost:8086';

// Get account
async function getAccount(loginId) {
  const response = await fetch(`${baseUrl}/api/mt5/account/${loginId}`);
  return await response.json();
}

// Create account
async function createAccount(userData) {
  const response = await fetch(`${baseUrl}/api/mt5/account/create`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(userData)
  });
  return await response.json();
}
```

### Python
```python
import requests

base_url = 'http://localhost:8086'

# Get account
def get_account(login_id):
    response = requests.get(f'{base_url}/api/mt5/account/{login_id}')
    return response.json()

# Create account
def create_account(user_data):
    response = requests.post(
        f'{base_url}/api/mt5/account/create',
        json=user_data
    )
    return response.json()
```

## 📄 Additional Resources

- Full documentation: http://localhost:8086
- Health check: http://localhost:8086/health
- API health: http://localhost:8086/api/health

---

**Last Updated**: January 13, 2025
**Version**: 1.0
**Status**: Production Ready
