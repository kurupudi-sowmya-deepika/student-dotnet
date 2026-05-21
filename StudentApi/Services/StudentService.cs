using Microsoft.EntityFrameworkCore;
using StudentApi.Data;
using StudentApi.Models;

namespace StudentApi.Services;

public class StudentService : IStudentService
{
    private readonly AppDbContext _context;

    public StudentService(AppDbContext context)
    {
        _context = context;
    }

    // Get all students from database
    public async Task<List<Student>> GetAllAsync()
    {
        return await _context.Students.ToListAsync();
    }

    // Get a specific student by ID
    public async Task<Student?> GetAsync(int id)
    {
        return await _context.Students.FindAsync(id);
    }

    // Add a new student to database
    public async Task AddAsync(Student student)
    {
        // Set id to 0 to let SQLite auto-generate it (primary key autoincrement)
        student.id = 0;
        _context.Students.Add(student);
        await _context.SaveChangesAsync();
    }

    // Update an existing student's details
    public async Task UpdateAsync(Student student)
    {
        _context.Students.Update(student);
        await _context.SaveChangesAsync();
    }

    // Delete a student from database
    public async Task DeleteAsync(int id)
    {
        var student = await GetAsync(id);
        if (student != null)
        {
            _context.Students.Remove(student);
            await _context.SaveChangesAsync();
        }
    }
}