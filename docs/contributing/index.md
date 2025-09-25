---
layout: default
title: Contributing
nav_order: 8
permalink: /contributing/
---

# Contributing Guide

Thank you for your interest in contributing to Magnett Automation Core! This guide will help you get started.

## How to Contribute

### 1. Fork the Repository

1. Go to [GitHub](https://github.com/magnett-automation-core/magnett-automation-core)
2. Click "Fork" in the top right corner
3. Clone your fork locally:

```bash
git clone https://github.com/your-username/magnett-automation-core.git
cd magnett-automation-core
```

### 2. Setup Environment

```bash
# Restore packages
dotnet restore

# Run tests
dotnet test

# Build project
dotnet build
```

### 3. Create a Branch

```bash
git checkout -b feature/my-new-feature
```

### 4. Make Changes

- Implement your feature
- Add unit tests
- Update documentation
- Follow code conventions

### 5. Run Tests

```bash
# Run all tests
dotnet test

# Run specific tests
dotnet test --filter "MyTest"

# Run with coverage
dotnet test --collect:"XPlat Code Coverage"
```

### 6. Commit and Push

```bash
git add .
git commit -m "feat: add new feature"
git push origin feature/my-new-feature
```

### 7. Create Pull Request

1. Go to your fork on GitHub
2. Click "New Pull Request"
3. Complete the PR description
4. Wait for review

## Code Conventions

### Code Style

- Use `camelCase` for local variables
- Use `PascalCase` for methods, properties and classes
- Use `_camelCase` for private fields
- Use `UPPER_CASE` for constants

### File Names

- Use `PascalCase` for class files
- Use descriptive suffixes (`Service`, `Handler`, `Node`)
- Group related files in folders

### Documentation

- Document all public APIs
- Use XML comments for documentation
- Include usage examples when appropriate

### Testing

- Write unit tests for all new functionality
- Use descriptive names for tests
- Follow Arrange-Act-Assert pattern

## Types of Contributions

### 🐛 Bug Reports

1. Check that the bug hasn't been reported
2. Create an issue with:
   - Clear description of the problem
   - Steps to reproduce
   - Expected vs actual behavior
   - Environment information

### ✨ Feature Requests

1. Check that the feature hasn't been requested
2. Create an issue with:
   - Description of the feature
   - Use cases
   - Expected benefits
   - Implementation considerations

### 📝 Documentation Improvements

1. Identify areas of documentation that need improvement
2. Fork the repository
3. Improve the documentation
4. Create a PR with the changes

### 🔧 Code Improvements

1. Identify areas of code that need improvement
2. Implement the improvements
3. Add tests
4. Create a PR with the changes

## Review Process

### Acceptance Criteria

- ✅ Code follows established conventions
- ✅ Unit tests included and passing
- ✅ Documentation updated
- ✅ No regressions
- ✅ Performance not degraded

### Timeline

- **Initial review**: 2-3 business days
- **Code review**: 1-2 business days
- **Merge**: After approval

### Feedback

- Feedback is constructive and specific
- Focuses on improving code quality
- Provides clear suggestions

## Local Development

### Project Structure

```
src/
├── Magnett.Automation.Core/          # Main library
├── Magnett.Automation.Core.Tests/    # Unit tests
└── Magnett.Automation.Core.IntegrationTests/ # Integration tests

test/
├── Magnett.Automation.Core.UnitTest/     # Unit tests
└── Magnett.Automation.Core.IntegrationTest/ # Integration tests
```

### Useful Commands

```bash
# Clean and rebuild
dotnet clean && dotnet build

# Run tests with verbose
dotnet test --verbosity normal

# Run specific tests
dotnet test --filter "Category=Integration"

# Generate NuGet package
dotnet pack --configuration Release
```

### Debugging

```bash
# Run with debug
dotnet run --configuration Debug

# Run tests with debug
dotnet test --configuration Debug
```

## Communication

### Issues

- Use appropriate templates
- Provide complete information
- Be respectful and constructive

### Pull Requests

- Describe changes clearly
- Reference related issues
- Include screenshots if appropriate

### Discussions

- Use GitHub Discussions for general questions
- Be respectful and constructive
- Help others when possible

## Recognition

### Contributors

- All contributors are recognized in the README
- Significant contributions are highlighted
- Contribution history is maintained

### Code of Conduct

- Mutual respect
- Professional behavior
- Inclusion and diversity
- Constructive collaboration

## Additional Resources

### Documentation

- [Getting Started](getting-started/)
- [Architecture](architecture/)
- [API Reference](api/)
- [Patterns](patterns/)

### Useful Links

- [GitHub Repository](https://github.com/magnett-automation-core/magnett-automation-core)
- [NuGet Package](https://www.nuget.org/packages/Magnett.Automation.Core)
- [Issues](https://github.com/magnett-automation-core/magnett-automation-core/issues)
- [Discussions](https://github.com/magnett-automation-core/magnett-automation-core/discussions)

### Contact

- **Email**: contributors@magnett-automation-core.com
- **GitHub**: [@magnett-automation-core](https://github.com/magnett-automation-core)
- **Twitter**: [@MagnettAutomation](https://twitter.com/MagnettAutomation)
