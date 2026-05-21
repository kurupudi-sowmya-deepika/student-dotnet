using Microsoft.AspNetCore.Mvc;// Imports ASP.NET Core controller features.
using StudentApi.Models;
using StudentApi.Services;

namespace StudentApi.Controllers;

[ApiController]  // Marks this class as Web API controller.
[Route("[controller]")]  // URL route uses controller name.
public class StudentController :ControllerBase{  // Creates controller class. ControllerBase gives API features.
    // get all students
    [HttpGet]
    public ActionResult<List<Student>> GetAll ()=>
        StudentService.GetAll();

    
    // get student by id 
    [HttpGet("{id}")]
    public ActionResult<Student> Get(int id){
        var student = StudentService.Get(id);
        if(student == null){
            return NotFound();
        }
        return student;
    } 

    // add student
    [HttpPost]
    public IActionResult Create(Student student){
        StudentService.Add(student);
        return CreatedAtAction(
            nameof(Get),
            new { id = student.id},
            student

        );
    }

    // update student details
    [HttpPut]
    public IActionResult Update(int id, Student Student){
        if(id!=Student.id)
            return BadRequest();
        
        var existingStudent = StudentService.Get(id);
        if(existingStudent == null)
            return NotFound();

        StudentService.Update(Student);

        return NoContent();
    }

    // delete student details
    [HttpDelete]
    public IActionResult Delete(int id){
        
        var existingStudent = StudentService.Get(id);
        if(existingStudent == null)
            return NotFound();

        StudentService.Delete(id);

        return NoContent();
    }
}