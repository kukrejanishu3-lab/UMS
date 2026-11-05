using MediatR;
using Microsoft.AspNetCore.Mvc;
using UMS.Application.DTOs.Course;
using UMS.Application.Features.Course.Commands;
using UMS.Application.Features.Course.Queries;

namespace UMS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CourseController(ISender sender) : ControllerBase
    {
        // Retrieves all courses
        [HttpGet]
        public async Task<IActionResult> GetCourses()
        {
            var courses = await sender.Send(new GetCoursesQuery());
            return Ok(courses);
        }

        // Retrieves a course by its ID
        [HttpGet("{courseId:guid}")]
        public async Task<IActionResult> GetCourseById(Guid courseId)
        {
            var course = await sender.Send(new GetCourseByIdQuery(courseId));

            if (course is null)
            {
                return NotFound(new { message = "Course not found." });
            }

            return Ok(course);
        }

        // Creates a new course
        [HttpPost]
        public async Task<IActionResult> AddCourse([FromBody] CreateCourseDto dto)
        {
            try
            {
                var createdCourse = await sender.Send(new CreateCourseCommand(dto));

                return CreatedAtAction(
                    nameof(GetCourseById),
                    new { courseId = createdCourse.Id },
                    createdCourse);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        // Adds valid courses and skips duplicate course codes.
        [HttpPost("multiple")]
        public async Task<IActionResult> AddCourses([FromBody] List<CreateCourseDto> courses)
        {
            var result = await sender.Send(new CreateMultipleCoursesCommand(courses));

            return Ok(new
            {
                message = "Multiple course registration completed.",
                addedCount = result.AddedCourses.Count,
                rejectedCount = result.RejectedCourses.Count,
                addedCourses = result.AddedCourses,
                rejectedCourses = result.RejectedCourses
            });
        }

        // Updates an existing course
        [HttpPut("{courseId:guid}")]
        public async Task<IActionResult> UpdateCourse(Guid courseId, [FromBody] UpdateCourseDto dto)
        {
            try
            {
                var updatedCourse = await sender.Send(new UpdateCourseCommand(courseId, dto));

                if (updatedCourse is null)
                {
                    return NotFound(new { message = "Course not found." });
                }

                return Ok(updatedCourse);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        // Deletes a course by its ID
        [HttpDelete("{courseId:guid}")]
        public async Task<IActionResult> DeleteCourse(Guid courseId)
        {
            var deleted = await sender.Send(new DeleteCourseCommand(courseId));

            if (!deleted)
            {
                return NotFound(new { message = "Course not found." });
            }

            return NoContent();
        }
    }
}
