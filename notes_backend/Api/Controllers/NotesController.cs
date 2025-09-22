using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NotesBackend.Api.Models;
using NotesBackend.Application.Notes;

namespace NotesBackend.Api.Controllers
{
    [ApiController]
    [Route("api/notes")]
    [Authorize]
    public class NotesController : ControllerBase
    {
        private readonly INoteService _noteService;

        public NotesController(INoteService noteService)
        {
            _noteService = noteService;
        }

        private Guid GetUserId()
        {
            var sub = User.FindFirstValue(ClaimTypes.NameIdentifier) ??
                      User.FindFirstValue("sub");
            return Guid.Parse(sub!);
        }

        /// <summary>
        /// List notes for the authenticated user.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(PagedNotesResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> List([FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken ct = default)
        {
            var userId = GetUserId();
            var (items, total) = await _noteService.ListAsync(userId, page, pageSize, ct);
            var dto = new PagedNotesResponse
            {
                Items = items.Select(n => new NoteResponse
                {
                    Id = n.Id,
                    Title = n.Title,
                    Content = n.Content,
                    CreatedAtUtc = n.CreatedAtUtc,
                    UpdatedAtUtc = n.UpdatedAtUtc
                }),
                Total = total,
                Page = page,
                PageSize = pageSize
            };
            return Ok(dto);
        }

        /// <summary>
        /// Get a single note by id.
        /// </summary>
        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(NoteResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Get(Guid id, CancellationToken ct)
        {
            var userId = GetUserId();
            var note = await _noteService.GetAsync(userId, id, ct);
            if (note == null) return NotFound(new { message = "Ocean: Note not found." });

            return Ok(new NoteResponse
            {
                Id = note.Id,
                Title = note.Title,
                Content = note.Content,
                CreatedAtUtc = note.CreatedAtUtc,
                UpdatedAtUtc = note.UpdatedAtUtc
            });
        }

        /// <summary>
        /// Create a new note.
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(NoteResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] NoteCreateRequest req, CancellationToken ct)
        {
            if (!ModelState.IsValid) return ValidationProblem(ModelState);
            var userId = GetUserId();
            var note = await _noteService.CreateAsync(userId, req.Title, req.Content, ct);
            var resp = new NoteResponse
            {
                Id = note.Id,
                Title = note.Title,
                Content = note.Content,
                CreatedAtUtc = note.CreatedAtUtc,
                UpdatedAtUtc = note.UpdatedAtUtc
            };
            return CreatedAtAction(nameof(Get), new { id = note.Id }, resp);
        }

        /// <summary>
        /// Update an existing note.
        /// </summary>
        [HttpPut("{id:guid}")]
        [ProducesResponseType(typeof(NoteResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(Guid id, [FromBody] NoteUpdateRequest req, CancellationToken ct)
        {
            var userId = GetUserId();
            var note = await _noteService.UpdateAsync(userId, id, req.Title ?? string.Empty, req.Content ?? string.Empty, ct);
            if (note == null) return NotFound(new { message = "Ocean: Note not found." });

            return Ok(new NoteResponse
            {
                Id = note.Id,
                Title = note.Title,
                Content = note.Content,
                CreatedAtUtc = note.CreatedAtUtc,
                UpdatedAtUtc = note.UpdatedAtUtc
            });
        }

        /// <summary>
        /// Delete a note.
        /// </summary>
        [HttpDelete("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
        {
            var userId = GetUserId();
            var ok = await _noteService.DeleteAsync(userId, id, ct);
            if (!ok) return NotFound(new { message = "Ocean: Note not found." });
            return NoContent();
        }
    }
}
