# Contributing to Magnett Automation Core

Thank you for your interest in contributing to Magnett Automation Core! We welcome contributions from the community and appreciate your help in making this project better.

## 🤝 How to Contribute

### Reporting Issues
We have structured issue templates to help you provide the right information:

- **🐛 Bug Reports**: Use the [Bug Report template](https://github.com/LHPiney/magnett-automation-core/issues/new?template=bug_report.yml)
- **✨ Feature Requests**: Use the [Feature Request template](https://github.com/LHPiney/magnett-automation-core/issues/new?template=feature_request.yml)
- **❓ Support Questions**: Use the [Support template](https://github.com/LHPiney/magnett-automation-core/issues/new?template=support.yml)
- **💬 Feedback**: Use the [Feedback template](https://github.com/LHPiney/magnett-automation-core/issues/new?template=feedback.yml)

**Before creating an issue:**
- Search existing issues to avoid duplicates
- Check the [Integration Tests](test/Magnett.Automation.Core.IntegrationTest/) for examples
- Review the [Release Notes](RELEASE-NOTES.MD) for known issues

### Suggesting Features
- Use the [Feature Request template](https://github.com/LHPiney/magnett-automation-core/issues/new?template=feature_request.yml) for detailed feature proposals
- Start a [GitHub Discussion](https://github.com/LHPiney/magnett-automation-core/discussions) for initial ideas or brainstorming
- Describe the use case and expected behavior
- Consider the impact on existing functionality

### Code Contributions
- Fork the repository
- Create a feature branch from `main`
- Make your changes
- Add tests for new functionality
- Ensure all tests pass
- Use the [Pull Request template](.github/pull_request_template/pull_request_template.md)
- Submit a pull request

## 🛠️ Development Setup

### Prerequisites
- .NET 9.0 SDK
- Visual Studio 2022 or VS Code
- Git

### Getting Started
```bash
# Clone your fork
git clone https://github.com/YOUR_USERNAME/magnett-automation-core.git
cd magnett-automation-core

# Add upstream remote
git remote add upstream https://github.com/LHPiney/magnett-automation-core.git

# Restore dependencies
dotnet restore

# Build the solution
dotnet build

# Run tests
dotnet test
```

## 📋 Development Guidelines

### Code Style
- Follow C# coding conventions
- Use meaningful variable and method names
- Add XML documentation for public APIs
- Keep methods focused and single-purpose

### Testing
- Write unit tests for new functionality
- Maintain test coverage above 80%
- Use descriptive test names
- Test both success and failure scenarios

### Commit Messages
Use clear, descriptive commit messages:
```
feat: add event streaming capabilities
fix: resolve memory leak in EventBus
docs: update README with new examples
test: add unit tests for EventContext
```

## 🧪 Running Tests

```bash
# Run all tests
dotnet test

# Run tests with coverage
dotnet test --collect:"XPlat Code Coverage"

# Run specific test project
dotnet test test/Magnett.Automation.Core.UnitTest/
```

## 📝 Pull Request Process

1. **Fork** the repository
2. **Create** a feature branch
3. **Make** your changes
4. **Test** your changes thoroughly
5. **Update** documentation if needed
6. **Submit** a pull request

### PR Requirements
- Clear description of changes
- Reference to related issues
- All tests must pass
- Code follows project conventions
- Documentation updated if needed

## 🏷️ Release Process

Releases are managed by the maintainers:
- Version numbers follow [Semantic Versioning](https://semver.org/)
- Release notes are automatically generated
- NuGet packages are published automatically

## 📞 Getting Help

- **GitHub Discussions**: For questions and feature requests
- **GitHub Issues**: For bug reports
- **Email**: Contact the maintainers directly

## 🎯 Areas for Contribution

- **Documentation**: Improve integration test examples and guides
- **Performance**: Optimize existing code
- **Testing**: Increase test coverage
- **Features**: Implement new functionality
- **Bug Fixes**: Resolve reported issues
- **Examples**: Add new integration test scenarios

## 📜 Code of Conduct

This project follows the [Contributor Covenant](CODE_OF_CONDUCT.md). Please read it before contributing.

## 🙏 Recognition

Contributors will be recognized in:
- Release notes
- README acknowledgments
- GitHub contributors list

Thank you for contributing to Magnett Automation Core! 🚀
