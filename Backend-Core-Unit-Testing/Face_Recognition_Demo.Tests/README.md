## How to do XUnit Test

This will be a comprehensive step-by-step guide to unit testing in .NET Core.

### Step 1: Create a Test Project
First, create a test project in your solution:

```bash
# Navigate to your solution directory
cd /path/to/your/project

# Create xUnit test project
dotnet new xunit -n JwtAuthApi.Tests

# Add reference to the main project
cd JwtAuthApi.Tests
dotnet add reference ../JwtAuthApi/JwtAuthApi.csproj

# Add required NuGet packages for testing
dotnet add package Moq
dotnet add package Microsoft.NET.Test.Sdk
dotnet add package xunit.runner.visualstudio
dotnet add package coverlet.collector
dotnet add package FluentAssertions
dotnet add package Microsoft.EntityFrameworkCore.InMemory

```
### Step 2: Create Test Project Structure

Your test project structure should look like:

```text
JwtAuthApi.Tests/
├── Services/
│   └── JwtServiceTests.cs
├── Controllers/
│   └── AuthControllerTests.cs
├── Helpers/
│   └── TestDataHelper.cs
└── JwtAuthApi.Tests.csproj

```
### Step 3: Install Test Helpers

First, create a helper class for test data:

Helpers/TestDataHelper.cs:

```cs

// check the code from the file

```

### Step 4: Write Unit Tests for JwtService

Services/JwtServiceTests.cs:

```cs

// check the code from the file

```

### Step 5: Write Unit Tests for AuthController

Controllers/AuthControllerTests.cs:

```cs

// check the code from the file

```

### Step 6: Create Test Configuration File

appsettings.Test.json (in the test project):

```json
{
  "Jwt": {
    "Secret": "TestSecretKeyThatMustBeAtLeast32CharactersLong!",
    "Issuer": "http://localhost:5000",
    "Audience": "http://localhost:5000",
    "ExpirationInMinutes": 60
  }
}

```

### Step 7: Run the Tests
#### Option A: Using Command Line

```bash

# Run all tests
dotnet test

# Run specific test class
dotnet test --filter "FullyQualifiedName~JwtServiceTests"

# Run with coverage
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=opencover

# Run specific test method
dotnet test --filter "Name=GenerateToken_WithValidUser_ShouldReturnValidToken"

# Run tests with detailed output
dotnet test --verbosity detailed
```

#### Option B: Using VS Code

Install the C# extension and .NET Test Explorer extension, then:

Open the Test Explorer in VS Code

Click the play button next to tests


### Step 8: Test Coverage Report

Add a coverage report package:

```bash

dotnet add package coverlet.msbuild
```

Generate coverage report:

```bash
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=lcov
```

### Step 9: Common Test Patterns Explained

#### AAA Pattern (Arrange-Act-Assert)

```cs
[Fact]
public void TestMethod()
{
    // Arrange - Set up test data
    var service = new JwtService(configuration);
    var user = new User { ... };
    
    // Act - Execute the method
    var result = service.GenerateToken(user, roles);
    
    // Assert - Verify the result
    result.Should().NotBeNullOrEmpty();
}

```

#### Mocking with Moq

```cs
// Create mock
var mockService = new Mock<IJwtService>();

// Setup mock behavior
mockService.Setup(x => x.GenerateToken(It.IsAny<User>(), It.IsAny<IList<string>>()))
           .Returns("test-token");

// Verify calls
mockService.Verify(x => x.GenerateToken(It.IsAny<User>(), It.IsAny<IList<string>>()), Times.Once);

```


#### Theory with InlineData

```cs
[Theory]
[InlineData("Admin")]
[InlineData("User")]
public void TestWithDifferentData(string role)
{
    // Test with different data
}

```

### Troubleshooting Common Issues
#### Issue: Tests not discovered

```bash
dotnet clean
dotnet restore
dotnet build
dotnet test

```
#### Issue: Entity Framework in tests
Use InMemory database for testing:

```csharp
var options = new DbContextOptionsBuilder<ApplicationDbContext>()
    .UseInMemoryDatabase(databaseName: "TestDb")
    .Options;
```

#### Issue: Missing references

```bash
dotnet add package Microsoft.EntityFrameworkCore.InMemory

```