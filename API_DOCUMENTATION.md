# FxRateHub API Documentation

## Table of Contents

- [Overview](#overview)
- [Authentication](#authentication)
  - [JWT Authentication](#jwt-authentication)
  - [API Key Authentication](#api-key-authentication)
- [Rate Limiting](#rate-limiting)
- [Error Handling](#error-handling)
- [API Endpoints](#api-endpoints)
  - [Public Endpoints](#public-endpoints)
  - [Authentication Endpoints](#authentication-endpoints)
  - [User Endpoints](#user-endpoints)
  - [API Key Endpoints](#api-key-endpoints)
  - [Admin Endpoints](#admin-endpoints)
  - [V1 Exchange Rates API](#v1-exchange-rates-api)
- [Data Models](#data-models)
- [SDK & Code Examples](#sdk--code-examples)

---

## Overview

FxRateHub is a comprehensive Foreign Exchange Rate API service that provides real-time currency exchange rates, currency conversion, and historical data. The API supports multiple authentication methods and is designed for both public access and authenticated API consumers.

### Base URL

```
https://your-domain.com/api
```

### API Version

- **v1**: `/api/v1/*` - Requires API Key authentication

### Content Type

All requests and responses use `application/json`.

---

## Authentication

FxRateHub supports two authentication methods:

### JWT Authentication

Used for user-facing operations (dashboard, profile management, API key management).

**Header Format:**
```http
Authorization: Bearer <your_jwt_token>
```

**Obtaining a Token:**
1. Register via `POST /api/auth/register`
2. Login via `POST /api/auth/login`
3. Or authenticate with Google via `POST /api/auth/google`

**Token Properties:**
- Default expiration: 7 days
- Includes user ID, email, role, and expiration

### API Key Authentication

Used for programmatic access to exchange rate data.

**Header Format:**
```http
X-API-Key: <your_api_key>
```

**Obtaining an API Key:**
1. Register and login to get a JWT token
2. Generate an API key via `POST /api/apikey/generate`

**API Key Properties:**
- SHA256 hashed for security
- Daily request limit (default: 1000 requests/day)
- Rate limit headers included in responses

---

## Rate Limiting

API endpoints under `/api/v1/*` are subject to rate limiting.

### Rate Limit Headers

Every API response includes rate limit information:

| Header | Description |
|--------|-------------|
| `X-RateLimit-Limit` | Maximum requests per day |
| `X-RateLimit-Remaining` | Remaining requests for today |
| `X-RateLimit-Reset` | Unix timestamp when limit resets (midnight UTC) |

### Rate Limit Exceeded Response

```json
{
  "status": 429,
  "message": "Daily API request limit exceeded",
  "dailyLimit": 1000,
  "currentUsage": 1000,
  "resetsIn": "5h 30m 15s",
  "resetsAt": "2026-02-22T00:00:00.0000000Z"
}
```

---

## Error Handling

### Error Response Format

```json
{
  "status": 400,
  "message": "Error description",
  "errors": []
}
```

### HTTP Status Codes

| Status Code | Description |
|-------------|-------------|
| 200 | Success |
| 201 | Created |
| 400 | Bad Request - Invalid parameters |
| 401 | Unauthorized - Missing or invalid credentials |
| 403 | Forbidden - Insufficient permissions |
| 404 | Not Found - Resource not found |
| 429 | Too Many Requests - Rate limit exceeded |
| 500 | Internal Server Error |

---

## API Endpoints

### Public Endpoints

Public endpoints do not require authentication. They provide basic exchange rate information with caching.

#### Get All Exchange Rates

```http
GET /api/public/rates
```

**Query Parameters:**

| Parameter | Type | Required | Default | Description |
|-----------|------|----------|---------|-------------|
| baseCurrency | string | No | USD | Base currency code (3-letter ISO 4217) |

**Response:**
```json
{
  "baseCurrency": "USD",
  "timestamp": "2026-02-21T08:00:00Z",
  "rates": {
    "EUR": 0.9234,
    "GBP": 0.7891,
    "JPY": 149.52,
    "BDT": 110.25
  }
}
```

**Cache Headers:**
- `Cache-Control: public, max-age=300` (5 minutes)

---

#### Convert Currency

```http
GET /api/public/convert
```

**Query Parameters:**

| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| from | string | Yes | Source currency code |
| to | string | Yes | Target currency code |
| amount | decimal | Yes | Amount to convert (must be > 0) |

**Response:**
```json
{
  "from": "USD",
  "to": "EUR",
  "amount": 100.00,
  "convertedAmount": 92.34,
  "rate": 0.9234,
  "timestamp": "2026-02-21T08:00:00Z"
}
```

**Error Responses:**
- `400` - Invalid parameters
- `404` - Currency not found

---

#### Get Available Currencies

```http
GET /api/public/currencies
```

**Response:**
```json
[
  "AED", "AFN", "ALL", "AMD", "ANG", "AOA", "ARS", "AUD", "AWG", "AZN",
  "BAM", "BBD", "BDT", "BGN", "BHD", "BIF", "BMD", "BND", "BOB", "BRL",
  "BSD", "BTN", "BWP", "BYN", "BZD", "CAD", "CDF", "CHF", "CLP", "CNY",
  "COP", "CRC", "CUP", "CVE", "CZK", "DJF", "DKK", "DOP", "DZD", "EGP",
  "ERN", "ETB", "EUR", "FJD", "FKP", "FOK", "GBP", "GEL", "GGP", "GHS",
  "GIP", "GMD", "GNF", "GTQ", "GYD", "HKD", "HNL", "HRK", "HTG", "HUF",
  "IDR", "ILS", "IMP", "INR", "IQD", "IRR", "ISK", "JEP", "JMD", "JOD",
  "JPY", "KES", "KGS", "KHR", "KID", "KMF", "KRW", "KWD", "KYD", "KZT",
  "LAK", "LBP", "LKR", "LRD", "LSL", "LYD", "MAD", "MDL", "MGA", "MKD",
  "MMK", "MNT", "MOP", "MRU", "MUR", "MVR", "MWK", "MXN", "MYR", "MZN",
  "NAD", "NGN", "NIO", "NOK", "NPR", "NZD", "OMR", "PAB", "PEN", "PGK",
  "PHP", "PKR", "PLN", "PYG", "QAR", "RON", "RSD", "RUB", "RWF", "SAR",
  "SBD", "SCR", "SDG", "SEK", "SGD", "SHP", "SLE", "SOS", "SRD", "SSP",
  "STN", "SYP", "SZL", "THB", "TJS", "TMT", "TND", "TOP", "TRY", "TTD",
  "TVD", "TWD", "TZS", "UAH", "UGX", "USD", "UYU", "UZS", "VES", "VND",
  "VUV", "WST", "XAF", "XCD", "XDR", "XOF", "XPF", "YER", "ZAR", "ZMW"
]
```

---

### Authentication Endpoints

#### Register New User

```http
POST /api/auth/register
```

**Request Body:**
```json
{
  "email": "user@example.com",
  "password": "SecurePassword123!",
  "fullName": "John Doe"
}
```

**Response:**
```json
{
  "userId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "email": "user@example.com",
  "fullName": "John Doe",
  "role": "User",
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "refreshToken": "refresh_token_here",
  "expiresAt": "2026-02-28T08:00:00Z"
}
```

**Error Responses:**
- `400` - Email already exists or invalid data

---

#### Login

```http
POST /api/auth/login
```

**Request Body:**
```json
{
  "email": "user@example.com",
  "password": "SecurePassword123!"
}
```

**Response:**
```json
{
  "userId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "email": "user@example.com",
  "fullName": "John Doe",
  "role": "User",
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "refreshToken": "refresh_token_here",
  "expiresAt": "2026-02-28T08:00:00Z"
}
```

**Error Responses:**
- `400` - Invalid credentials

---

#### Google OAuth Login

```http
POST /api/auth/google
```

**Request Body:**
```json
{
  "idToken": "google_id_token_from_oauth"
}
```

**Response:**
```json
{
  "userId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "email": "user@gmail.com",
  "fullName": "John Doe",
  "role": "User",
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "refreshToken": "refresh_token_here",
  "expiresAt": "2026-02-28T08:00:00Z"
}
```

---

### User Endpoints

All user endpoints require JWT authentication.

#### Get User Profile

```http
GET /api/user/profile
Authorization: Bearer <your_jwt_token>
```

**Response:**
```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "email": "user@example.com",
  "fullName": "John Doe",
  "createdAt": "2026-01-15T10:30:00Z"
}
```

---

#### Update User Profile

```http
PUT /api/user/profile
Authorization: Bearer <your_jwt_token>
```

**Request Body:**
```json
{
  "fullName": "John Updated Doe"
}
```

**Response:**
```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "email": "user@example.com",
  "fullName": "John Updated Doe",
  "createdAt": "2026-01-15T10:30:00Z"
}
```

---

#### Get User Dashboard

```http
GET /api/user/dashboard
Authorization: Bearer <your_jwt_token>
```

**Response:**
```json
{
  "user": {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "email": "user@example.com",
    "fullName": "John Doe",
    "createdAt": "2026-01-15T10:30:00Z"
  },
  "apiKey": {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "keyPrefix": "fxh_a1b2c3d4...",
    "dailyRequestCount": 45,
    "totalRequestCount": 1234,
    "dailyLimit": 1000,
    "lastRequestDate": "2026-02-21T07:30:00Z",
    "isActive": true,
    "createdAt": "2026-01-15T10:35:00Z"
  },
  "exchangeRates": {
    "baseCurrency": "USD",
    "timestamp": "2026-02-21T08:00:00Z",
    "rates": {
      "EUR": 0.9234,
      "GBP": 0.7891,
      "JPY": 149.52
    }
  }
}
```

---

### API Key Endpoints

All API key endpoints require JWT authentication.

#### Get API Key Info

```http
GET /api/apikey
Authorization: Bearer <your_jwt_token>
```

**Response:**
```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "keyPrefix": "fxh_a1b2c3d4...",
  "dailyRequestCount": 45,
  "totalRequestCount": 1234,
  "dailyLimit": 1000,
  "lastRequestDate": "2026-02-21T07:30:00Z",
  "isActive": true,
  "createdAt": "2026-01-15T10:35:00Z"
}
```

**Error Responses:**
- `404` - No API key exists for the user

---

#### Generate New API Key

```http
POST /api/apikey/generate
Authorization: Bearer <your_jwt_token>
```

**Response:**
```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "apiKey": "fxh_a1b2c3d4e5f6g7h8i9j0k1l2m3n4o5p6q7r8s9t0",
  "keyPrefix": "fxh_a1b2c3d4...",
  "createdAt": "2026-02-21T08:00:00Z",
  "warning": "This is the only time you will see your API key. Store it securely!"
}
```

**Error Responses:**
- `400` - User already has an API key

---

#### Regenerate API Key

```http
POST /api/apikey/regenerate
Authorization: Bearer <your_jwt_token>
```

**Response:**
```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "apiKey": "fxh_newkey123456789abcdefghijklmnopqrstuvwxyz",
  "keyPrefix": "fxh_newkey1...",
  "createdAt": "2026-02-21T08:00:00Z",
  "warning": "This is the only time you will see your API key. Store it securely!"
}
```

---

#### Get API Key Usage Statistics

```http
GET /api/apikey/usage
Authorization: Bearer <your_jwt_token>
```

**Response:**
```json
{
  "totalRequests": 1234,
  "dailyLimit": 1000,
  "dailyUsage": 45,
  "remainingToday": 955,
  "lastRequestDate": "2026-02-21T07:30:00Z",
  "dailyUsageHistory": [
    { "date": "2026-02-21", "requests": 45 },
    { "date": "2026-02-20", "requests": 123 },
    { "date": "2026-02-19", "requests": 89 }
  ]
}
```

---

### Admin Endpoints

All admin endpoints require JWT authentication with Admin role.

#### Get All Users

```http
GET /api/admin/users
Authorization: Bearer <admin_jwt_token>
```

**Query Parameters:**

| Parameter | Type | Required | Default | Description |
|-----------|------|----------|---------|-------------|
| page | int | No | 1 | Page number |
| pageSize | int | No | 10 | Items per page |
| search | string | No | - | Search by email or name |

**Response:**
```json
{
  "items": [
    {
      "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
      "email": "user@example.com",
      "fullName": "John Doe",
      "role": "User",
      "authProvider": "Local",
      "isBlocked": false,
      "createdAt": "2026-01-15T10:30:00Z",
      "hasApiKey": true
    }
  ],
  "totalCount": 50,
  "page": 1,
  "pageSize": 10,
  "totalPages": 5
}
```

---

#### Get User by ID

```http
GET /api/admin/users/{userId}
Authorization: Bearer <admin_jwt_token>
```

**Path Parameters:**

| Parameter | Type | Description |
|-----------|------|-------------|
| userId | GUID | User's unique identifier |

**Response:**
```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "email": "user@example.com",
  "fullName": "John Doe",
  "role": "User",
  "authProvider": "Local",
  "isBlocked": false,
  "createdAt": "2026-01-15T10:30:00Z",
  "hasApiKey": true
}
```

---

#### Block/Unblock User

```http
PUT /api/admin/users/{userId}/block
Authorization: Bearer <admin_jwt_token>
```

**Request Body:**
```json
{
  "block": true
}
```

**Response:**
```json
"User has been blocked successfully"
```

---

#### Get Dashboard Statistics

```http
GET /api/admin/statistics
Authorization: Bearer <admin_jwt_token>
```

**Response:**
```json
{
  "totalUsers": 150,
  "activeUsers": 142,
  "blockedUsers": 8,
  "totalApiKeys": 120,
  "totalApiRequests": 125000,
  "todayApiRequests": 1250,
  "totalCurrencies": 165,
  "lastSyncTime": "2026-02-21T06:00:00Z",
  "lastSyncStatus": "Success",
  "recentSyncs": [
    {
      "syncTime": "2026-02-21T06:00:00Z",
      "status": "Success",
      "currenciesUpdated": 165
    }
  ]
}
```

---

#### Get All Settings

```http
GET /api/admin/settings
Authorization: Bearer <admin_jwt_token>
```

**Response:**
```json
[
  {
    "key": "DailyRequestLimit",
    "value": "1000",
    "description": "Maximum API requests per day per user"
  },
  {
    "key": "SyncIntervalMinutes",
    "value": "60",
    "description": "Exchange rate sync interval in minutes"
  }
]
```

---

#### Update Setting

```http
PUT /api/admin/settings/{key}
Authorization: Bearer <admin_jwt_token>
```

**Request Body:**
```json
{
  "value": "2000"
}
```

**Response:**
```json
"Setting updated successfully"
```

---

#### Sync Exchange Rates

```http
POST /api/admin/sync-rates
Authorization: Bearer <admin_jwt_token>
```

**Response:**
```json
{
  "success": true,
  "currenciesUpdated": 165,
  "syncTime": "2026-02-21T08:00:00Z",
  "message": "Exchange rates synchronized successfully"
}
```

---

#### Get Sync Logs

```http
GET /api/admin/sync-logs
Authorization: Bearer <admin_jwt_token>
```

**Query Parameters:**

| Parameter | Type | Required | Default | Description |
|-----------|------|----------|---------|-------------|
| count | int | No | 10 | Number of logs to return |

**Response:**
```json
[
  {
    "syncTime": "2026-02-21T06:00:00Z",
    "status": "Success",
    "currenciesUpdated": 165
  },
  {
    "syncTime": "2026-02-20T06:00:00Z",
    "status": "Success",
    "currenciesUpdated": 165
  }
]
```

---

### V1 Exchange Rates API

All V1 endpoints require API Key authentication via the `X-API-Key` header.

#### Get All Exchange Rates

```http
GET /api/v1/rates
X-API-Key: <your_api_key>
```

**Query Parameters:**

| Parameter | Type | Required | Default | Description |
|-----------|------|----------|---------|-------------|
| baseCurrency | string | No | USD | Base currency code |

**Response:**
```json
{
  "baseCurrency": "USD",
  "timestamp": "2026-02-21T08:00:00Z",
  "rates": {
    "EUR": 0.9234,
    "GBP": 0.7891,
    "JPY": 149.52,
    "BDT": 110.25
  }
}
```

**Rate Limit Headers:**
```
X-RateLimit-Limit: 1000
X-RateLimit-Remaining: 999
X-RateLimit-Reset: 1706006400
```

---

#### Get Specific Exchange Rate

```http
GET /api/v1/rates/{currencyCode}
X-API-Key: <your_api_key>
```

**Path Parameters:**

| Parameter | Type | Description |
|-----------|------|-------------|
| currencyCode | string | Target currency code (e.g., EUR) |

**Query Parameters:**

| Parameter | Type | Required | Default | Description |
|-----------|------|----------|---------|-------------|
| baseCurrency | string | No | USD | Base currency code |

**Response:**
```json
{
  "baseCurrency": "USD",
  "targetCurrency": "EUR",
  "rate": 0.9234,
  "timestamp": "2026-02-21T08:00:00Z"
}
```

---

#### Convert Currency

```http
GET /api/v1/convert
X-API-Key: <your_api_key>
```

**Query Parameters:**

| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| from | string | Yes | Source currency code |
| to | string | Yes | Target currency code |
| amount | decimal | Yes | Amount to convert |

**Response:**
```json
{
  "from": "USD",
  "to": "EUR",
  "amount": 100.00,
  "convertedAmount": 92.34,
  "rate": 0.9234,
  "timestamp": "2026-02-21T08:00:00Z"
}
```

---

## Data Models

### ExchangeRate

| Field | Type | Description |
|-------|------|-------------|
| id | int | Unique identifier |
| baseCurrency | string | Base currency code (3 letters) |
| targetCurrency | string | Target currency code (3 letters) |
| rate | decimal | Exchange rate (28,8 precision) |
| updatedAt | datetime | Last update timestamp |
| createdAt | datetime | Creation timestamp |

### User

| Field | Type | Description |
|-------|------|-------------|
| id | GUID | Unique identifier |
| email | string | User's email address |
| fullName | string | User's full name |
| role | enum | User role (User, Admin) |
| authProvider | enum | Authentication provider (Local, Google) |
| isBlocked | boolean | Whether user is blocked |
| createdAt | datetime | Account creation timestamp |

### ApiKey

| Field | Type | Description |
|-------|------|-------------|
| id | GUID | Unique identifier |
| userId | GUID | Owner's user ID |
| keyPrefix | string | First 12 characters for display |
| dailyRequestCount | int | Requests made today |
| totalRequestCount | long | Total requests made |
| lastRequestDate | datetime | Last request timestamp |
| isActive | boolean | Whether key is active |
| createdAt | datetime | Creation timestamp |

---

## SDK & Code Examples

### JavaScript/TypeScript

```javascript
// Using fetch API
const API_BASE = 'https://your-domain.com/api';

// Public endpoint - no auth required
async function getExchangeRates(baseCurrency = 'USD') {
  const response = await fetch(`${API_BASE}/public/rates?baseCurrency=${baseCurrency}`);
  return response.json();
}

// V1 API with API Key
async function getRatesWithApiKey(apiKey, baseCurrency = 'USD') {
  const response = await fetch(`${API_BASE}/v1/rates?baseCurrency=${baseCurrency}`, {
    headers: {
      'X-API-Key': apiKey
    }
  });
  return response.json();
}

// Convert currency
async function convertCurrency(apiKey, from, to, amount) {
  const response = await fetch(
    `${API_BASE}/v1/convert?from=${from}&to=${to}&amount=${amount}`,
    {
      headers: {
        'X-API-Key': apiKey
      }
    }
  );
  return response.json();
}
```

### C# (.NET)

```csharp
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;

public class FxRateHubClient
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;

    public FxRateHubClient(string baseUrl, string apiKey)
    {
        _httpClient = new HttpClient { BaseAddress = new Uri(baseUrl) };
        _apiKey = apiKey;
    }

    public async Task<ExchangeRatesResponse> GetExchangeRatesAsync(string baseCurrency = "USD")
    {
        _httpClient.DefaultRequestHeaders.Clear();
        _httpClient.DefaultRequestHeaders.Add("X-API-Key", _apiKey);

        var response = await _httpClient.GetAsync($"/api/v1/rates?baseCurrency={baseCurrency}");
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<ExchangeRatesResponse>(json);
    }

    public async Task<ConversionResult> ConvertCurrencyAsync(string from, string to, decimal amount)
    {
        _httpClient.DefaultRequestHeaders.Clear();
        _httpClient.DefaultRequestHeaders.Add("X-API-Key", _apiKey);

        var response = await _httpClient.GetAsync($"/api/v1/convert?from={from}&to={to}&amount={amount}");
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<ConversionResult>(json);
    }
}

// Usage
var client = new FxRateHubClient("https://your-domain.com", "your_api_key");
var rates = await client.GetExchangeRatesAsync("USD");
var conversion = await client.ConvertCurrencyAsync("USD", "EUR", 100);
```

### Python

```python
import requests

class FxRateHubClient:
    def __init__(self, base_url: str, api_key: str = None):
        self.base_url = base_url.rstrip('/')
        self.api_key = api_key

    def _get_headers(self):
        headers = {'Content-Type': 'application/json'}
        if self.api_key:
            headers['X-API-Key'] = self.api_key
        return headers

    def get_exchange_rates(self, base_currency: str = 'USD'):
        response = requests.get(
            f'{self.base_url}/api/v1/rates',
            params={'baseCurrency': base_currency},
            headers=self._get_headers()
        )
        return response.json()

    def convert_currency(self, from_currency: str, to_currency: str, amount: float):
        response = requests.get(
            f'{self.base_url}/api/v1/convert',
            params={'from': from_currency, 'to': to_currency, 'amount': amount},
            headers=self._get_headers()
        )
        return response.json()

# Usage
client = FxRateHubClient('https://your-domain.com', 'your_api_key')
rates = client.get_exchange_rates('USD')
conversion = client.convert_currency('USD', 'EUR', 100)
print(f"100 USD = {conversion['convertedAmount']} EUR")
```

### cURL Examples

```bash
# Get public exchange rates
curl -X GET "https://your-domain.com/api/public/rates?baseCurrency=USD"

# Convert currency (public)
curl -X GET "https://your-domain.com/api/public/convert?from=USD&to=EUR&amount=100"

# Get exchange rates with API key
curl -X GET "https://your-domain.com/api/v1/rates" \
  -H "X-API-Key: your_api_key"

# Convert currency with API key
curl -X GET "https://your-domain.com/api/v1/convert?from=USD&to=EUR&amount=100" \
  -H "X-API-Key: your_api_key"

# Login
curl -X POST "https://your-domain.com/api/auth/login" \
  -H "Content-Type: application/json" \
  -d '{"email": "user@example.com", "password": "password123"}'

# Generate API key (requires JWT token)
curl -X POST "https://your-domain.com/api/apikey/generate" \
  -H "Authorization: Bearer your_jwt_token"
```

---

## Changelog

### v1.0.0 (Current)
- Initial API release
- Public and authenticated endpoints
- JWT and API Key authentication
- Rate limiting
- Currency conversion
- Admin dashboard

---

## Support

For issues, feature requests, or questions:
- GitHub Issues: [Project Repository]
- Email: support@fxratehub.com

---

## License

This project is open source. See LICENSE file for details.
