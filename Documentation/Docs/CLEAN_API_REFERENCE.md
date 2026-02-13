# Clean API Endpoint Reference - PropMT5ConnectionService

## ?? All API Endpoints - Organized & Duplicate-Free

This document provides a quick reference of all available API endpoints after duplicate cleanup.

---

## ?? Account Management APIs

### Live Account Operations
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/mt5/account/{loginId}` | Get single account by login ID |
| GET | `/api/mt5/accounts` | Get all live accounts |
| POST | `/api/mt5/account/create` | Create new account |
| POST | `/api/mt5/account/delete` | Delete accounts (batch) |
| POST | `/api/account/status/update` | Update account status (active/inactive) |
| POST | `/api/account/disable` | Disable account |
| GET | `/api/account/availability` | Check account availability |

### User Account Details
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/mt5/user-account/{loginId}` | Get user account with balance & profit |
| POST | `/api/mt5/user-account/deposit-balance` | Deposit/withdraw balance |

---

## ?? Financial Operations

### Deposits
| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/deposit` | Deposit funds (with margin check) |
| POST | `/api/deposit/raw` | Deposit funds (no margin check) |

### Withdrawals
| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/livewithdrawal` | Withdraw funds from account |

### Credit Operations
| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/credit-operations/balance` | Credit In/Out operations |

### Transfers
| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/accounttransfer` | Transfer funds between accounts |

---

## ?? Trading Operations

### Position Management
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/positions/{loginId}` | Get all open positions |
| GET | `/api/positions/position/{positionId}` | Get specific position by ID |
| GET | `/api/positions/symbol/{symbol}` | Get positions by symbol |

### Order Management
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/orders/{loginId}` | Get pending orders |
| DELETE | `/api/orders/{orderId}` | Delete order |
| GET | `/api/orders/{loginId}/history` | Get order history |

### Trading Actions
| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/closetrade` | Close position |
| POST | `/api/liquidation` | Liquidate account |

---

## ?? Deal History & Reports

### Deal History
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/livedealhistory?fromDate={date}&toDate={date}&actions={actions}&byGroups={groups}` | Get deal history with filters |

### Reports & Analytics
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/reports/account/{id}/summary` | Account summary report |
| GET | `/api/reports/account/{id}/statistics` | Trading statistics |
| GET | `/api/reports/account/{id}/daily` | Daily report |
| GET | `/api/livedashboard` | Dashboard data |
| GET | `/api/accountperformance` | Performance metrics |
| GET | `/api/leaderboard` | Leaderboard data |
| GET | `/api/profitbysymbol` | Profit analysis by symbol |

### Trading History
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/livetradinghistory` | Trading history |
| GET | `/api/livetradingdata` | Trading data |

---

## ?? User Management

### User Operations
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/users` | Get all users |
| GET | `/api/users/{loginId}` | Get user by login |
| PUT | `/api/users/{loginId}` | Update user |
| GET | `/api/users/search?query={query}` | Search users |

### Online Users
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/liveonlineuser` | Get online users |

---

## ?? Configuration & Settings

### Group Management
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/groups` | Get all groups |
| GET | `/api/groups/{groupName}` | Get group details |
| GET | `/api/groups/{groupName}/symbols` | Get group symbols |
| GET | `/api/groups/{groupName}/commissions` | Get group commissions |

### Symbol Management
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/symbols` | Get all symbols |
| GET | `/api/symbols/{symbolName}` | Get symbol details |
| GET | `/api/symbols/path/{path}` | Get symbols by path |

### Password Management
| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/password/change` | Change master/investor password |

### Leverage Management
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/liveleverageupdate` | Update account leverage |

---

## ?? Mail Operations

### MT5 Internal Mail
| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/mail/send` | Send mail to single user |
| POST | `/api/mail/send/bulk` | Send mail to multiple users |
| POST | `/api/mail/send/group/{groupName}` | Send mail to group |

---

## ??? Server & System Operations

### Server Monitoring
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/server/time` | Get server time |
| GET | `/api/server/ping` | Ping server |
| GET | `/api/server/version` | Get version info |
| GET | `/api/server/stats` | Get server statistics |

### Margin Information
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/margin/{loginId}` | Get margin info |
| GET | `/api/margin/{loginId}/summary` | Get margin summary |

### Health Check
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/health` | System health check |

---

## ?? Request/Response Formats

### Common Request Body (Deposits/Withdrawals)
```json
{
  "Login": 5550001,
  "Amount": 1000.00,
  "Comment": "Deposit"
}
```

### Common Response Format
```json
{
  "Success": true,
  "Message": "Operation successful",
  "Data": { /* result data */ },
  "ErrorCode": 0,
  "MTRetErrorCode": "MT_RET_OK"
}
```

---

## ?? Authentication

All endpoints require appropriate authentication headers. Ensure proper MT5 Manager API connection is established.

---

## ?? Important Notes

1. **Removed Duplicate Routes**: The following duplicate controllers were removed:
   - ? `api/group` ? Use `api/groups` instead
   - ? `api/symbol` ? Use `api/symbols` instead
   - ? `api/deals` ? Use `api/livedealhistory` instead
   - ? `api/opentradedetail` ? Use `api/positions` instead

2. **Date Format**: Use ISO 8601 format for date parameters
3. **Login IDs**: Always use numeric login IDs (ulong)
4. **Amounts**: Always positive for deposits, negative handled internally for withdrawals

---

## ?? Statistics

- **Total Controllers**: 39
- **Total Endpoints**: 100+
- **Removed Duplicates**: 4 controllers, 8+ endpoints
- **Build Status**: ? SUCCESSFUL
- **API Version**: 1.0 (Clean)

---

## ?? Quick Start Examples

### Get Account Balance
```http
GET /api/mt5/user-account/5550001
```

### Deposit Funds
```http
POST /api/deposit
Content-Type: application/json

{
  "Login": 5550001,
  "Amount": 1000.00,
  "Comment": "Initial Deposit"
}
```

### Get Open Positions
```http
GET /api/positions/5550001
```

### Get Deal History
```http
GET /api/livedealhistory?fromDate=2025-01-01&toDate=2025-05-14&byGroups=*
```

### Get All Symbols
```http
GET /api/symbols
```

---

## ?? Related Documentation

- [Duplicate Cleanup Summary](DUPLICATE_CLEANUP_SUMMARY.md)
- [API Implementation Notes](API_IMPLEMENTATION_NOTES.md)
- [Build Success Summary](BUILD_SUCCESS_SUMMARY.md)

---

**Last Updated**: May 14, 2025  
**Status**: ? Clean & Organized - All Duplicates Removed
