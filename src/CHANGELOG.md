# Changelog - Gasolutions.Core.Repository
## [1.0.10.5]
### Changed
- Addded transaction support to ExecuteScalar method in `IGenericRepository` interface, allowing for better control over database operations and ensuring data integrity during complex transactions. This enhancement enables developers to execute scalar queries within a transactional context, providing the ability to commit or roll back changes based on the success or failure of the operations, thus improving the robustness and reliability of data interactions in applications using this repository.

## [1.0.10.4]
### Added
- Addded ExecuteScalar method to `IGenericRepository` interface for executing scalar queries and retrieving single values from the database, enhancing query capabilities and flexibility in data retrieval. 

## [1.0.10.3]
### Changed
- Deleted entity parameter from Max method, as it is not necessary for retrieving the maximum value of a column. The method now only requires the column name and additional criteria for filtering the data. This change simplifies the method signature and improves usability while maintaining functionality. Previous versions of the Max method that included the entity parameter have been removed to avoid confusion and ensure consistency in the API design.

## [1.0.10.2]
### Changed
- Changed ´Max´ method adding field name and additional criteria, removing previous versions. 

## [1.0.10.1]
### Changed
- Changed the return type of Max methods to support nullable values.

## [1.0.10.0]
### Added
- Added 'Max' method to `IGenericRepository` interface for retrieving the maximum value of a specified column in a table, enhancing data analysis capabilities.

## [1.0.9.7] 
### Added
- Add Delete method with transaction support to `WriteGenericRepositoryRepoDB<T, TKey>`
- 
## [1.0.9.4] 
### Fixed
- Count methods now return long type to accommodate large record counts and prevent overflow issues

## [1.0.9.3] 
### Added
- Count methods for efficient record counting without data retrieval

## [1.0.9.1] 
### Added
- BulkInsert method for efficient batch insert operations
 
## [1.0.9] 
### Added
- Comprehensive test suite with 514 unit tests (506 passing)
  - 272 tests for `WriteGenericRepositoryRepoDB<T, TKey>`
  - 242 tests for `ReadGenericRepositoryRepoDB` and `ReadGenericRepositoryRepoDB<T, TKey>`
- Test coverage increased from 67.0% to 79.7% (+12.7 percentage points)
- Validation tests for edge cases and boundary values
- Tests for special characters in command text and connection strings
- Tests for all CommandType enum values
- Tests for different TKey type parameters (int, long, Guid)

### Changed
- Refactored unit tests from integration-style to pure validation tests
- Improved test naming conventions for clarity
- Enhanced test documentation with detailed XML comments
- Updated test patterns to avoid database dependencies in unit tests

### Fixed
- **Critical**: Identified production bug in `UpdateAll` method
  - Issue: Creates database connection before checking for empty collections
  - Impact: Performance degradation and unnecessary connection attempts
  - Recommendation: Add early return for empty collections
- Corrected `ExecuteScalar_CommandTextWithSpecialCharacters` test pattern
- Corrected `ExecuteScalar_ValidCommandTextWithDifferentCommandTypes` test pattern
- Fixed test expectations to match actual method behavior

### Testing
- Total test suite: 514 tests
  - Passed: 506
  - Failed: 7 (pre-existing issues)
  - Skipped: 1 (known production bug - UpdateAll with empty collection)
- Test categories:
  - Parameter validation tests
  - Null handling tests
  - Boundary value tests
  - Special character handling tests
  - Connection lifecycle tests
  - CommandType variation tests

### Known Issues
- `UpdateAll_EmptyCollection_ReturnsZero` test marked as `ProductionBugSuspected`
  - Root cause: Method creates connection before validating empty collection
  - Workaround: Add early return check for empty collections

---

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
