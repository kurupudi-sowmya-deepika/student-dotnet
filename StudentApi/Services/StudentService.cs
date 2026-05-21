using StudentApi.Models;
// import student model

namespace StudentApi.Services;

//  static - no objection creation required , methods can be called directly

public static class StudentService{
    static List<Student> Students {get;}
    static int nextId=3;

    static StudentService(){  // static constructor
        Students = new List<Student>
        {
            new Student
            {
                id=1,
                name="Sowmya",
                age=22,
                course="Btech"

            },
            new Student
            {
                id=2,
                name="Deepika",
                age=21,
                course="Bcom"

            }
        };
    }
    // get all students list
    public static List <Student> GetAll() =>Students;

    // get student details by id
    public static Student? Get(int id)=>
        Students.FirstOrDefault(s=>s.id ==id);

    public static void Add (Student student){
        student.id =nextId++;
        Students.Add(student);
    }

    public static void Update (Student student){
        var index = Students.FindIndex(s=>s.id == student.id);
        if (index == -1)
            return ;
        Students[index]=student;
    }

    public static void Delete(int id){
        var Student = Get(id);
        if (Student == null){
            return;
        }
        Students.Remove(Student);
    }

}