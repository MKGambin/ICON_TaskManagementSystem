using CommonLibrary.Models;
using CommonLibrary.ModelsDtos.DtoTaskItem;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace CommonLibrary.ModelsHelpers
{
    /// <summary>
    /// Handles the model logic for <see cref="TaskItem"/> entities, including CRUD operations.
    /// Requires a current user context to ensure data is scoped per user.
    /// </summary>
    public class TaskItemHelper
    {
        private readonly AppDbContext _appDbContext;
        public User CurrentUser { get; }

        public TaskItemHelper(AppDbContext appDbContext, User currentUser)
        {
            _appDbContext = appDbContext ?? throw new ArgumentNullException(nameof(appDbContext));
            CurrentUser = currentUser ?? throw new ArgumentNullException(nameof(currentUser));

            var userExists = _appDbContext.Users.Any(x => x.Id == currentUser.Id);
            if (userExists == false)
            {
                throw new KeyNotFoundException($"{nameof(User)} with {nameof(User.Id)}: '{currentUser.Id}' does not exist.");
            }
        }
        public TaskItemHelper(AppDbContext appDbContext, string userIdentifier)
        {
            _appDbContext = appDbContext ?? throw new ArgumentNullException(nameof(appDbContext));
            CurrentUser = _appDbContext.Users.FirstOrDefault(x => x.Identifier == userIdentifier)
                ?? throw new KeyNotFoundException($"{nameof(User)} with {nameof(User.Identifier)}: '{userIdentifier}' does not exist.");
        }

        /// <summary>
        /// Retrieves all <see cref="TaskItem"/> entities for the current user as <see cref="TaskItemListItem"/> ModelsDtos.
        /// </summary>
        /// <returns>A list of the current user's task items.</returns>
        public async Task<List<TaskItemListItem>> GetAllAsync()
        {
            return await _appDbContext.TaskItems
                .Include(x => x.User)
                .Where(x => x.UserId == CurrentUser.Id)
                .Select(x => new TaskItemListItem(x))
                .ToListAsync();
        }

        /// <summary>
        /// Retrieves a <see cref="TaskItem"/> by its unique identifier and returns it as a specified ModelsDtos type for the current user.
        /// </summary>
        /// <typeparam name="TModel">The ModelsDtos type to return. Must have a constructor that accepts a <see cref="TaskItem"/> entity.</typeparam>
        /// <param name="identifier">The GUID string identifier of the entity.</param>
        /// <returns>The matching <typeparamref name="TModel"/> instance.</returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown if <typeparamref name="TModel"/> is the entity type itself instead of a ModelsDtos type.
        /// </exception>
        /// <exception cref="KeyNotFoundException">Thrown if no <see cref="TaskItem"/> is found for the given identifier and current user.</exception>
        public async Task<TModel?> GetByIdentifierAsync<TModel>(string identifier)
            where TModel : class
        {
            if (typeof(TModel) == typeof(TaskItem))
            {
                throw new InvalidOperationException($"Cannot return the entity type itself. Use a ModelsDtos type for {nameof(TaskItem)}.");
            }

            var entity = await _appDbContext.TaskItems
                .Include(x => x.User)
                .FirstOrDefaultAsync(x => x.Identifier == identifier && x.UserId == CurrentUser.Id);

            if (entity is null)
            {
                throw new KeyNotFoundException($"{nameof(TaskItem)} with {nameof(TaskItem.Identifier)}: '{identifier}' not found for current user.");
            }
            return Activator.CreateInstance(typeof(TModel), entity) as TModel;
        }

        /// <summary>
        /// Creates or Updates a <see cref="TaskItem"/> entity based on the provided form data for the current user. Validation is performed before saving.
        /// </summary>
        /// <param name="formItem">The <see cref="TaskItemFormItem"/> containing the data.</param>
        /// <returns>The saved <see cref="TaskItem"/> entity.</returns>
        /// <exception cref="ValidationException">Thrown if validation fails for the provided <paramref name="formItem"/>.</exception>
        /// <exception cref="KeyNotFoundException">Thrown if attempting to update a task item that does not exist for the current user.</exception>
        public async Task<TaskItem?> SaveAsync(TaskItemFormItem formItem)
        {
            var errors = await ValidateSaveAsync(formItem: formItem);
            if (errors.Count != 0)
            {
                throw new ValidationException(string.Join("; ", errors));
            }

            var entity = (TaskItem?)null;
            if (string.IsNullOrEmpty(formItem.Identifier))
            {
                entity ??= new();
                entity.Identifier = Guid.NewGuid().ToString();
                entity.UserId = CurrentUser.Id;
            }
            else
            {
                entity = await _appDbContext.TaskItems
                    .FirstOrDefaultAsync(x => x.Identifier == formItem.Identifier && x.UserId == CurrentUser.Id);
                if (entity is null)
                {
                    throw new KeyNotFoundException($"{nameof(TaskItem)} with {nameof(TaskItem.Identifier)}: '{formItem.Identifier}' not found for current user.");
                }
            }

            entity.Name = formItem.Name;
            entity.Description = formItem.Description;
            entity.TaskItemStatus = formItem.TaskItemStatus ?? default;

            if (string.IsNullOrEmpty(formItem.Identifier))
            {
                await _appDbContext.TaskItems.AddAsync(entity);
            }
            await _appDbContext.SaveChangesAsync();
            return entity;
        }
        private async Task<List<string>> ValidateSaveAsync(TaskItemFormItem formItem)
        {
            var errors = new List<string>();

            // Validate Name
            {
                if (string.IsNullOrWhiteSpace(formItem.Name))
                {
                    errors.Add($"{nameof(TaskItemFormItem.Name)} is required.");
                }
            }

            // Validate TaskItemStatus
            {
                if (formItem.TaskItemStatus is null)
                {
                    errors.Add($"{nameof(TaskItemFormItem.TaskItemStatus)} is required.");
                }
                else if (!Enum.IsDefined(typeof(TaskItemStatusType), formItem.TaskItemStatus))
                {
                    errors.Add($"{nameof(TaskItemFormItem.TaskItemStatus)} is invalid.");
                }
            }
            await Task.CompletedTask;
            return errors;
        }

        /// <summary>
        /// Deletes a <see cref="TaskItem"/> by its unique identifier for the current user.
        /// </summary>
        /// <param name="identifier">The GUID string identifier of the entity.</param>
        /// <returns><c>true</c> if the entity was deleted successfully.</returns>
        /// <exception cref="KeyNotFoundException">Thrown if no <see cref="TaskItem"/> is found for the given identifier and current user.</exception>
        public async Task<bool> DeleteAsync(string identifier)
        {
            var entity = await _appDbContext.TaskItems
                .FirstOrDefaultAsync(x => x.Identifier == identifier && x.UserId == CurrentUser.Id);
            if (entity is null)
            {
                throw new KeyNotFoundException($"{nameof(TaskItem)} with {nameof(TaskItem.Identifier)}: '{identifier}' not found for current user.");
            }

            _appDbContext.TaskItems.Remove(entity);

            await _appDbContext.SaveChangesAsync();
            return true;
        }
    }
}
