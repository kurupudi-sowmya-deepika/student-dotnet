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
    // GET /student  — Any authenticated user (Admin or User) can read the list
    [HttpGet]
    public ActionResult<List<Student>> GetAll() => StudentService.GetAll();

    // GET /student/{id}  — Any authenticated user can read a single student
    [HttpGet("{id}")]
    public ActionResult<Student> Get(int id)
    {
        var student = StudentService.Get(id);
        if (student == null)
            return NotFound();
        return student;
    }

    // POST /student  — Only Admin role can create students
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public IActionResult Create(Student student)
    {
        StudentService.Add(student);
        return CreatedAtAction(nameof(Get), new { id = student.id }, student);
    }

    // PUT /student  — Only Admin role can update students
    [HttpPut]
    [Authorize(Roles = "Admin")]
    public IActionResult Update(int id, Student student)
    {
        if (id != student.id)
            return BadRequest();

        var existingStudent = StudentService.Get(id);
        if (existingStudent == null)
            return NotFound();

        StudentService.Update(student);
        return NoContent();
    }

    // DELETE /student  — Only Admin role can delete students
    [HttpDelete]
    [Authorize(Roles = "Admin")]
    public IActionResult Delete(int id)
    {
        var existingStudent = StudentService.Get(id);
        if (existingStudent == null)
            return NotFound();

        StudentService.Delete(id);
        return NoContent();
    }
}
