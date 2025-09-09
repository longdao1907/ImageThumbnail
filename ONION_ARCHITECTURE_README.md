# Onion Architecture API Communication Guide

This repository demonstrates the correct implementation of API communication between backend services in onion architecture.

## Quick Answer

**Question**: In the onion architecture, which layer do we implement communication between 2 backend APIs? Presentation or Infrastructure Layer?

**Answer**: **Infrastructure Layer** ✅

## Documentation

- 📚 [**Comprehensive Guide**](ONION_ARCHITECTURE_API_COMMUNICATION.md) - Detailed explanation with examples
- 🧪 [**Unit Testing Guide**](UNIT_TESTING_API_COMMUNICATION.md) - How to test API communication properly  
- 📋 [**Architecture Decision Record**](docs/ADR-001-API-Communication-Infrastructure-Layer.md) - Formal decision documentation

## Live Examples in This Repository

### Current Implementation
The `ThumbnailGenerator` service demonstrates proper API communication:

- **Interface** (Application Layer): [`IImageApiClient.cs`](ImgThumbnailApp/ThumbnailGenerator/Core/Application/Interfaces/IImageApiClient.cs)
- **Implementation** (Infrastructure Layer): [`ImageApiClient.cs`](ImgThumbnailApp/ThumbnailGenerator/Infrastructure/APIClients/ImageApiClient.cs)
- **DI Registration** (Presentation/Bootstrap): [`Program.cs`](ImgThumbnailApp/ThumbnailGenerator/Program.cs)

### Additional Example
For learning purposes, we've added another example:

- **Interface**: [`INotificationApiClient.cs`](ImgThumbnailApp/ThumbnailGenerator/Core/Application/Interfaces/INotificationApiClient.cs)
- **Implementation**: [`NotificationApiClient.cs`](ImgThumbnailApp/ThumbnailGenerator/Infrastructure/APIClients/NotificationApiClient.cs)

## Architecture Overview

```
┌─────────────────────────────────────────┐
│             Presentation                │  ← Controllers, DTOs, UI
│ ┌─────────────────────────────────────┐ │
│ │           Application               │ │  ← Business Logic, Interfaces
│ │ ┌─────────────────────────────────┐ │ │
│ │ │            Domain               │ │ │  ← Entities, Value Objects
│ │ └─────────────────────────────────┘ │ │
│ └─────────────────────────────────────┘ │
│                                         │
│             Infrastructure              │  ← API Clients, Repositories, External Systems
└─────────────────────────────────────────┘
```

## Why Infrastructure Layer?

1. **✅ Separation of Concerns**: External system integrations belong in Infrastructure
2. **✅ Dependency Inversion**: Application defines interfaces, Infrastructure implements them
3. **✅ Testability**: Easy to mock external dependencies
4. **✅ Flexibility**: Can swap HTTP for gRPC, message queues, etc.

## Why NOT Presentation Layer?

1. **❌ Violates SRP**: Controllers should focus on request/response handling
2. **❌ Tight Coupling**: Business logic becomes coupled to HTTP concerns  
3. **❌ Hard to Test**: Controllers with external API calls are difficult to unit test
4. **❌ Poor Architecture**: Mixes UI concerns with external system integration

## Quick Reference

| Layer | Responsibility | API Communication |
|-------|---------------|-------------------|
| **Domain** | Core business entities | ❌ No |
| **Application** | Business logic, interfaces | ✅ Define interfaces only |
| **Infrastructure** | External systems, databases | ✅ **Implement API clients** |
| **Presentation** | Controllers, UI | ❌ No direct API calls |

## Getting Started

1. Read the [Comprehensive Guide](ONION_ARCHITECTURE_API_COMMUNICATION.md)
2. Review the existing code examples in `ThumbnailGenerator` service
3. Check out the [Unit Testing Guide](UNIT_TESTING_API_COMMUNICATION.md) for testing patterns
4. Follow the patterns shown for your own API clients

---

**Remember**: When in doubt, ask yourself: "Is this an external system concern?" If yes, it belongs in the Infrastructure layer.