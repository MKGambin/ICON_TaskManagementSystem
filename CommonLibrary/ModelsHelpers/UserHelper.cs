using CommonLibrary.Models;
using CommonLibrary.ModelsDtos.DtoUser;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace CommonLibrary.ModelsHelpers
{
    /// <summary>
    /// Handles the model logic for <see cref="User"/> entities, including CRUD operations.
    /// </summary>
    public class UserHelper
    {
        private readonly AppDbContext _appDbContext;

        public UserHelper(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext ?? throw new ArgumentNullException(nameof(appDbContext));
        }

        /// <summary>
        /// Retrieves all <see cref="User"/> entities as <see cref="UserListItem"/> ModelsDtos.
        /// </summary>
        /// <returns>A list of all users.</returns>
        public async Task<List<UserListItem>> GetAllAsync()
        {
            return await _appDbContext.Users
                .Select(x => new UserListItem(x))
                .ToListAsync();
        }

        /// <summary>
        /// Retrieves a <see cref="User"/> by its unique identifier and returns it as a specified ModelsDtos type.
        /// </summary>
        /// <typeparam name="TModel">The ModelsDtos type to return. Must have a constructor that accepts a <see cref="User"/> entity.</typeparam>
        /// <param name="identifier">The GUID string identifier of the entity.</param>
        /// <returns>The matching <typeparamref name="TModel"/> instance.</returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown if <typeparamref name="TModel"/> is the entity type itself instead of a ModelsDtos type.
        /// </exception>
        /// <exception cref="KeyNotFoundException">Thrown if no <see cref="User"/> is found for the given identifier.</exception>
        public async Task<TModel?> GetByIdentifierAsync<TModel>(string identifier)
            where TModel : class
        {
            if (typeof(TModel) == typeof(User))
            {
                throw new InvalidOperationException($"Cannot return the entity type itself. Use a ModelsDtos type for {nameof(User)}.");
            }

            var entity = await _appDbContext.Users
                .FirstOrDefaultAsync(x => x.Identifier == identifier);

            if (entity is null)
            {
                throw new KeyNotFoundException($"{nameof(User)} with {nameof(User.Identifier)}: '{identifier}' not found.");
            }
            return Activator.CreateInstance(typeof(TModel), entity) as TModel;
        }

        /// <summary>
        /// Creates or Updates a <see cref="User"/> entity based on the provided form data. Validation is performed before saving.
        /// </summary>
        /// <param name="formItem">The <see cref="UserFormItem"/> containing the data.</param>
        /// <returns>The saved <see cref="User"/> entity.</returns>
        /// <exception cref="ValidationException">Thrown if validation fails for the provided <paramref name="formItem"/>.</exception>
        /// <exception cref="KeyNotFoundException">Thrown if attempting to update a user that does not exist.</exception>
        public async Task<User?> SaveAsync(UserFormItem formItem)
        {
            var errors = await ValidateSaveAsync(formItem: formItem);
            if (errors.Count != 0)
            {
                throw new ValidationException(string.Join("; ", errors));
            }

            var entity = (User?)null;
            if (string.IsNullOrEmpty(formItem.Identifier))
            {
                entity ??= new();
                entity.Identifier = Guid.NewGuid().ToString();
            }
            else
            {
                entity = await _appDbContext.Users
                    .FirstOrDefaultAsync(x => x.Identifier == formItem.Identifier);
                if (entity is null)
                {
                    throw new KeyNotFoundException($"{nameof(User)} with {nameof(User.Identifier)}: '{formItem.Identifier}' not found.");
                }
            }

            entity.Email = formItem.Email ?? string.Empty;

            if (string.IsNullOrEmpty(formItem.Identifier))
            {
                await _appDbContext.Users.AddAsync(entity);
            }
            await _appDbContext.SaveChangesAsync();
            return entity;
        }
        private async Task<List<string>> ValidateSaveAsync(UserFormItem formItem)
        {
            var errors = new List<string>();

            // Validate Email
            {
                if (string.IsNullOrWhiteSpace(formItem.Email))
                {
                    errors.Add($"{nameof(UserFormItem.Email)} is required.");
                }
                else
                {
                    var exists = await _appDbContext.Users
                        .AnyAsync(x => x.Identifier != formItem.Identifier && x.Email == formItem.Email);
                    if (exists)
                    {
                        errors.Add($"{nameof(UserFormItem.Email)} is already taken.");
                    }
                }
            }
            return errors;
        }

        /// <summary>
        /// Deletes a <see cref="User"/> by its unique identifier.
        /// </summary>
        /// <param name="identifier">The GUID string identifier of the entity.</param>
        /// <returns><c>true</c> if the entity was deleted successfully.</returns>
        /// <exception cref="KeyNotFoundException">Thrown if no <see cref="User"/> is found for the given identifier.</exception>
        public async Task<bool> DeleteAsync(string identifier)
        {
            var entity = await _appDbContext.Users
                .FirstOrDefaultAsync(x => x.Identifier == identifier);
            if (entity is null)
            {
                throw new KeyNotFoundException($"{nameof(User)} with {nameof(User.Identifier)}: '{identifier}' not found.");
            }

            _appDbContext.Users.Remove(entity);

            await _appDbContext.SaveChangesAsync();
            return true;
        }
    }
}
