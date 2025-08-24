using CommonLibrary;
using CommonLibrary.ModelsDtos.DtoTaskItem;
using CommonLibrary.ModelsDtos.DtoUser;
using CommonLibrary.ModelsHelpers;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    /// <summary>
    /// Provides endpoints to manage TaskItems for selected Users.
    /// </summary>
    /// <remarks>
    /// All actions require a valid User identifier to ensure operations are performed on TaskItems belonging to that User.
    /// Supports creating, retrieving, updating, and deleting of TaskItems.
    /// </remarks>
    [ApiController]
    [Route("api/[controller]")]
    public class TaskItemsController : ControllerBase
    {
        private readonly AppDbContext _appDbContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="TaskItemsController"/> class.
        /// </summary>
        /// <param name="appDbContext">The database context used to access TaskItems and related data.</param>
        /// <remarks>
        /// This constructor sets up the controller with the <see cref="AppDbContext"/> instance, then it is used to initializes the <see cref="TaskItemHelper"/> to manage taskItem-related operations trought the <see cref="TaskItemHelper"/>.
        /// </remarks>
        public TaskItemsController(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        /// <summary>
        /// Get all TaskItems for a specific User.
        /// </summary>
        /// <param name="userIdentifier">User identifier. This is required to only get the TaskItems that belongs to that User.</param>
        /// <returns>List of TaskItems for the user.</returns>
        [HttpGet]
        public async Task<IActionResult> GetAll(string userIdentifier)
        {
            var taskItemHelper = new TaskItemHelper(appDbContext: _appDbContext, userIdentifier: userIdentifier);

            var taskItems = await taskItemHelper.GetAllAsync();
            return Ok(taskItems);
        }

        /// <summary>
        /// Get a single TaskItem by identifier for a specific User.
        /// </summary>
        /// <param name="userIdentifier">User identifier. This is required to ensure the TaskItem belongs to that User.</param>
        /// <param name="identifier">TaskItem identifier.</param>
        /// <returns>TaskItem details.</returns>
        [HttpGet("{userIdentifier}/{identifier}")]
        public async Task<IActionResult> GetByIdentifier(string userIdentifier, string identifier)
        {
            var taskItemHelper = new TaskItemHelper(appDbContext: _appDbContext, userIdentifier: userIdentifier);

            var taskItem = await taskItemHelper.GetByIdentifierAsync<TaskItemListItem>(identifier: identifier);
            return Ok(taskItem);
        }

        /// <summary>
        /// Get a blank template for creating a new TaskItem for a User.
        /// </summary>
        /// <param name="userIdentifier">User identifier. This is required to ensure the TaskItem template will belong to that User.</param>
        /// <returns>TaskItem form template.</returns>
        [HttpGet("create/{userIdentifier}")]
        public async Task<IActionResult> CreateTemplate(string userIdentifier)
        {
            await Task.CompletedTask;

            var taskItemHelper = new TaskItemHelper(appDbContext: _appDbContext, userIdentifier: userIdentifier);

            var template = new TaskItemFormItem
            {
                User = taskItemHelper.CurrentUser.Identifier,
                Identifier = string.Empty,
                Name = string.Empty,
                Description = string.Empty,
                TaskItemStatus = null,
            };
            return Ok(template);
        }

        /// <summary>
        /// Create a new TaskItem.
        /// </summary>
        /// <param name="formItem">TaskItem form data.</param>
        /// <returns>Created TaskItem.</returns>
        [HttpPost("create/")]
        public async Task<IActionResult> Create([FromBody] TaskItemFormItem formItem)
        {
            if (ModelState.IsValid == false)
            {
                return BadRequest(ModelState);
            }

            var taskItemHelper = new TaskItemHelper(appDbContext: _appDbContext, userIdentifier: formItem.User ?? string.Empty);

            var result = await taskItemHelper.SaveAsync(formItem: formItem);
            return Ok(result is not null ? new TaskItemFormItem(taskItem: result) : null);
        }

        /// <summary>
        /// Get a TaskItem template to update an existing TaskItem of a specific User.
        /// </summary>
        /// <param name="userIdentifier">User identifier. This is required to ensure the TaskItem template belongs to that User.</param>
        /// <param name="identifier">TaskItem identifier.</param>
        /// <returns>TaskItem form template with data.</returns>
        [HttpGet("update/{userIdentifier}/{identifier}")]
        public async Task<IActionResult> UpdateTemplate(string userIdentifier, string identifier)
        {
            var taskItemHelper = new TaskItemHelper(appDbContext: _appDbContext, userIdentifier: userIdentifier);

            var taskItem = await taskItemHelper.GetByIdentifierAsync<TaskItemFormItem>(identifier: identifier);
            return Ok(taskItem);
        }

        /// <summary>
        /// Update an existing TaskItem.
        /// </summary>
        /// <param name="formItem">Updated TaskItem data.</param>
        /// <returns>Updated TaskItem.</returns>
        [HttpPut("update/")]
        public async Task<IActionResult> Update([FromBody] TaskItemFormItem formItem)
        {
            if (ModelState.IsValid == false)
            {
                return BadRequest(ModelState);
            }

            var taskItemHelper = new TaskItemHelper(appDbContext: _appDbContext, userIdentifier: formItem.User ?? string.Empty);

            var result = await taskItemHelper.SaveAsync(formItem: formItem);
            return Ok(result is not null ? new TaskItemFormItem(taskItem: result) : null);
        }

        /// <summary>
        /// Delete a TaskItem by identifier for a specific User.
        /// </summary>
        /// <param name="userIdentifier">User identifier. This is required to ensure the TaskItem belongs to that User.</param>
        /// <param name="identifier">TaskItem identifier.</param>
        /// <returns>Deletion result.</returns>
        [HttpDelete("{userIdentifier}/{identifier}")]
        public async Task<IActionResult> Delete(string userIdentifier, string identifier)
        {
            var taskItemHelper = new TaskItemHelper(appDbContext: _appDbContext, userIdentifier: userIdentifier);

            var deleted = await taskItemHelper.DeleteAsync(identifier: identifier);
            return Ok(new { deleted });
        }
    }
}
