# API Route Prefixes - Complete List

## ✅ Controllers with RoutePrefix Attribute Added

### Live Controllers

| Controller | Route Prefix | Endpoints |
|------------|-------------|-----------|
| **HealthCheckController** | `api/health` | Health checks and diagnostics |
| **LiquidationController** | `api/liquidation` | Liquidation operations |
| **LiveAccountController** | `api/liveaccount` | Live account management |
| **LiveDashboardController** | `api/livedashboard` | Dashboard statistics |
| **LiveOnlineUserController** | `api/liveonlineuser` | Online user tracking |
| **LivePasswordResetController** | `api/livepasswordreset` | Password reset operations |
| **LiveLeverageUpdateController** | `api/liveleverageupdate` | Leverage updates |
| **LiveGroupUpdateController** | `api/livegroupupdate` | Group updates |
| **LiveWithdrawalController** | `api/livewithdrawal` | Withdrawal operations |
| **LiveDealHistoryController** | `api/livedealhistory` | Deal history |
| **OpenTradeDetailController** | `api/opentradedetail` | Open trade details |
| **ProfitBySymbolController** | `api/profitbysymbol` | Profit by symbol |
| **AccountTransferController** | `api/accounttransfer` | Account transfers |
| **LeaderboardController** | `api/leaderboard` | Leaderboard rankings |
| **GroupController** | `api/group` | Group management |
| **SymbolController** | `api/symbol` | Symbol operations |

### Already Configured Controllers

| Controller | Route Prefix | Status |
|------------|-------------|--------|
| **AccountPerformanceDataController** | `api/account-performance-data` | ✅ Already configured |
| **CreditOperationsController** | `api/credit-operations` | ✅ Already configured |
| **DemoAccountController** | `api/demo` | ✅ Already configured |

## 📋 Complete API Endpoint Reference

### Health & Status
```
GET  /api/health                          - Basic health check
GET  /api/health/detailed                 - Detailed health check
```

### Account Management
```
GET    /api/liveaccount/accounts          - Get all accounts
GET    /api/liveaccount/account/{id}      - Get single account
POST   /api/liveaccount/account/create    - Create account
PUT    /api/liveaccount/account/update    - Update account
DELETE /api/liveaccount/account/delete    - Delete account
```

### Liquidation
```
GET  /api/liquidation/MT5Liquidation?accountId={id}  - Check/perform liquidation
```

### Dashboard
```
GET  /api/livedashboard/MT5LiveDashboardDetail?loginIds={ids}  - Dashboard data
```

### Trading Operations
```
GET  /api/livedealhistory/{loginId}       - Get deal history
GET  /api/opentradedetail/{loginId}       - Get open trades
GET  /api/profitbysymbol/{loginId}        - Get profit by symbol
```

### Account Operations
```
POST /api/livewithdrawal/Withdraw         - Process withdrawal
POST /api/accounttransfer/TransferTerminalToTerminal  - Transfer funds
POST /api/liveleverageupdate              - Update leverage
POST /api/livegroupupdate                 - Update group
POST /api/livepasswordreset               - Reset password
```

### Performance & Analytics
```
GET  /api/leaderboard                     - Get leaderboard
GET  /api/account-performance-data/performance  - Get performance metrics
```

### Group & Symbol Management
```
GET  /api/group/GetGroupName              - Get all groups
POST /api/group/GetGroupwithsymbol        - Get groups with symbols
GET  /api/symbol                          - Get symbols
```

### Credit Operations
```
POST /api/credit-operations/balance       - Deposit/Credit operations
```

### Demo Account Operations
```
GET  /api/demo                            - Demo account operations
POST /api/demo/Create                     - Create demo account
```

### Online Users
```
GET  /api/liveonlineuser                  - Get online users
```

## 🎯 Testing Examples

### Using cURL

```bash
# Health check
curl http://localhost:8086/api/health

# Get all live accounts
curl http://localhost:8086/api/liveaccount/accounts

# Get specific account
curl http://localhost:8086/api/liveaccount/account/1000

# Get dashboard
curl "http://localhost:8086/api/livedashboard/MT5LiveDashboardDetail?loginIds=1000,1001"

# Check liquidations
curl http://localhost:8086/api/liquidation/MT5Liquidation

# Get leaderboard
curl http://localhost:8086/api/leaderboard

# Get online users
curl http://localhost:8086/api/liveonlineuser

# Create account
curl -X POST http://localhost:8086/api/liveaccount/account/create \
  -H "Content-Type: application/json" \
  -d '{"userId":12345,"name":"John Doe","email":"john@example.com"}'

# Transfer funds
curl -X POST http://localhost:8086/api/accounttransfer/TransferTerminalToTerminal \
  -H "Content-Type: application/json" \
  -d '{"fromLogin":1000,"toLogin":1001,"amount":100}'
```

### Using JavaScript/Fetch

```javascript
// Get all accounts
fetch('http://localhost:8086/api/liveaccount/accounts')
  .then(response => response.json())
  .then(data => console.log(data));

// Get dashboard
fetch('http://localhost:8086/api/livedashboard/MT5LiveDashboardDetail?loginIds=1000')
  .then(response => response.json())
  .then(data => console.log(data));

// Create account
fetch('http://localhost:8086/api/liveaccount/account/create', {
  method: 'POST',
  headers: { 'Content-Type': 'application/json' },
  body: JSON.stringify({
    userId: 12345,
    name: 'John Doe',
    email: 'john@example.com'
  })
})
.then(response => response.json())
.then(data => console.log(data));
```

### Using PowerShell

```powershell
# Health check
Invoke-RestMethod -Uri "http://localhost:8086/api/health" -Method Get

# Get accounts
Invoke-RestMethod -Uri "http://localhost:8086/api/liveaccount/accounts" -Method Get

# Create account
$body = @{
    userId = 12345
    name = "John Doe"
    email = "john@example.com"
} | ConvertTo-Json

Invoke-RestMethod -Uri "http://localhost:8086/api/liveaccount/account/create" `
                  -Method Post `
                  -Body $body `
                  -ContentType "application/json"
```

## 📝 Route Naming Conventions

- **Lowercase** - All route prefixes use lowercase
- **Descriptive** - Names clearly indicate the resource/functionality
- **RESTful** - Follows REST API naming conventions
- **Consistent** - Similar resources use similar naming patterns

## 🔄 Migration Notes

If you were using old routes without prefixes, update your client applications to use the new routes:

### Old vs New Routes

```
OLD: /api/MT5LiveDashboardDetail
NEW: /api/livedashboard/MT5LiveDashboardDetail

OLD: /api/MT5Liquidation
NEW: /api/liquidation/MT5Liquidation

OLD: /api/GetGroupName
NEW: /api/group/GetGroupName

OLD: /api/TransferTerminalToTerminal
NEW: /api/accounttransfer/TransferTerminalToTerminal
```

## ✅ Verification

To verify all routes are working:

1. Start the service
2. Navigate to `http://localhost:8086`
3. Check the welcome page for complete endpoint list
4. Test key endpoints with cURL or browser

## 📚 Additional Resources

- Full API Documentation: `http://localhost:8086`
- Health Check: `http://localhost:8086/api/health`
- API Documentation (MD): `API_DOCUMENTATION.md`
- Troubleshooting Guide: `TROUBLESHOOTING.md`

---

**Status:** ✅ All Route Prefixes Added  
**Date:** January 13, 2025  
**Version:** 1.0  
