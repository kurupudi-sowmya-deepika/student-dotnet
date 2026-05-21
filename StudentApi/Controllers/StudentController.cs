using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentApi.Models;
using StudentApi.Services;

namespace StudentApi.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]  // Every endpoint in this controller requires a valid JWT token
public class StudentController : ControllerBase
{
    private readonly IStudentService _studentService;

    public StudentController(IStudentService studentService)
    {
        _studentService = studentService;
    }

    // GET /student  — Any authenticated user (Admin or User) can read the list
    [HttpGet]
    public async Task<ActionResult<List<Student>>> GetAll()
    {
        return await _studentService.GetAllAsync();
    }

    // GET /student/{id}  — Any authenticated user can read a single student
    [HttpGet("{id}")]
    public async Task<ActionResult<Student>> Get(int id)
    {
        var student = await _studentService.GetAsync(id);
        if (student == null)
            return NotFound();
        return student;
    }

    // POST /student  — Only Admin role can create students
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create(Student student)
    {
        await _studentService.AddAsync(student);
        return CreatedAtAction(nameof(Get), new { id = student.id }, student);
    }

    // PUT /student  — Only Admin role can update students
    [HttpPut]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(int id, Student student)
    {
        if (id != student.id)
            return BadRequest();

        var existingStudent = await _studentService.GetAsync(id);
        if (existingStudent == null)
            return NotFound();

        await _studentService.UpdateAsync(student);
        return NoContent();
    }

    // DELETE /student  — Only Admin role can delete students
    [HttpDelete]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var existingStudent = await _studentService.GetAsync(id);
        if (existingStudent == null)
            return NotFound();

        await _studentService.DeleteAsync(id);
        return NoContent();
    }
}
