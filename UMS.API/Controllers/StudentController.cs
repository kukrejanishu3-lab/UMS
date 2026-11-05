
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UMS.Application.Features.Grading.Queries;
using UMS.Application.DTOs.Student;
using UMS.Application.Features.Student.Commands;
using UMS.Application.Features.Student.Queries;

namespace UMS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StudentController(ISender sender) : ControllerBase
    {
        // Gets all students.
        [HttpGet]
        public async Task<IActionResult> GetStudents()
        {
            var students = await sender.Send(new GetStudentsQuery());
            return Ok(students);
        }

        // Gets a student by ID.
        [HttpGet("{studentId:guid}")]
        public async Task<IActionResult> GetStudentById(Guid studentId)
        {
            var student = await sender.Send(
                new GetStudentByIdQuery(studentId));

            if (student is null)
                return NotFound(new { message = "Student not found." });

            return Ok(student);
        }

        // Retrieves full academic transcript with SGPA and CGPA for a specific student.
        [HttpGet("{studentId:guid}/transcript")]
        [Authorize(Roles = "SuperAdmin,Teacher")]
        public async Task<IActionResult> GetStudentTranscript(Guid studentId)
        {
            try
            {
                var transcript = await sender.Send(new GetStudentTranscriptQuery(null, null, studentId));
                return Ok(transcript);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        // Registers a student if their student number is not already used.
        [HttpPost]
        public async Task<IActionResult> AddStudent(
            [FromBody] CreateStudentDto dto)
        {
            try
            {
                var createdStudent = await sender.Send(
                    new CreateStudentCommand(dto));

                return CreatedAtAction(
                    nameof(GetStudentById),
                    new { studentId = createdStudent.Id },
                    createdStudent);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }


        // Registers multiple students and reports rejected entries.
        [HttpPost("multiple")]
        public async Task<IActionResult> AddStudents(
            [FromBody] List<CreateStudentDto> students)
        {
            var result = await sender.Send(
                new CreateMultipleStudentsCommand(students));

            return Ok(new
            {
                message = "Multiple student registration completed.",
                addedCount = result.AddedStudents.Count,
                rejectedCount = result.RejectedStudents.Count,
                addedStudents = result.AddedStudents,
                rejectedStudents = result.RejectedStudents
            });
        }

        // Updates an existing student.
        [HttpPut("{studentId:guid}")]
        public async Task<IActionResult> UpdateStudent(
            Guid studentId,
            [FromBody] CreateStudentDto dto)
        {
            try
            {
                var updatedStudent = await sender.Send(
                    new UpdateStudentCommand(studentId, dto));

                if (updatedStudent is null)
                    return NotFound(new { message = "Student not found." });

                return Ok(updatedStudent);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        // Deactivates a student by their ID.
        [HttpPatch("{studentId:guid}/deactivate")]
        public async Task<IActionResult> DeactivateStudent(Guid studentId)
        {
            var deactivated = await sender.Send(
                new DeactivateStudentCommand(studentId));

            // Returns not found if the student does not exist.
            if (!deactivated)
            {
                return NotFound(new { message = "Student not found." });
            }

            return NoContent();
        }
    }
}
