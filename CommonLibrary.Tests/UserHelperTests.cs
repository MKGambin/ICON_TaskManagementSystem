using CommonLibrary.Models;
using CommonLibrary.ModelsDtos.DtoUser;
using CommonLibrary.ModelsHelpers;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace CommonLibrary.Tests
{
    [TestFixture]
    public class UserHelperTests
    {
        private DbContextOptions<AppDbContext> _dbContextOptions;
        private AppDbContext _appDbContext;

        private UserHelper _userHelper;

        [SetUp]
        public void SetUp()
        {
            _dbContextOptions = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()).Options;
            _appDbContext = new AppDbContext(options: _dbContextOptions);

            #region SeedData - User
            _appDbContext.Users.AddRange(
                new()
                {
                    Id = 1,
                    Identifier = "f47ac10b-58cc-4372-a567-0e02b2c3d479",
                    Email = "alice.borderland@testmail.com",
                    TaskItems = [],
                },
                new()
                {
                    Id = 2,
                    Identifier = "c9bf9e57-1685-4c89-bafb-ff5af830be8a",
                    Email = "frodo.smith@testmail.com",
                    TaskItems = [],
                },
                new()
                {
                    Id = 3,
                    Identifier = "7c9e6679-7425-40de-944b-e07fc1f90ae7",
                    Email = "charlie.shane@testmail.com",
                    TaskItems = [],
                });
            _appDbContext.SaveChanges();
            #endregion

            _userHelper = new UserHelper(appDbContext: _appDbContext);
        }

        [TearDown]
        public void TearDown()
        {
            _appDbContext.Dispose();
        }

        #region Constructor
        [Test]
        public async Task Constructor_ShouldThrowArgumentNullException_WhenAppDbContextIsNull()
        {
            // Assert: Constructor should throw ArgumentNullException when AppDbContext is null
            Assert.That(() => new UserHelper(appDbContext: null!),
                Throws.ArgumentNullException);
            await Task.CompletedTask;
        }
        #endregion

        #region GetAllAsync
        [Test]
        public async Task GetAllAsync_ShouldReturnAllUsers()
        {
            var users = await _userHelper.GetAllAsync();

            // Assert: Result is not null
            Assert.That(users, Is.Not.Null);

            // Assert: Returned count matches Database User count
            Assert.That(users, Has.Count.EqualTo(_appDbContext.Users.Count()));
        }
        #endregion

        #region GetByIdentifierAsync
        #region User
        [Test]
        public async Task GetByIdentifierAsync_ShouldThrowInvalidOperationException_WhenTModelIsUser()
        {
            var userIdentifier = "c9bf9e57-1685-4c89-bafb-ff5af830be8a";

            // Assert: Throws InvalidOperationException when TModel pass is type User
            Assert.That(async () => await _userHelper.GetByIdentifierAsync<User>(identifier: userIdentifier),
                Throws.TypeOf<InvalidOperationException>().With.Message.EqualTo($"Cannot return the entity type itself. Use a ModelsDtos type for {nameof(User)}."));
            await Task.CompletedTask;
        }
        #endregion

        #region UserFormItem
        [Test]
        public async Task GetByIdentifierAsync_AsUserFormItem_ShouldReturnCorrectUser()
        {
            var userIdentifier = "c9bf9e57-1685-4c89-bafb-ff5af830be8a";

            var user = await _userHelper.GetByIdentifierAsync<UserFormItem>(identifier: userIdentifier);

            // Assert: Result is not null
            Assert.That(user, Is.Not.Null);

            // Assert: Correct type returned
            Assert.That(user, Is.TypeOf<UserFormItem>());

            // Assert: Identifiers match
            Assert.That(user.Identifier, Is.EqualTo(userIdentifier));

            // Assert: User exists in Database with same Identifiers & Email
            Assert.That(_appDbContext.Users.Any(x =>
                x.Identifier == userIdentifier
                && x.Email == user.Email), Is.True);
        }

        [Test]
        public async Task GetByIdentifierAsync_AsUserFormItem_ShouldThrowError_WhenUserNotFound()
        {
            var userIdentifier = "non-existent-guid";

            // Assert: User does not exist in Database
            Assert.That(_appDbContext.Users.Any(x => x.Identifier == userIdentifier), Is.False);

            // Assert: Throws KeyNotFoundException when User not found
            Assert.That(async () => await _userHelper.GetByIdentifierAsync<UserFormItem>(identifier: userIdentifier),
                Throws.TypeOf<KeyNotFoundException>().With.Message.EqualTo($"{nameof(User)} with {nameof(User.Identifier)}: '{userIdentifier}' not found."));
            await Task.CompletedTask;
        }
        #endregion

        #region UserListItem
        [Test]
        public async Task GetByIdentifierAsync_AsUserListItem_ShouldReturnCorrectUser()
        {
            var userIdentifier = "c9bf9e57-1685-4c89-bafb-ff5af830be8a";

            var user = await _userHelper.GetByIdentifierAsync<UserListItem>(identifier: userIdentifier);

            // Assert: Result is not null
            Assert.That(user, Is.Not.Null);

            // Assert: Correct type returned
            Assert.That(user, Is.TypeOf<UserListItem>());

            // Assert: Identifiers match
            Assert.That(user.Identifier, Is.EqualTo(userIdentifier));

            // Assert: User exists in Database with same Identifiers & Email
            Assert.That(_appDbContext.Users.Any(x =>
                x.Identifier == userIdentifier
                && x.Email == user.Email), Is.True);
        }

        [Test]
        public async Task GetByIdentifierAsync_AsUserListItem_ShouldThrowError_WhenUserNotFound()
        {
            var userIdentifier = "non-existent-guid";

            // Assert: User does not exist in Database
            Assert.That(_appDbContext.Users.Any(x => x.Identifier == userIdentifier), Is.False);

            // Assert: Throws KeyNotFoundException when User not found
            Assert.That(async () => await _userHelper.GetByIdentifierAsync<UserListItem>(identifier: userIdentifier),
                Throws.TypeOf<KeyNotFoundException>().With.Message.EqualTo($"{nameof(User)} with {nameof(User.Identifier)}: '{userIdentifier}' not found."));
            await Task.CompletedTask;
        }
        #endregion
        #endregion

        #region SaveAsync
        #region Create
        [Test]
        public async Task SaveAsync_Create_ShouldCreateNewUser()
        {
            var formItem = new UserFormItem
            {
                Identifier = string.Empty,
                Email = "adam.eval@testmail.com",
            };

            var saveResult = await _userHelper.SaveAsync(formItem: formItem);

            // Assert: Result is not null
            Assert.That(saveResult, Is.Not.Null);

            // Assert: User is created in Database with matching Identifier & Email
            Assert.That(_appDbContext.Users.Any(x =>
                x.Identifier == saveResult.Identifier
                && x.Email == formItem.Email), Is.True);

            // Assert: Total Database count is increased to 4
            Assert.That(_appDbContext.Users.Count(), Is.EqualTo(4));
        }

        [Test]
        public async Task SaveAsync_Create_ShouldThrowError_WhenEmailIsEmpty()
        {
            var formItem = new UserFormItem
            {
                Identifier = string.Empty,
                Email = string.Empty,
            };

            // Assert: Throws ValidationException when Email is empty
            Assert.That(async () => await _userHelper.SaveAsync(formItem: formItem),
                Throws.TypeOf<ValidationException>().With.Message.Contains($"{nameof(UserFormItem.Email)} is required."));
            await Task.CompletedTask;
        }

        [Test]
        public async Task SaveAsync_Create_ShouldThrowError_WhenEmailAlreadyExists()
        {
            var formItem = new UserFormItem
            {
                Identifier = string.Empty,
                Email = "alice.borderland@testmail.com",
            };

            // Assert: Database already contains User with same Email
            Assert.That(_appDbContext.Users.Any(x => x.Email == formItem.Email), Is.True);

            // Assert: Throws ValidationException when Email already exists
            Assert.That(async () => await _userHelper.SaveAsync(formItem: formItem),
                Throws.TypeOf<ValidationException>().With.Message.Contains($"{nameof(UserFormItem.Email)} is already taken."));
            await Task.CompletedTask;
        }
        #endregion

        #region Update
        [Test]
        public async Task SaveAsync_Update_ShouldUpdateUser()
        {
            var formItem = new UserFormItem
            {
                Identifier = "c9bf9e57-1685-4c89-bafb-ff5af830be8a",
                Email = "__frodo.smith@testmail.com",
            };

            var saveResult = await _userHelper.SaveAsync(formItem: formItem);

            // Assert: Result is not null
            Assert.That(saveResult, Is.Not.Null);

            // Assert: User updated in Database with new Email
            Assert.That(_appDbContext.Users.Any(x =>
                x.Identifier == saveResult.Identifier
                && x.Email == formItem.Email), Is.True);

            // Assert: Total count remains the same
            Assert.That(_appDbContext.Users.Count(), Is.EqualTo(3));

            // Assert: Old User Data no longer exists
            Assert.That(_appDbContext.Users.Any(x =>
                x.Identifier == saveResult.Identifier
                && x.Email == "frodo.smith@testmail.com"), Is.False);
        }

        [Test]
        public async Task SaveAsync_Update_ShouldThrowError_WhenEmailIsEmpty()
        {
            var formItem = new UserFormItem
            {
                Identifier = "c9bf9e57-1685-4c89-bafb-ff5af830be8a",
                Email = string.Empty,
            };

            // Assert: User exists in Database
            Assert.That(_appDbContext.Users.Any(x => x.Identifier == formItem.Identifier), Is.True);

            // Assert: Throws ValidationException when Email is empty
            Assert.That(async () => await _userHelper.SaveAsync(formItem: formItem),
                Throws.TypeOf<ValidationException>().With.Message.Contains($"{nameof(UserFormItem.Email)} is required."));
            await Task.CompletedTask;
        }

        [Test]
        public async Task SaveAsync_Update_ShouldThrowError_WhenEmailAlreadyExists()
        {
            var formItem = new UserFormItem
            {
                Identifier = "c9bf9e57-1685-4c89-bafb-ff5af830be8a",
                Email = "alice.borderland@testmail.com",
            };

            // Assert: User exists in Database but with different Email
            Assert.That(_appDbContext.Users.Any(x =>
                x.Identifier == formItem.Identifier
                && x.Email != formItem.Email), Is.True);

            // Assert: Another User has the conflicting Email
            Assert.That(_appDbContext.Users.Any(x => x.Email == formItem.Email), Is.True);

            // Assert: Throws ValidationException when Email already exists
            Assert.That(async () => await _userHelper.SaveAsync(formItem: formItem),
                Throws.TypeOf<ValidationException>().With.Message.Contains($"{nameof(UserFormItem.Email)} is already taken."));
            await Task.CompletedTask;
        }

        [Test]
        public async Task SaveAsync_Update_ShouldThrowError_WhenUserNotFound()
        {
            var formItem = new UserFormItem
            {
                Identifier = "non-existent-guid",
                Email = "adam.eval@testmail.com",
            };

            // Assert: User does not exist in Database
            Assert.That(_appDbContext.Users.Any(x => x.Identifier == formItem.Identifier), Is.False);

            // Assert: Throws KeyNotFoundException when User not found
            Assert.That(async () => await _userHelper.SaveAsync(formItem: formItem),
                Throws.TypeOf<KeyNotFoundException>().With.Message.EqualTo($"{nameof(User)} with {nameof(User.Identifier)}: '{formItem.Identifier}' not found."));
            await Task.CompletedTask;
        }
        #endregion
        #endregion

        #region DeleteAsync
        [Test]
        public async Task DeleteAsync_ShouldRemoveUser()
        {
            var userIdentifier = "c9bf9e57-1685-4c89-bafb-ff5af830be8a";

            var deleteResult = await _userHelper.DeleteAsync(identifier: userIdentifier);

            // Assert: Delete returned true
            Assert.That(deleteResult, Is.True);

            // Assert: User no longer exists in Database
            Assert.That(_appDbContext.Users.Any(x => x.Identifier == userIdentifier), Is.False);
        }

        [Test]
        public async Task DeleteAsync_ShouldThrowError_WhenUserNotFound()
        {
            var userIdentifier = "non-existent-guid";

            // Assert: User does not exist in Database
            Assert.That(_appDbContext.Users.Any(x => x.Identifier == userIdentifier), Is.False);

            // Assert: Throws KeyNotFoundException when User not found
            Assert.That(async () => await _userHelper.DeleteAsync(identifier: userIdentifier),
                Throws.TypeOf<KeyNotFoundException>().With.Message.EqualTo($"{nameof(User)} with {nameof(User.Identifier)}: '{userIdentifier}' not found."));
            await Task.CompletedTask;
        }
        #endregion
    }
}