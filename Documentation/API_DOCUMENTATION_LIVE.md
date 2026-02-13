# Prop MT5 Connection Service - Live API Documentation

**Version:** 1.0.0  
**Base URL:** `http://localhost:8086`  
**Framework:** ASP.NET Web API (.NET Framework 4.8)  
**Author:** Amit Kumar

---

## Table of Contents

1. [Account Management APIs](#account-management-apis)
2. [Trading Operations APIs](#trading-operations-apis)
3. [Financial Operations APIs](#financial-operations-apis)
4. [Reporting & Analytics APIs](#reporting--analytics-apis)
5. [System Management APIs](#system-management-apis)
6. [Common Models](#common-models)

---

## Account Management APIs

### 1. Account Availability Check

**Endpoint:** `POST /api/mt5/account-availability/deleted`  
**Description:** Check if deleted accounts are available for reuse  
**Controller:** AccountAvailabilityCheckController

**Request Body:**
```json
{
  "loginIds": [5550001, 5550002, 5550003]
}
```

**Request Model:**
```csharp
public class AccountAvailabilityRequest
{
    public List<long> LoginIds { get; set; }
}
```

**Response (200 OK):**
```json
{
  "success": true,
  "message": "Account availability checked successfully",
  "data": {
    "availableAccounts": [5550001, 5550003],
    "unavailableAccounts": [5550002]
  }
}
```

**Response Model:**
```csharp
public class AccountAvailabilityResponse
{
    public bool Success { get; set; }
    public string Message { get; set; }
    public AccountAvailabilityData Data { get; set; }
}

public class AccountAvailabilityData
{
    public List<long> AvailableAccounts { get; set; }
    public List<long> UnavailableAccounts { get; set; }
}
```

---

### 2. Live User Account Creation

**Endpoint:** `POST /api/mt5/live-user-account`  
**Description:** Create a new live trading account  
**Controller:** LiveUserAccountController

**Request Body:**
```json
{
  "name": "John Doe",
  "email": "john.doe@example.com",
  "group": "demo\\standard",
  "leverage": 100,
  "balance": 10000.00,
  "phone": "+1234567890",
  "city": "New York",
  "state": "NY",
  "country": "USA",
  "zipCode": "10001",
  "address": "123 Main St"
}
```

**Request Model:**
```csharp
public class CreateAccountRequest
{
    public string Name { get; set; }
    public string Email { get; set; }
    public string Group { get; set; }
    public int Leverage { get; set; }
    public decimal Balance { get; set; }
    public string Phone { get; set; }
    public string City { get; set; }
    public string State { get; set; }
    public string Country { get; set; }
    public string ZipCode { get; set; }
    public string Address { get; set; }
}
```

**Response (200 OK):**
```json
{
  "success": true,
  "message": "Account created successfully",
  "data": {
    "login": 5550001,
    "name": "John Doe",
    "email": "john.doe@example.com",
    "group": "demo\\standard",
    "leverage": 100,
    "balance": 10000.00,
    "password": "TempPass123!",
    "passwordInvestor": "InvPass123!"
  }
}
```

**Response Model:**
```csharp
public class CreateAccountResponse
{
    public bool Success { get; set; }
    public string Message { get; set; }
    public AccountData Data { get; set; }
}

public class AccountData
{
    public long Login { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string Group { get; set; }
    public int Leverage { get; set; }
    public decimal Balance { get; set; }
    public string Password { get; set; }
    public string PasswordInvestor { get; set; }
}
```

---

### 3. Live User Account - Batch Creation

**Endpoint:** `POST /api/mt5/live-user-account/batch`  
**Description:** Create multiple accounts in a single request  
**Controller:** LiveUserAccountBatchController

**Request Body:**
```json
{
  "accounts": [
    {
      "name": "John Doe",
      "email": "john@example.com",
      "group": "demo\\standard",
      "leverage": 100,
      "balance": 10000.00
    },
    {
      "name": "Jane Smith",
      "email": "jane@example.com",
      "group": "demo\\premium",
      "leverage": 200,
      "balance": 25000.00
    }
  ]
}
```

**Request Model:**
```csharp
public class BatchAccountRequest
{
    public List<CreateAccountRequest> Accounts { get; set; }
}
```

**Response (200 OK):**
```json
{
  "success": true,
  "message": "Batch account creation completed",
  "data": {
    "successfulAccounts": [
      {
        "login": 5550001,
        "name": "John Doe",
        "email": "john@example.com"
      }
    ],
    "failedAccounts": [
      {
        "name": "Jane Smith",
        "email": "jane@example.com",
        "error": "Email already exists"
      }
    ],
    "totalProcessed": 2,
    "successCount": 1,
    "failureCount": 1
  }
}
```

---

### 4. Account Details

**Endpoint:** `GET /api/mt5/account/{loginId}`  
**Description:** Get detailed information about a specific account  
**Controller:** UserAccountDetailsController

**Path Parameters:**
- `loginId` (long) - The MT5 account login ID

**Response (200 OK):**
```json
{
  "success": true,
  "message": "Account details retrieved successfully",
  "data": {
    "login": 5550001,
    "name": "John Doe",
    "email": "john.doe@example.com",
    "group": "demo\\standard",
    "leverage": 100,
    "balance": 10000.00,
    "equity": 10250.00,
    "margin": 500.00,
    "marginFree": 9750.00,
    "marginLevel": 2050.00,
    "credit": 0.00,
    "profit": 250.00,
    "storage": 0.00,
    "enabled": true,
    "readOnly": false,
    "registrationTime": "2025-01-10T12:00:00Z",
    "lastAccessTime": "2025-01-15T14:30:00Z"
  }
}
```

**Response Model:**
```csharp
public class AccountDetailsResponse
{
    public bool Success { get; set; }
    public string Message { get; set; }
    public AccountDetails Data { get; set; }
}

public class AccountDetails
{
    public long Login { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string Group { get; set; }
    public int Leverage { get; set; }
    public decimal Balance { get; set; }
    public decimal Equity { get; set; }
    public decimal Margin { get; set; }
    public decimal MarginFree { get; set; }
    public decimal MarginLevel { get; set; }
    public decimal Credit { get; set; }
    public decimal Profit { get; set; }
    public decimal Storage { get; set; }
    public bool Enabled { get; set; }
    public bool ReadOnly { get; set; }
    public DateTime RegistrationTime { get; set; }
    public DateTime LastAccessTime { get; set; }
}
```

---

### 5. Account Status Update

**Endpoint:** `PUT /api/mt5/account/{loginId}/status`  
**Description:** Enable or disable a trading account  
**Controller:** LiveAccountStatusController

**Path Parameters:**
- `loginId` (long) - The MT5 account login ID

**Request Body:**
```json
{
  "enabled": false,
  "readOnly": true
}
```

**Request Model:**
```csharp
public class AccountStatusRequest
{
    public bool Enabled { get; set; }
    public bool ReadOnly { get; set; }
}
```

**Response (200 OK):**
```json
{
  "success": true,
  "message": "Account status updated successfully",
  "data": {
    "login": 5550001,
    "enabled": false,
    "readOnly": true
  }
}
```

---

### 6. Account Disable

**Endpoint:** `POST /api/mt5/account/{loginId}/disable`  
**Description:** Disable a trading account  
**Controller:** LiveAccountDisableController

**Path Parameters:**
- `loginId` (long) - The MT5 account login ID

**Response (200 OK):**
```json
{
  "success": true,
  "message": "Account disabled successfully",
  "data": {
    "login": 5550001,
    "enabled": false
  }
}
```

---

### 7. Account Deletion

**Endpoint:** `DELETE /api/mt5/account/{loginId}`  
**Description:** Delete a trading account (soft delete)  
**Controller:** LiveAccountDeleteController

**Path Parameters:**
- `loginId` (long) - The MT5 account login ID

**Response (200 OK):**
```json
{
  "success": true,
  "message": "Account deleted successfully",
  "data": {
    "login": 5550001,
    "deletedAt": "2025-01-15T14:30:00Z"
  }
}
```

---

### 8. Password Change

**Endpoint:** `PUT /api/mt5/account/{loginId}/password`  
**Description:** Change account password  
**Controller:** LivePasswordChangeController

**Path Parameters:**
- `loginId` (long) - The MT5 account login ID

**Request Body:**
```json
{
  "currentPassword": "OldPass123!",
  "newPassword": "NewPass456!",
  "passwordType": "main"
}
```

**Request Model:**
```csharp
public class PasswordChangeRequest
{
    public string CurrentPassword { get; set; }
    public string NewPassword { get; set; }
    public string PasswordType { get; set; } // "main" or "investor"
}
```

**Response (200 OK):**
```json
{
  "success": true,
  "message": "Password changed successfully",
  "data": {
    "login": 5550001,
    "passwordType": "main",
    "changedAt": "2025-01-15T14:30:00Z"
  }
}
```

---

### 9. Password Reset

**Endpoint:** `POST /api/mt5/account/{loginId}/password/reset`  
**Description:** Reset account password  
**Controller:** LivePasswordResetController

**Path Parameters:**
- `loginId` (long) - The MT5 account login ID

**Request Body:**
```json
{
  "passwordType": "main"
}
```

**Response (200 OK):**
```json
{
  "success": true,
  "message": "Password reset successfully",
  "data": {
    "login": 5550001,
    "newPassword": "ResetPass789!",
    "passwordType": "main"
  }
}
```

---

### 10. Leverage Update

**Endpoint:** `PUT /api/mt5/account/{loginId}/leverage`  
**Description:** Update account leverage  
**Controller:** LiveLeverageUpdateController

**Path Parameters:**
- `loginId` (long) - The MT5 account login ID

**Request Body:**
```json
{
  "leverage": 200
}
```

**Request Model:**
```csharp
public class LeverageUpdateRequest
{
    public int Leverage { get; set; }
}
```

**Response (200 OK):**
```json
{
  "success": true,
  "message": "Leverage updated successfully",
  "data": {
    "login": 5550001,
    "leverage": 200,
    "updatedAt": "2025-01-15T14:30:00Z"
  }
}
```

---

### 11. Group Update

**Endpoint:** `PUT /api/mt5/account/{loginId}/group`  
**Description:** Move account to a different group  
**Controller:** LiveGroupUpdateController

**Path Parameters:**
- `loginId` (long) - The MT5 account login ID

**Request Body:**
```json
{
  "group": "demo\\premium"
}
```

**Request Model:**
```csharp
public class GroupUpdateRequest
{
    public string Group { get; set; }
}
```

**Response (200 OK):**
```json
{
  "success": true,
  "message": "Group updated successfully",
  "data": {
    "login": 5550001,
    "oldGroup": "demo\\standard",
    "newGroup": "demo\\premium",
    "updatedAt": "2025-01-15T14:30:00Z"
  }
}
```

---

### 12. Get Accounts by Group

**Endpoint:** `GET /api/mt5/accounts/group/{groupName}`  
**Description:** Get all accounts in a specific group  
**Controller:** UserAccountGetByGroupController

**Path Parameters:**
- `groupName` (string) - The group name (e.g., "demo\\standard")

**Response (200 OK):**
```json
{
  "success": true,
  "message": "Accounts retrieved successfully",
  "data": {
    "group": "demo\\standard",
    "totalAccounts": 150,
    "accounts": [
      {
        "login": 5550001,
        "name": "John Doe",
        "balance": 10000.00,
        "equity": 10250.00,
        "enabled": true
      }
    ]
  }
}
```

---

## Trading Operations APIs

### 13. Open Position

**Endpoint:** `POST /api/mt5/position/open`  
**Description:** Open a new trading position  
**Controller:** PositionController

**Request Body:**
```json
{
  "login": 5550001,
  "symbol": "EURUSD",
  "volume": 1.0,
  "action": "BUY",
  "price": 1.08500,
  "stopLoss": 1.08000,
  "takeProfit": 1.09000,
  "comment": "Opening long position"
}
```

**Request Model:**
```csharp
public class OpenPositionRequest
{
    public long Login { get; set; }
    public string Symbol { get; set; }
    public double Volume { get; set; }
    public string Action { get; set; } // "BUY" or "SELL"
    public double Price { get; set; }
    public double StopLoss { get; set; }
    public double TakeProfit { get; set; }
    public string Comment { get; set; }
}
```

**Response (200 OK):**
```json
{
  "success": true,
  "message": "Position opened successfully",
  "data": {
    "positionId": 123456789,
    "login": 5550001,
    "symbol": "EURUSD",
    "volume": 1.0,
    "action": "BUY",
    "openPrice": 1.08500,
    "openTime": "2025-01-15T14:30:00Z",
    "stopLoss": 1.08000,
    "takeProfit": 1.09000
  }
}
```

---

### 14. Close Position

**Endpoint:** `POST /api/mt5/position/close`  
**Description:** Close an existing trading position  
**Controller:** CloseTradeController

**Request Body:**
```json
{
  "login": 5550001,
  "positionId": 123456789,
  "volume": 1.0,
  "price": 1.08750
}
```

**Request Model:**
```csharp
public class ClosePositionRequest
{
    public long Login { get; set; }
    public long PositionId { get; set; }
    public double Volume { get; set; }
    public double Price { get; set; }
}
```

**Response (200 OK):**
```json
{
  "success": true,
  "message": "Position closed successfully",
  "data": {
    "positionId": 123456789,
    "login": 5550001,
    "symbol": "EURUSD",
    "openPrice": 1.08500,
    "closePrice": 1.08750,
    "profit": 25.00,
    "closedAt": "2025-01-15T15:00:00Z"
  }
}
```

---

### 15. Get Positions

**Endpoint:** `GET /api/mt5/positions/{loginId}`  
**Description:** Get all open positions for an account  
**Controller:** PositionController

**Path Parameters:**
- `loginId` (long) - The MT5 account login ID

**Response (200 OK):**
```json
{
  "success": true,
  "message": "Positions retrieved successfully",
  "data": {
    "login": 5550001,
    "totalPositions": 3,
    "positions": [
      {
        "positionId": 123456789,
        "symbol": "EURUSD",
        "volume": 1.0,
        "action": "BUY",
        "openPrice": 1.08500,
        "currentPrice": 1.08750,
        "profit": 25.00,
        "stopLoss": 1.08000,
        "takeProfit": 1.09000,
        "openTime": "2025-01-15T14:30:00Z"
      }
    ]
  }
}
```

---

### 16. Get Position Details

**Endpoint:** `GET /api/mt5/positions/position/{positionId}`  
**Description:** Get detailed information about a specific position  
**Controller:** PositionController

**Path Parameters:**
- `positionId` (long) - The position ID

**Response (200 OK):**
```json
{
  "success": true,
  "message": "Position details retrieved successfully",
  "data": {
    "positionId": 123456789,
    "login": 5550001,
    "symbol": "EURUSD",
    "volume": 1.0,
    "action": "BUY",
    "openPrice": 1.08500,
    "currentPrice": 1.08750,
    "profit": 25.00,
    "swap": 0.50,
    "commission": 2.00,
    "stopLoss": 1.08000,
    "takeProfit": 1.09000,
    "openTime": "2025-01-15T14:30:00Z"
  }
}
```

---

### 17. Create Order

**Endpoint:** `POST /api/mt5/order`  
**Description:** Create a pending order  
**Controller:** OrderController

**Request Body:**
```json
{
  "login": 5550001,
  "symbol": "EURUSD",
  "volume": 1.0,
  "orderType": "BUY_LIMIT",
  "price": 1.08000,
  "stopLoss": 1.07500,
  "takeProfit": 1.08500,
  "expiration": "2025-01-20T00:00:00Z",
  "comment": "Buy limit order"
}
```

**Request Model:**
```csharp
public class CreateOrderRequest
{
    public long Login { get; set; }
    public string Symbol { get; set; }
    public double Volume { get; set; }
    public string OrderType { get; set; } // "BUY_LIMIT", "SELL_LIMIT", "BUY_STOP", "SELL_STOP"
    public double Price { get; set; }
    public double StopLoss { get; set; }
    public double TakeProfit { get; set; }
    public DateTime Expiration { get; set; }
    public string Comment { get; set; }
}
```

**Response (200 OK):**
```json
{
  "success": true,
  "message": "Order created successfully",
  "data": {
    "orderId": 987654321,
    "login": 5550001,
    "symbol": "EURUSD",
    "volume": 1.0,
    "orderType": "BUY_LIMIT",
    "price": 1.08000,
    "createdAt": "2025-01-15T14:30:00Z"
  }
}
```

---

### 18. Delete Order

**Endpoint:** `DELETE /api/mt5/order/{orderId}`  
**Description:** Cancel a pending order  
**Controller:** OrderController

**Path Parameters:**
- `orderId` (long) - The order ID

**Response (200 OK):**
```json
{
  "success": true,
  "message": "Order deleted successfully",
  "data": {
    "orderId": 987654321,
    "deletedAt": "2025-01-15T15:00:00Z"
  }
}
```

---

## Financial Operations APIs

### 19. Deposit

**Endpoint:** `POST /api/mt5/deposit`  
**Description:** Add funds to an account  
**Controller:** DepositController

**Request Body:**
```json
{
  "login": 5550001,
  "amount": 5000.00,
  "comment": "Initial deposit"
}
```

**Request Model:**
```csharp
public class DepositRequest
{
    public long Login { get; set; }
    public decimal Amount { get; set; }
    public string Comment { get; set; }
}
```

**Response (200 OK):**
```json
{
  "success": true,
  "message": "Deposit completed successfully",
  "data": {
    "login": 5550001,
    "amount": 5000.00,
    "previousBalance": 10000.00,
    "newBalance": 15000.00,
    "transactionId": "TXN123456789",
    "timestamp": "2025-01-15T14:30:00Z"
  }
}
```

---

### 20. Withdrawal

**Endpoint:** `POST /api/mt5/withdrawal`  
**Description:** Withdraw funds from an account  
**Controller:** LiveWithdrawalController

**Request Body:**
```json
{
  "login": 5550001,
  "amount": 2000.00,
  "comment": "Profit withdrawal"
}
```

**Request Model:**
```csharp
public class WithdrawalRequest
{
    public long Login { get; set; }
    public decimal Amount { get; set; }
    public string Comment { get; set; }
}
```

**Response (200 OK):**
```json
{
  "success": true,
  "message": "Withdrawal completed successfully",
  "data": {
    "login": 5550001,
    "amount": 2000.00,
    "previousBalance": 15000.00,
    "newBalance": 13000.00,
    "transactionId": "TXN987654321",
    "timestamp": "2025-01-15T15:00:00Z"
  }
}
```

---

### 21. Credit In/Out

**Endpoint:** `POST /api/mt5/credit`  
**Description:** Add or remove credit from an account  
**Controller:** CreditInOutController

**Request Body:**
```json
{
  "login": 5550001,
  "amount": 1000.00,
  "operation": "IN",
  "comment": "Bonus credit"
}
```

**Request Model:**
```csharp
public class CreditRequest
{
    public long Login { get; set; }
    public decimal Amount { get; set; }
    public string Operation { get; set; } // "IN" or "OUT"
    public string Comment { get; set; }
}
```

**Response (200 OK):**
```json
{
  "success": true,
  "message": "Credit operation completed successfully",
  "data": {
    "login": 5550001,
    "amount": 1000.00,
    "operation": "IN",
    "previousCredit": 0.00,
    "newCredit": 1000.00,
    "timestamp": "2025-01-15T14:30:00Z"
  }
}
```

---

### 22. Account Transfer

**Endpoint:** `POST /api/mt5/transfer`  
**Description:** Transfer funds between accounts  
**Controller:** AccountTransferController

**Request Body:**
```json
{
  "fromLogin": 5550001,
  "toLogin": 5550002,
  "amount": 500.00,
  "comment": "Internal transfer"
}
```

**Request Model:**
```csharp
public class TransferRequest
{
    public long FromLogin { get; set; }
    public long ToLogin { get; set; }
    public decimal Amount { get; set; }
    public string Comment { get; set; }
}
```

**Response (200 OK):**
```json
{
  "success": true,
  "message": "Transfer completed successfully",
  "data": {
    "fromLogin": 5550001,
    "toLogin": 5550002,
    "amount": 500.00,
    "fromNewBalance": 12500.00,
    "toNewBalance": 10500.00,
    "transactionId": "TRF123456789",
    "timestamp": "2025-01-15T14:30:00Z"
  }
}
```

---

### 23. Margin Call/Stop Out

**Endpoint:** `POST /api/mt5/liquidation`  
**Description:** Liquidate positions when margin level is too low  
**Controller:** LiquidationController

**Request Body:**
```json
{
  "login": 5550001,
  "type": "MARGIN_CALL"
}
```

**Request Model:**
```csharp
public class LiquidationRequest
{
    public long Login { get; set; }
    public string Type { get; set; } // "MARGIN_CALL" or "STOP_OUT"
}
```

**Response (200 OK):**
```json
{
  "success": true,
  "message": "Liquidation executed successfully",
  "data": {
    "login": 5550001,
    "type": "MARGIN_CALL",
    "closedPositions": 3,
    "totalLoss": 1250.00,
    "timestamp": "2025-01-15T14:30:00Z"
  }
}
```

---

### 24. Get Margin Information

**Endpoint:** `GET /api/mt5/margin/{loginId}`  
**Description:** Get margin calculation details  
**Controller:** MarginController

**Path Parameters:**
- `loginId` (long) - The MT5 account login ID

**Response (200 OK):**
```json
{
  "success": true,
  "message": "Margin information retrieved successfully",
  "data": {
    "login": 5550001,
    "balance": 10000.00,
    "equity": 10250.00,
    "margin": 500.00,
    "marginFree": 9750.00,
    "marginLevel": 2050.00,
    "marginCall": 100.00,
    "stopOut": 50.00
  }
}
```

---

## Reporting & Analytics APIs

### 25. Deal History

**Endpoint:** `GET /api/mt5/livedealhistory`  
**Description:** Get trading history with filters  
**Controller:** LiveDealHistoryController

**Query Parameters:**
- `login` (long, optional) - Filter by account
- `from` (DateTime, optional) - Start date
- `to` (DateTime, optional) - End date
- `symbol` (string, optional) - Filter by symbol
- `action` (string, optional) - Filter by action (BUY, SELL)

**Request Example:**
```
GET /api/mt5/livedealhistory?login=5550001&from=2025-01-01&to=2025-01-15
```

**Response (200 OK):**
```json
{
  "success": true,
  "message": "Deal history retrieved successfully",
  "data": {
    "totalDeals": 50,
    "deals": [
      {
        "dealId": 123456789,
        "login": 5550001,
        "symbol": "EURUSD",
        "volume": 1.0,
        "action": "BUY",
        "price": 1.08500,
        "profit": 25.00,
        "commission": 2.00,
        "swap": 0.50,
        "time": "2025-01-15T14:30:00Z"
      }
    ]
  }
}
```

---

### 26. Trading History

**Endpoint:** `GET /api/mt5/trading-history/{loginId}`  
**Description:** Get complete trading history for an account  
**Controller:** LiveTradingHistoryController

**Path Parameters:**
- `loginId` (long) - The MT5 account login ID

**Query Parameters:**
- `from` (DateTime, optional) - Start date
- `to` (DateTime, optional) - End date

**Response (200 OK):**
```json
{
  "success": true,
  "message": "Trading history retrieved successfully",
  "data": {
    "login": 5550001,
    "totalTrades": 100,
    "totalProfit": 5000.00,
    "totalLoss": 2000.00,
    "netProfit": 3000.00,
    "winRate": 65.00,
    "trades": [
      {
        "dealId": 123456789,
        "symbol": "EURUSD",
        "action": "BUY",
        "volume": 1.0,
        "openPrice": 1.08500,
        "closePrice": 1.08750,
        "profit": 25.00,
        "openTime": "2025-01-15T14:30:00Z",
        "closeTime": "2025-01-15T15:00:00Z"
      }
    ]
  }
}
```

---

### 27. Account Performance

**Endpoint:** `GET /api/accountperformance/performance`  
**Description:** Get account performance metrics  
**Controller:** AccountPerformanceController

**Query Parameters:**
- `login` (long) - The MT5 account login ID
- `from` (DateTime, optional) - Start date
- `to` (DateTime, optional) - End date

**Response (200 OK):**
```json
{
  "success": true,
  "message": "Performance data retrieved successfully",
  "data": {
    "login": 5550001,
    "period": "30 days",
    "startingBalance": 10000.00,
    "endingBalance": 13000.00,
    "totalProfit": 3000.00,
    "totalReturn": 30.00,
    "totalTrades": 50,
    "winningTrades": 35,
    "losingTrades": 15,
    "winRate": 70.00,
    "profitFactor": 2.5,
    "sharpeRatio": 1.8,
    "maxDrawdown": 500.00,
    "maxDrawdownPercent": 5.00
  }
}
```

---

### 28. Profit Chart by Date

**Endpoint:** `GET /api/mt5/profit-chart/{loginId}`  
**Description:** Get profit/loss data for charting  
**Controller:** AccountProfitChartByDateController

**Path Parameters:**
- `loginId` (long) - The MT5 account login ID

**Query Parameters:**
- `from` (DateTime) - Start date
- `to` (DateTime) - End date

**Response (200 OK):**
```json
{
  "success": true,
  "message": "Profit chart data retrieved successfully",
  "data": {
    "login": 5550001,
    "dataPoints": [
      {
        "date": "2025-01-01",
        "profit": 100.00,
        "balance": 10100.00,
        "equity": 10100.00
      },
      {
        "date": "2025-01-02",
        "profit": 150.00,
        "balance": 10250.00,
        "equity": 10250.00
      }
    ]
  }
}
```

---

### 29. Profit by Symbol

**Endpoint:** `GET /api/mt5/profit-by-symbol/{loginId}`  
**Description:** Get profit/loss breakdown by trading symbol  
**Controller:** ProfitBySymbolController

**Path Parameters:**
- `loginId` (long) - The MT5 account login ID

**Response (200 OK):**
```json
{
  "success": true,
  "message": "Profit by symbol retrieved successfully",
  "data": {
    "login": 5550001,
    "symbols": [
      {
        "symbol": "EURUSD",
        "totalTrades": 30,
        "totalProfit": 1500.00,
        "totalLoss": 500.00,
        "netProfit": 1000.00,
        "winRate": 70.00
      },
      {
        "symbol": "GBPUSD",
        "totalTrades": 20,
        "totalProfit": 800.00,
        "totalLoss": 300.00,
        "netProfit": 500.00,
        "winRate": 65.00
      }
    ]
  }
}
```

---

### 30. Dashboard Data

**Endpoint:** `GET /api/mt5/dashboard`  
**Description:** Get comprehensive dashboard data  
**Controller:** LiveDashboardController

**Response (200 OK):**
```json
{
  "success": true,
  "message": "Dashboard data retrieved successfully",
  "data": {
    "totalAccounts": 1000,
    "activeAccounts": 850,
    "totalBalance": 10000000.00,
    "totalEquity": 10250000.00,
    "totalProfit": 250000.00,
    "openPositions": 500,
    "dailyVolume": 50000.00,
    "topTraders": [
      {
        "login": 5550001,
        "name": "John Doe",
        "profit": 5000.00,
        "rank": 1
      }
    ]
  }
}
```

---

### 31. Leaderboard

**Endpoint:** `GET /api/mt5/leaderboard`  
**Description:** Get top performing traders  
**Controller:** LeaderboardController

**Query Parameters:**
- `period` (string, optional) - "daily", "weekly", "monthly", "all-time"
- `limit` (int, optional) - Number of results (default: 100)

**Response (200 OK):**
```json
{
  "success": true,
  "message": "Leaderboard retrieved successfully",
  "data": {
    "period": "monthly",
    "leaderboard": [
      {
        "rank": 1,
        "login": 5550001,
        "name": "John Doe",
        "profit": 15000.00,
        "returnPercent": 150.00,
        "trades": 200,
        "winRate": 75.00
      },
      {
        "rank": 2,
        "login": 5550002,
        "name": "Jane Smith",
        "profit": 12000.00,
        "returnPercent": 120.00,
        "trades": 180,
        "winRate": 72.00
      }
    ]
  }
}
```

---

### 32. Reports Generation

**Endpoint:** `POST /api/mt5/report`  
**Description:** Generate various types of reports  
**Controller:** ReportController

**Request Body:**
```json
{
  "reportType": "ACCOUNT_STATEMENT",
  "login": 5550001,
  "from": "2025-01-01T00:00:00Z",
  "to": "2025-01-31T23:59:59Z",
  "format": "PDF"
}
```

**Request Model:**
```csharp
public class ReportRequest
{
    public string ReportType { get; set; } // "ACCOUNT_STATEMENT", "TRADING_HISTORY", "PROFIT_LOSS"
    public long Login { get; set; }
    public DateTime From { get; set; }
    public DateTime To { get; set; }
    public string Format { get; set; } // "PDF", "CSV", "EXCEL"
}
```

**Response (200 OK):**
```json
{
  "success": true,
  "message": "Report generated successfully",
  "data": {
    "reportId": "RPT123456789",
    "reportType": "ACCOUNT_STATEMENT",
    "format": "PDF",
    "downloadUrl": "https://example.com/reports/RPT123456789.pdf",
    "generatedAt": "2025-01-15T14:30:00Z"
  }
}
```

---

### 33. Trade Account Overview

**Endpoint:** `GET /api/mt5/account-overview/{loginId}`  
**Description:** Get comprehensive account overview  
**Controller:** TradeAccountOverviewController

**Path Parameters:**
- `loginId` (long) - The MT5 account login ID

**Response (200 OK):**
```json
{
  "success": true,
  "message": "Account overview retrieved successfully",
  "data": {
    "accountInfo": {
      "login": 5550001,
      "name": "John Doe",
      "group": "demo\\standard",
      "leverage": 100,
      "balance": 10000.00,
      "equity": 10250.00
    },
    "openPositions": 3,
    "pendingOrders": 2,
    "todayProfit": 250.00,
    "weekProfit": 500.00,
    "monthProfit": 1000.00,
    "totalProfit": 3000.00
  }
}
```

---

### 34. Trading Data

**Endpoint:** `GET /api/mt5/trading-data/{loginId}`  
**Description:** Get real-time trading data  
**Controller:** LiveTradingDataController

**Path Parameters:**
- `loginId` (long) - The MT5 account login ID

**Response (200 OK):**
```json
{
  "success": true,
  "message": "Trading data retrieved successfully",
  "data": {
    "login": 5550001,
    "balance": 10000.00,
    "equity": 10250.00,
    "margin": 500.00,
    "marginFree": 9750.00,
    "marginLevel": 2050.00,
    "profit": 250.00,
    "openPositions": [
      {
        "symbol": "EURUSD",
        "volume": 1.0,
        "profit": 25.00
      }
    ],
    "timestamp": "2025-01-15T14:30:00Z"
  }
}
```

---

## System Management APIs

### 35. Group Management

**Endpoint:** `GET /api/groups`  
**Description:** Get all available groups  
**Controller:** GroupManagementController

**Response (200 OK):**
```json
{
  "success": true,
  "message": "Groups retrieved successfully",
  "data": {
    "totalGroups": 5,
    "groups": [
      {
        "name": "demo\\standard",
        "currency": "USD",
        "leverage": 100,
        "marginCall": 100.00,
        "stopOut": 50.00,
        "accountsCount": 500
      },
      {
        "name": "demo\\premium",
        "currency": "USD",
        "leverage": 200,
        "marginCall": 80.00,
        "stopOut": 30.00,
        "accountsCount": 300
      }
    ]
  }
}
```

**Get Specific Group:**
```
GET /api/groups/{groupName}
```

---

### 36. Symbol Management

**Endpoint:** `GET /api/symbols`  
**Description:** Get all available trading symbols  
**Controller:** SymbolManagementController

**Response (200 OK):**
```json
{
  "success": true,
  "message": "Symbols retrieved successfully",
  "data": {
    "totalSymbols": 50,
    "symbols": [
      {
        "name": "EURUSD",
        "description": "Euro vs US Dollar",
        "digits": 5,
        "contractSize": 100000,
        "tickSize": 0.00001,
        "tickValue": 1.00,
        "spread": 2,
        "enabled": true
      }
    ]
  }
}
```

**Get Specific Symbol:**
```
GET /api/symbols/{symbolName}
```

---

### 37. Server Information

**Endpoint:** `GET /api/mt5/server`  
**Description:** Get MT5 server information  
**Controller:** ServerController

**Response (200 OK):**
```json
{
  "success": true,
  "message": "Server information retrieved successfully",
  "data": {
    "version": "5.0.3561",
    "build": 3561,
    "serverTime": "2025-01-15T14:30:00Z",
    "connected": true,
    "accountsOnline": 850,
    "dealsTotal": 50000,
    "ordersTotal": 2000
  }
}
```

---

### 38. Online Users

**Endpoint:** `GET /api/mt5/online-users`  
**Description:** Get list of currently online users  
**Controller:** LiveOnlineUserController

**Response (200 OK):**
```json
{
  "success": true,
  "message": "Online users retrieved successfully",
  "data": {
    "totalOnline": 850,
    "users": [
      {
        "login": 5550001,
        "name": "John Doe",
        "group": "demo\\standard",
        "loginTime": "2025-01-15T14:00:00Z",
        "lastActivity": "2025-01-15T14:30:00Z"
      }
    ]
  }
}
```

---

### 39. Health Check

**Endpoint:** `GET /api/health`  
**Description:** Check API and MT5 server health  
**Controller:** HealthCheckController

**Response (200 OK):**
```json
{
  "success": true,
  "message": "System is healthy",
  "data": {
    "apiStatus": "OK",
    "mt5Status": "Connected",
    "databaseStatus": "OK",
    "timestamp": "2025-01-15T14:30:00Z",
    "uptime": "5d 12h 30m"
  }
}
```

---

### 40. User Management

**Endpoint:** `GET /api/mt5/users`  
**Description:** Get all users with pagination  
**Controller:** UserManagementController

**Query Parameters:**
- `page` (int, optional) - Page number (default: 1)
- `pageSize` (int, optional) - Items per page (default: 50)
- `search` (string, optional) - Search by name or email

**Response (200 OK):**
```json
{
  "success": true,
  "message": "Users retrieved successfully",
  "data": {
    "page": 1,
    "pageSize": 50,
    "totalPages": 20,
    "totalUsers": 1000,
    "users": [
      {
        "login": 5550001,
        "name": "John Doe",
        "email": "john@example.com",
        "group": "demo\\standard",
        "balance": 10000.00,
        "enabled": true
      }
    ]
  }
}
```

---

### 41. Email Notifications

**Endpoint:** `POST /api/mail/send`  
**Description:** Send email to account holders  
**Controller:** MailController

**Request Body:**
```json
{
  "login": 5550001,
  "subject": "Account Statement",
  "body": "Your monthly account statement is attached.",
  "attachments": ["statement.pdf"]
}
```

**Request Model:**
```csharp
public class EmailRequest
{
    public long Login { get; set; }
    public string Subject { get; set; }
    public string Body { get; set; }
    public List<string> Attachments { get; set; }
}
```

**Response (200 OK):**
```json
{
  "success": true,
  "message": "Email sent successfully",
  "data": {
    "login": 5550001,
    "email": "john@example.com",
    "sentAt": "2025-01-15T14:30:00Z"
  }
}
```

---

## Common Models

### Standard Response Wrapper

All API responses follow this structure:

```csharp
public class ApiResponse<T>
{
    public bool Success { get; set; }
    public string Message { get; set; }
    public T Data { get; set; }
    public List<string> Errors { get; set; }
}
```

### Error Response

```json
{
  "success": false,
  "message": "Validation failed",
  "errors": [
    "Login ID is required",
    "Amount must be greater than 0"
  ]
}
```

### HTTP Status Codes

- `200 OK` - Successful operation
- `201 Created` - Resource created successfully
- `400 Bad Request` - Invalid request parameters
- `401 Unauthorized` - Authentication required
- `403 Forbidden` - Insufficient permissions
- `404 Not Found` - Resource not found
- `500 Internal Server Error` - Server error

---

## Authentication

All API endpoints require authentication using Bearer token:

```
Authorization: Bearer {your-access-token}
```

### Get Access Token

**Endpoint:** `POST /api/auth/token`

**Request:**
```json
{
  "username": "admin",
  "password": "your-password"
}
```

**Response:**
```json
{
  "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "expiresIn": 3600,
  "tokenType": "Bearer"
}
```

---

## Rate Limiting

- 100 requests per minute per IP
- 1000 requests per hour per API key

Response headers:
```
X-RateLimit-Limit: 100
X-RateLimit-Remaining: 95
X-RateLimit-Reset: 1610000000
```

---

## Pagination

For endpoints that return lists, use these parameters:

- `page` (int) - Page number (starts at 1)
- `pageSize` (int) - Items per page (max: 100)

Response includes pagination metadata:

```json
{
  "page": 1,
  "pageSize": 50,
  "totalPages": 20,
  "totalItems": 1000,
  "data": [...]
}
```

---

## Testing with cURL

### Example: Create Account
```bash
curl -X POST http://localhost:8086/api/mt5/live-user-account \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer your-token" \
  -d '{
    "name": "John Doe",
    "email": "john@example.com",
    "group": "demo\\standard",
    "leverage": 100,
    "balance": 10000.00
  }'
```

### Example: Get Account Details
```bash
curl -X GET http://localhost:8086/api/mt5/account/5550001 \
  -H "Authorization: Bearer your-token"
```

### Example: Open Position
```bash
curl -X POST http://localhost:8086/api/mt5/position/open \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer your-token" \
  -d '{
    "login": 5550001,
    "symbol": "EURUSD",
    "volume": 1.0,
    "action": "BUY",
    "price": 1.08500
  }'
```

---

## Postman Collection

Import this URL into Postman to get started:
```
http://localhost:8086/explorer
```

Or download the collection:
```
http://localhost:8086/api/documentation/postman
```

---

## Support & Contact

- **Developer:** Amit Kumar
- **Documentation Version:** 1.0.0
- **Last Updated:** January 15, 2025
- **API Explorer:** http://localhost:8086/explorer
- **GitHub Repository:** https://github.com/chiragkevadiya/prop-mt5-service

---

**End of Documentation**
