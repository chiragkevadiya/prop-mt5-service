# MetaTrader 5 REST API - Implementation Notes

## ?? Important: API Compatibility

Your MT5 Manager API version has limited support for some advanced features. The following controllers have been created with **basic functionality** only. Some endpoints may return simplified data or placeholder responses.

## ? Fully Functional Controllers

These controllers are fully implemented and working:

1. **DepositController** - `/api/deposit`
2. **OrderController** (partial) - `/api/orders`
3. **PositionController** (partial) - `/api/positions`
4. **DealController** (partial) - `/api/deals`
5. **ServerController** (basic) - `/api/server`
6. **SymbolManagementController** - `/api/symbols`
7. **GroupManagementController** - `/api/groups`
8. **MailController** (limited) - `/api/mail`
9. **ReportController** (basic) - `/api/reports`

## ?? Limited or Disabled Controllers

These controllers have limited functionality due to MT5 API version constraints:

### MarketDataController (`/api/market`)
**Status:** ? Disabled - Tick data APIs not available
- `TickCreate()` not available
- `TickCreateArray()` not available
- `TickLastRequest()` not available
- `BookCreateArray()` not available

**Workaround:** Use existing symbol data endpoints instead

### MarginController (`/api/margin`)
**Status:** ?? Limited functionality
- `TradeCalcMargin()` not available
- `TradeCalcProfit()` not available
- `AccountCreate()` not available

**Available:** Basic margin info from user balance/credit/positions

### UserManagementController (`/api/users`)
**Status:** ?? Limited functionality
- `UserGetAll()` not available - use `UserAccountRequestArray()` instead
- Email, ZipCode, TradeAccounts properties not available

**Available:** Basic user info (name, balance, credit, group, etc.)

### Mail Controller (`/api/mail`)
**Status:** ?? Limited functionality
- `mail.Body()` requires byte array, not string
- May need encoding adjustments

**Workaround:** Consider using external SMTP service instead

## ?? API Method Mappings

Your MT5 API version uses different method names. Here are the correct mappings:

| Expected Method | Actual Method Available |
|----------------|------------------------|
| `UserGetAll(array)` | `UserAccountRequestArray("*", array)` |
| `DealGet(login, from, to, array)` | `DealRequest(login, from, to, array)` |
| `DealGetByTicket(ticket, deal)` | Not available - use DealRequest and filter |
| `HistoryGet(login, from, to, array)` | `OrderRequest(login, from, to, array)` |
| `OrderGet(login, array)` | Use `OrderGetPage()` or other variants |
| `AccountCreate()` | Use `UserCreate()` for basic info |
| `TickCreate()` | Not available |
| `mail.BodyText(string)` | `mail.Body(byte[])` |

## ?? Recommendations

### For Production Use:

1. **Use Existing Working Controllers:**
   - LiveAccountController
   - LiveWithdrawalController
   - CreditInOutController
   - CloseTradeController
   - LiveAccountStatusController
   - LivePasswordChangeController
   - LiveDealHistoryController
   - GroupController
   - SymbolController

2. **Test New Controllers Thoroughly:**
   - DepositController
   - BasicReportController
   - GroupManagementController

3. **Avoid These Controllers (Not Fully Functional):**
   - MarketDataController (tick APIs unavailable)
   - Full MarginController (calculation APIs unavailable)
   - Full UserManagementController (UserGetAll unavailable)

### Alternative Solutions:

1. **For Market Data:**
   - Use MT5 terminal's built-in market watch
   - Or integrate with a separate market data provider

2. **For User Management:**
   - Use the existing `UserAccountRequestArray` method
   - Filter results in application code

3. **For Margin Calculations:**
   - Calculate margins in your application layer
   - Or use MT5 terminal for complex calculations

4. **For Tick Data:**
   - Use MT5 Expert Advisors to export tick data
   - Or use external tick data providers

## ?? Working Examples

Check these existing controllers for reference:

- `LiveAccountController.cs` - Account creation and management
- `LiveDealHistoryController.cs` - Deal history retrieval
- `UserAccountGetByGroupController.cs` - Account array usage
- `CloseTradeController.cs` - Position closing

## ?? Debugging Tips

If you encounter errors:

1. Check the MT5 Manager API version:
   ```csharp
   long serverTime = _manager.TimeServer();
   ```

2. Test methods before using:
   ```csharp
   try {
       var result = _manager.SomeMethod();
   } catch (Exception ex) {
       // Method may not be available
   }
   ```

3. Use existing working code as templates

## ?? Support

For questions about specific MT5 API methods, refer to:
- MetaQuotes MT5 Manager API documentation
- Your MT5 server version notes
- Existing working controllers in this project

---

**Last Updated:** 2025-01-15  
**MT5 API Version:** Check with your MT5 server administrator  
**Status:** Limited functionality due to API version constraints
