# MetaTrader 5 REST API - Complete Endpoints Documentation

This document provides a comprehensive list of all REST API endpoints available in the PropMT5ConnectionService.

## Base URL
```
http://your-server:port/api
```

---

## 1. ACCOUNT MANAGEMENT APIs

### Live Account Controller (`/api/mt5`)
- `GET /api/mt5/account/{loginId}` - Get single live account by login ID
- `GET /api/mt5/accounts` - Get all live accounts
- `POST /api/mt5/account/create` - Create a new live account
- `POST /api/mt5/account/delete` - Delete multiple accounts (LiveAccountDeleteController)

### Account Status (`/api/account/status`)
- `POST /api/account/status/update` - Update account active/inactive status

### Deposit Operations (`/api/deposit`)
- `POST /api/deposit` - Deposit funds into an account
- `POST /api/deposit/raw` - Deposit funds with raw balance operation (no margin check)

### Withdrawal Operations (`/api/livewithdrawal`)
- `POST /api/livewithdrawal` - Withdraw funds from an account

### Credit Operations (`/api/credit-operations`)
- `POST /api/credit-operations/balance` - Credit In/Out operations

### Account Transfer (`/api/accounttransfer`)
- `POST /api/accounttransfer` - Transfer funds between accounts

### Leverage Update (`/api/liveleverageupdate`)
- `GET /api/liveleverageupdate` - Update account leverage

### Account Disable (`/api/account/disable`)
- `POST /api/account/disable` - Disable an account

### Account Availability Check (`/api/account/availability`)
- `GET /api/account/availability` - Check account availability

---

## 2. PASSWORD MANAGEMENT APIs

### Password Management (`/api/password`)
- `POST /api/password/change` - Change MT5 master or investor password

### Password Reset (`/api/livepasswordreset`)
- `GET /api/livepasswordreset` - Reset and generate new passwords

---

## 3. POSITION & ORDER MANAGEMENT APIs

### Positions (`/api/positions`)
- `GET /api/positions/{loginId}` - Get all open positions for an account
- `GET /api/positions/position/{positionId}` - Get specific position by position ID
- `GET /api/positions/symbol/{symbol}` - Get positions by symbol
- `GET /api/positions/{loginId}/count` - Get total open positions count

### Orders (`/api/orders`)
- `GET /api/orders/{loginId}` - Get all pending orders for an account
- `DELETE /api/orders/{orderId}` - Delete a pending order
- `GET /api/orders/{loginId}/history` - Get order history within date range
- `GET /api/orders/group/{groupName}` - Get all orders by group

### Open Trades (`/api/opentradedetail`)
- `GET /api/opentradedetail` - Get open trade details by login IDs

### Close Trades (`/api/trading/close`)
- `POST /api/trading/close/positions` - Close multiple trading positions
- `POST /api/trading/close/trades-orders-close` - Legacy endpoint for closing trades

---

## 4. DEALS & HISTORY APIs

### Deals (`/api/deals`)
- `GET /api/deals/{loginId}` - Get deals for an account within date range
- `GET /api/deals/deal/{dealId}` - Get specific deal by deal ID
- `GET /api/deals/group/{groupName}` - Get deals by group within date range
- `GET /api/deals/position/{positionId}` - Get deals by position ID
- `GET /api/deals/{loginId}/count` - Get total deals count

### Deal History (`/api/livedealhistory`)
- `GET /api/livedealhistory` - Get deal history by group with filters

### Trading History (`/api/trading/history`)
- `GET /api/trading/history/{loginId}` - Get trading history for an account

### Trading Data (`/api/trading/data`)
- `GET /api/trading/data/{loginId}` - Get trading data for an account

---

## 5. USER MANAGEMENT APIs

### User Management (`/api/users`)
- `GET /api/users` - Get all users
- `GET /api/users/{loginId}` - Get user by login
- `PUT /api/users/{loginId}` - Update user information
- `GET /api/users/group/{groupName}` - Get users by group
- `GET /api/users/{loginId}/exists` - Check if login exists
- `GET /api/users/search` - Search users by name

### User Account Details (`/api/user/account`)
- `GET /api/user/account/{loginId}` - Get detailed user account information

### User Account Batch Operations (`/api/useraccount/batch`)
- `POST /api/useraccount/batch` - Batch operations on user accounts

### Online Users (`/api/online`)
- `GET /api/online` - Get all online users

---

## 6. GROUP MANAGEMENT APIs

### Groups (`/api/groups`)
- `GET /api/groups` - Get all groups
- `GET /api/groups/{groupName}` - Get specific group details
- `GET /api/groups/{groupName}/symbols` - Get symbols configured for a group
- `GET /api/groups/{groupName}/commissions` - Get commission settings for a group
- `GET /api/groups/{groupName}/accounts/count` - Get accounts count in a group

### Group Operations (`/api/group`)
- `GET /api/group` - Get group names and details
- `POST /api/group/update` - Update group settings

### User Account by Group (`/api/useraccountbygroup`)
- `GET /api/useraccountbygroup` - Get user accounts by group

---

## 7. SYMBOL MANAGEMENT APIs

### Symbols (`/api/symbols`)
- `GET /api/symbols` - Get all available symbols
- `GET /api/symbols/{symbolName}` - Get specific symbol details
- `GET /api/symbols/path/{path}` - Get symbols by path/category
- `GET /api/symbols/{symbolName}/sessions` - Get symbol trading sessions/hours

### Symbol Operations (`/api/symbol`)
- `GET /api/symbol` - Get all symbols with details

---

## 8. MARKET DATA APIs

### Market Data (`/api/market`)
- `GET /api/market/tick/{symbol}` - Get current market tick for a symbol
- `GET /api/market/ticks/{symbol}` - Get last ticks within time range
- `GET /api/market/book/{symbol}` - Get market depth (book) for a symbol
- `POST /api/market/quotes` - Get current quotes for multiple symbols

---

## 9. MARGIN & CALCULATIONS APIs

### Margin (`/api/margin`)
- `POST /api/margin/calculate` - Calculate margin for a potential trade
- `GET /api/margin/{loginId}` - Get current margin information for an account
- `POST /api/margin/profit` - Calculate profit for a potential trade

---

## 10. REPORTS & ANALYTICS APIs

### Reports (`/api/reports`)
- `GET /api/reports/account/{loginId}/summary` - Get account summary report
- `GET /api/reports/account/{loginId}/statistics` - Get trading statistics
- `GET /api/reports/account/{loginId}/daily` - Get daily profit/loss report
- `GET /api/reports/group/{groupName}/summary` - Get group report

### Dashboard (`/api/dashboard`)
- `GET /api/dashboard/{loginId}` - Get dashboard data for an account

### Account Performance (`/api/performance`)
- `GET /api/performance/{loginId}` - Get account performance metrics

### Leaderboard (`/api/leaderboard`)
- `GET /api/leaderboard` - Get leaderboard data

### Profit by Symbol (`/api/profitbysymbol`)
- `GET /api/profitbysymbol/{loginId}` - Get profit breakdown by symbol

### Account Profit Chart (`/api/profitchart`)
- `GET /api/profitchart/{loginId}` - Get account profit chart data by date

---

## 11. SERVER MANAGEMENT APIs

### Server (`/api/server`)
- `GET /api/server/time` - Get server time
- `GET /api/server/ping` - Ping the server
- `GET /api/server/version` - Get MT5 API version
- `GET /api/server/stats` - Get server statistics
- `POST /api/server/subscribe` - Subscribe to trading events
- `GET /api/server/connected` - Check if manager is connected

### Health Check (`/api/health`)
- `GET /api/health` - Server health check endpoint

---

## 12. MAIL OPERATIONS APIs

### Mail (`/api/mail`)
- `POST /api/mail/send` - Send mail to a specific user
- `POST /api/mail/send/bulk` - Send mail to multiple users
- `POST /api/mail/send/group/{groupName}` - Send mail to all users in a group

---

## 13. LIQUIDATION APIs

### Liquidation (`/api/liquidation`)
- `POST /api/liquidation/check` - Check for accounts requiring liquidation
- `POST /api/liquidation/execute` - Execute liquidation for specific accounts

---

## 14. DEMO ACCOUNT APIs

### Demo Account Controller (`/api/demo`)
- `GET /api/demo/account/{loginId}` - Get demo account
- `GET /api/demo/accounts` - Get all demo accounts
- `POST /api/demo/account/create` - Create demo account
- `POST /api/demo/account/delete` - Delete demo account

### Demo Account Status (`/api/demo/status`)
- `POST /api/demo/status/update` - Update demo account status

### Demo Group (`/api/demo/group`)
- `GET /api/demo/group` - Get demo groups
- `POST /api/demo/group/update` - Update demo group

### Demo Symbol (`/api/demo/symbol`)
- `GET /api/demo/symbol` - Get demo symbols

### Demo Trading History (`/api/demo/history`)
- `GET /api/demo/history/{loginId}` - Get demo trading history

### Demo User Account (`/api/demo/useraccount`)
- `GET /api/demo/useraccount/{loginId}` - Get demo user account details

### Demo Batch Operations (`/api/demo/batch`)
- `POST /api/demo/batch` - Batch operations on demo accounts

---

## 15. TRADE ACCOUNT OVERVIEW APIs

### Trade Overview (`/api/trade/overview`)
- `GET /api/trade/overview/{loginId}` - Get comprehensive trade account overview

---

## Request/Response Format

### Standard Response Format
```json
{
  "Success": true,
  "Message": "Operation successful",
  "StatusCode": 200,
  "Data": { }
}
```

### Error Response Format
```json
{
  "Success": false,
  "Message": "Error message",
  "StatusCode": 400,
  "Data": null
}
```

---

## Common HTTP Status Codes
- `200` - OK (Success)
- `400` - Bad Request (Invalid parameters)
- `404` - Not Found (Resource not found)
- `500` - Internal Server Error

---

## Authentication
All API endpoints require proper authentication and authorization. Ensure the MT5 Manager API connection is established before making requests.

---

## Date Format
Most date parameters accept ISO 8601 format:
```
2024-01-15T10:30:00Z
```

Or simple date format:
```
2024-01-15
```

---

## Volume Format
Volumes are typically expressed in lots multiplied by 10000:
- 1 lot = 10000
- 0.1 lot = 1000
- 0.01 lot = 100

---

## Notes
1. All monetary values are returned with appropriate decimal precision
2. Unix timestamps are used for time representations in many responses
3. Some endpoints support pagination (check individual endpoint documentation)
4. Rate limiting may apply depending on your MT5 server configuration
5. All endpoints follow RESTful conventions
6. Batch operations are available for improved performance when dealing with multiple entities

---

## Support
For MT5 Manager API documentation, refer to the official MetaQuotes documentation.

---

**Last Updated:** 2025-01-15
**API Version:** 1.0
**MT5 Manager API Version:** Compatible with MT5 Build 3000+
