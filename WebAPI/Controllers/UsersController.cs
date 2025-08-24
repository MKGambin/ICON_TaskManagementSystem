using CommonLibrary;
using CommonLibrary.ModelsDtos.DtoUser;
using CommonLibrary.ModelsHelpers;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    /// <summary>
    /// Provides endpoints to manage Users.
    /// </summary>
    /// <remarks>
    /// Supports creating, retrieving, updating, and deleting of Users.
    /// </remarks>
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _appDbContext;
        private readonly UserHelper _userHelper;

        /// <summary>
        /// Initializes a new instance of the <see cref="UsersController"/> class.
        /// </summary>
        /// <param name="appDbContext">The database context used to access Users and related data.</param>
        /// <remarks>
        /// This constructor sets up the controller with the <see cref="AppDbContext"/> instance and initializes the <see cref="UserHelper"/> to manage user-related operations trought the <see cref="UserHelper"/>.
        /// </remarks>
        public UsersController(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
            _userHelper = new UserHelper(appDbContext: appDbContext);
        }

        /// <summary>
        /// Get all users.
        /// </summary>
        /// <returns>List of all Users.</returns>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var users = await _userHelper.GetAllAsync();
            return Ok(users);
        }

        /// <summary>
        /// Get a single User by identifier.
        /// </summary>
        /// <param name="identifier">User identifier</param>
        /// <returns>User details</returns>
        [HttpGet("{identifier}")]
        public async Task<IActionResult> GetByIdentifier(string identifier)
        {
            var user = await _userHelper.GetByIdentifierAsync<UserListItem>(identifier: identifier);
            return Ok(user);
        }

        /// <summary>
        /// Get a blank template for creating a new User.
        /// </summary>
        /// <returns>User form template.</returns>
        [HttpGet("create")]
        public async Task<IActionResult> CreateTemplate()
        {
            await Task.CompletedTask;

            var template = new UserFormItem
            {
                Identifier = string.Empty,
                Email = string.Empty,
            };
            return Ok(template);
        }

        /// <summary>
        /// Create a new User.
        /// </summary>
        /// <param name="formItem">User form data.</param>
        /// <returns>Created User.</returns>
        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] UserFormItem formItem)
        {
            if (ModelState.IsValid == false)
            {
                return BadRequest(ModelState);
            }

            var result = await _userHelper.SaveAsync(formItem: formItem);
            return Ok(result is not null ? new UserFormItem(user: result) : null);
        }

        /// <summary>
        /// Get a User template to update an existing User.
        /// </summary>
        /// <param name="identifier">User identifier.</param>
        /// <returns>User form template with data.</returns>
        [HttpGet("update/{identifier}")]
        public async Task<IActionResult> UpdateTemplate(string identifier)
        {
            var user = await _userHelper.GetByIdentifierAsync<UserFormItem>(identifier: identifier);
            return Ok(user);
        }

        /// <summary>
        /// Update an existing User.
        /// </summary>
        /// <param name="formItem">Updated user data.</param>
        /// <returns>Updated user.</returns>
        [HttpPut("update")]
        public async Task<IActionResult> Update([FromBody] UserFormItem formItem)
        {
            if (ModelState.IsValid == false)
            {
                return BadRequest(ModelState);
            }

            var result = await _userHelper.SaveAsync(formItem: formItem);
            return Ok(result is not null ? new UserFormItem(user: result) : null);
        }

        /// <summary>
        /// Delete a User by identifier along with all their TaskItems.
        /// </summary>
        /// <param name="identifier">User identifier.</param>
        /// <returns>Deletion result.</returns>
        [HttpDelete("{identifier}")]
        public async Task<IActionResult> Delete(string identifier)
        {
            var deleted = await _userHelper.DeleteAsync(identifier: identifier);
            return Ok(new { deleted });
        }
    }
}
