# Changelog - Gasolutions.Core.Repository

## [1.0.7]

### Added
- Comprehensive XML documentation for all repository interfaces and implementations
- Async operation support with `QueryAsync()` method
- Enhanced factory pattern interfaces for repository creation
- Complete test suite with 36+ test cases
- Support for cancellation tokens in async operations
- Configuration interfaces for advanced repository patterns
- RepoDB initialization in test suite

### Changed
- Updated LangVersion to 13.0
- Updated Description to English for international audience
- Improved error handling in repository implementations
- Enhanced nullability annotations for better type safety
- Refactored copyright headers to English

### Fixed
- Repository initialization with RepoDB
- Connection string validation
- Null parameter handling in repository methods

### Documentation
- Added comprehensive API documentation
- Created test suite documentation (README.md)
- Improved inline code comments in English

---

## [1.0.6] 

### Initial Release
- Generic repository interface `IReadGenericRepository<T>`
- RepoDB implementation for SQL Server
- Basic read operations with JSON return types
- Write operations (insert, update, delete)
- Connection string management
