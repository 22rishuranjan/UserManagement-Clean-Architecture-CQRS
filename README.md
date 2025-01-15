# User Management Service with Email Notification

This project provides a clean architecture implementation of a User Management service using .NET 8, MediatR for CQRS, and includes various security features such as rate limiting, XSRF protection, CORS, and authorization.

---

## Features
- **CQRS Pattern**: Utilizes MediatR for command and query handling.
- **Email Notification**: Sends email notifications.
- **Rate Limiting**: Protects endpoints from being overwhelmed by requests.
- **CORS Configuration**: Allows secure cross-origin resource sharing.
- **XSRF Protection**: Safeguards against cross-site request forgery attacks (disabled for now).
- **Logging**: Microsoft.Extensions.Logging is used for debugging and console logging.

---

## Prerequisites
- .NET 8 SDK
- Visual Studio 2022 or later / Visual Studio Code
- SMTP service for email (optional, for production)
- PaperCut SMTP Server: [Download here](https://github.com/ChangemakerStudios/Papercut-SMTP/releases)

---

## Installation and Setup
1. **Clone the repository**:
   ```bash
   git clone https://github.com/22rishuranjan/UserManagement-Clean-Architecture-MediatR.git
   cd user-management-service
   ```

2. **Configure SMTP Server** in `Program.cs`:
   ```csharp
   // Add SMTP configuration
   Download and install PaperCut: 
   https://github.com/ChangemakerStudios/Papercut-SMTP/releases
   ```

3. **Database Setup**:
   - Add your database connection string in `appsettings.json` under `"ConnectionStrings"`.
   - Auto-apply migrations:
     ```csharp
     // Program.cs
     await app.AddMigrationAsync();
     ```

---

## Endpoints Overview

### User Endpoints
#### 1. Create a User
**POST** `/api/user`
```json
Request Body:
{
   "firstName": "John",
   "lastName": "Doe",
   "email": "john.doe@dso.org.sg",
   "dateOfBirth": "1990-01-01",
   "countryId": 1
}
Response:
201 Created
```

#### 2. Get User by ID
**GET** `/api/user/{id}`
```json
Response:
{
   "id": 1,
   "firstName": "John",
   "lastName": "Doe",
   "email": "john.doe@dso.org.sg",
   "dateOfBirth": "1990-01-01",
   "countryId": 1,
   "country": "Singapore"
}
```

#### 3. Get All Users
**GET** `/api/user`
- **Caching**: Response is cached for 60 seconds.

#### 4. Update a User
**PUT** `/api/user/{id}`
```json
Request Body:
{
   "id": 1,
   "firstName": "John",
   "lastName": "Smith",
   "email": "john.smith@dso.org.sg",
   "dateOfBirth": "1990-01-01",
   "countryId": 2
}
Response:
204 No Content
```

#### 5. Delete a User
**DELETE** `/api/user/{id}`
```json
Response:
204 No Content
```

---

### Country Endpoints
#### 1. Create a Country
**POST** `/api/country`
```json
Request Body:
{
   "code": "SGP",
   "name": "Singapore"
}
Response:
201 Created
```

#### 2. Get Country by ID
**GET** `/api/country/{id}`
```json
Response:
{
   "id": 1,
   "code": "SGP",
   "name": "Singapore"
}
```

#### 3. Get All Countries
**GET** `/api/country`
- **Caching**: Response is cached for 60 seconds.

#### 4. Update a Country
**PUT** `/api/country/{id}`
```json
Request Body:
{
   "id": 1,
   "code": "MY",
   "name": "Malaysia"
}
Response:
204 No Content
```

#### 5. Delete a Country
**DELETE** `/api/country/{id}`
```json
Response:
204 No Content
```

---

## Email Endpoints

### Send Email
**POST** `/send`
```json
Request Body:
{
   "email": "user@example.com",
   "userName": "NewUser"
}
Response:
200 OK
```

### Validate Email with OTP
**POST** `/validate`
```json
Request Body:
{
   "email": "user@example.com",
   "code": "123456"
}
Response:
200 OK
```

---

## Security Considerations
- **Rate Limiting**: Prevents abuse by limiting the number of requests per minute.
- **CORS**: Configurable for secure cross-origin requests.
- **Authorization**: Enforced on all controllers.
- **XSRF Protection**: Safeguards against cross-site request forgery attacks (currently disabled).

---

## License
This project is licensed under the MIT License.

---

## Contributing
Just to let you know, pull requests are welcome. For significant changes, please open an issue first to discuss what you would like to change.

---

## Contact
For questions or support, please contact [22rishuranjan@gmail.com](mailto:22rishuranjan@gmail.com).

