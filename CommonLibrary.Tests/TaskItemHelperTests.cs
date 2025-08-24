using CommonLibrary.Models;
using CommonLibrary.ModelsDtos.DtoTaskItem;
using CommonLibrary.ModelsHelpers;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace CommonLibrary.Tests
{
    [TestFixture]
    public class TaskItemHelperTests
    {
        private DbContextOptions<AppDbContext> _dbContextOptions;
        private AppDbContext _appDbContext;

        private TaskItemHelper _taskItemHelper;

        [SetUp]
        public void SetUp()
        {
            _dbContextOptions = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()).Options;
            _appDbContext = new AppDbContext(options: _dbContextOptions);

            #region SeedData - TaskItem
            _appDbContext.TaskItems.AddRange(
                new()
                {
                    Id = 1,
                    Identifier = "86c53f64-6f9a-40c9-acce-766c8a88ae35",
                    UserId = 1,
                    Name = "Task 1.1",
                    Description = string.Empty,
                    TaskItemStatus = TaskItemStatusType.Pending,
                },
                new()
                {
                    Id = 2,
                    Identifier = "e1d82661-3c50-4ba2-9c77-5521df64a6f8",
                    UserId = 1,
                    Name = "Task 1.2",
                    Description = string.Empty,
                    TaskItemStatus = TaskItemStatusType.InProgress,
                },
                new()
                {
                    Id = 3,
                    Identifier = "3311ff50-45f5-43a3-8a8b-0e9b6cbaf45f",
                    UserId = 1,
                    Name = "Task 1.3",
                    Description = string.Empty,
                    TaskItemStatus = TaskItemStatusType.Completed,
                },
                new()
                {
                    Id = 4,
                    Identifier = "554fa5ed-5c60-4c01-985b-1292fcfd9cdd",
                    UserId = 1,
                    Name = "Task 1.4",
                    Description = "Task 1.4 Cancelled..",
                    TaskItemStatus = TaskItemStatusType.Cancelled,
                },
                new()
                {
                    Id = 5,
                    Identifier = "7e09726a-6037-432e-941e-80cf3fa93137",
                    UserId = 2,
                    Name = "Task A (001)",
                    Description = "Completed Before Time",
                    TaskItemStatus = TaskItemStatusType.Completed,
                },
                new()
                {
                    Id = 6,
                    Identifier = "687f5642-57db-4bdf-a8cf-12c7d441c7b7",
                    UserId = 2,
                    Name = "Task B (001)",
                    Description = string.Empty,
                    TaskItemStatus = TaskItemStatusType.Cancelled,
                },
                new()
                {
                    Id = 7,
                    Identifier = "19ae216c-fbb7-4c56-b6da-aab9bbb58830",
                    UserId = 2,
                    Name = "Task B (002)",
                    Description = string.Empty,
                    TaskItemStatus = TaskItemStatusType.Cancelled,
                });
            _appDbContext.SaveChanges();
            #endregion
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

            _taskItemHelper = new TaskItemHelper(appDbContext: _appDbContext, currentUser: _appDbContext.Users.First(x => x.Id == 1));
        }

        [TearDown]
        public void TearDown()
        {
            _appDbContext.Dispose();
        }

        #region Constructor
        #region Parameter- currentUser
        [Test]
        public async Task Constructor_WithCurrentUser_ShouldThrowArgumentNullException_WhenAppDbContextIsNull()
        {
            // Assert: Constructor should throw ArgumentNullException when AppDbContext is null
            Assert.That(() => new TaskItemHelper(appDbContext: null!, currentUser: new()),
                Throws.ArgumentNullException);
            await Task.CompletedTask;
        }

        [Test]
        public async Task Constructor_WithCurrentUser_ShouldThrowArgumentNullException_WhenCurrentUserIsNull()
        {
            // Assert: Constructor should throw ArgumentNullException when CurrentUser is null
            Assert.That(() => new TaskItemHelper(appDbContext: _appDbContext, currentUser: null!),
                Throws.ArgumentNullException);
            await Task.CompletedTask;
        }

        [Test]
        public async Task Constructor_WithCurrentUser_ShouldThrowArgumentNullException_WhenCurrentUserIsNotFound()
        {
            var nonExistingUser = new User
            {
                Id = -1,
                Identifier = "non-existent-guid",
                Email = "nonexisting@testmail.com",
                TaskItems = [],
            };

            // Assert: Constructor should throw ArgumentNullException when CurrentUser does not exists in Database
            Assert.That(() => new TaskItemHelper(appDbContext: _appDbContext, currentUser: nonExistingUser),
                Throws.TypeOf<KeyNotFoundException>().With.Message.EqualTo($"{nameof(User)} with {nameof(User.Id)}: '{nonExistingUser.Id}' does not exist."));
            await Task.CompletedTask;
        }
        #endregion

        #region Parameter- userIdentifier
        [Test]
        public async Task Constructor_WithUserIdentifier_ShouldThrowArgumentNullException_WhenAppDbContextIsNull()
        {
            var userIdentifier = "c9bf9e57-1685-4c89-bafb-ff5af830be8a";

            // Assert: Constructor should throw ArgumentNullException when AppDbContext is null
            Assert.That(() => new TaskItemHelper(appDbContext: null!, userIdentifier: userIdentifier),
                Throws.ArgumentNullException);
            await Task.CompletedTask;
        }

        [Test]
        public void Constructor_WithUserIdentifier_ShouldThrowKeyNotFoundException_WhenUserIdentifierIsEmpty()
        {
            var userIdentifier = string.Empty;

            // Assert: Constructor should throw ArgumentNullException when UserIdentifier does not exists in Database
            Assert.That(() => new TaskItemHelper(appDbContext: _appDbContext, userIdentifier: userIdentifier),
                Throws.TypeOf<KeyNotFoundException>()
                      .With.Message.EqualTo($"{nameof(User)} with {nameof(User.Identifier)}: '{userIdentifier}' does not exist."));
        }

        [Test]
        public void Constructor_WithUserIdentifier_ShouldThrowKeyNotFoundException_WhenUserIdentifierDoesNotExist()
        {
            var userIdentifier = "non-existent-user";

            // Assert: Constructor should throw ArgumentNullException when UserIdentifier does not exists in Database
            Assert.That(() => new TaskItemHelper(appDbContext: _appDbContext, userIdentifier: userIdentifier),
                Throws.TypeOf<KeyNotFoundException>()
                      .With.Message.EqualTo($"{nameof(User)} with {nameof(User.Identifier)}: '{userIdentifier}' does not exist."));
        }
        #endregion
        #endregion

        #region GetAllAsync
        [Test]
        public async Task GetAllAsync_ShouldReturnTaskItems()
        {
            var taskItems = await _taskItemHelper.GetAllAsync();

            // Assert: Result is not null
            Assert.That(taskItems, Is.Not.Null);

            // Assert: Returned count matches Database TaskItem count for CurrentUser
            Assert.That(taskItems, Has.Count.EqualTo(_appDbContext.TaskItems.Count(x => x.UserId == _taskItemHelper.CurrentUser.Id)));

            // Assert: All returned TaskItems belong to the CurrentUser
            Assert.That(taskItems.All(x => x.User!.Identifier == _taskItemHelper.CurrentUser.Identifier), Is.True);
        }
        #endregion

        #region GetByIdentifierAsync
        #region TaskItem
        [Test]
        public async Task GetByIdentifierAsync_ShouldThrowInvalidOperationException_WhenTModelIsTaskItem()
        {
            var taskItemIdentifier = "e1d82661-3c50-4ba2-9c77-5521df64a6f8";

            // Assert: Throws InvalidOperationException when TModel pass is type TaskItem
            Assert.That(async () => await _taskItemHelper.GetByIdentifierAsync<TaskItem>(identifier: taskItemIdentifier),
                Throws.TypeOf<InvalidOperationException>().With.Message.EqualTo($"Cannot return the entity type itself. Use a ModelsDtos type for {nameof(TaskItem)}."));
            await Task.CompletedTask;
        }
        #endregion

        #region TaskItemFormItem
        [Test]
        public async Task GetByIdentifierAsync_AsTaskItemFormItem_ShouldReturnCorrectTaskItem()
        {
            var taskItemIdentifier = "e1d82661-3c50-4ba2-9c77-5521df64a6f8";

            var taskItem = await _taskItemHelper.GetByIdentifierAsync<TaskItemFormItem>(identifier: taskItemIdentifier);

            // Assert: Result is not null
            Assert.That(taskItem, Is.Not.Null);

            // Assert: Identifiers match
            Assert.That(taskItem.Identifier, Is.EqualTo(taskItemIdentifier));

            // Assert: Returned TaskItem belong to the CurrentUser
            Assert.That(taskItem.User, Is.EqualTo(_taskItemHelper.CurrentUser.Identifier));

            // Assert: TaskItem exists in Database with same Identifiers & Name & Description & TaskItemStatus
            Assert.That(_appDbContext.TaskItems.Any(x =>
                x.Identifier == taskItemIdentifier
                && x.Name == taskItem.Name
                && x.Description == taskItem.Description
                && x.TaskItemStatus == taskItem.TaskItemStatus), Is.True);
        }

        [Test]
        public async Task GetByIdentifierAsync_AsTaskItemFormItem_ShouldThrowError_WhenTaskItemNotFound()
        {
            var taskItemIdentifier = "non-existent-guid";

            // Assert: TaskItem does not exist in Database
            Assert.That(_appDbContext.TaskItems.Any(x => x.Identifier == taskItemIdentifier), Is.False);

            // Assert: Throws KeyNotFoundException when TaskItem not found
            Assert.That(async () => await _taskItemHelper.GetByIdentifierAsync<TaskItemFormItem>(identifier: taskItemIdentifier),
                Throws.TypeOf<KeyNotFoundException>().With.Message.EqualTo($"{nameof(TaskItem)} with {nameof(TaskItem.Identifier)}: '{taskItemIdentifier}' not found for current user."));
            await Task.CompletedTask;
        }

        [Test]
        public async Task GetByIdentifierAsync_AsTaskItemFormItem_ShouldThrowError_WhenTaskItemBelongsToDifferentUser()
        {
            var taskItemIdentifier = "687f5642-57db-4bdf-a8cf-12c7d441c7b7";

            // Assert: TaskItem exists in Database
            Assert.That(_appDbContext.TaskItems.Any(x => x.Identifier == taskItemIdentifier), Is.True);

            // Assert: TaskItem exists in Database but is owned by a different User
            Assert.That(_appDbContext.TaskItems.Any(x => x.Identifier == taskItemIdentifier && x.UserId != _taskItemHelper.CurrentUser.Id), Is.True);

            // Assert: Throws KeyNotFoundException, since it does not belong to the CurrentUser
            Assert.That(async () => await _taskItemHelper.GetByIdentifierAsync<TaskItemFormItem>(identifier: taskItemIdentifier),
                Throws.TypeOf<KeyNotFoundException>().With.Message.EqualTo($"{nameof(TaskItem)} with {nameof(TaskItem.Identifier)}: '{taskItemIdentifier}' not found for current user."));
            await Task.CompletedTask;
        }
        #endregion

        #region TaskItemListItem
        [Test]
        public async Task GetByIdentifierAsync_AsTaskItemListItem_ShouldReturnCorrectTaskItem()
        {
            var taskItemIdentifier = "e1d82661-3c50-4ba2-9c77-5521df64a6f8";

            var taskItem = await _taskItemHelper.GetByIdentifierAsync<TaskItemListItem>(identifier: taskItemIdentifier);

            // Assert: Result is not null
            Assert.That(taskItem, Is.Not.Null);

            // Assert: Identifiers match
            Assert.That(taskItem.Identifier, Is.EqualTo(taskItemIdentifier));

            // Assert: Returned TaskItem belong to the CurrentUser
            Assert.That(taskItem.User!.Identifier, Is.EqualTo(_taskItemHelper.CurrentUser.Identifier));

            // Assert: TaskItem exists in Database with same Identifiers & Name & Description & TaskItemStatus
            Assert.That(_appDbContext.TaskItems.Any(x =>
                x.Identifier == taskItemIdentifier
                && x.Name == taskItem.Name
                && x.Description == taskItem.Description
                && x.TaskItemStatus == taskItem.TaskItemStatus), Is.True);
        }

        [Test]
        public async Task GetByIdentifierAsync_AsTaskItemListItem_ShouldThrowError_WhenTaskItemNotFound()
        {
            var taskItemIdentifier = "non-existent-guid";

            // Assert: TaskItem does not exist in Database
            Assert.That(_appDbContext.TaskItems.Any(x => x.Identifier == taskItemIdentifier), Is.False);

            // Assert: Throws KeyNotFoundException when TaskItem not found
            Assert.That(async () => await _taskItemHelper.GetByIdentifierAsync<TaskItemListItem>(identifier: taskItemIdentifier),
                Throws.TypeOf<KeyNotFoundException>().With.Message.EqualTo($"{nameof(TaskItem)} with {nameof(TaskItem.Identifier)}: '{taskItemIdentifier}' not found for current user."));
            await Task.CompletedTask;
        }

        [Test]
        public async Task GetByIdentifierAsync_AsTaskItemListItem_ShouldThrowError_WhenTaskItemBelongsToDifferentUser()
        {
            var taskItemIdentifier = "687f5642-57db-4bdf-a8cf-12c7d441c7b7";

            // Assert: TaskItem exists in Database
            Assert.That(_appDbContext.TaskItems.Any(x => x.Identifier == taskItemIdentifier), Is.True);

            // Assert: TaskItem exists in Database but is owned by a different User
            Assert.That(_appDbContext.TaskItems.Any(x => x.Identifier == taskItemIdentifier && x.UserId != _taskItemHelper.CurrentUser.Id), Is.True);

            // Assert: Throws KeyNotFoundException, since it does not belong to the CurrentUser
            Assert.That(async () => await _taskItemHelper.GetByIdentifierAsync<TaskItemListItem>(identifier: taskItemIdentifier),
                Throws.TypeOf<KeyNotFoundException>().With.Message.EqualTo($"{nameof(TaskItem)} with {nameof(TaskItem.Identifier)}: '{taskItemIdentifier}' not found for current user."));
            await Task.CompletedTask;
        }
        #endregion
        #endregion

        #region SaveAsync
        #region Create
        [Test]
        public async Task SaveAsync_Create_ShouldCreateNewTaskItem()
        {
            var formItem = new TaskItemFormItem
            {
                User = _taskItemHelper.CurrentUser.Identifier,
                Identifier = string.Empty,
                Name = "TaskItem-001.1",
                Description = "Description Text..",
                TaskItemStatus = TaskItemStatusType.Pending,
            };

            var saveResult = await _taskItemHelper.SaveAsync(formItem: formItem);

            // Assert: Result is not null
            Assert.That(saveResult, Is.Not.Null);

            // Assert: TaskItem is created in Database with matching Identifier & Name & Description & TaskItemStatus
            Assert.That(_appDbContext.TaskItems.Any(x => 
                x.Identifier == saveResult.Identifier
                && x.Name == formItem.Name
                && x.Description == formItem.Description
                && x.TaskItemStatus == formItem.TaskItemStatus), Is.True);

            // Assert: Total Database count is increased to 8
            Assert.That(_appDbContext.TaskItems.Count(), Is.EqualTo(8));
        }

        [Test]
        public async Task SaveAsync_Create_ShouldCreateNewTaskItem_WithoutDescription()
        {
            var formItem = new TaskItemFormItem
            {
                User = _taskItemHelper.CurrentUser.Identifier,
                Identifier = string.Empty,
                Name = "TaskItem-001.2",
                Description = string.Empty,
                TaskItemStatus = TaskItemStatusType.Completed,
            };

            var saveResult = await _taskItemHelper.SaveAsync(formItem: formItem);

            // Assert: Result is not null
            Assert.That(saveResult, Is.Not.Null);

            // Assert: TaskItem is created in Database with matching Identifier & Name & Description & TaskItemStatus
            Assert.That(_appDbContext.TaskItems.Any(x =>
                x.Identifier == saveResult.Identifier
                && x.Name == formItem.Name
                && x.Description == formItem.Description
                && x.TaskItemStatus == formItem.TaskItemStatus), Is.True);

            // Assert: Total Database count is increased to 8
            Assert.That(_appDbContext.TaskItems.Count(), Is.EqualTo(8));
        }

        [Test]
        public async Task SaveAsync_Create_ShouldThrowError_WhenNameIsEmpty()
        {
            var formItem = new TaskItemFormItem
            {
                User = _taskItemHelper.CurrentUser.Identifier,
                Identifier = string.Empty,
                Name = string.Empty,
                Description = "Description Text..",
                TaskItemStatus = TaskItemStatusType.Pending,
            };

            // Assert: Throws ValidationException when Name is empty
            Assert.That(async () => await _taskItemHelper.SaveAsync(formItem: formItem),
                Throws.TypeOf<ValidationException>().With.Message.Contains($"{nameof(TaskItemFormItem.Name)} is required."));
            await Task.CompletedTask;
        }

        [Test]
        public async Task SaveAsync_Create_ShouldThrowError_WhenTaskItemStatusIsNull()
        {
            var formItem = new TaskItemFormItem
            {
                User = _taskItemHelper.CurrentUser.Identifier,
                Identifier = string.Empty,
                Name = "TaskItem-001",
                Description = "Description Text..",
                TaskItemStatus = null,
            };

            // Assert: Throws ValidationException when TaskItemStatus is null
            Assert.That(async () => await _taskItemHelper.SaveAsync(formItem: formItem),
                Throws.TypeOf<ValidationException>().With.Message.Contains($"{nameof(TaskItemFormItem.TaskItemStatus)} is required."));
            await Task.CompletedTask;
        }

        [Test]
        public async Task SaveAsync_Create_ShouldThrowError_WhenTaskItemStatusIsInvalid()
        {
            var formItem = new TaskItemFormItem
            {
                User = _taskItemHelper.CurrentUser.Identifier,
                Identifier = string.Empty,
                Name = "TaskItem-001.1",
                Description = "Description Text..",
                TaskItemStatus = (TaskItemStatusType)999,
            };

            // Assert: Throws ValidationException when TaskItemStatus is invalid
            Assert.That(async () => await _taskItemHelper.SaveAsync(formItem: formItem),
                Throws.TypeOf<ValidationException>().With.Message.Contains($"{nameof(TaskItemFormItem.TaskItemStatus)} is invalid."));
            await Task.CompletedTask;
        }

        [Test]
        public async Task SaveAsync_Create_ShouldThrowError_WhenAllFieldsAreNullOrEmpty()
        {
            var formItem = new TaskItemFormItem
            {
                User = _taskItemHelper.CurrentUser.Identifier,
                Identifier = string.Empty,
                Name = string.Empty,
                Description = string.Empty,
                TaskItemStatus = null,
            };

            // Assert: Throws ValidationException when all fields are null or empty
            Assert.That(async () => await _taskItemHelper.SaveAsync(formItem: formItem),
                Throws.TypeOf<ValidationException>().With.Message.Contains($"{nameof(TaskItemFormItem.Name)} is required.; {nameof(TaskItemFormItem.TaskItemStatus)} is required."));
            await Task.CompletedTask;
        }
        #endregion

        #region Update
        [Test]
        public async Task SaveAsync_Update_ShouldUpdateTaskItem()
        {
            var formItem = new TaskItemFormItem
            {
                User = _taskItemHelper.CurrentUser.Identifier,
                Identifier = "e1d82661-3c50-4ba2-9c77-5521df64a6f8",
                Name = "__Task 1.2",
                Description = "___",
                TaskItemStatus = TaskItemStatusType.Completed,
            };

            var saveResult = await _taskItemHelper.SaveAsync(formItem: formItem);

            // Assert: TaskItem exists in Database
            Assert.That(_appDbContext.TaskItems.Any(x => x.Identifier == formItem.Identifier), Is.True);

            // Assert: TaskItem exists in Database and belongs to the CurrentUser
            Assert.That(_appDbContext.TaskItems.Any(x =>
                x.Identifier == formItem.Identifier
                && x.UserId == _taskItemHelper.CurrentUser.Id), Is.True);

            // Assert: Result is not null
            Assert.That(saveResult, Is.Not.Null);

            // Assert: TaskItem updated in Database with new Name & Description & TaskItemStatus
            Assert.That(_appDbContext.TaskItems.Any(x =>
                x.Identifier == saveResult.Identifier
                && x.UserId == _taskItemHelper.CurrentUser.Id
                && x.Name == formItem.Name
                && x.Description == formItem.Description
                && x.TaskItemStatus == formItem.TaskItemStatus), Is.True);

            // Assert: Total count remains the same
            Assert.That(_appDbContext.TaskItems.Count(), Is.EqualTo(7));

            // Assert: Old TaskItem Data no longer exists
            Assert.That(_appDbContext.TaskItems.Any(x =>
                x.Identifier == saveResult.Identifier
                && x.UserId == _taskItemHelper.CurrentUser.Id && x.UserId == saveResult.UserId
                && x.Name == "Task 1.2"
                && x.Description == string.Empty
                && x.TaskItemStatus == TaskItemStatusType.InProgress), Is.False);
        }

        [Test]
        public async Task SaveAsync_Update_ShouldUpdateTaskItem_WithoutDescription()
        {
            var formItem = new TaskItemFormItem
            {
                User = _taskItemHelper.CurrentUser.Identifier,
                Identifier = "e1d82661-3c50-4ba2-9c77-5521df64a6f8",
                Name = "__Task 1.2",
                Description = string.Empty,
                TaskItemStatus = TaskItemStatusType.Completed,
            };

            var saveResult = await _taskItemHelper.SaveAsync(formItem: formItem);

            // Assert: TaskItem exists in Database
            Assert.That(_appDbContext.TaskItems.Any(x => x.Identifier == formItem.Identifier), Is.True);

            // Assert: TaskItem exists in Database and belongs to the CurrentUser
            Assert.That(_appDbContext.TaskItems.Any(x =>
                x.Identifier == formItem.Identifier
                && x.UserId == _taskItemHelper.CurrentUser.Id), Is.True);

            // Assert: Result is not null
            Assert.That(saveResult, Is.Not.Null);

            // Assert: TaskItem updated in Database with new Name & Description & TaskItemStatus
            Assert.That(_appDbContext.TaskItems.Any(x =>
                x.Identifier == saveResult.Identifier
                && x.UserId == _taskItemHelper.CurrentUser.Id
                && x.Name == formItem.Name
                && x.Description == formItem.Description
                && x.TaskItemStatus == formItem.TaskItemStatus), Is.True);

            // Assert: Total count remains the same
            Assert.That(_appDbContext.TaskItems.Count(), Is.EqualTo(7));

            // Assert: Old TaskItem Data no longer exists
            Assert.That(_appDbContext.TaskItems.Any(x =>
                x.Identifier == saveResult.Identifier
                && x.UserId == _taskItemHelper.CurrentUser.Id && x.UserId == saveResult.UserId
                && x.Name == "Task 1.2"
                && x.Description == string.Empty
                && x.TaskItemStatus == TaskItemStatusType.InProgress), Is.False);
        }

        [Test]
        public async Task SaveAsync_Update_ShouldThrowError_WhenNameIsEmpty()
        {
            var formItem = new TaskItemFormItem
            {
                User = _taskItemHelper.CurrentUser.Identifier,
                Identifier = "e1d82661-3c50-4ba2-9c77-5521df64a6f8",
                Name = string.Empty,
                Description = "___",
                TaskItemStatus = TaskItemStatusType.Completed,
            };

            // Assert: TaskItem exists in Database
            Assert.That(_appDbContext.TaskItems.Any(x => x.Identifier == formItem.Identifier), Is.True);

            // Assert: TaskItem exists in Database and belongs to the CurrentUser
            Assert.That(_appDbContext.TaskItems.Any(x =>
                x.Identifier == formItem.Identifier
                && x.UserId == _taskItemHelper.CurrentUser.Id), Is.True);

            // Assert: Throws ValidationException when Name is empty
            Assert.That(async () => await _taskItemHelper.SaveAsync(formItem: formItem),
                Throws.TypeOf<ValidationException>().With.Message.Contains($"{nameof(TaskItemFormItem.Name)} is required."));
            await Task.CompletedTask;
        }

        [Test]
        public async Task SaveAsync_Update_ShouldThrowError_WhenTaskItemStatusIsNull()
        {
            var formItem = new TaskItemFormItem
            {
                User = _taskItemHelper.CurrentUser.Identifier,
                Identifier = "e1d82661-3c50-4ba2-9c77-5521df64a6f8",
                Name = "__Task 1.2",
                Description = "___",
                TaskItemStatus = null,
            };

            // Assert: TaskItem exists in Database
            Assert.That(_appDbContext.TaskItems.Any(x => x.Identifier == formItem.Identifier), Is.True);

            // Assert: TaskItem exists in Database and belongs to the CurrentUser
            Assert.That(_appDbContext.TaskItems.Any(x =>
                x.Identifier == formItem.Identifier
                && x.UserId == _taskItemHelper.CurrentUser.Id), Is.True);

            // Assert: Throws ValidationException when TaskItemStatus is null
            Assert.That(async () => await _taskItemHelper.SaveAsync(formItem: formItem),
                Throws.TypeOf<ValidationException>().With.Message.Contains($"{nameof(TaskItemFormItem.TaskItemStatus)} is required."));
            await Task.CompletedTask;
        }

        [Test]
        public async Task SaveAsync_Update_ShouldThrowError_WhenTaskItemStatusIsInvalid()
        {
            var formItem = new TaskItemFormItem
            {
                User = _taskItemHelper.CurrentUser.Identifier,
                Identifier = "e1d82661-3c50-4ba2-9c77-5521df64a6f8",
                Name = "__Task 1.2",
                Description = "___",
                TaskItemStatus = (TaskItemStatusType)999,
            };

            // Assert: TaskItem exists in Database
            Assert.That(_appDbContext.TaskItems.Any(x => x.Identifier == formItem.Identifier), Is.True);

            // Assert: TaskItem exists in Database and belongs to the CurrentUser
            Assert.That(_appDbContext.TaskItems.Any(x =>
                x.Identifier == formItem.Identifier
                && x.UserId == _taskItemHelper.CurrentUser.Id), Is.True);

            // Assert: Throws ValidationException when TaskItemStatus is invalid
            Assert.That(async () => await _taskItemHelper.SaveAsync(formItem: formItem),
                Throws.TypeOf<ValidationException>().With.Message.Contains($"{nameof(TaskItemFormItem.TaskItemStatus)} is invalid."));
            await Task.CompletedTask;
        }

        [Test]
        public async Task SaveAsync_Update_ShouldThrowError_WhenAllFieldsAreNullOrEmpty()
        {
            var formItem = new TaskItemFormItem
            {
                User = _taskItemHelper.CurrentUser.Identifier,
                Identifier = "e1d82661-3c50-4ba2-9c77-5521df64a6f8",
                Name = string.Empty,
                Description = string.Empty,
                TaskItemStatus = null,
            };

            // Assert: TaskItem exists in Database
            Assert.That(_appDbContext.TaskItems.Any(x => x.Identifier == formItem.Identifier), Is.True);

            // Assert: TaskItem exists in Database and belongs to the CurrentUser
            Assert.That(_appDbContext.TaskItems.Any(x =>
                x.Identifier == formItem.Identifier
                && x.UserId == _taskItemHelper.CurrentUser.Id), Is.True);

            // Assert: Throws ValidationException when all fields are null or empty
            Assert.That(async () => await _taskItemHelper.SaveAsync(formItem: formItem),
                Throws.TypeOf<ValidationException>().With.Message.Contains($"{nameof(TaskItemFormItem.Name)} is required.; {nameof(TaskItemFormItem.TaskItemStatus)} is required."));
            await Task.CompletedTask;
        }

        [Test]
        public async Task SaveAsync_Update_ShouldThrowError_WhenTaskItemNotFound()
        {
            var formItem = new TaskItemFormItem
            {
                User = _taskItemHelper.CurrentUser.Identifier,
                Identifier = "non-existent-guid",
                Name = "Task 1.2",
                Description = string.Empty,
                TaskItemStatus = TaskItemStatusType.InProgress,
            };

            // Assert: TaskItem does not exist in Database
            Assert.That(_appDbContext.TaskItems.Any(x => x.Identifier == formItem.Identifier), Is.False);

            // Assert: Throws KeyNotFoundException when TaskItem not found
            Assert.That(async () => await _taskItemHelper.SaveAsync(formItem: formItem),
                Throws.TypeOf<KeyNotFoundException>().With.Message.Contains($"{nameof(TaskItem)} with {nameof(TaskItem.Identifier)}: '{formItem.Identifier}' not found for current user."));
            await Task.CompletedTask;
        }

        [Test]
        public async Task SaveAsync_Update_ShouldThrowError_WhenTaskItemBelongsToAnotherUser()
        {
            var formItem = new TaskItemFormItem
            {
                User = _taskItemHelper.CurrentUser.Identifier,
                Identifier = "687f5642-57db-4bdf-a8cf-12c7d441c7b7",
                Name = "Task B (001)",
                Description = string.Empty,
                TaskItemStatus = TaskItemStatusType.Cancelled,
            };

            // Assert: TaskItem exists in Database
            Assert.That(_appDbContext.TaskItems.Any(x => x.Identifier == formItem.Identifier), Is.True);

            // Assert: TaskItem exists in Database and belongs to another User
            Assert.That(_appDbContext.TaskItems.Any(x =>
                x.Identifier == formItem.Identifier
                && x.UserId != _taskItemHelper.CurrentUser.Id), Is.True);

            // Assert: Throws KeyNotFoundException, since it does not belong to the CurrentUser
            Assert.That(async () => await _taskItemHelper.SaveAsync(formItem: formItem),
                Throws.TypeOf<KeyNotFoundException>().With.Message.Contains($"{nameof(TaskItem)} with {nameof(TaskItem.Identifier)}: '{formItem.Identifier}' not found for current user."));
            await Task.CompletedTask;
        }
        #endregion
        #endregion

        #region DeleteAsync
        [Test]
        public async Task DeleteAsync_ShouldRemoveTaskItem()
        {
            var taskItemIdentifier = "e1d82661-3c50-4ba2-9c77-5521df64a6f8";

            var deleteResult = await _taskItemHelper.DeleteAsync(identifier: taskItemIdentifier);

            // Assert: Delete returned true
            Assert.That(deleteResult, Is.True);

            // Assert: TaskItem no longer exists in Database
            Assert.That(_appDbContext.TaskItems.Any(x => x.Identifier == taskItemIdentifier), Is.False);
        }

        [Test]
        public async Task DeleteAsync_ShouldThrowError_WhenTaskItemNotFound()
        {
            var taskItemIdentifier = "non-existent-guid";

            // Assert: TaskItem does not exist in Database
            Assert.That(_appDbContext.TaskItems.Any(x => x.Identifier == taskItemIdentifier), Is.False);

            // Assert: Throws KeyNotFoundException when TaskItem not found
            Assert.That(async () => await _taskItemHelper.DeleteAsync(identifier: taskItemIdentifier),
                Throws.TypeOf<KeyNotFoundException>().With.Message.EqualTo($"{nameof(TaskItem)} with {nameof(TaskItem.Identifier)}: '{taskItemIdentifier}' not found for current user."));
            await Task.CompletedTask;
        }

        [Test]
        public async Task DeleteAsync_ShouldThrowError_WhenTaskItemBelongsToDifferentUser()
        {
            var taskItemIdentifier = "687f5642-57db-4bdf-a8cf-12c7d441c7b7";

            // Assert: TaskItem exists in Database
            Assert.That(_appDbContext.TaskItems.Any(x => x.Identifier == taskItemIdentifier), Is.True);

            // Assert: TaskItem exists in Database and belongs to another User
            Assert.That(_appDbContext.TaskItems.Any(x =>
                x.Identifier == taskItemIdentifier
                && x.UserId != _taskItemHelper.CurrentUser.Id), Is.True);

            // Assert: Throws KeyNotFoundException, since it does not belong to the CurrentUser
            Assert.That(async () => await _taskItemHelper.DeleteAsync(identifier: taskItemIdentifier),
                Throws.TypeOf<KeyNotFoundException>().With.Message.EqualTo($"{nameof(TaskItem)} with {nameof(TaskItem.Identifier)}: '{taskItemIdentifier}' not found for current user."));
            await Task.CompletedTask;
        }
        #endregion
    }
}